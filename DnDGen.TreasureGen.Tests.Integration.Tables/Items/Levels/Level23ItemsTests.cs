﻿using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level23ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 23); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Medium, AmountConstants.Range2d8, 1, 23)] //2-16
        [TestCase(PowerConstants.Major, AmountConstants.Range2d6, 24, 76)] //2-12
        [TestCase(PowerConstants.Major, AmountConstants.Range1d3Plus4, 77, 100)] //5-7
        public override void TypeAndAmountPercentile(string type, string amount, int lower, int upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}