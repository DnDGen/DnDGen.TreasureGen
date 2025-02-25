using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane
{
    [TestFixture]
    public class MundaneItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERItems(PowerConstants.Mundane); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase(ItemTypeConstants.AlchemicalItem, 1, 17)]
        [TestCase(ItemTypeConstants.Armor, 18, 50)]
        [TestCase(ItemTypeConstants.Weapon, 51, 83)]
        [TestCase(ItemTypeConstants.Tool, 84, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}