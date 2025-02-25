using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    [TestFixture]
    public class ReplacementStringsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.ReplacementStrings; }
        }

        [TestCase(ReplacementStringConstants.Gender,
            "Male",
            "Female")]
        [TestCase(ReplacementStringConstants.Height,
            "shrink",
            "grow")]
        [TestCase(ReplacementStringConstants.KnowledgeCategory,
            "Arcana",
            "Architecture",
            "Dungeoneering",
            "Geography",
            "History",
            "Local",
            "Nature",
            "Nobility",
            "Religion",
            "The Planes")]
        [TestCase(ReplacementStringConstants.Energy,
            "Acid",
            "Cold",
            "Electricity",
            "Fire",
            "Sonic")]
        [TestCase(ReplacementStringConstants.PartialAlignment,
            "Chaos",
            "Evil",
            "Good",
            "Law")]
        [TestCase(ReplacementStringConstants.FullAlignment,
            "Lawful Good",
            "Neutral Good",
            "Chaotic Good",
            "Lawful Neutral",
            "True Neutral",
            "Chaotic Neutral",
            "Lawful Evil",
            "Neutral Evil",
            "Chaotic Evil")]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void Collection(string name, params string[] entries)
        {
            base.AssertCollection(name, entries);
        }

        [Test]
        public void DesignatedFoesCollection()
        {
            var foes = new[]
            {
                "Aquatic-humanoid",
                "Gnoll",
                "Gnome",
                "Halfling",
                "Air-outsider",
                "Earth-outsider",
                "Fire-outsider",
                "Water-outsider",
                "Aberration",
                "Aberration",
                "Aberration",
                "Aberration",
                "Aberration",
                "Animal",
                "Animal",
                "Animal",
                "Animal",
                "Construct",
                "Construct",
                "Construct",
                "Construct",
                "Construct",
                "Construct",
                "Construct",
                "Dragon",
                "Dragon",
                "Dragon",
                "Dragon",
                "Dragon",
                "Dragon",
                "Elemental",
                "Elemental",
                "Elemental",
                "Elemental",
                "Elemental",
                "Fey",
                "Fey",
                "Fey",
                "Fey",
                "Fey",
                "Giant",
                "Giant",
                "Giant",
                "Giant",
                "Giant",
                "Giant",
                "Giant",
                "Dwarf",
                "Dwarf",
                "Elf",
                "Elf",
                "Goblinoid",
                "Goblinoid",
                "Goblinoid",
                "Human",
                "Human",
                "Human",
                "Human",
                "Reptilian-humanoid",
                "Reptilian-humanoid",
                "Reptilian-humanoid",
                "Orc",
                "Orc",
                "Orc",
                "Magical-beast",
                "Magical-beast",
                "Magical-beast",
                "Magical-beast",
                "Magical-beast",
                "Monstrous-humanoid",
                "Monstrous-humanoid",
                "Monstrous-humanoid",
                "Monstrous-humanoid",
                "Monstrous-humanoid",
                "Ooze",
                "Ooze",
                "Chaotic-outsider",
                "Chaotic-outsider",
                "Chaotic-outsider",
                "Evil-outsider",
                "Evil-outsider",
                "Evil-outsider",
                "Good-outsider",
                "Good-outsider",
                "Good-outsider",
                "Lawful-outsider",
                "Lawful-outsider",
                "Lawful-outsider",
                "Plant",
                "Plant",
                "Undead",
                "Undead",
                "Undead",
                "Undead",
                "Undead",
                "Undead",
                "Undead",
                "Undead",
                "Vermin",
                "Vermin",
            };

            Assert.That(foes, Has.Length.EqualTo(100));
            base.AssertCollection(ReplacementStringConstants.DesignatedFoe, foes);
        }
    }
}