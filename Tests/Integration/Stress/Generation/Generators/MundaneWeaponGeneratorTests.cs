﻿using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentGen.Core.Data.Items.Constants;
using EquipmentGen.Core.Generation.Factories.Interfaces;
using EquipmentGen.Core.Generation.Generators.Interfaces;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Generation.Generators
{
    [TestFixture]
    public class MundaneWeaponGeneratorTests : StressTests
    {
        [Inject]
        public IMundaneGearGeneratorFactory mundaneGearGeneratorFactory { get; set; }

        private IMundaneGearGenerator mundaneWeaponGenerator;

        private IEnumerable<String> commonality;
        private IEnumerable<String> range;

        [SetUp]
        public void Setup()
        {
            mundaneWeaponGenerator = mundaneGearGeneratorFactory.CreateWith(ItemTypeConstants.Weapon);
            commonality = new[] { AttributeConstants.Common, AttributeConstants.Uncommon };
            range = new[] { AttributeConstants.Melee, AttributeConstants.Ranged };

            StartTest();
        }

        [TearDown]
        public void TearDown()
        {
            StopTest();
        }

        [Test]
        public void StressedMundaneWeaponGenerator()
        {
            while (TestShouldKeepRunning())
            {
                var weapon = mundaneWeaponGenerator.Generate();

                Assert.That(weapon.Name, Is.Not.Empty);
                Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
                Assert.That(weapon.Attributes, Contains.Item(ItemTypeConstants.Weapon));
                Assert.That(weapon.Quantity, Is.GreaterThan(0));
                Assert.That(weapon.Magic, Is.Empty);

                var intersection = commonality.Intersect(weapon.Attributes);
                Assert.That(intersection, Is.Not.Empty, "Commonality");

                intersection = range.Intersect(weapon.Attributes);
                Assert.That(intersection, Is.Not.Empty, "Range");
            }

            AssertIterations();
        }
    }
}