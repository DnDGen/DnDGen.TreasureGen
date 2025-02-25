using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor
{
    [TestFixture]
    public class ArmorTypesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.ARMORTYPETypes(ItemTypeConstants.Armor);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(ArmorConstants.PaddedArmor, 1, 1)]
        [TestCase(ArmorConstants.LeatherArmor, 2, 2)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, 3, 17)]
        [TestCase(ArmorConstants.ChainShirt, 18, 32)]
        [TestCase(ArmorConstants.HideArmor, 33, 42)]
        [TestCase(ArmorConstants.ScaleMail, 43, 43)]
        [TestCase(ArmorConstants.Chainmail, 44, 44)]
        [TestCase(ArmorConstants.Breastplate, 45, 57)]
        [TestCase(ArmorConstants.SplintMail, 58, 58)]
        [TestCase(ArmorConstants.BandedMail, 59, 59)]
        [TestCase(ArmorConstants.HalfPlate, 60, 60)]
        [TestCase(ArmorConstants.FullPlate, 61, 100)]
        public void ArmorTypesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}