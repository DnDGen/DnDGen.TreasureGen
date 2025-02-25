using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceDedicatedPowerTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.IntelligenceDedicatedPowers;

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

        [TestCase("Item can use Ice Storm", 1, 6)]
        [TestCase("Item can use Confusion", 7, 12)]
        [TestCase("Item can use Phantasmal Killer", 13, 17)]
        [TestCase("Item can use Crushing Despair", 18, 24)]
        [TestCase("Item can use Dimension Door on itself and wielder", 25, 31)]
        [TestCase("Item can use Contagion (heightened to 4th level) as touch attack", 32, 36)]
        [TestCase("Item can use Poison (heightened to 4th level) as touch attack", 37, 43)]
        [TestCase("Item can use Rusting Grasp as touch attack", 44, 50)]
        [TestCase("Item can cast 10d6 Lightning Bolt", 51, 56)]
        [TestCase("Item can cast 10d6 Fireball", 57, 62)]
        [TestCase("Wielder gets +2 luck bonus on attacks, saves, and checks", 63, 68)]
        [TestCase("Item can use Mass Inflict Light Wounds", 69, 74)]
        [TestCase("Item can use Song of Discord", 75, 81)]
        [TestCase("Item can use Prying Eyes", 82, 87)]
        [TestCase("Item can cast 15d6 Greater Shout 3/day", 88, 92)]
        [TestCase("Item can use Waves of Exhaustion", 93, 98)]
        [TestCase("Item can use True Resurrection on wielder 1/month", 99, 100)]
        public void IntelligenceDedicatedPowersPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}