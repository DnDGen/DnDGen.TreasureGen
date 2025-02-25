using DnDGen.Infrastructure.Mappers.Percentiles;
using DnDGen.TreasureGen.Selectors.Percentiles;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    [TestFixture]
    public abstract class PercentileTests : TableTests
    {
        protected PercentileMapper PercentileMapper;
        protected Dictionary<int, string> table;

        private readonly Regex allCapsRegex;
        private readonly IEnumerable<string> replacementStrings;

        public PercentileTests()
        {
            allCapsRegex = new Regex("[A-Z]{4}[A-Z]+");
            replacementStrings = ReplacementStringConstants.GetAll();
        }

        [SetUp]
        public void PercentileSetup()
        {
            PercentileMapper = GetNewInstanceOf<PercentileMapper>();
            table = PercentileMapper.Map(Name, tableName);
        }

        public abstract void TableIsComplete();

        protected void AssertTableIsComplete()
        {
            for (var roll = 100; roll > 0; roll--)
                Assert.That(table.Keys, Contains.Item(roll), tableName);

            Assert.That(table.Keys.Count, Is.EqualTo(100), tableName);
        }

        public abstract void ReplacementStringsAreValid();

        protected void AssertReplacementStringsAreValid()
        {
            var distinctValues = table.Values.Distinct();
            foreach (var value in distinctValues)
                AssertValidReplacementStrings(value);
        }

        private void AssertValidReplacementStrings(string value)
        {
            var matches = allCapsRegex.Matches(value);
            foreach (var match in matches)
                Assert.That(replacementStrings, Contains.Item(match.ToString()));
        }

        protected void AssertPercentile(string content, int lower, int upper)
        {
            for (var roll = lower; roll <= upper; roll++)
                AssertPercentile(content, roll);
        }

        protected void AssertPercentile(string content, int roll)
        {
            Assert.That(table.Keys, Contains.Item(roll), tableName);

            var message = string.Format("Roll: {0}", roll);
            Assert.That(table[roll], Is.EqualTo(content), message);
        }
    }
}