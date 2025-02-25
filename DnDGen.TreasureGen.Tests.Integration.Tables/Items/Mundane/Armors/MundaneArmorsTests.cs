using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Armors
{
    [TestFixture]
    public class MundaneArmorsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Mundane, ItemTypeConstants.Armor); }
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

        [TestCase(ArmorConstants.ChainShirt, 1, 12)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, 13, 18)]
        [TestCase(ArmorConstants.Breastplate, 19, 26)]
        [TestCase(ArmorConstants.BandedMail, 27, 34)]
        [TestCase(ArmorConstants.HalfPlate, 35, 54)]
        [TestCase(ArmorConstants.FullPlate, 55, 80)]
        [TestCase(AttributeConstants.Shield, 81, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}