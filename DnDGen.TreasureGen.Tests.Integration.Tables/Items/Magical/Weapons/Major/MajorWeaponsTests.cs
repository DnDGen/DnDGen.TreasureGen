using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Major
{
    [TestFixture]
    public class MajorWeaponsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Weapon); }
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

        [TestCase(ItemTypeConstants.Weapon, 50, 63)]
        [TestCase("SpecialAbility", 64, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(3, 1, 20)]
        [TestCase(4, 21, 38)]
        [TestCase(5, 39, 49)]
        public void Percentile(int bonus, int lower, int upper)
        {
            var content = Convert.ToString(bonus);
            AssertPercentile(content, lower, upper);
        }
    }
}