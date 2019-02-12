using System;
using System.Collections.Generic;
using ObjectDistributor.Test.Infrastructure.Model;

namespace ObjectDistributor.Test.Infrastructure.Helper
{
    internal class UnitTestHelper
    {
        private int _amountConsumer;
        private int _amountData = 2;
        private int _counter;
        private bool _done;
        private int _factor;

        public UnitTestHelper(int amountData, int factor, int amountConsumer)
        {
            PrepareTest(amountData, factor, amountConsumer);
        }

        public IList<TestModel> Results { get; private set; }

        public int ResultAmount()
        {
            return _amountData * _factor * _amountConsumer;
        }

        public void SetResult(IList<TestModel> resultList)
        {
            foreach (var testModel in resultList)
            {
                Results.Add(testModel);
                _counter++;
            }

            if (_counter >= ResultAmount()) _done = true;
        }

        public List<TType> PrepareData<TType>()
        {
            var values = new List<TType>();

            if (typeof(TType) == typeof(string))
            {
                var li = new List<string>();
                for (var i = 0; i < _amountData; i++) li.Add($"{i}.NameS{i}");
                foreach (var s in li) values.Add((TType) Convert.ChangeType(s, typeof(TType)));
            }
            else if (typeof(TType) == typeof(int))
            {
                var li = new List<int>();
                for (var i = 0; i < _amountData; i++) li.Add(i);
                foreach (var s in li) values.Add((TType) Convert.ChangeType(s, typeof(TType)));
            }

            return values;
        }

        private void PrepareTest(int amountData, int factor, int amountConsumer)
        {
            _amountData = amountData;
            _done = false;
            _counter = 0;
            Results = new List<TestModel>();
            _factor = factor;
            _amountConsumer = amountConsumer;
        }

        public void AwaitTest()
        {
            var date = DateTime.Now;
            while (!_done)
            {
            }
        }
    }
}