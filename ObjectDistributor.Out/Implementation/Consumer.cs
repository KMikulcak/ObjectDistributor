using System;
using System.Collections.Generic;
using ObjectDistributor.Core.ObjectHandler;

namespace ObjectDistributor.Out.Implementation
{
    internal class Consumer : IConsumer<string>
    {
        public void Completed(IList<string> result)
        {
            EndConsumer.AddToStack(result);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        public void WasNull(Type wasNullType)
        {
            throw new NotImplementedException();
        }
    }
}