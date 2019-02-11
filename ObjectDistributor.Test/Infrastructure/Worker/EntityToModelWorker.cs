using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.Worker;
using ObjectDistributor.Test.Infrastructure.Entity;
using ObjectDistributor.Test.Infrastructure.Model;

namespace ObjectDistributor.Test.Infrastructure.Worker
{
    public class EntityToModelWorker : WorkerCell<TestEntity, TestModel>
    {
        public EntityToModelWorker() : base(3, new[] {1})
        {
        }

        protected override List<TestModel> ProcessThis(IList<TestEntity> valueObjects)
        {
            return valueObjects.Select(valueObject => new TestModel
            {
                Id = valueObject.Id,
                Name = valueObject.Name
            }).ToList();
        }
    }
}