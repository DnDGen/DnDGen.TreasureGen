using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System.Data;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    internal class ExtraItemsTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.ExtraItems;

        [Test]
        public void ExtraItemsNames()
        {
            var levels = Enumerable.Range(LevelLimits.Minimum, LevelLimits.Maximum_Epic).Select(l => l.ToString());
            AssertCollection(table.Keys, levels);
        }

        [TestCase(LevelLimits.Minimum, "", 0)]
        [TestCase(2, "", 0)]
        [TestCase(3, "", 0)]
        [TestCase(4, "", 0)]
        [TestCase(5, "", 0)]
        [TestCase(6, "", 0)]
        [TestCase(7, "", 0)]
        [TestCase(8, "", 0)]
        [TestCase(9, "", 0)]
        [TestCase(10, "", 0)]
        [TestCase(11, "", 0)]
        [TestCase(12, "", 0)]
        [TestCase(13, "", 0)]
        [TestCase(14, "", 0)]
        [TestCase(15, "", 0)]
        [TestCase(16, "", 0)]
        [TestCase(17, "", 0)]
        [TestCase(18, "", 0)]
        [TestCase(19, "", 0)]
        [TestCase(LevelLimits.Maximum_Standard, "", 0)]
        [TestCase(21, PowerConstants.Major, 1)]
        [TestCase(22, PowerConstants.Major, 2)]
        [TestCase(23, PowerConstants.Major, 4)]
        [TestCase(24, PowerConstants.Major, 6)]
        [TestCase(25, PowerConstants.Major, 9)]
        [TestCase(26, PowerConstants.Major, 12)]
        [TestCase(27, PowerConstants.Major, 17)]
        [TestCase(28, PowerConstants.Major, 23)]
        [TestCase(29, PowerConstants.Major, 31)]
        [TestCase(LevelLimits.Maximum_Epic, PowerConstants.Major, 42)]
        public void ExtraItems(int level, string type, int amount)
        {
            var data = DataHelper.Parse(new TypeAndAmountDataSelection { Type = type, Roll = amount.ToString() });
            AssertCollection(level.ToString(), data);
        }
    }
}
