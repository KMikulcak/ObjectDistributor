using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;

namespace ObjectDistributor.Out.Implementation
{
    public class TimeWorker : WorkerCell<string, char[]>
    {
        public TimeWorker() : base(1, new[] {1})
        {
        }

        protected override List<char[]> ProcessThis(IList<string> valueObjects)
        {
            return valueObjects.Select(valueObject => valueObject.ToCharArray()).ToList();
        }
    }
}