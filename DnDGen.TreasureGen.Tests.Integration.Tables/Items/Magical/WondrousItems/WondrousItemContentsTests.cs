using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class WondrousItemContentsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.WondrousItemContents; }
        }

        [TestCase(WondrousItemConstants.RobeOfUsefulItems,
            WeaponConstants.Dagger,
            WeaponConstants.Dagger,
            "Bullseye lantern (filled and lit)",
            "Bullseye lantern (filled and lit)",
            "Highly polished 2-foot-by-4-foot steel mirror",
            "Highly polished 2-foot-by-4-foot steel mirror",
            "10-foot pole",
            "10-foot pole",
            "50-foot hempen rope",
            "50-foot hempen rope",
            "Sack",
            "Sack")]
        [TestCase(WondrousItemConstants.RobeOfBones,
            "Small Goblin Skeleton",
            "Small Goblin Skeleton",
            "Medium Human Commoner Skeleton",
            "Medium Human Commoner Skeleton",
            "Medium Wolf Skeleton",
            "Medium Wolf Skeleton",
            "Small Goblin Zombie",
            "Small Goblin Zombie",
            "Medium Human Commoner Zombie",
            "Medium Human Commoner Zombie",
            "Medium Wolf Zombie",
            "Medium Wolf Zombie")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_I, "3d6", "3d6", "5d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_II, "2d6", "2d6", "4d6", "4d6", "6d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_III, "3d6", "3d6", "3d6", "3d6", "5d6", "5d6", "7d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_IV, "2d6", "2d6", "2d6", "2d6", "4d6", "4d6", "6d6", "6d6", "8d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_V, "3d6", "3d6", "5d6", "5d6", "7d6", "7d6", "9d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VI, "4d6", "4d6", "4d6", "4d6", "6d6", "6d6", "8d6", "8d6", "10d6")]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VII, "3d6", "3d6", "5d6", "5d6", "7d6", "7d6", "9d6", "9d6", "10d6")]
        public void Collection(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        //HACK: have to make this separate - too many attributes for NUnit to handle as test name
        [Test]
        public void DeckOfIllusionsContents()
        {
            var attributes = new[]
            {
                "Ace of hearts",
                "King of hearts",
                "Queen of hearts",
                "Jack of hearts",
                "10 of hearts",
                "9 of hearts",
                "8 of hearts",
                "2 of hearts",
                "Ace of diamonds",
                "King of diamonds",
                "Queen of diamonds",
                "Jack of diamonds",
                "10 of diamonds",
                "9 of diamonds",
                "8 of diamonds",
                "2 of diamonds",
                "Ace of spades",
                "King of spades",
                "Queen of spades",
                "Jack of spades",
                "10 of spades",
                "9 of spades",
                "8 of spades",
                "2 of spades",
                "Ace of clubs",
                "King of clubs",
                "Queen of clubs",
                "Jack of clubs",
                "10 of clubs",
                "9 of clubs",
                "8 of clubs",
                "2 of clubs",
                "Joker",
                "Joker"
            };

            AssertCollection(WondrousItemConstants.DeckOfIllusions, attributes);
        }
    }
}