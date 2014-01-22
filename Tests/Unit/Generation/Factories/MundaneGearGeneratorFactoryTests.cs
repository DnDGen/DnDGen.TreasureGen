﻿using System;
using EquipmentGen.Core.Data.Items;
using EquipmentGen.Core.Generation.Factories;
using EquipmentGen.Core.Generation.Factories.Interfaces;
using EquipmentGen.Core.Generation.Generators;
using EquipmentGen.Core.Generation.Generators.Interfaces;
using EquipmentGen.Core.Generation.Providers.Interfaces;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generation.Factories
{
    [TestFixture]
    public class MundaneGearGeneratorFactoryTests
    {
        private IMundaneGearGeneratorFactory factory;

        [SetUp]
        public void Setup()
        {
            var mockPercentileResultProvider = new Mock<IPercentileResultProvider>();
            var mockAmmunitionGenerator = new Mock<IAmmunitionGenerator>();
            var mockMaterialsProvider = new Mock<IMaterialsProvider>();
            var mockGearTypeProvider = new Mock<IGearTypesProvider>();
            factory = new MundaneGearGeneratorFactory(mockPercentileResultProvider.Object, mockAmmunitionGenerator.Object,
                mockMaterialsProvider.Object, mockGearTypeProvider.Object);
        }

        [Test]
        public void MundaneGearGeneratorFactoryProducesArmorGenerator()
        {
            var generator = factory.CreateWith(ItemsConstants.ItemTypes.Armor);
            Assert.That(generator, Is.TypeOf<MundaneArmorGenerator>());
        }

        [Test]
        public void MundaneGearGeneratorFactoryProducesWeaponGenerator()
        {
            var generator = factory.CreateWith(ItemsConstants.ItemTypes.Weapon);
            Assert.That(generator, Is.TypeOf<MundaneWeaponGenerator>());
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidTypeThrowsError()
        {
            factory.CreateWith("invalid type");
        }
    }
}