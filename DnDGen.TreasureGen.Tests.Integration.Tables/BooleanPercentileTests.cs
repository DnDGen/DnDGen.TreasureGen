using System;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    public abstract class BooleanPercentileTests : PercentileTests
    {
        public virtual void BooleanPercentile(bool isTrue, int lower, int upper)
        {
            var content = Convert.ToString(isTrue);
            AssertPercentile(content, lower, upper);
        }

        public virtual void BooleanPercentile(bool isTrue, int roll)
        {
            var content = Convert.ToString(isTrue);
            AssertPercentile(content, roll);
        }
    }
}