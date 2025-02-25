using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Stress;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DnDGen.TreasureGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        protected Stressor stressor;
        protected ICollectionSelector collectionSelector;

        private string[] powers;
        private IEnumerable<int> standardLevels;
        private IEnumerable<int> epicLevels;
        private IEnumerable<int> excessLevels;

        [OneTimeSetUp]
        public void StressOneTimeSetup()
        {
            var options = new StressorOptions();
            options.RunningAssembly = Assembly.GetExecutingAssembly();

            //INFO: Non-stress operations take about 5 minutes, or 9% of the total runtime
            options.TimeLimitPercentage = .91;

#if STRESS
            options.IsFullStress = true;
#else
            options.IsFullStress = false;
#endif

            stressor = new Stressor(options);
            powers = [PowerConstants.Minor, PowerConstants.Medium, PowerConstants.Major];
            standardLevels = Enumerable.Range(LevelLimits.Minimum, LevelLimits.Maximum_Standard);
            epicLevels = Enumerable.Range(LevelLimits.Maximum_Standard + 1, 10);
            excessLevels = Enumerable.Range(LevelLimits.Maximum_Epic + 1, 10);

            Assert.That(standardLevels.First(), Is.EqualTo(LevelLimits.Minimum));
            Assert.That(standardLevels.Last(), Is.EqualTo(LevelLimits.Maximum_Standard));
            Assert.That(epicLevels.First(), Is.EqualTo(LevelLimits.Maximum_Standard + 1));
            Assert.That(epicLevels.Last(), Is.EqualTo(LevelLimits.Maximum_Epic));
            Assert.That(excessLevels.First(), Is.EqualTo(LevelLimits.Maximum_Epic + 1));
            Assert.That(excessLevels.Last(), Is.EqualTo(LevelLimits.Maximum_Epic + 10));
        }

        [SetUp]
        public void StressSetup()
        {
            collectionSelector = GetNewInstanceOf<ICollectionSelector>();
        }

        protected int GetNewLevel()
        {
            return collectionSelector.SelectRandomFrom(standardLevels, epicLevels, excessLevels);
        }

        protected string GetNewPower(bool allowMinor = true)
        {
            if (allowMinor)
                return collectionSelector.SelectRandomFrom(powers);

            return collectionSelector.SelectRandomFrom(powers.Except([PowerConstants.Minor]));
        }
    }
}