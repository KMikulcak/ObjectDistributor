using System;
using System.Collections.Generic;
using NLog;

namespace ObjectDistributor.Core.ObjectHandler
{
    internal class Package<TResult> : IPackage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConsumer _consumer;
        private List<object> _valueObjects;

        public Package(IConsumer consumer, int workId)
        {
            _valueObjects = new List<object>();
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            GetResultType = typeof(TResult);

            _consumer = consumer;
            WorkId = workId;

            SetStatus(Status.Created);
        }

        public DateTime Created { get; }

        public Guid Id { get; }

        public int WorkId { get; }

        public Status Status { get; private set; }

        public Type GetCurrentValueObjectType { get; private set; }

        public Type GetResultType { get; }

        public void AddAndResetValueObjects<TValueObject>(IList<TValueObject> valueObjects)
        {
            _valueObjects.Clear();

            foreach (var valueObject in valueObjects) _valueObjects.Add(valueObject);
            GetCurrentValueObjectType = valueObjects[0].GetType();

            SetStatus(GetCurrentValueObjectType == typeof(TResult) ? Status.Resolved : Status.Waiting);
        }

        public IList<TValueObject> GetValueObjects<TValueObject>()
        {
            SetStatus(Status.InProgress);
            return (IList<TValueObject>) _valueObjects;
        }

        private void SetStatus(Status status)
        {
            Status = status;
            Logger.Debug($"Package[{Id}] StatusChange[{status}]");

            if (Status != Status.Resolved) return;

            var results = new List<TResult>();
            foreach (var valueObject in _valueObjects) results.Add((TResult) valueObject);
            Logger.Debug($"produced {results.Count} results of type {typeof(TResult).Name}");

            ((IConsumer<TResult>) _consumer).Completed(results);

            _valueObjects = null;
        }
    }
}