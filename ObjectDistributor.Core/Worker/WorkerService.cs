using System.Collections.Generic;
using System.Linq;
using NLog;

namespace ObjectDistributor.Core.Worker
{
    internal class WorkerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly List<IWorkerCell> _workerCells;

        public WorkerService()
        {
            _workerCells = new List<IWorkerCell>();
        }

        public void AddWorker(IWorkerCell workerCell)
        {
            if (!_workerCells.Contains(workerCell)) _workerCells.Add(workerCell);
        }

        public IList<IWorkerCell> GetWorkerById(int workId)
        {
            var workers = new List<IWorkerCell>();

            foreach (var workerCell in _workerCells)
                if (workerCell.WorkId.Any(id => id == workId))
                    workers.Add(workerCell);

            return workers;
        }
    }
}