using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit
{
    [TestFixture]
    public class LevelLimitsTests
    {
        [TestCase(LevelLimits.Minimum, 1)]
        [TestCase(LevelLimits.Maximum_Standard, 20)]
        [TestCase(LevelLimits.Maximum_Epic, 30)]
        public void LevelLimit(int limit, int value)
        {
            Assert.That(limit, Is.EqualTo(value));
        }
    }
}
