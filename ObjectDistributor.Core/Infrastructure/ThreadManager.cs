using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace ObjectDistributor.Core.Infrastructure
{
    internal class ThreadManager
    {
        public enum TaskType
        {
            AddData,
            Package
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Thread _mainThread;

        private static int _maxAddThreads;
        private static int _maxPackageThreads;
        private static int _currentAddThreads;
        private static int _currentPackageThreads;

        private static bool _canRunAdd;
        private static bool _canRunPackage;

        private static CancellationToken _token;

        private static IList<Task> _manageTasks;

        private static int _warningBlock;

        public ThreadManager(int maxAddThreads, int maxPackageThreads, CancellationToken token, IList<Task> manageTasks)
        {
            _maxAddThreads = maxAddThreads;
            _maxPackageThreads = maxPackageThreads;
            _token = token;

            _mainThread = new Thread(Run);
            _manageTasks = manageTasks;
        }

        public void Start()
        {
            _mainThread.Start();
        }

        private static void Run()
        {
            foreach (var task in _manageTasks) task.Start();
        }

        public static void LogicRun(BlockingCollection<Action> actions, TaskType taskType)
        {
            foreach (var result in actions.GetConsumingEnumerable())
            {
                HandleThreadNumbers(taskType, OperationType.Increase);
                while (!CanRunNewThread(taskType))
                {
                    _warningBlock++;
                    Thread.Sleep(200);
                    if (_warningBlock < 20) continue;
                    Logger.Warn($"MAX Threads reached, waiting for new Thread opening for {taskType}");
                    _warningBlock = 0;
                }

                Task.Factory.StartNew(() =>
                {
                    result.Invoke();
                    HandleThreadNumbers(taskType, OperationType.Decrease);
                    Logger.Debug($"pulled {taskType} out of queue");
                }, _token);
            }
        }

        private static void HandleThreadNumbers(TaskType taskType, OperationType operationType)
        {
            switch (taskType)
            {
                case TaskType.AddData:
                    switch (operationType)
                    {
                        case OperationType.Increase:
                            _currentAddThreads++;
                            break;
                        case OperationType.Decrease:
                            _currentAddThreads--;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
                    }

                    _canRunAdd = _currentAddThreads <= _maxAddThreads;
                    break;
                case TaskType.Package:
                    switch (operationType)
                    {
                        case OperationType.Increase:
                            _currentPackageThreads++;
                            break;
                        case OperationType.Decrease:
                            _currentPackageThreads--;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
                    }

                    _canRunPackage = _currentPackageThreads <= _maxPackageThreads;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
            }
        }

        private static bool CanRunNewThread(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.AddData:
                    return _canRunAdd;
                case TaskType.Package:
                    return _canRunPackage;
                default:
                    return false;
            }
        }

        private enum OperationType
        {
            Increase,
            Decrease
        }
    }
}