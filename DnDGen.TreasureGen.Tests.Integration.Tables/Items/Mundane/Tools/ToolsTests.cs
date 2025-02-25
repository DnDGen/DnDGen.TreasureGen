using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items.Mundane;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Tools
{
    [TestFixture]
    public class ToolsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Tools; }
        }

        [TestCase(ToolConstants.Backpack_Empty, 1, 3)]
        [TestCase(ToolConstants.Crowbar, 4, 6)]
        [TestCase(ToolConstants.Lantern_Bullseye, 7, 11)]
        [TestCase(ToolConstants.Lock_Simple, 12, 16)]
        [TestCase(ToolConstants.Lock_Average, 17, 21)]
        [TestCase(ToolConstants.Lock_Good, 22, 28)]
        [TestCase(ToolConstants.Lock_Superior, 29, 35)]
        [TestCase(ToolConstants.Manacles_Masterwork, 36, 40)]
        [TestCase(ToolConstants.Mirror_SmallSteel, 41, 43)]
        [TestCase(ToolConstants.Rope_Silk, 44, 46)]
        [TestCase(ToolConstants.Spyglass, 47, 53)]
        [TestCase(ToolConstants.ArtisansTools_Masterwork, 54, 58)]
        [TestCase(ToolConstants.ClimbersKit, 59, 63)]
        [TestCase(ToolConstants.DisguiseKit, 64, 68)]
        [TestCase(ToolConstants.HealersKit, 69, 73)]
        [TestCase(ToolConstants.HolySymbol_Silver, 74, 77)]
        [TestCase(ToolConstants.Hourglass, 78, 81)]
        [TestCase(ToolConstants.MagnifyingGlass, 82, 88)]
        [TestCase(ToolConstants.MusicalInstrument_Masterwork, 89, 95)]
        [TestCase(ToolConstants.ThievesTools_Masterwork, 96, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }
    }
}