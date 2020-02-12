﻿using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumSpecificShieldsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, PowerConstants.Medium, AttributeConstants.Shield); }
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

        [TestCase(ArmorConstants.Buckler, 0, 1, 20)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 0, 21, 45)]
        [TestCase(ArmorConstants.HeavySteelShield, 0, 46, 70)]
        [TestCase(ArmorConstants.CastersShield, 1, 71, 85)]
        [TestCase(ArmorConstants.SpinedShield, 1, 86, 90)]
        [TestCase(ArmorConstants.LionsShield, 2, 91, 95)]
        [TestCase(ArmorConstants.WingedShield, 3, 96, 100)]
        public override void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}