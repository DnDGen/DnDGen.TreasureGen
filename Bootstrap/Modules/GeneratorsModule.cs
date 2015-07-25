﻿using Ninject;
using Ninject.Modules;
using TreasureGen.Common.Items;
using TreasureGen.Generators;
using TreasureGen.Generators.Coins;
using TreasureGen.Generators.Domain;
using TreasureGen.Generators.Domain.Coins;
using TreasureGen.Generators.Domain.Goods;
using TreasureGen.Generators.Domain.Items;
using TreasureGen.Generators.Domain.Items.Magical;
using TreasureGen.Generators.Domain.Items.Mundane;
using TreasureGen.Generators.Domain.RuntimeFactories;
using TreasureGen.Generators.Domain.RuntimeFactories.Domain;
using TreasureGen.Generators.Goods;
using TreasureGen.Generators.Items;
using TreasureGen.Generators.Items.Magical;
using TreasureGen.Generators.Items.Mundane;

namespace TreasureGen.Bootstrap.Modules
{
    public class GeneratorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAmmunitionGenerator>().To<AmmunitionGenerator>();
            Bind<IChargesGenerator>().To<ChargesGenerator>();
            Bind<ICoinGenerator>().To<CoinGenerator>();
            Bind<ICurseGenerator>().To<CurseGenerator>();
            Bind<IGoodsGenerator>().To<GoodsGenerator>();
            Bind<IIntelligenceGenerator>().To<IntelligenceGenerator>();
            Bind<IItemsGenerator>().To<ItemsGenerator>();
            Bind<IMagicalItemGeneratorFactory>().To<MagicalItemGeneratorFactory>();
            Bind<IMagicalItemTraitsGenerator>().To<MagicalItemTraitsGenerator>();
            Bind<IMundaneItemGeneratorFactory>().To<MundaneItemGeneratorFactory>();
            Bind<ISpecialAbilitiesGenerator>().To<SpecialAbilitiesGenerator>();
            Bind<ISpecialMaterialGenerator>().To<SpecialMaterialGenerator>();
            Bind<ISpecificGearGenerator>().To<SpecificGearGenerator>();
            Bind<ISpellGenerator>().To<SpellGenerator>();
            Bind<ITreasureGenerator>().To<TreasureGenerator>();

            Bind<IMundaneItemGenerator>().ToMethod(c => c.Kernel.Get<IMundaneItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Tool)).Named(ItemTypeConstants.Tool);
            Bind<IMundaneItemGenerator>().ToMethod(c => c.Kernel.Get<IMundaneItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.AlchemicalItem)).Named(ItemTypeConstants.AlchemicalItem);
            Bind<IMundaneItemGenerator>().ToMethod(c => c.Kernel.Get<IMundaneItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Armor)).Named(ItemTypeConstants.Armor);
            Bind<IMundaneItemGenerator>().ToMethod(c => c.Kernel.Get<IMundaneItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Weapon)).Named(ItemTypeConstants.Weapon);

            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Armor)).Named(ItemTypeConstants.Armor);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Weapon)).Named(ItemTypeConstants.Weapon);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Potion)).Named(ItemTypeConstants.Potion);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Ring)).Named(ItemTypeConstants.Ring);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Rod)).Named(ItemTypeConstants.Rod);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Scroll)).Named(ItemTypeConstants.Scroll);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Staff)).Named(ItemTypeConstants.Staff);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.Wand)).Named(ItemTypeConstants.Wand);
            Bind<IMagicalItemGenerator>().ToMethod(c => c.Kernel.Get<IMagicalItemGeneratorFactory>().CreateGeneratorOf(ItemTypeConstants.WondrousItem)).Named(ItemTypeConstants.WondrousItem);
        }
    }
}