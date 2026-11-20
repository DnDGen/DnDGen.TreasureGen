using DnDGen.TreasureGen.Items.Magical;
using NUnit.Framework;
using System.Text.Json;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class IntelligenceTests
    {
        private Intelligence intelligence;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            intelligence = new Intelligence();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void IntelligenceStatInitialized()
        {
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(0));
        }

        [Test]
        public void CharismaStatInitialized()
        {
            Assert.That(intelligence.CharismaStat, Is.EqualTo(0));
        }

        [Test]
        public void WisdomStatInitialized()
        {
            Assert.That(intelligence.WisdomStat, Is.EqualTo(0));
        }

        [Test]
        public void PowersInitialized()
        {
            Assert.That(intelligence.Powers, Is.Empty);
        }

        [Test]
        public void AlignmentInitialized()
        {
            Assert.That(intelligence.Alignment, Is.Empty);
        }

        [Test]
        public void CommunicationInitialized()
        {
            Assert.That(intelligence.Communication, Is.Empty);
        }

        [Test]
        public void DedicatedPowerInitialized()
        {
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void EgoInitialized()
        {
            Assert.That(intelligence.Ego, Is.EqualTo(0));
        }

        [Test]
        public void SensesInitialized()
        {
            Assert.That(intelligence.Senses, Is.Empty);
        }

        [Test]
        public void SpecialPurposeInitialized()
        {
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
        }

        [Test]
        public void LanguagesAreInitialized()
        {
            Assert.That(intelligence.Languages, Is.Empty);
        }

        [Test]
        public void PersonalityIsInitialized()
        {
            Assert.That(intelligence.Personality, Is.Empty);
        }

        [Test]
        public void CopyIntelligence()
        {
            intelligence = itemVerifier.CreateRandomTemplate(string.Empty).Magic.Intelligence;

            var copy = intelligence.Clone();
            Assert.That(copy, Is.Not.SameAs(intelligence));
            Assert.That(JsonSerializer.Serialize(copy), Is.EqualTo(JsonSerializer.Serialize(intelligence)));
            Assert.That(copy.Alignment, Is.EqualTo(intelligence.Alignment));
            Assert.That(copy.CharismaStat, Is.EqualTo(intelligence.CharismaStat));
            Assert.That(copy.Communication, Is.EquivalentTo(intelligence.Communication));
            Assert.That(copy.DedicatedPower, Is.EqualTo(intelligence.DedicatedPower));
            Assert.That(copy.Ego, Is.EqualTo(intelligence.Ego));
            Assert.That(copy.IntelligenceStat, Is.EqualTo(intelligence.IntelligenceStat));
            Assert.That(copy.Languages, Is.EquivalentTo(intelligence.Languages));
            Assert.That(copy.Personality, Is.EqualTo(intelligence.Personality));
            Assert.That(copy.Powers, Is.EquivalentTo(intelligence.Powers));
            Assert.That(copy.Senses, Is.EqualTo(intelligence.Senses));
            Assert.That(copy.SpecialPurpose, Is.EqualTo(intelligence.SpecialPurpose));
            Assert.That(copy.WisdomStat, Is.EqualTo(intelligence.WisdomStat));
        }
    }
}