﻿using DnDGen.EventGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using Ninject;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class RodGeneratorTests : IntegrationTests
    {
        [Inject, Named(ItemTypeConstants.Rod)]
        public MagicalItemGenerator RodGenerator { get; set; }
        [Inject]
        public ClientIDManager ClientIDManager { get; set; }

        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            ClientIDManager.SetClientID(Guid.NewGuid());
        }

        [TestCase(RodConstants.Absorption, PowerConstants.Major)]
        [TestCase(RodConstants.Absorption_Full, PowerConstants.Major, Ignore = "This is really only an internal construct")]
        [TestCase(RodConstants.Alertness, PowerConstants.Major)]
        [TestCase(RodConstants.Cancellation, PowerConstants.Medium)]
        [TestCase(RodConstants.Cancellation, PowerConstants.Major)]
        [TestCase(RodConstants.EnemyDetection, PowerConstants.Major)]
        [TestCase(RodConstants.Flailing, PowerConstants.Major)]
        [TestCase(RodConstants.FlameExtinguishing, PowerConstants.Medium)]
        [TestCase(RodConstants.FlameExtinguishing, PowerConstants.Major)]
        [TestCase(RodConstants.ImmovableRod, PowerConstants.Medium)]
        [TestCase(RodConstants.LordlyMight, PowerConstants.Major)]
        [TestCase(RodConstants.MetalAndMineralDetection, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Empower, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Empower, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Empower_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Empower_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Enlarge, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Enlarge, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Enlarge_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Enlarge_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Extend, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Extend, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Extend_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Extend_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Maximize, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Maximize, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Maximize_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Maximize_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Quicken, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Quicken, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Quicken_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Quicken_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Silent, PowerConstants.Medium)]
        [TestCase(RodConstants.Metamagic_Silent, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Silent_Greater, PowerConstants.Major)]
        [TestCase(RodConstants.Metamagic_Silent_Lesser, PowerConstants.Medium)]
        [TestCase(RodConstants.Negation, PowerConstants.Major)]
        [TestCase(RodConstants.Python, PowerConstants.Medium)]
        [TestCase(RodConstants.Python, PowerConstants.Major)]
        [TestCase(RodConstants.Rulership, PowerConstants.Major)]
        [TestCase(RodConstants.Security, PowerConstants.Major)]
        [TestCase(RodConstants.Splendor, PowerConstants.Major)]
        [TestCase(RodConstants.ThunderAndLightning, PowerConstants.Major)]
        [TestCase(RodConstants.Viper, PowerConstants.Medium)]
        [TestCase(RodConstants.Viper, PowerConstants.Major)]
        [TestCase(RodConstants.Withering, PowerConstants.Major)]
        [TestCase(RodConstants.Wonder, PowerConstants.Medium)]
        [TestCase(RodConstants.Wonder, PowerConstants.Major)]
        public void GenerateRod(string itemName, string power)
        {
            var isOfPower = RodGenerator.IsItemOfPower(itemName, power);
            Assert.That(isOfPower, Is.True);

            var item = RodGenerator.GenerateFrom(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [Test]
        public void AllRodsCanBeGenerated()
        {
            var rods = RodConstants.GetAllRods();

            foreach (var rod in rods)
            {
                var isMedium = RodGenerator.IsItemOfPower(rod, PowerConstants.Medium);
                var isMajor = RodGenerator.IsItemOfPower(rod, PowerConstants.Major);

                Assert.That(true, Is.EqualTo(isMedium)
                    .Or.EqualTo(isMajor), rod);
            }
        }
    }
}
