using System;
using ObjectDistributor.Core;
using ObjectDistributor.Out.Implementation;

namespace ObjectDistributor.Out
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dis = new Distributor(20, 20);

            dis.AddWorkerCell(new TimeWorker());
            dis.AddWorkerCell(new OutputWorker());

            var injector = new Injector(dis);
            injector.Start();

            Console.ReadKey();
        }
    }
}