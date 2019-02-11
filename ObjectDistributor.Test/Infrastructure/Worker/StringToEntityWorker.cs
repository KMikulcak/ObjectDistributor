using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;
using ObjectDistributor.Test.Infrastructure.Entity;

namespace ObjectDistributor.Test.Infrastructure.Worker
{
    public class StringToEntityWorker : WorkerCell<string, TestEntity>
    {
        public StringToEntityWorker() : base(1, new[] {1})
        {
        }

        protected override List<TestEntity> ProcessThis(IList<string> valueObjects)
        {
            return valueObjects.Select(valueObject => valueObject
                .Split('.')).Select(
                props => new TestEntity
                {
                    Id = int.Parse(props[0]),
                    Name = props[1]
                }
            ).ToList();
        }
    }
}