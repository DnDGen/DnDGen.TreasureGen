using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    public abstract class TypeAndAmountPercentileTests : PercentileTests
    {
        public void AssertTypeAndAmountPercentile(string type, string amount, int lower, int upper)
        {
            var content = DataHelper.Parse(new TypeAndAmountDataSelection { Type = type, Roll = amount });
            AssertPercentile(content, lower, upper);
        }

        public void AssertTypeAndAmountPercentile(string type, string amount, int roll)
        {
            var content = DataHelper.Parse(new TypeAndAmountDataSelection { Type = type, Roll = amount });
            AssertPercentile(content, roll);
        }

        public void AssertTypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            var amountString = Convert.ToString(amount);
            AssertTypeAndAmountPercentile(type, amountString, lower, upper);
        }

        public void AssertTypeAndAmountPercentile(string type, int amount, int roll)
        {
            var amountString = Convert.ToString(amount);
            AssertTypeAndAmountPercentile(type, amountString, roll);
        }
    }
}