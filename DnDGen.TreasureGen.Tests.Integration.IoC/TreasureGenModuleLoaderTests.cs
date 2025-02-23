using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators;
using DnDGen.TreasureGen.IoC;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Integration.IoC
{
    [TestFixture]
    public class TreasureGenModuleLoaderTests : IoCTests
    {
        [Test]
        public void ModuleLoaderCanBeRunTwice()
        {
            //INFO: First time was in the IntegrationTest one-time setup
            var treasureGenLoader = new TreasureGenModuleLoader();
            treasureGenLoader.LoadModules(kernel);

            var treasureGenerator = GetNewInstanceOf<ITreasureGenerator>();
            Assert.That(treasureGenerator, Is.Not.Null);
        }

        [Test]
        public void ModuleLoaderLoadsRollGenDependency()
        {
            AssertNotSingleton<Dice>();
            AssertSingleton<Random>();
        }

        [Test]
        public void ModuleLoaderLoadsInfrastructureDependency()
        {
            AssertNotSingleton<JustInTimeFactory>();
            AssertNotSingleton<IPercentileSelector>();
            AssertNotSingleton<ICollectionSelector>();
        }
    }
}
