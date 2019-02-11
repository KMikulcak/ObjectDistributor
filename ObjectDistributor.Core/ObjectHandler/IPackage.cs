using System;
using System.Collections.Generic;

namespace ObjectDistributor.Core.ObjectHandler
{
    internal interface IPackage
    {
        DateTime Created { get; }

        Guid Id { get; }

        Type GetCurrentValueObjectType { get; }

        Type GetResultType { get; }

        int WorkId { get; }

        Status Status { get; }

        void AddAndResetValueObjects<TValueObject>(IList<TValueObject> valueObjects);

        IList<TValueObject> GetValueObjects<TValueObject>();
    }

    public enum Status
    {
        Created,
        Waiting,
        InProgress,
        Resolved
    }
}