using System;
using System.Collections.Generic;
using ObjectDistributor.Core.ObjectHandler;
using ObjectDistributor.Test.Infrastructure.Model;

namespace ObjectDistributor.Test.Infrastructure.Consumer
{
    public class ModelConsumer : IConsumer<TestModel>
    {
        private readonly Action<IList<TestModel>> _finished;

        public ModelConsumer(Action<IList<TestModel>> finished)
        {
            _finished = finished;
        }

        public void Completed(IList<TestModel> result)
        {
            _finished(result);
        }

        public void Error(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void WasNull(Type wasNullType)
        {
            throw new NotImplementedException();
        }
    }
}