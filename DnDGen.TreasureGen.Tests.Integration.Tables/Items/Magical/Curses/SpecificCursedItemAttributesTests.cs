using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class SpecificCursedItemAttributesTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.SpecificCursedItemAttributes;

        [TestCase(ArmorConstants.ArmorOfArrowAttraction,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.ArmorOfRage,
            AttributeConstants.Metal,
            AttributeConstants.Specific)]
        [TestCase(PotionConstants.Poison,
            AttributeConstants.Specific,
            AttributeConstants.OneTimeUse)]
        [TestCase(RingConstants.Clumsiness, AttributeConstants.Specific)]
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
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.BagOfDevouring, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.BootsOfDancing, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.DustOfSneezingAndChoking,
            AttributeConstants.Specific,
            AttributeConstants.OneTimeUse)]
        [TestCase(WondrousItemConstants.FlaskOfCurses,
            AttributeConstants.Specific,
            AttributeConstants.OneTimeUse)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.HelmOfOppositeAlignment, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.IncenseOfObsession,
            AttributeConstants.OneTimeUse,
            AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.RobeOfVermin, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, AttributeConstants.Specific)]
        [TestCase(WondrousItemConstants.VacousGrimoire, AttributeConstants.Specific)]
        public void SpecificCursedItemAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

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

        [TestCase(WeaponConstants.BerserkingSword)]
        [TestCase(WeaponConstants.MaceOfBlood)]
        [TestCase(WeaponConstants.NetOfSnaring)]
        [TestCase(WeaponConstants.CursedBackbiterSpear)]
        [TestCase(WeaponConstants.CursedMinus2Sword)]
        public void SpecificCursedWeaponMatchesAttributes(string item)
        {
            var specificWeaponAttributesTable = TableNameConstants.Collections.SpecificITEMTYPEAttributes(ItemTypeConstants.Weapon);
            var specificWeaponAttributes = CollectionMapper.Map(Name, specificWeaponAttributesTable);
            Assert.That(table[item], Is.EquivalentTo(specificWeaponAttributes[item]));
        }
    }
}