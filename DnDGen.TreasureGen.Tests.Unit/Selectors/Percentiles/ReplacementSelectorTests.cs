using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Percentiles
{
    [TestFixture]
    public class ReplacementSelectorTests
    {
        private IReplacementSelector replacementSelector;
        private Mock<ICollectionSelector> mockCollectionSelector;
        private Dictionary<string, IEnumerable<string>> replacements;

        [SetUp]
        public void Setup()
        {
            mockCollectionSelector = new Mock<ICollectionSelector>();

            replacements = new Dictionary<string, IEnumerable<string>>();
            replacements["FIRST"] = new[] { "1st", "the first" };
            replacements["SECOND"] = new[] { "2nd", "the second" };
            replacements["SINGLE"] = new[] { "the only one" };

            mockCollectionSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.ReplacementStrings))
                .Returns(replacements);

            var index = 0;
            mockCollectionSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> c) => c.ElementAt(index++ % c.Count()));

            replacementSelector = new ReplacementSelector(mockCollectionSelector.Object);
        }

        [Test]
        public void SelectRandom_NoReplace()
        {
            var result = replacementSelector.SelectRandom("my value");
            Assert.That(result, Is.EqualTo("my value"));
        }

        [Test]
        public void SelectAll_Single_NoReplace()
        {
            var results = replacementSelector.SelectAll("my value");
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo("my value"));
        }

        [Test]
        public void SelectAll_Collection_NoReplace()
        {
            var values = new List<string> { "my value", "my other value" };
            var result = replacementSelector.SelectAll(values);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Contains.Item("my value")
                .And.Contains("my other value"));
        }

        [Test]
        public void SelectSingle_NoReplace()
        {
            var result = replacementSelector.SelectSingle("my value");
            Assert.That(result, Is.EqualTo("my value"));
        }

        [Test]
        public void SelectRandom_PerformsReplacement()
        {
            var result = replacementSelector.SelectRandom("my SECOND value");
            Assert.That(result, Is.EqualTo("my 2nd value"));
        }

        [Test]
        public void SelectRandom_PerformsRandomReplacement()
        {
            var result = replacementSelector.SelectRandom("my SECOND value");
            Assert.That(result, Is.EqualTo("my 2nd value"));

            result = replacementSelector.SelectRandom("my SECOND value");
            Assert.That(result, Is.EqualTo("my the second value"));
        }

        [Test]
        public void SelectRandom_PerformsMultipleReplacements()
        {
            var result = replacementSelector.SelectRandom("my SECOND value with FIRST");
            Assert.That(result, Is.EqualTo("my the second value with 1st"));
        }

        [Test]
        public void SelectAll_Collection_PerformsAllReplacements()
        {
            var values = new[]
            {
                "this is FIRST",
                "this is the SECOND percentile",
                "non-replaced",
                "FIRST also"
            };

            var results = replacementSelector.SelectAll(values);
            Assert.That(results, Contains.Item("this is 1st")
                .And.Contains("this is the first")
                .And.Contains("this is the 2nd percentile")
                .And.Contains("this is the the second percentile")
                .And.Contains("1st also")
                .And.Contains("the first also")
                .And.Contains("non-replaced")
                .And.Not.Contains("this is FIRST")
                .And.Not.Contains("this is the SECOND percentile")
                .And.Not.Contains("FIRST also"));
            Assert.That(results.Count(), Is.EqualTo(7));
        }

        [Test]
        public void SelectAll_Collection_PerformsAllMultipleReplacements()
        {
            var values = new[]
            {
                "this is FIRST",
                "this is the SECOND percentile",
                "non-replaced",
                "FIRST also with SECOND"
            };

            var results = replacementSelector.SelectAll(values);
            Assert.That(results, Contains.Item("this is 1st")
                .And.Contains("this is the first")
                .And.Contains("this is the 2nd percentile")
                .And.Contains("this is the the second percentile")
                .And.Contains("1st also with 2nd")
                .And.Contains("1st also with the second")
                .And.Contains("the first also with 2nd")
                .And.Contains("the first also with the second")
                .And.Contains("non-replaced")
                .And.Not.Contains("this is FIRST")
                .And.Not.Contains("this is the SECOND percentile")
                .And.Not.Contains("FIRST also with SECOND"));
            Assert.That(results.Count(), Is.EqualTo(9));
        }

        [Test]
        public void SelectAll_Single_PerformsAllReplacements()
        {
            var results = replacementSelector.SelectAll("this is FIRST");
            Assert.That(results, Contains.Item("this is 1st")
                .And.Contains("this is the first")
                .And.Not.Contains("this is FIRST"));
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SelectAll_Single_PerformsAllMultipleReplacements()
        {
            var results = replacementSelector.SelectAll("FIRST also with SECOND");
            Assert.That(results, Contains.Item("1st also with 2nd")
                .And.Contains("1st also with the second")
                .And.Contains("the first also with 2nd")
                .And.Contains("the first also with the second")
                .And.Not.Contains("FIRST also with SECOND"));
            Assert.That(results.Count(), Is.EqualTo(4));
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        [TestCase("whatever", "because I said so")]
        public void SelectAll_ReplacesWholeString(string value, string replacement)
        {
            replacements[value] = new[] { replacement };

            var result = replacementSelector.SelectAll(value, true);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single(), Is.EqualTo(replacement));
        }

        [Test]
        public void SelectSingle_PerformsReplacement()
        {
            var result = replacementSelector.SelectSingle("my SINGLE value");
            Assert.That(result, Is.EqualTo("my the only one value"));
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        [TestCase("whatever", "because I said so")]
        public void SelectSingle_ReplacesWholeString(string value, string replacement)
        {
            replacements[value] = new[] { replacement };

            var result = replacementSelector.SelectSingle(value);
            Assert.That(result, Is.EqualTo(replacement));
        }

        [Test]
        public void SelectSingle_ThrowsException_IfMoreThan1Option()
        {
            Assert.That(() => replacementSelector.SelectSingle("my SECOND value"),
                Throws.Exception);
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_SelectRandom_DoesNotReplaceWholeName(string value, string replacement)
        {
            replacements[value] = new[] { replacement };

            var result = replacementSelector.SelectRandom(value);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_SelectRandom_DoesNotReplacePartialName(string partialValue, string replacement)
        {
            var value = $"{partialValue}/9266";
            replacements[partialValue] = new[] { replacement };

            var result = replacementSelector.SelectRandom(value);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_SelectAll_DoesNotReplaceWholeName(string value, string replacement)
        {
            replacements[value] = new[] { replacement };

            var result = replacementSelector.SelectAll(value);
            Assert.That(result.Single(), Is.EqualTo(value));
        }

        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.CompositeLongbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.CompositeShortbow)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_SelectAll_DoesNotReplacePartialName(string partialValue, string replacement)
        {
            var value = $"{partialValue}/9266";
            replacements[partialValue] = new[] { replacement };

            var result = replacementSelector.SelectAll(value);
            Assert.That(result.Single(), Is.EqualTo(value));
        }
    }
}