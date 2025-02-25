using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Specific
{
    [TestFixture]
    public class SpecificWeaponsAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPEAttributes(ItemTypeConstants.Weapon); }
        }

        [TestCase(WeaponConstants.SleepArrow,
            AttributeConstants.Ammunition,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Martial,
            AttributeConstants.Wood,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.ScreamingBolt,
            AttributeConstants.Simple,
            AttributeConstants.Ammunition,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Dagger_Silver,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Longsword,
            AttributeConstants.Melee,
            AttributeConstants.Specific,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.JavelinOfLightning,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Thrown,
            AttributeConstants.Simple,
            AttributeConstants.OneTimeUse)]
        [TestCase(WeaponConstants.SlayingArrow,
            AttributeConstants.Ammunition,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Martial,
            AttributeConstants.Wood,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Dagger_Adamantine,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine,
            AttributeConstants.Specific,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.GreaterSlayingArrow,
            AttributeConstants.Ammunition,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Martial,
            AttributeConstants.Wood,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Shatterspike,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.DaggerOfVenom,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.TridentOfWarning,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.AssassinsDagger,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.ShiftersSorrow,
            AttributeConstants.Specific,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.Metal,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.TridentOfFishCommand,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.FlameTongue,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.LuckBlade0,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Charged)]
        [TestCase(WeaponConstants.LuckBlade,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Charged)]
        [TestCase(WeaponConstants.SwordOfSubtlety,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.SwordOfThePlanes,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.NineLivesStealer,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.SwordOfLifeStealing,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Oathbow,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Martial,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.MaceOfTerror,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.LifeDrinker,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.Metal,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.SylvanScimitar,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.RapierOfPuncturing,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.SunBlade,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Exotic,
            AttributeConstants.OneHanded,
            AttributeConstants.TwoHanded,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.FrostBrand,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.DwarvenThrower,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.LuckBlade1,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Charged)]
        [TestCase(WeaponConstants.MaceOfSmiting,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.LuckBlade2,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Charged)]
        [TestCase(WeaponConstants.HolyAvenger,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.LuckBlade3,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Charged)]
        [TestCase(WeaponConstants.BerserkingSword,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Martial,
            AttributeConstants.Metal,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.CursedBackbiterSpear,
            AttributeConstants.Specific,
            AttributeConstants.Melee,
            AttributeConstants.Simple,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Ranged,
            AttributeConstants.OneHanded,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.CursedMinus2Sword,
            AttributeConstants.Specific,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.MaceOfBlood,
            AttributeConstants.Specific,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.NetOfSnaring,
            AttributeConstants.Specific,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Exotic)]
        public void SpecificWeaponAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [TestCase(WeaponConstants.BerserkingSword)]
        [TestCase(WeaponConstants.MaceOfBlood)]
        [TestCase(WeaponConstants.NetOfSnaring)]
        [TestCase(WeaponConstants.CursedBackbiterSpear)]
        [TestCase(WeaponConstants.CursedMinus2Sword)]
        public void SpecificCursedWeaponMatchesAttributes(string item)
        {
            var specificCursedAttributes = CollectionMapper.Map(Name, TableNameConstants.Collections.SpecificCursedItemAttributes);
            Assert.That(table[item], Is.EquivalentTo(specificCursedAttributes[item]));
        }

        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.AssassinsDagger, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.DaggerOfVenom, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.DwarvenThrower, WeaponConstants.Warhammer)]
        [TestCase(WeaponConstants.FlameTongue, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.FrostBrand, WeaponConstants.Greatsword)]
        [TestCase(WeaponConstants.HolyAvenger, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.JavelinOfLightning, WeaponConstants.Javelin)]
        [TestCase(WeaponConstants.LifeDrinker, WeaponConstants.Greataxe)]
        [TestCase(WeaponConstants.LuckBlade, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.MaceOfSmiting, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.MaceOfTerror, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.Longsword, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.NineLivesStealer, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.Oathbow, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.RapierOfPuncturing, WeaponConstants.Rapier)]
        [TestCase(WeaponConstants.ScreamingBolt, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.Shatterspike, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.ShiftersSorrow, WeaponConstants.TwoBladedSword)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.SlayingArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.GreaterSlayingArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.SleepArrow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.SunBlade, WeaponConstants.BastardSword)]
        [TestCase(WeaponConstants.SunBlade, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.SwordOfLifeStealing, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.SwordOfThePlanes, WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.SwordOfSubtlety, WeaponConstants.ShortSword)]
        [TestCase(WeaponConstants.SylvanScimitar, WeaponConstants.Scimitar)]
        [TestCase(WeaponConstants.TridentOfFishCommand, WeaponConstants.Trident)]
        [TestCase(WeaponConstants.TridentOfWarning, WeaponConstants.Trident)]
        [TestCase(WeaponConstants.BerserkingSword, WeaponConstants.Greatsword)]
        [TestCase(WeaponConstants.MaceOfBlood, WeaponConstants.HeavyMace)]
        [TestCase(WeaponConstants.NetOfSnaring, WeaponConstants.Net)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, WeaponConstants.Shortspear)]
        [TestCase(WeaponConstants.CursedMinus2Sword, WeaponConstants.Longsword)]
        public void AttributesMatchWeapon(string specificWeapon, string weapon)
        {
            var specificWeaponAttributes = table[specificWeapon];

            var weaponAttributesTableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Weapon);
            var weaponAttributesTable = CollectionMapper.Map(Name, weaponAttributesTableName);
            var weaponAttributes = weaponAttributesTable[weapon];

            Assert.That(specificWeaponAttributes, Is.SupersetOf(weaponAttributes));
        }
    }
}