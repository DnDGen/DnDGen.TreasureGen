using NUnit.Framework;
using System;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class IsDeckOFIllusionsFullyChargedTests : BooleanPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.IsDeckOfIllusionsFullyCharged; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase(true, 1, 90)]
        [TestCase(false, 91, 100)]
        public override void BooleanPercentile(Boolean isTrue, int lower, int upper)
        {
            base.BooleanPercentile(isTrue, lower, upper);
        }
    }
}