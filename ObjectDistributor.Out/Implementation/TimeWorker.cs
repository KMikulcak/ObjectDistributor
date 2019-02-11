using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;

namespace ObjectDistributor.Out.Implementation
{
    public class TimeWorker : WorkerCell<int, int>
    {
        public TimeWorker() : base(1, new[] {1})
        {
        }

        protected override List<int> ProcessThis(IList<int> valueObjects)
        {
            return valueObjects.Select(valueObject => valueObject / 1000 * 2 + 150).ToList();
        }
    }
}