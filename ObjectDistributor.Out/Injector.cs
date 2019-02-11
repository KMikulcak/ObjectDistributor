using System;
using System.Collections.Generic;
using System.Threading;
using ObjectDistributor.Core;
using ObjectDistributor.Out.Implementation;

namespace ObjectDistributor.Out
{
    internal class Injector
    {
        private static Thread _mainThread;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);
        private readonly Distributor _dis;

        public Injector(Distributor dis)
        {
            _dis = dis;

            _mainThread = new Thread(Run);
            Cts = new CancellationTokenSource();
        }

        private static CancellationTokenSource Cts { get; set; }

        public void Start()
        {
            _dis.Start();
            _mainThread.Start();
            Console.WriteLine("Injector Started");
        }

        public void Stop()
        {
            _dis.Stop();
            Cts.Cancel();
            Console.WriteLine("Injector Stopped");
        }

        private void Run()
        {
            while (!Cts.IsCancellationRequested)
            {
                GenerateAndInject();
                var sleep = GenerateRandomNumber(500, 5000);
                Console.WriteLine($"Injector sleeps now for {sleep / 1000} seconds");
                Thread.Sleep(sleep);
            }
        }

        private void GenerateAndInject()
        {
            Console.WriteLine($"Injector starting pushing data at:{DateTime.Now}");

            var amount = GenerateRandomNumber(1, 1000);

            for (var i = 0; i <= amount; i++)
            {
                var values = new List<int>();

                for (var ii = 0; ii < GenerateRandomNumber(1, 100); ii++) values.Add(GenerateRandomNumber(0, 3000000));

                _dis.AddData(values, new Consumer(), 1);
            }

            Console.WriteLine($"Injector done pushing[{amount}] data at:{DateTime.Now}");
        }

        private static int GenerateRandomNumber(int min, int max)
        {
            return Rnd.Next(min, max);
        }
    }
}