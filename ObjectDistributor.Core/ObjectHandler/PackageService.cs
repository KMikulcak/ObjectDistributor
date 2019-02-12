using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using ObjectDistributor.Core.Worker;

namespace ObjectDistributor.Core.ObjectHandler
{
    internal class PackageService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly WorkerService _workerService;

        public PackageService()
        {
            _workerService = new WorkerService();
        }

        public Action CreatePackageTask(IPackage package)
        {
            return () =>
            {
                try
                {
                    var workers = _workerService.GetWorkerById(package.WorkId);

                    while (!package.IsResolved)
                    {
                        var w = GetNextWorkerCell(workers, package);
                        if (w != null)
                        {
                            workers.Remove(w);
                            var results = w.Process(package.GetValueObjects<object>());
                            package.AddAndResetValueObjects(results);
                        }
                        else
                        {
                            Logger.Debug(
                                $"uncompleted chain for Package: Id[{package.Id}] on Current[{package.GetCurrentValueObjectType.Name}] for {package.GetResultType.Name}");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    package.HandleError(ex);
                }
            };
        }

        public void AddWorker(IWorkerCell workerCell)
        {
            _workerService.AddWorker(workerCell);
        }

        public Package<TResult> CreatePackage<TValueObject, TResult>(IList<TValueObject> valueObjects,
            IConsumer consumer, int workId)
        {
            var package = new Package<TResult>(consumer, workId);
            package.AddAndResetValueObjects(valueObjects);

            Logger.Debug($"Package[{package.Id}] created {package.Created}");
            return package;
        }

        private static IWorkerCell GetNextWorkerCell(IEnumerable<IWorkerCell> workers, IPackage package)
        {
            var workerResult = workers.ToList().Where(x => x.ValueObjectType == package.GetCurrentValueObjectType)
                .OrderBy(x => x.Priority);
            return workerResult.Any() ? workerResult.First() : null;
        }
    }
}