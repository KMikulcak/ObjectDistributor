using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private static BlockingCollection<Action> _requests;

        private static IList<Task> _manageTasks;

        private static int _warningBlock;
        private static int _warningMem;
        private static long _maxMemory;

        public ThreadManager(int maxAddThreads, int maxPackageThreads, CancellationToken token, IList<Task> manageTasks,
            int maxMemoryInMegaBytes)
        {
            _maxAddThreads = maxAddThreads;
            _maxPackageThreads = maxPackageThreads;
            _token = token;
            _maxMemory = CalculateMemoryByMegaBytes(maxMemoryInMegaBytes);

            _mainThread = new Thread(Run);
            _manageTasks = manageTasks;

            _requests = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        }

        public void Start()
        {
            _mainThread.Start();
        }

        public static void LogicRun(BlockingCollection<Action> actions, TaskType taskType)
        {
            foreach (var result in actions.GetConsumingEnumerable())
            {
                HandleThreadNumbers(taskType, OperationType.Increase);
                while (!CanRunNewThread(taskType))
                {
                    _warningBlock++;
                    Thread.Sleep(100);
                    if (_warningBlock < 40) continue;
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

        public void AddProcess(Action action)
        {
            _requests.TryAdd(action);
        }

        public static void ProcessRequests()
        {
            var initialMemory = GC.GetTotalMemory(false);
            Task.Factory.StartNew(() =>
            {
                var processes = _requests;
                while (!_token.IsCancellationRequested)
                    if (GC.GetTotalMemory(false) - initialMemory < _maxMemory)
                    {
                        if (processes.Any())
                            foreach (var result in processes.GetConsumingEnumerable())
                                result.Invoke();
                        else
                            Thread.Sleep(100);
                    }
                    else
                    {
                        _warningMem++;
                        if (_warningMem < 20) continue;
                        Logger.Warn($"MAX memory reached[{_maxMemory}], waiting for decreasing");
                        _warningMem = 0;
                        Thread.Sleep(100);
                    }
            }, _token);
        }

        private static long CalculateMemoryByMegaBytes(int megaBytes)
        {
            return 1024 * 1024 * megaBytes;
        }

        private static void Run()
        {
            foreach (var task in _manageTasks) task.Start();
            ProcessRequests();
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