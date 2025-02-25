using DnDGen.Infrastructure.Mappers.Percentiles;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods
{
    [TestFixture]
    public class LevelGoodsTests : TableTests
    {
        private PercentileMapper percentileMapper;

        protected override string tableName => throw new NotImplementedException();

        [SetUp]
        public void Setup()
        {
            percentileMapper = GetNewInstanceOf<PercentileMapper>();
        }

        [Test]
        public void LevelCoinsExistForAllLevels()
        {
            for (var level = LevelLimits.Minimum; level <= LevelLimits.Maximum_Standard; level++)
            {
                var levelTableName = TableNameConstants.Percentiles.LevelXGoods(level);
                var table = percentileMapper.Map(Name, levelTableName);
                Assert.That(table, Is.Not.Null, levelTableName);
                Assert.That(table.Keys, Is.EqualTo(Enumerable.Range(1, 100)), levelTableName);
            }
        }
    }
}
