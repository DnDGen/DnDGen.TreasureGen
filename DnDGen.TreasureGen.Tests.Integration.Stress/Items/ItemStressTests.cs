using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items
{
    [TestFixture]
    public abstract class ItemStressTests : StressTests
    {
        protected ItemVerifier ItemVerifier;

        [SetUp]
        public void ItemStressSetup()
        {
            ItemVerifier = new ItemVerifier();
        }

        protected abstract Item GenerateItem();
        protected abstract Item GenerateItemFromName(string name, string power = null);
        protected abstract IEnumerable<string> GetItemNames();
        protected abstract void MakeSpecificAssertionsAgainst(Item item);

        protected void AssertItem(Item item)
        {
            ItemVerifier.AssertItem(item);
            MakeSpecificAssertionsAgainst(item);
        }

        protected string GetRandomName()
        {
            var names = GetItemNames();
            return collectionSelector.SelectRandomFrom(names);
        }

        protected Item GenerateItem(Func<Item, bool> additionalFilters)
        {
            var item = stressor.Generate(GenerateItem, i => additionalFilters(i));
            return item;
        }
    }
}
