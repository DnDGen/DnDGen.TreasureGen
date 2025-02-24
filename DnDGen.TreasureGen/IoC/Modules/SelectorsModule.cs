using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using Ninject.Modules;

namespace DnDGen.TreasureGen.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITreasurePercentileSelector>().To<PercentileSelectorStringReplacementDecorator>();
            Bind<ITreasurePercentileTypeAndAmountSelector>().To<PercentileTypeAndAmountSelectorStringReplacementDecorator>();
            Bind<IReplacementSelector>().To<ReplacementSelector>();
            Bind<IWeaponDataSelector>().To<WeaponDataSelector>();

            Kernel.BindDataSelection<ArmorDataSelection>();
            Kernel.BindDataSelection<WeaponDataSelection>();
            Kernel.BindDataSelection<DamageDataSelection>();
            Kernel.BindDataSelection<SpecialAbilityDataSelection>();
            Kernel.BindDataSelection<IntelligenceDataSelection>();
        }
    }
}