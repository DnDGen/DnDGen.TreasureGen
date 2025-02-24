using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class SelectorsModuleTests : IoCTests
    {
        [Test]
        public void TreasurePercentileSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ITreasurePercentileSelector>();
        }

        [Test]
        public void TreasurePercentileSelectorHasReplacementDecorator()
        {
            var selector = GetNewInstanceOf<ITreasurePercentileSelector>();
            Assert.That(selector, Is.InstanceOf<PercentileSelectorStringReplacementDecorator>());
        }

        [Test]
        public void TreasurePercentileTypeAndAmountSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ITreasurePercentileTypeAndAmountSelector>();
        }

        [Test]
        public void TreasurePercentileTypeAndAmountSelectorHasReplacementDecorator()
        {
            var selector = GetNewInstanceOf<ITreasurePercentileTypeAndAmountSelector>();
            Assert.That(selector, Is.InstanceOf<PercentileTypeAndAmountSelectorStringReplacementDecorator>());
        }

        [Test]
        public void CollectionData_SpecialAbilityDataSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<SpecialAbilityDataSelection>>();
        }

        [Test]
        public void CollectionData_IntelligenceDataSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<IntelligenceDataSelection>>();
        }

        [Test]
        public void CollectionData_ArmorDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<ArmorDataSelection>>();
        }

        [Test]
        public void CollectionData_WeaponDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<WeaponDataSelection>>();
        }

        [Test]
        public void WeaponDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<IWeaponDataSelector>();
        }

        [Test]
        public void CollectionData_DamageDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<DamageDataSelection>>();
        }

        [Test]
        public void ReplacementSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<IReplacementSelector>();
        }

        [Test]
        public void CollectionData_SpecialAbilityDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<SpecialAbilityDataSelection>>();
        }

        [Test]
        public void CollectionData_IntelligenceDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<IntelligenceDataSelection>>();
        }
    }
}