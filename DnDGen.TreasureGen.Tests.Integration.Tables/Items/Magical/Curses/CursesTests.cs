using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class CursesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.Curses;

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

        [TestCase(CurseConstants.Delusion, 1, 15)]
        [TestCase(CurseConstants.OppositeEffect, 16, 35)]
        [TestCase(CurseConstants.Intermittent, 36, 45)]
        [TestCase(CurseConstants.Requirement, 46, 60)]
        [TestCase(CurseConstants.Drawback, 61, 75)]
        [TestCase(CurseConstants.DifferentEffect, 76, 90)]
        [TestCase(TableNameConstants.Percentiles.SpecificCursedItems, 91, 100)]
        public void CursesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}