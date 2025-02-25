using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items
{
    [TestFixture]
    public class ItemGroupsTests : CollectionsTests
    {
        protected override string tableName
        {
            get
            {
                return TableNameConstants.Collections.ItemGroups;
            }
        }

        [TestCase(ArmorConstants.AbsorbingShield, ArmorConstants.HeavySteelShield)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.ArmorOfRage, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.BandedMail, ArmorConstants.BandedMail)]
        [TestCase(ArmorConstants.BandedMailOfLuck, ArmorConstants.BandedMail)]
        [TestCase(ArmorConstants.Breastplate, ArmorConstants.Breastplate)]
        [TestCase(ArmorConstants.BreastplateOfCommand, ArmorConstants.Breastplate)]
        [TestCase(ArmorConstants.Buckler, ArmorConstants.Buckler)]
        [TestCase(ArmorConstants.CastersShield, ArmorConstants.LightWoodenShield)]
        [TestCase(ArmorConstants.CelestialArmor, ArmorConstants.Chainmail)]
        [TestCase(ArmorConstants.Chainmail, ArmorConstants.Chainmail)]
        [TestCase(ArmorConstants.ChainShirt, ArmorConstants.ChainShirt)]
        [TestCase(ArmorConstants.DemonArmor, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.DwarvenPlate, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.ElvenChain, ArmorConstants.Chainmail)]
        [TestCase(ArmorConstants.FullPlate, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.FullPlateOfSpeed, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.HalfPlate, ArmorConstants.HalfPlate)]
        [TestCase(ArmorConstants.HeavySteelShield, ArmorConstants.HeavySteelShield)]
        [TestCase(ArmorConstants.HeavyWoodenShield, ArmorConstants.HeavyWoodenShield)]
        [TestCase(ArmorConstants.HideArmor, ArmorConstants.HideArmor)]
        [TestCase(ArmorConstants.LeatherArmor, ArmorConstants.LeatherArmor)]
        [TestCase(ArmorConstants.LightSteelShield, ArmorConstants.LightSteelShield)]
        [TestCase(ArmorConstants.LightWoodenShield, ArmorConstants.LightWoodenShield)]
        [TestCase(ArmorConstants.LionsShield, ArmorConstants.HeavySteelShield)]
        [TestCase(ArmorConstants.PaddedArmor, ArmorConstants.PaddedArmor)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep, ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.RhinoHide, ArmorConstants.HideArmor)]
        [TestCase(ArmorConstants.ScaleMail, ArmorConstants.ScaleMail)]
        [TestCase(ArmorConstants.SpinedShield, ArmorConstants.HeavySteelShield)]
        [TestCase(ArmorConstants.SplintMail, ArmorConstants.SplintMail)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, ArmorConstants.StuddedLeatherArmor)]
        [TestCase(ArmorConstants.TowerShield, ArmorConstants.TowerShield)]
        [TestCase(ArmorConstants.WingedShield, ArmorConstants.HeavyWoodenShield)]
        [TestCase(PotionConstants.Poison, PotionConstants.Poison)]
        [TestCase(RingConstants.Clumsiness, RingConstants.Clumsiness)]
        [TestCase(RodConstants.Absorption, RodConstants.Absorption)]
        [TestCase(RodConstants.Alertness, RodConstants.Alertness, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Cancellation, RodConstants.Cancellation)]
        [TestCase(RodConstants.EnemyDetection, RodConstants.EnemyDetection)]
        [TestCase(RodConstants.Flailing, RodConstants.Flailing, WeaponConstants.DireFlail)]
        [TestCase(RodConstants.FlameExtinguishing, RodConstants.FlameExtinguishing)]
        [TestCase(RodConstants.Absorption_Full, RodConstants.Absorption)]
        [TestCase(RodConstants.ImmovableRod, RodConstants.ImmovableRod)]
        [TestCase(RodConstants.LordlyMight, RodConstants.LordlyMight, WeaponConstants.LightMace)]
        [TestCase(RodConstants.MetalAndMineralDetection, RodConstants.MetalAndMineralDetection)]
        [TestCase(RodConstants.Metamagic_Empower, RodConstants.Metamagic_Empower)]
        [TestCase(RodConstants.Metamagic_Empower_Greater, RodConstants.Metamagic_Empower, RodConstants.Metamagic_Empower_Greater)]
        [TestCase(RodConstants.Metamagic_Empower_Lesser, RodConstants.Metamagic_Empower, RodConstants.Metamagic_Empower_Lesser)]
        [TestCase(RodConstants.Metamagic_Enlarge, RodConstants.Metamagic_Enlarge)]
        [TestCase(RodConstants.Metamagic_Enlarge_Greater, RodConstants.Metamagic_Enlarge, RodConstants.Metamagic_Enlarge_Greater)]
        [TestCase(RodConstants.Metamagic_Enlarge_Lesser, RodConstants.Metamagic_Enlarge, RodConstants.Metamagic_Enlarge_Lesser)]
        [TestCase(RodConstants.Metamagic_Extend, RodConstants.Metamagic_Extend)]
        [TestCase(RodConstants.Metamagic_Extend_Greater, RodConstants.Metamagic_Extend, RodConstants.Metamagic_Extend_Greater)]
        [TestCase(RodConstants.Metamagic_Extend_Lesser, RodConstants.Metamagic_Extend, RodConstants.Metamagic_Extend_Lesser)]
        [TestCase(RodConstants.Metamagic_Maximize, RodConstants.Metamagic_Maximize)]
        [TestCase(RodConstants.Metamagic_Maximize_Greater, RodConstants.Metamagic_Maximize, RodConstants.Metamagic_Maximize_Greater)]
        [TestCase(RodConstants.Metamagic_Maximize_Lesser, RodConstants.Metamagic_Maximize, RodConstants.Metamagic_Maximize_Lesser)]
        [TestCase(RodConstants.Metamagic_Quicken, RodConstants.Metamagic_Quicken)]
        [TestCase(RodConstants.Metamagic_Quicken_Greater, RodConstants.Metamagic_Quicken, RodConstants.Metamagic_Quicken_Greater)]
        [TestCase(RodConstants.Metamagic_Quicken_Lesser, RodConstants.Metamagic_Quicken, RodConstants.Metamagic_Quicken_Lesser)]
        [TestCase(RodConstants.Metamagic_Silent, RodConstants.Metamagic_Silent)]
        [TestCase(RodConstants.Metamagic_Silent_Greater, RodConstants.Metamagic_Silent, RodConstants.Metamagic_Silent_Greater)]
        [TestCase(RodConstants.Metamagic_Silent_Lesser, RodConstants.Metamagic_Silent, RodConstants.Metamagic_Silent_Lesser)]
        [TestCase(RodConstants.Negation, RodConstants.Negation)]
        [TestCase(RodConstants.Python, RodConstants.Python, WeaponConstants.Quarterstaff)]
        [TestCase(RodConstants.Rulership, RodConstants.Rulership)]
        [TestCase(RodConstants.Security, RodConstants.Security)]
        [TestCase(RodConstants.Splendor, RodConstants.Splendor)]
        [TestCase(RodConstants.ThunderAndLightning, RodConstants.ThunderAndLightning, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Viper, RodConstants.Viper, WeaponConstants.HeavyMace)]
        [TestCase(RodConstants.Withering, RodConstants.Withering, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Wonder, RodConstants.Wonder)]
        [TestCase(StaffConstants.Abjuration, StaffConstants.Abjuration)]
        [TestCase(StaffConstants.Charming, StaffConstants.Charming)]
        [TestCase(StaffConstants.Conjuration, StaffConstants.Conjuration)]
        [TestCase(StaffConstants.Defense, StaffConstants.Defense)]
        [TestCase(StaffConstants.Divination, StaffConstants.Divination)]
        [TestCase(StaffConstants.EarthAndStone, StaffConstants.EarthAndStone)]
        [TestCase(StaffConstants.Enchantment, StaffConstants.Enchantment)]
        [TestCase(StaffConstants.Evocation, StaffConstants.Evocation)]
        [TestCase(StaffConstants.Fire, StaffConstants.Fire)]
        [TestCase(StaffConstants.Frost, StaffConstants.Frost)]
        [TestCase(StaffConstants.Healing, StaffConstants.Healing)]
        [TestCase(StaffConstants.Illumination, StaffConstants.Illumination)]
        [TestCase(StaffConstants.Illusion, StaffConstants.Illusion)]
        [TestCase(StaffConstants.Life, StaffConstants.Life)]
        [TestCase(StaffConstants.Necromancy, StaffConstants.Necromancy)]
        [TestCase(StaffConstants.Passage, StaffConstants.Passage)]
        [TestCase(StaffConstants.Power, StaffConstants.Power, WeaponConstants.Quarterstaff)]
        [TestCase(StaffConstants.SizeAlteration, StaffConstants.SizeAlteration)]
        [TestCase(StaffConstants.SwarmingInsects, StaffConstants.SwarmingInsects)]
        [TestCase(StaffConstants.Transmutation, StaffConstants.Transmutation)]
        [TestCase(StaffConstants.Woodlands, StaffConstants.Woodlands, WeaponConstants.Quarterstaff)]
        [TestCase(WeaponConstants.Arrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.AssassinsDagger, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.BastardSword, WeaponConstants.BastardSword)]
        [TestCase(WeaponConstants.Battleaxe, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.BerserkingSword, WeaponConstants.Greatsword)]
        [TestCase(WeaponConstants.Bolas, WeaponConstants.Bolas)]
        [TestCase(WeaponConstants.Club, WeaponConstants.Club)]
        [TestCase(WeaponConstants.CompositeLongbow, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CrossbowBolt, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, WeaponConstants.Shortspear)]
        [TestCase(WeaponConstants.CursedMinus2Sword, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.Dagger, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.DaggerOfVenom, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dart, WeaponConstants.Dart)]
        [TestCase(WeaponConstants.DireFlail, WeaponConstants.DireFlail)]
        [TestCase(WeaponConstants.DwarvenThrower, WeaponConstants.Warhammer)]
        [TestCase(WeaponConstants.DwarvenUrgrosh, WeaponConstants.DwarvenUrgrosh)]
        [TestCase(WeaponConstants.DwarvenWaraxe, WeaponConstants.DwarvenWaraxe)]
        [TestCase(WeaponConstants.Falchion, WeaponConstants.Falchion)]
        [TestCase(WeaponConstants.FlameTongue, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.FrostBrand, WeaponConstants.Greatsword)]
        [TestCase(WeaponConstants.Gauntlet, WeaponConstants.Gauntlet)]
        [TestCase(WeaponConstants.Glaive, WeaponConstants.Glaive)]
        [TestCase(WeaponConstants.GnomeHookedHammer, WeaponConstants.GnomeHookedHammer)]
        [TestCase(WeaponConstants.Greataxe, WeaponConstants.Greataxe)]
        [TestCase(WeaponConstants.Greatclub, WeaponConstants.Greatclub)]
        [TestCase(WeaponConstants.GreaterSlayingArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.Greatsword, WeaponConstants.Greatsword)]
        [TestCase(WeaponConstants.Guisarme, WeaponConstants.Guisarme)]
        [TestCase(WeaponConstants.Halberd, WeaponConstants.Halberd)]
        [TestCase(WeaponConstants.Spear, WeaponConstants.Spear)]
        [TestCase(WeaponConstants.Handaxe, WeaponConstants.Handaxe)]
        [TestCase(WeaponConstants.HandCrossbow, WeaponConstants.HandCrossbow)]
        [TestCase(WeaponConstants.HeavyCrossbow, WeaponConstants.HeavyCrossbow)]
        [TestCase(WeaponConstants.HeavyFlail, WeaponConstants.HeavyFlail)]
        [TestCase(WeaponConstants.HeavyMace, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.HeavyPick, WeaponConstants.HeavyPick)]
        [TestCase(WeaponConstants.HeavyRepeatingCrossbow, WeaponConstants.HeavyRepeatingCrossbow)]
        [TestCase(WeaponConstants.HolyAvenger, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.Javelin, WeaponConstants.Javelin)]
        [TestCase(WeaponConstants.JavelinOfLightning, WeaponConstants.Javelin)]
        [TestCase(WeaponConstants.Kama, WeaponConstants.Kama)]
        [TestCase(WeaponConstants.Kukri, WeaponConstants.Kukri)]
        [TestCase(WeaponConstants.Lance, WeaponConstants.Lance)]
        [TestCase(WeaponConstants.LifeDrinker, WeaponConstants.Greataxe)]
        [TestCase(WeaponConstants.LightCrossbow, WeaponConstants.LightCrossbow)]
        [TestCase(WeaponConstants.Flail, WeaponConstants.Flail)]
        [TestCase(WeaponConstants.LightHammer, WeaponConstants.LightHammer)]
        [TestCase(WeaponConstants.LightMace, WeaponConstants.LightMace)]
        [TestCase(WeaponConstants.LightPick, WeaponConstants.LightPick)]
        [TestCase(WeaponConstants.LightRepeatingCrossbow, WeaponConstants.LightRepeatingCrossbow)]
        [TestCase(WeaponConstants.Longbow, WeaponConstants.Longbow)]
        [TestCase(WeaponConstants.Longspear, WeaponConstants.Longspear)]
        [TestCase(WeaponConstants.Longsword, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.LuckBlade, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.MaceOfBlood, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.MaceOfSmiting, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.MaceOfTerror, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.Morningstar, WeaponConstants.Morningstar)]
        [TestCase(WeaponConstants.Net, WeaponConstants.Net)]
        [TestCase(WeaponConstants.NetOfSnaring, WeaponConstants.Net)]
        [TestCase(WeaponConstants.NineLivesStealer, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.Nunchaku, WeaponConstants.Nunchaku)]
        [TestCase(WeaponConstants.Oathbow, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.OrcDoubleAxe, WeaponConstants.OrcDoubleAxe)]
        [TestCase(WeaponConstants.PincerStaff, WeaponConstants.PincerStaff)]
        [TestCase(WeaponConstants.PunchingDagger, WeaponConstants.PunchingDagger)]
        [TestCase(WeaponConstants.Quarterstaff, WeaponConstants.Quarterstaff)]
        [TestCase(WeaponConstants.Ranseur, WeaponConstants.Ranseur)]
        [TestCase(WeaponConstants.Rapier, WeaponConstants.Rapier)]
        [TestCase(WeaponConstants.RapierOfPuncturing, WeaponConstants.Rapier)]
        [TestCase(WeaponConstants.Sai, WeaponConstants.Sai)]
        [TestCase(WeaponConstants.Sap, WeaponConstants.Sap)]
        [TestCase(WeaponConstants.Scimitar, WeaponConstants.Scimitar)]
        [TestCase(WeaponConstants.ScreamingBolt, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.Scythe, WeaponConstants.Scythe)]
        [TestCase(WeaponConstants.Shatterspike, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.ShiftersSorrow, WeaponConstants.TwoBladedSword)]
        [TestCase(WeaponConstants.Shortbow, WeaponConstants.Shortbow)]
        [TestCase(WeaponConstants.Shortspear, WeaponConstants.Shortspear)]
        [TestCase(WeaponConstants.ShortSword, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.Shuriken, WeaponConstants.Shuriken)]
        [TestCase(WeaponConstants.Siangham, WeaponConstants.Siangham)]
        [TestCase(WeaponConstants.Sickle, WeaponConstants.Sickle)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.SlayingArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.SleepArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.Sling, WeaponConstants.Sling)]
        [TestCase(WeaponConstants.SlingBullet, WeaponConstants.SlingBullet)]
        [TestCase(WeaponConstants.SpikedChain, WeaponConstants.SpikedChain)]
        [TestCase(WeaponConstants.SpikedGauntlet, WeaponConstants.SpikedGauntlet)]
        [TestCase(WeaponConstants.SunBlade, WeaponConstants.ShortSword, WeaponConstants.BastardSword)]
        [TestCase(WeaponConstants.SwordOfLifeStealing, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.SwordOfSubtlety, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.SwordOfThePlanes, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.SylvanScimitar, WeaponConstants.Scimitar)]
        [TestCase(WeaponConstants.ThrowingAxe, WeaponConstants.ThrowingAxe)]
        [TestCase(WeaponConstants.Trident, WeaponConstants.Trident)]
        [TestCase(WeaponConstants.TridentOfFishCommand, WeaponConstants.Trident)]
        [TestCase(WeaponConstants.TridentOfWarning, WeaponConstants.Trident)]
        [TestCase(WeaponConstants.TwoBladedSword, WeaponConstants.TwoBladedSword)]
        [TestCase(WeaponConstants.Warhammer, WeaponConstants.Warhammer)]
        [TestCase(WeaponConstants.Whip, WeaponConstants.Whip)]
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, WondrousItemConstants.AmuletOfInescapableLocation)]
        [TestCase(WondrousItemConstants.BagOfDevouring, WondrousItemConstants.BagOfDevouring)]
        [TestCase(WondrousItemConstants.BootsOfDancing, WondrousItemConstants.BootsOfDancing)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, WondrousItemConstants.BracersOfDefenselessness)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, WondrousItemConstants.BroomOfAnimatedAttack)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, WondrousItemConstants.CloakOfPoisonousness)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, WondrousItemConstants.CrystalBall_Hypnosis)]
        [TestCase(WondrousItemConstants.DustOfSneezingAndChoking, WondrousItemConstants.DustOfSneezingAndChoking)]
        [TestCase(WondrousItemConstants.FlaskOfCurses, WondrousItemConstants.FlaskOfCurses)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, WondrousItemConstants.GauntletsOfFumbling)]
        [TestCase(WondrousItemConstants.HelmOfOppositeAlignment, WondrousItemConstants.HelmOfOppositeAlignment)]
        [TestCase(WondrousItemConstants.IncenseOfObsession, WondrousItemConstants.IncenseOfObsession)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, WondrousItemConstants.MedallionOfThoughtProjection)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, WondrousItemConstants.NecklaceOfStrangulation)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, WondrousItemConstants.PeriaptOfFoulRotting)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, WondrousItemConstants.RobeOfPowerlessness)]
        [TestCase(WondrousItemConstants.RobeOfVermin, WondrousItemConstants.RobeOfVermin)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, WondrousItemConstants.ScarabOfDeath)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, WondrousItemConstants.StoneOfWeight_Loadstone)]
        [TestCase(WondrousItemConstants.VacousGrimoire, WondrousItemConstants.VacousGrimoire)]
        public void BaseNameGroup(string name, params string[] baseNames)
        {
            base.AssertCollection(name, baseNames);
        }

        [Test]
        public void SpecificCursedItems()
        {
            var items = new[]
            {
                ArmorConstants.ArmorOfArrowAttraction,
                ArmorConstants.ArmorOfRage,
                WeaponConstants.BerserkingSword,
                WeaponConstants.CursedBackbiterSpear,
                WeaponConstants.CursedMinus2Sword,
                WeaponConstants.NetOfSnaring,
                PotionConstants.Poison,
                RingConstants.Clumsiness,
                WondrousItemConstants.AmuletOfInescapableLocation,
                WondrousItemConstants.BagOfDevouring,
                WondrousItemConstants.BracersOfDefenselessness,
                WondrousItemConstants.BroomOfAnimatedAttack,
                WondrousItemConstants.CloakOfPoisonousness,
                WondrousItemConstants.DustOfSneezingAndChoking,
                WondrousItemConstants.RobeOfVermin,
                WondrousItemConstants.CrystalBall_Hypnosis,
                WondrousItemConstants.ScarabOfDeath,
                WondrousItemConstants.NecklaceOfStrangulation,
                WondrousItemConstants.BootsOfDancing,
                WondrousItemConstants.StoneOfWeight_Loadstone,
                WondrousItemConstants.PeriaptOfFoulRotting,
                WondrousItemConstants.GauntletsOfFumbling,
                WondrousItemConstants.VacousGrimoire,
                WondrousItemConstants.MedallionOfThoughtProjection,
                WondrousItemConstants.IncenseOfObsession,
                WondrousItemConstants.HelmOfOppositeAlignment,
                WondrousItemConstants.FlaskOfCurses,
                WondrousItemConstants.RobeOfPowerlessness,
                WeaponConstants.MaceOfBlood
            };

            base.AssertCollection(CurseConstants.SpecificCursedItem, items);
        }
    }
}
