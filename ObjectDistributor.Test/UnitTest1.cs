using System.Collections.Generic;
using ObjectDistributor.Core;
using ObjectDistributor.Test.Infrastructure.Consumer;
using ObjectDistributor.Test.Infrastructure.Helper;
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

        private const int Amount = 2;

        private void PrintTestResults(IEnumerable<TestModel> results)
        {
            foreach (var testModel in results)
                _output.WriteLine($"ResultOutput: Model Id[{testModel.Id}] Name[{testModel.Name}]");
        }

        [Fact]
        public void WorkId1And2Test()
        {
            var helper = new UnitTestHelper(Amount, 2, 3);
            var valuesS = helper.PrepareData<string>();
            var valuesI = helper.PrepareData<int>();

            var distributor = new Distributor();
            distributor.AddWorkerCell(new IntToEntityWorker());
            distributor.AddWorkerCell(new StringToEntityWorker());
            distributor.AddWorkerCell(new EntityToEntityWorker());
            distributor.AddWorkerCell(new EntityToModelWorker());

            distributor.Start();

            distributor.AddData(valuesS, new ModelConsumer(helper.SetResult), 1);
            distributor.AddData(valuesS, new ModelConsumer(helper.SetResult), 1);
            distributor.AddData(valuesS, new ModelConsumer(helper.SetResult), 1);

            distributor.AddData(valuesI, new ModelConsumer(helper.SetResult), 2);
            distributor.AddData(valuesI, new ModelConsumer(helper.SetResult), 2);
            distributor.AddData(valuesI, new ModelConsumer(helper.SetResult), 2);

            helper.AwaitTest();
            distributor.Stop();

            PrintTestResults(helper.Results);

            Assert.Equal(helper.ResultAmount(), helper.Results.Count);
        }

        [Fact]
        public void WorkId1BaseTest()
        {
            var helper = new UnitTestHelper(Amount, 1, 3);
            var values = helper.PrepareData<string>();

            var distributor = new Distributor();
            distributor.AddWorkerCell(new StringToEntityWorker());
            distributor.AddWorkerCell(new EntityToEntityWorker());
            distributor.AddWorkerCell(new EntityToModelWorker());

            distributor.Start();

            distributor.AddData(values, new ModelConsumer(helper.SetResult), 1);
            distributor.AddData(values, new ModelConsumer(helper.SetResult), 1);
            distributor.AddData(values, new ModelConsumer(helper.SetResult), 1);

            helper.AwaitTest();
            distributor.Stop();

            PrintTestResults(helper.Results);

            Assert.Equal(helper.ResultAmount(), helper.Results.Count);
        }

        [Fact]
        public void WorkId2BaseTest()
        {
            var helper = new UnitTestHelper(Amount, 1, 3);
            var values = helper.PrepareData<int>();

            var distributor = new Distributor();
            distributor.AddWorkerCell(new IntToEntityWorker());
            distributor.AddWorkerCell(new StringToEntityWorker());
            distributor.AddWorkerCell(new EntityToEntityWorker());
            distributor.AddWorkerCell(new EntityToModelWorker());

            distributor.Start();

            distributor.AddData(values, new ModelConsumer(helper.SetResult), 2);
            distributor.AddData(values, new ModelConsumer(helper.SetResult), 2);
            distributor.AddData(values, new ModelConsumer(helper.SetResult), 2);

            helper.AwaitTest();
            distributor.Stop();

            PrintTestResults(helper.Results);

            Assert.Equal(helper.ResultAmount(), helper.Results.Count);
        }
    }
}