using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Rods
{
    [TestFixture]
    public class MajorRodsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Rod); }
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

        [TestCase(RodConstants.Cancellation, 0, 1, 4)]
        [TestCase(RodConstants.Metamagic_Enlarge, 0, 5, 6)]
        [TestCase(RodConstants.Metamagic_Extend, 0, 7, 8)]
        [TestCase(RodConstants.Metamagic_Silent, 0, 9, 10)]
        [TestCase(RodConstants.Wonder, 0, 11, 14)]
        [TestCase(RodConstants.Python, 1, 15, 18)]
        [TestCase(RodConstants.FlameExtinguishing, 0, 19, 21)]
        [TestCase(RodConstants.Viper, 2, 22, 25)]
        [TestCase(RodConstants.EnemyDetection, 0, 26, 30)]
        [TestCase(RodConstants.Metamagic_Enlarge_Greater, 0, 31, 36)]
        [TestCase(RodConstants.Metamagic_Extend_Greater, 0, 37, 42)]
        [TestCase(RodConstants.Metamagic_Silent_Greater, 0, 43, 48)]
        [TestCase(RodConstants.Splendor, 4, 49, 53)]
        [TestCase(RodConstants.Withering, 1, 54, 58)]
        [TestCase(RodConstants.Metamagic_Empower, 0, 59, 64)]
        [TestCase(RodConstants.ThunderAndLightning, 2, 65, 69)]
        [TestCase(RodConstants.Metamagic_Quicken_Lesser, 0, 70, 73)]
        [TestCase(RodConstants.Negation, 0, 74, 77)]
        [TestCase(RodConstants.Absorption, 0, 78, 80)]
        [TestCase(RodConstants.Flailing, 3, 81, 84)]
        [TestCase(RodConstants.Metamagic_Maximize, 0, 85, 86)]
        [TestCase(RodConstants.Rulership, 0, 87, 88)]
        [TestCase(RodConstants.Security, 0, 89, 90)]
        [TestCase(RodConstants.LordlyMight, 2, 91, 92)]
        [TestCase(RodConstants.Metamagic_Empower_Greater, 0, 93, 94)]
        [TestCase(RodConstants.Metamagic_Quicken, 0, 95, 96)]
        [TestCase(RodConstants.Alertness, 1, 97, 98)]
        [TestCase(RodConstants.Metamagic_Maximize_Greater, 0, 99, 99)]
        [TestCase(RodConstants.Metamagic_Quicken_Greater, 0, 100, 100)]
        public void MajorRodsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}