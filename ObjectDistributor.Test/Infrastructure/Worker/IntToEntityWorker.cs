using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;
using ObjectDistributor.Test.Infrastructure.Entity;

namespace ObjectDistributor.Test.Infrastructure.Worker
{
    internal class IntToEntityWorker : WorkerCell<int, TestEntity>
    {
        public IntToEntityWorker() : base(1, new[] {2})
        {
        }

        protected override List<TestEntity> ProcessThis(IList<int> valueObjects)
        {
            return valueObjects.Select(valueObject => valueObject)
                .Select(
                    props => new TestEntity
                    {
                        Id = props,
                        Name = $"Name[{props}]"
                    }
                ).ToList();
        }
    }
}