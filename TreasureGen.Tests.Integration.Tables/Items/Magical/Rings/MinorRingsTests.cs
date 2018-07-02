﻿using NUnit.Framework;
using TreasureGen.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Rings
{
    [TestFixture]
    public class MinorRingsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Minor, ItemTypeConstants.Ring); }
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

        [TestCase(RingConstants.Protection, 1, 1, 18)]
        [TestCase(RingConstants.FeatherFalling, 0, 19, 28)]
        [TestCase(RingConstants.Sustenance, 0, 29, 36)]
        [TestCase(RingConstants.Climbing, 0, 37, 44)]
        [TestCase(RingConstants.Jumping, 0, 45, 52)]
        [TestCase(RingConstants.Swimming, 0, 53, 60)]
        [TestCase(RingConstants.Counterspells, 0, 61, 70)]
        [TestCase(RingConstants.MindShielding, 0, 71, 75)]
        [TestCase(RingConstants.Protection, 2, 76, 80)]
        [TestCase(RingConstants.ForceShield, 0, 81, 85)]
        [TestCase(RingConstants.Ram, 0, 86, 90)]
        [TestCase(RingConstants.AnimalFriendship, 0, 91, 93)]
        [TestCase(RingConstants.ENERGYResistance_Minor, 0, 94, 96)]
        [TestCase(RingConstants.ChameleonPower, 0, 97, 98)]
        [TestCase(RingConstants.WaterWalking, 0, 99, 100)]
        public override void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}