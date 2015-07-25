﻿using System;
using TreasureGen.Common.Items;
using TreasureGen.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Mundane.Armors
{
    [TestFixture]
    public class DarkwoodShieldsTests : PercentileTests
    {
        protected override String tableName
        {
            get { return TableNameConstants.Percentiles.Set.DarkwoodShields; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(ArmorConstants.Buckler, 1, 50)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 51, 100)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}