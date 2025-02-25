using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IsIntelligentTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.IsIntelligent;

        [TestCase(ItemTypeConstants.Armor, 100)]
        [TestCase(ItemTypeConstants.Ring, 100)]
        [TestCase(ItemTypeConstants.Rod, 100)]
        [TestCase(ItemTypeConstants.WondrousItem, 100)]
        [TestCase(AttributeConstants.Melee, 86)]
        [TestCase(AttributeConstants.Ranged, 96)]
        public void IsIntelligentThreshold(string itemType, int threshold)
        {
            var data = DataHelper.Parse(new TypeAndAmountDataSelection { Type = itemType, Roll = threshold.ToString() });
            AssertCollection(itemType, data);
        }
    }
}