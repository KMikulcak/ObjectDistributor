using System.Collections.Generic;

namespace ObjectDistributor.Core.ObjectHandler
{
    public interface IConsumer
    {
    }

    public interface IConsumer<TResult> : IConsumer
    {
        void Completed(IList<TResult> result);
    }
}