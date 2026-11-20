using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Generators;
using DnDGen.TreasureGen.Generators.Coins;
using DnDGen.TreasureGen.Generators.Goods;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Generators.Items.Mundane;
using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.IoC.Extensions;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using Ninject.Modules;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.IoC.Modules
{
    internal class GeneratorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITreasureGenerator>().To<TreasureGenerator>();
            Bind<ICoinGenerator>().To<CoinGenerator>();
            Bind<IGoodsGenerator>().To<GoodsGenerator>();
            Bind<IItemsGenerator>().To<ItemsGenerator>();
            Bind<ICurseGenerator>().To<CurseGenerator>();
            Bind<ISpecificGearGenerator>().To<SpecificGearGenerator>();
            Bind<ISpecialAbilitiesGenerator>().To<SpecialAbilitiesGenerator>();
            Bind<IChargesGenerator>().To<ChargesGenerator>();
            Bind<IIntelligenceGenerator>().To<IntelligenceGenerator>();
            Bind<IMagicalItemTraitsGenerator>().To<MagicalItemTraitsGenerator>();
            Bind<ISpecialMaterialGenerator>().To<SpecialMaterialGenerator>();
            Bind<ISpellGenerator>().To<SpellGenerator>();

            var decorators = new[]
            {
                typeof(MundaneItemGeneratorSpecialMaterialDecorator),
            };

            NameAndDecorate<MundaneItemGenerator, ToolGenerator>(ItemTypeConstants.Tool, decorators);
            NameAndDecorate<MundaneItemGenerator, AlchemicalItemGenerator>(ItemTypeConstants.AlchemicalItem, decorators);
            NameAndDecorate<MundaneItemGenerator, MundaneArmorGenerator>(ItemTypeConstants.Armor, decorators);
            NameAndDecorate<MundaneItemGenerator, MundaneWeaponGenerator>(ItemTypeConstants.Weapon, decorators);

            decorators =
            [
                typeof(MagicalItemGeneratorMundaneProxy),
                typeof(MagicalItemGeneratorSpecialMaterialDecorator),
                typeof(MagicalItemGeneratorTraitsDecorator),
                typeof(MagicalItemGeneratorIntelligenceDecorator),
                typeof(MagicalItemGeneratorCurseDecorator),
            ];

            NameAndDecorate<MagicalItemGenerator, MagicalArmorGenerator>(ItemTypeConstants.Armor, decorators);
            NameAndDecorate<MagicalItemGenerator, PotionGenerator>(ItemTypeConstants.Potion, decorators);
            NameAndDecorate<MagicalItemGenerator, RingGenerator>(ItemTypeConstants.Ring, decorators);
            NameAndDecorate<MagicalItemGenerator, RodGenerator>(ItemTypeConstants.Rod, decorators);
            NameAndDecorate<MagicalItemGenerator, ScrollGenerator>(ItemTypeConstants.Scroll, decorators);
            NameAndDecorate<MagicalItemGenerator, StaffGenerator>(ItemTypeConstants.Staff, decorators);
            NameAndDecorate<MagicalItemGenerator, WandGenerator>(ItemTypeConstants.Wand, decorators);
            NameAndDecorate<MagicalItemGenerator, MagicalWeaponGenerator>(ItemTypeConstants.Weapon, decorators);
            NameAndDecorate<MagicalItemGenerator, WondrousItemGenerator>(ItemTypeConstants.WondrousItem, decorators);
        }

        private void NameAndDecorate<I, T>(string name, params Type[] decorators)
            where T : I
        {
            Bind<I>().To(decorators[0]).Named(name);

            for (var i = 1; i < decorators.Length; i++)
            {
                Bind<I>().To(decorators[i]).WhenInjectedInto(decorators[i - 1], name);
            }

            Bind<I>().To<T>().WhenInjectedInto(decorators.Last(), name);
        }
    }
}