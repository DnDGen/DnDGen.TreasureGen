using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class IronFlaskContentsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.IronFlaskContents; }
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

        [TestCase("", 1, 50)]
        [TestCase("Large air elemental", 51, 54)]
        [TestCase("Arrowhawk", 55, 58)]
        [TestCase("Large earth elemental", 59, 62)]
        [TestCase("Xorn", 63, 66)]
        [TestCase("Large fire elemental", 67, 70)]
        [TestCase("Salamander", 71, 74)]
        [TestCase("Large water elemental", 75, 78)]
        [TestCase("Adult tojanida", 79, 82)]
        [TestCase("Chaos beast", 83, 84)]
        [TestCase("Formian taskmaster", 85, 86)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("Vrock", 87)]
        [TestCase("Hezrou", 88)]
        [TestCase("Glabrezu", 89)]
        [TestCase("Succubus", 90)]
        [TestCase("Osyluth", 91)]
        [TestCase("Barbazu", 92)]
        [TestCase("Erinyes", 93)]
        [TestCase("Cornugon", 94)]
        [TestCase("Avoral", 95)]
        [TestCase("Ghaele", 96)]
        [TestCase("Formian myrmarch", 97)]
        [TestCase("Elder arrowhawk", 98)]
        [TestCase("Rakshasa", 99)]
        [TestCase("BalorOrPitFiend", 100)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}