using System.Collections.Generic;
using ObjectDistributor.Core.Worker;

namespace ObjectDistributor.Out.Implementation
{
    public class OutputWorker : WorkerCell<char[], string>
    {
        public OutputWorker() : base(1, new[] {1})
        {
        }

        protected override List<string> ProcessThis(IList<char[]> valueObjects)
        {
            var dic = new Dictionary<char, int>();
            foreach (var valueObject in valueObjects)
            foreach (var c in valueObject)
                if (dic.ContainsKey(c))
                    dic[c] = dic[c] + 1;
                else
                    dic.Add(c, +1);

            var results = new List<string>();

            foreach (var entry in dic) results.Add($"Char[{entry.Key}] was generated {entry.Value} times");

            return results;
        }
    }
}