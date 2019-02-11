using System;
using System.Collections.Generic;
using System.Linq;
using ObjectDistributor.Core.ObjectHandler;

namespace ObjectDistributor.Out.Implementation
{
    internal class Consumer : IConsumer<string>
    {
        private const int StackSize = 250;
        private static readonly List<string> Stack = new List<string>();

        public void Completed(IList<string> result)
        {
            AddToStack(result);
        }

        private static void AddToStack(IList<string> result)
        {
            lock (Stack)
            {
                var size = StackSize - Stack.Count;
                var it = result.Count;
                var insertedValues = new List<string>();

                if (size <= it) it = size;

                for (var i = 0; i < it; i++)
                {
                    Stack.Add(result[i]);
                    insertedValues.Add(result[i]);
                }

                if (!Stack.Count.Equals(StackSize)) return;
                var values = "";
                var counter = 0;

                Console.WriteLine($"Results finished at: {DateTime.Now}");
                foreach (var res in Stack)
                {
                    values = values + $"({res})";
                    counter++;
                    if (counter != 50) continue;
                    Console.WriteLine(values);
                    values = "";
                    counter = 0;
                }

                Console.WriteLine("##################################");

                Stack.Clear();

                foreach (var value in insertedValues) result.Remove(value);
                if (result.Any()) AddToStack(result);
            }
        }
    }
}