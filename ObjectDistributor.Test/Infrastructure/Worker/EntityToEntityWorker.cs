using System;
using System.Collections.Generic;
using System.Globalization;
using ObjectDistributor.Core.Worker;
using ObjectDistributor.Test.Infrastructure.Entity;

namespace ObjectDistributor.Test.Infrastructure.Worker
{
    public class EntityToEntityWorker : WorkerCell<TestEntity, TestEntity>
    {
        public EntityToEntityWorker() : base(2, new[] {1})
        {
        }

        protected override List<TestEntity> ProcessThis(IList<TestEntity> valueObjects)
        {
            foreach (var valueObject in valueObjects)
                valueObject.Name = $"{valueObject.Name}_{DateTime.Now.ToString(CultureInfo.CurrentCulture)}";

            return (List<TestEntity>) valueObjects;
        }
    }
}