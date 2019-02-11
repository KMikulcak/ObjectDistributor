using System.Collections.Generic;
using ObjectDistributor.Core;
using ObjectDistributor.Test.Infrastructure.Consumer;
using ObjectDistributor.Test.Infrastructure.Model;
using ObjectDistributor.Test.Infrastructure.Worker;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDistributor.Test
{
    public class UnitTest1
    {
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        private IList<TestModel> _results;
        private bool _done;
        private int _counter;
        private const int Amount = 2;

        private void SetResult(IList<TestModel> resultList)
        {
            foreach (var testModel in resultList) _results.Add(testModel);
            _counter++;
            if (_counter == 3) _done = true;
        }

        private List<string> PrepareTest()
        {
            _done = false;
            _results = new List<TestModel>();

            var strings = new List<string>();
            for (var i = 0; i < Amount; i++) strings.Add($"{i}.NameS{i}");

            return strings;
        }

        private void AwaitTest()
        {
            while (!_done)
            {
            }
        }

        private void PrintTestResults()
        {
            foreach (var testModel in _results)
                _output.WriteLine($"ResultOutput: Model Id[{testModel.Id}] Name[{testModel.Name}]");
        }

        [Fact]
        public void BaseTest()
        {
            var strings = PrepareTest();

            var distributor = new Distributor();
            distributor.AddWorkerCell(new StringToEntityWorker());
            distributor.AddWorkerCell(new EntityToEntityWorker());
            distributor.AddWorkerCell(new EntityToModelWorker());

            distributor.Start();

            distributor.AddData(strings, new ModelConsumer(SetResult), 1);
            distributor.AddData(strings, new ModelConsumer(SetResult), 1);
            distributor.AddData(strings, new ModelConsumer(SetResult), 1);

            AwaitTest();
            distributor.Stop();

            PrintTestResults();

            Assert.Equal(Amount * _counter, _results.Count);
        }
    }
}