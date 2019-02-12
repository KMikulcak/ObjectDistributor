using System;
using System.Collections.Generic;
using NLog;

namespace ObjectDistributor.Core.ObjectHandler
{
    internal class Package<TResult> : IPackage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConsumer _consumer;
        private Exception _ex;
        private List<object> _valueObjects;

        public Package(IConsumer consumer, int workId)
        {
            _valueObjects = new List<object>();
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            GetResultType = typeof(TResult);

            _consumer = consumer;
            WorkId = workId;

            IsResolved = false;

            SetStatus(Status.Created);
        }

        public DateTime Created { get; }

        public Guid Id { get; }

        public int WorkId { get; }

        public Status Status { get; private set; }

        public Type GetCurrentValueObjectType { get; private set; }

        public Type GetResultType { get; }

        public bool IsResolved { get; private set; }

        public void AddAndResetValueObjects<TValueObject>(IList<TValueObject> valueObjects)
        {
            try
            {
                _valueObjects.Clear();
                if (valueObjects != null)
                {
                    foreach (var valueObject in valueObjects) _valueObjects.Add(valueObject);
                    GetCurrentValueObjectType = valueObjects[0].GetType();

                    SetStatus(GetCurrentValueObjectType == typeof(TResult) && Status != Status.Created
                        ? Status.Resolved
                        : Status.Waiting);
                }
                else
                {
                    SetStatus(Status.WasNull);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                _ex = ex;
                SetStatus(Status.Error);
            }
        }

        public IList<TValueObject> GetValueObjects<TValueObject>()
        {
            SetStatus(Status.InProgress);
            return (IList<TValueObject>) _valueObjects;
        }

        public void HandleError(Exception ex)
        {
            _ex = ex;
            ((IConsumer<TResult>) _consumer).Error(ex);
        }

        private void SetStatus(Status status)
        {
            Status = status;
            Logger.Debug($"Package[{Id}] StatusChange[{status}]");

            if (Status != Status.Resolved && Status != Status.WasNull && Status != Status.Error) return;
            switch (Status)
            {
                case Status.Resolved:
                    Resolved();
                    break;
                case Status.WasNull:
                    WasNull();
                    break;
                case Status.Error:
                    Error();
                    break;
            }

            _valueObjects = null;
            IsResolved = true;
        }

        private void Resolved()
        {
            var results = new List<TResult>();
            foreach (var valueObject in _valueObjects) results.Add((TResult) valueObject);
            Logger.Debug($"produced {results.Count} results of type {typeof(TResult).Name}");

            ((IConsumer<TResult>) _consumer).Completed(results);
        }

        private void WasNull()
        {
            Logger.Warn($"Package[{Id}] recieved NULL for {GetCurrentValueObjectType.Name}");
            ((IConsumer<TResult>) _consumer).WasNull(GetCurrentValueObjectType);
        }

        private void Error()
        {
            Logger.Error($"Package[{Id}] ran into error for {GetCurrentValueObjectType.Name}");
            ((IConsumer<TResult>) _consumer).Error(_ex);
        }
    }
}