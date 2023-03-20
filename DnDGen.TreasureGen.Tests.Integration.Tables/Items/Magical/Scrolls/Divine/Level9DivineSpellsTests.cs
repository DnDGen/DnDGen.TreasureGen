﻿using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level9DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXSPELLTYPESpells, 9, "Divine"); }
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

        [TestCase("Antipathy", 1, 4)]
        [TestCase("Astral Projection", 5, 7)]
        [TestCase("Elemental Swarm", 8, 13)]
        [TestCase("Energy Drain", 14, 19)]
        [TestCase("Etherealness", 20, 25)]
        [TestCase("Foresight", 26, 31)]
        [TestCase("Gate", 32, 37)]
        [TestCase("Mass Heal", 38, 46)]
        [TestCase("Implosion", 47, 53)]
        [TestCase("Miracle", 54, 55)]
        [TestCase("Regenerate", 56, 61)]
        [TestCase("Shambler", 62, 66)]
        [TestCase("Shapechange", 67, 72)]
        [TestCase("Soul Bind", 73, 77)]
        [TestCase("Storm of Vengeance", 78, 83)]
        [TestCase("Summon Monster IX", 84, 89)]
        [TestCase("Summon Nature's Ally IX", 90, 95)]
        [TestCase("Sympathy", 96, 99)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase("True Resurrection", 100)]
        public override void Percentile(string content, int roll)
        {
            base.Percentile(content, roll);
        }
    }
}