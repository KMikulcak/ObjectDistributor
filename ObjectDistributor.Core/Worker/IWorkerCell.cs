using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDistributor.Core.Worker
{
    internal interface IWorkerCell
    {
        int Priority { get; }

        int[] WorkId { get; }

        Type ValueObjectType { get; }

        Type ResultType { get; }

        IList<object> Process(IList<object> valueObjects);
    }

    public abstract class WorkerCell<TValueObject, TResult> : IWorkerCell
    {
        protected WorkerCell(int priority, int[] workId)
        {
            Priority = priority;
            WorkId = workId;
        }

        public IList<object> Process(IList<object> valueObjects)
        {
            var thisValueObjects = valueObjects.Cast<TValueObject>().ToList();

            var thisResults = ProcessThis(thisValueObjects);
            var results = new List<object>();
            foreach (var result in thisResults) results.Add(result);
            return results;
        }

        public int Priority { get; }

        public int[] WorkId { get; }

        public Type ValueObjectType => typeof(TValueObject);

        public Type ResultType => typeof(TResult);

        protected abstract List<TResult> ProcessThis(IList<TValueObject> valueObjects);
    }
}