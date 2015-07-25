﻿using System;
using TreasureGen.Common.Items;
using TreasureGen.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Rods
{
    [TestFixture]
    public class MediumRodsTests : TypeAndAmountPercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod); }
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

        [TestCase(RodConstants.Metamagic_Enlarge_Lesser, 0, 1, 7)]
        [TestCase(RodConstants.Metamagic_Extend_Lesser, 0, 8, 14)]
        [TestCase(RodConstants.Metamagic_Silent_Lesser, 0, 15, 21)]
        [TestCase(RodConstants.ImmovableRod, 0, 22, 28)]
        [TestCase(RodConstants.Metamagic_Empower_Lesser, 0, 29, 35)]
        [TestCase(RodConstants.MetalAndMineralDetection, 0, 36, 42)]
        [TestCase(RodConstants.Cancellation, 0, 43, 53)]
        [TestCase(RodConstants.Metamagic_Enlarge, 0, 54, 57)]
        [TestCase(RodConstants.Metamagic_Extend, 0, 58, 61)]
        [TestCase(RodConstants.Metamagic_Silent, 0, 62, 65)]
        [TestCase(RodConstants.Wonder, 0, 66, 71)]
        [TestCase(RodConstants.Python, 1, 72, 79)]
        [TestCase(RodConstants.Metamagic_Maximize_Lesser, 0, 80, 83)]
        [TestCase(RodConstants.FlameExtinguishing, 0, 84, 89)]
        [TestCase(RodConstants.Viper, 2, 90, 97)]
        [TestCase(RodConstants.Metamagic_Empower, 0, 98, 99)]
        public override void TypeAndAmountPercentile(String type, Int32 amount, Int32 lower, Int32 upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }

        [TestCase(RodConstants.Metamagic_Quicken_Lesser, 0, 100)]
        public override void TypeAndAmountPercentile(String type, Int32 amount, Int32 roll)
        {
            base.TypeAndAmountPercentile(type, amount, roll);
        }
    }
}