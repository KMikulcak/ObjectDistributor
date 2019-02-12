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

        bool IsResolved { get; }

        void AddAndResetValueObjects<TValueObject>(IList<TValueObject> valueObjects);

        IList<TValueObject> GetValueObjects<TValueObject>();

        void HandleError(Exception ex);
    }

    public enum Status
    {
        Created,
        Waiting,
        InProgress,
        Resolved,
        WasNull,
        Error
    }
}