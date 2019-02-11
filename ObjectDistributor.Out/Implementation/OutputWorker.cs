using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;

namespace ObjectDistributor.Out.Implementation
{
    public class OutputWorker : WorkerCell<int, string>
    {
        public OutputWorker() : base(1, new[] {1})
        {
        }

        protected override List<string> ProcessThis(IList<int> valueObjects)
        {
            return valueObjects.Select(valueObject => $"[{valueObject / 0.5}]").ToList();
        }
    }
}