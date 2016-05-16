﻿using Ninject;
using NUnit.Framework;
using System.Linq;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class RingGeneratorTests : MagicalItemGeneratorStressTests
    {
        [Inject, Named(ItemTypeConstants.Ring)]
        public MagicalItemGenerator RingGenerator { get; set; }

        [TestCase("Ring generator")]
        public override void Stress(string thingToStress)
        {
            Stress();
        }

        protected override string itemType
        {
            get { return ItemTypeConstants.Ring; }
        }

        protected override void MakeAssertionsAgainst(Item ring)
        {
            Assert.That(ring.Name, Does.StartWith("Ring of "));
            Assert.That(ring.Traits, Is.Not.Null);
            Assert.That(ring.Attributes, Is.Not.Null);
            Assert.That(ring.Quantity, Is.EqualTo(1));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.Contents, Is.Not.Null);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.Not.Negative);
            Assert.That(ring.Magic.Charges, Is.Not.Negative);
            Assert.That(ring.Magic.SpecialAbilities, Is.Empty);

            var itemMaterials = ring.Traits.Intersect(materials);
            Assert.That(itemMaterials, Is.Empty);
        }

        protected override Item GenerateItem()
        {
            var power = GetNewPower();
            return RingGenerator.GenerateAtPower(power);
        }

        [Test]
        public void ChargesHappen()
        {
            var ring = GenerateOrFail(r => r.Attributes.Contains(AttributeConstants.Charged));
            Assert.That(ring.Magic.Charges, Is.Positive);
        }

        [Test]
        public void ChargesDoNotHappen()
        {
            var ring = GenerateOrFail(r => r.Attributes.Contains(AttributeConstants.Charged) == false);
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void ContentsHappen()
        {
            GenerateOrFail(r => r.Contents.Any());
        }

        [Test]
        public void ContentsDoNotHappen()
        {
            GenerateOrFail(r => r.Contents.Any() == false);
        }

        [Test]
        public override void IntelligenceHappens()
        {
            base.IntelligenceHappens();
        }

        [Test]
        public override void TraitsHappen()
        {
            base.TraitsHappen();
        }

        [Test]
        public override void CursesHappen()
        {
            AssertCursesHappen();
        }

        [Test]
        public override void SpecificCursesHappen()
        {
            AssertSpecificCursesHappen();
        }

        [Test]
        public override void NoDecorationsHappen()
        {
            AssertNoDecorationsHappen();
        }

        [Test]
        public override void SpecificCursedItemsAreNotDecorated()
        {
            AssertSpecificCursedItemsAreNotDecorated();
        }

        [Test]
        public override void SpecificCursedItemsHaveTraits()
        {
            AssertSpecificCursedItemsHaveTraits();
        }

        [Test]
        public override void SpecificCursedItemsDoNotHaveSpecialMaterials()
        {
            AssertSpecificCursedItemsDoNotHaveSpecialMaterials();
        }
    }
}