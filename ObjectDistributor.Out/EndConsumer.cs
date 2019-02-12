using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDistributor.Out
{
    internal class EndConsumer
    {
        private const int StackSize = 250;
        private static readonly List<string> Stack = new List<string>();

        public static void AddToStack(IList<string> result)
        {
            var results = result;

            lock (Stack)
            {
                var size = StackSize - Stack.Count;
                var it = results.Count;
                var insertedValues = new List<string>();

                if (size <= it) it = size;

                for (var i = 0; i < it; i++)
                {
                    Stack.Add(results[i]);
                    insertedValues.Add(results[i]);
                }

                if (!Stack.Count.Equals(StackSize)) return;
                var values = new List<string>();
                var counter = 0;

                Console.WriteLine($"Results finished at: {DateTime.Now}");
                foreach (var res in Stack)
                {
                    values.Add(res);
                    counter++;
                    if (counter != 50) continue;
                    foreach (var val in values) Console.WriteLine(val);
                    values.Clear();
                    counter = 0;
                }

                Console.WriteLine("##################################");

                Stack.Clear();

                foreach (var value in insertedValues) results.Remove(value);
                if (results.Any()) AddToStack(results);
            }
        }
    }
}