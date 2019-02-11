﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using ObjectDistributor.Core.Infrastructure;
using ObjectDistributor.Core.ObjectHandler;
using ObjectDistributor.Core.Worker;
using static ObjectDistributor.Core.Infrastructure.ThreadManager;

namespace ObjectDistributor.Core
{
    public class Distributor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly BlockingCollection<Action> _addDataTasks;
        private readonly PackageService _packageService;
        private readonly BlockingCollection<Action> _packageTasks;
        private readonly ThreadManager _threadManager;

        public Distributor(int maxAddThreads = 10, int maxPackageThreads = 10)
        {
            Bootstrapper.Init();

            _packageTasks = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
            _addDataTasks = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
            _packageService = new PackageService();

            Cts = new CancellationTokenSource();

            _threadManager = new ThreadManager(maxAddThreads,
                maxPackageThreads,
                Cts.Token,
                new List<Task>
                {
                    ManageTaskAction(TaskType.AddData),
                    ManageTaskAction(TaskType.Package)
                });

            Logger.Debug("Created");
        }

        private static CancellationTokenSource Cts { get; set; }

        public void AddWorkerCell<TValueObject, TResult>(WorkerCell<TValueObject, TResult> workerCell)
        {
            _packageService.AddWorker(workerCell);
        }

        public void AddData<TValueObject, TResult>(List<TValueObject> valueObjects, IConsumer<TResult> consumer,
            int workId)
        {
            _addDataTasks.TryAdd(AddDataTask<TValueObject, TResult>(valueObjects, consumer, workId));
        }

        public void Start()
        {
            _threadManager.Start();
            Logger.Debug("Started");
        }

        public void Stop()
        {
            Cts.Cancel();
            Logger.Debug("Stopped");
        }

        private Action AddDataTask<TValueObject, TResult>(IList<TValueObject> valueObjects, IConsumer consumer,
            int workId)
        {
            return () =>
            {
                _packageTasks.TryAdd(
                    _packageService.CreatePackageTask(
                        _packageService.CreatePackage<TValueObject, TResult>(valueObjects, consumer, workId)));
            };
        }

        private Task ManageTaskAction(TaskType taskType)
        {
            return new Task(() =>
            {
                BlockingCollection<Action> actions;

                switch (taskType)
                {
                    case TaskType.AddData:
                        actions = _addDataTasks;
                        break;
                    case TaskType.Package:
                        actions = _packageTasks;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
                }

                while (!Cts.IsCancellationRequested)
                {
                    var isQueryable = actions.Any();
                    if (isQueryable) LogicRun(actions, taskType);
                }
            }, Cts.Token);
        }
    }
}