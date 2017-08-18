﻿using Ninject.Modules;
using TreasureGen.Domain.Selectors.Collections;
using TreasureGen.Domain.Selectors.Percentiles;

namespace TreasureGen.Domain.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IIntelligenceDataSelector>().To<IntelligenceDataSelector>();
            Bind<IRangeDataSelector>().To<RangeDataSelector>();
            Bind<IArmorDataSelector>().To<ArmorDataSelector>();
            Bind<IWeaponDataSelector>().To<WeaponDataSelector>();
            Bind<ITreasurePercentileSelector>().To<PercentileSelectorStringReplacementDecorator>();

            Bind<ITypeAndAmountPercentileSelector>().To<TypeAndAmountPercentileSelector>().WhenInjectedInto<TypeAndAmountPercentileSelectorEventDecorator>();
            Bind<ITypeAndAmountPercentileSelector>().To<TypeAndAmountPercentileSelectorEventDecorator>();

            Bind<ISpecialAbilityDataSelector>().To<SpecialAbilityDataSelector>().WhenInjectedInto<SpecialAbilityDataSelectorEventDecorator>();
            Bind<ISpecialAbilityDataSelector>().To<SpecialAbilityDataSelectorEventDecorator>();
        }
    }
}