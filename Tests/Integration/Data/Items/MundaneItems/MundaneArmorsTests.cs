﻿using System;
using EquipmentGen.Common.Items;
using EquipmentGen.Tests.Integration.Tables.TestAttributes;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Items.MundaneItems
{
    [TestFixture, PercentileTable("MundaneArmors")]
    public class MundaneArmorsTests : PercentileTests
    {
        [TestCase(ArmorConstants.ChainShirt, 1, 12)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, 13, 18)]
        [TestCase(ArmorConstants.Breastplate, 19, 26)]
        [TestCase(ArmorConstants.BandedMail, 27, 34)]
        [TestCase(ArmorConstants.HalfPlate, 35, 54)]
        [TestCase(ArmorConstants.FullPlate, 55, 80)]
        [TestCase(TraitConstants.Darkwood, 81, 90)]
        [TestCase(TraitConstants.Masterwork, 91, 100)]
        public void Percentile(String content, Int32 lower, Int32 upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}