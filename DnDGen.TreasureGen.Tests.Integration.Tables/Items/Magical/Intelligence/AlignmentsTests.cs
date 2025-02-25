using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class AlignmentsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.IntelligenceAlignments; }
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

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void ChaoticGoodPercentile()
        {
            base.AssertPercentile(AlignmentConstants.ChaoticGood, 1, 5);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void ChaoticNeutralPercentile()
        {
            base.AssertPercentile(AlignmentConstants.ChaoticNeutral, 6, 15);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void ChaoticEvilPercentile()
        {
            base.AssertPercentile(AlignmentConstants.ChaoticEvil, 16, 20);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void NeutralEvilPercentile()
        {
            base.AssertPercentile(AlignmentConstants.NeutralEvil, 21, 25);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void LawfulEvilPercentile()
        {
            base.AssertPercentile(AlignmentConstants.LawfulEvil, 26, 30);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void LawfulGoodPercentile()
        {
            base.AssertPercentile(AlignmentConstants.LawfulGood, 31, 55);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void LawfulNeutralPercentile()
        {
            base.AssertPercentile(AlignmentConstants.LawfulNeutral, 56, 60);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void NeutralGoodPercentile()
        {
            base.AssertPercentile(AlignmentConstants.NeutralGood, 61, 80);
        }

        //INFO: Doing this because the full alignment constants are static properties, not constants
        [Test]
        public void TrueNeutralPercentile()
        {
            base.AssertPercentile(AlignmentConstants.TrueNeutral, 81, 100);
        }
    }
}