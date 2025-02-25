using DnDGen.Infrastructure.Mappers.Percentiles;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class LevelCoinsTests : TableTests
    {
        private PercentileMapper percentileMapper;

        [SetUp]
        public void Setup()
        {
            percentileMapper = GetNewInstanceOf<PercentileMapper>();
        }

        protected override string tableName => throw new NotImplementedException();

        [Test]
        public void LevelCoinsExistForAllLevels()
        {
            for (var level = LevelLimits.Minimum; level <= LevelLimits.Maximum_Standard; level++)
            {
                var table = percentileMapper.Map(Name, TableNameConstants.Percentiles.LevelXCoins(level));
                Assert.That(table, Is.Not.Null);
                Assert.That(table.Keys, Is.EqualTo(Enumerable.Range(1, 100)));
            }
        }
    }
}
