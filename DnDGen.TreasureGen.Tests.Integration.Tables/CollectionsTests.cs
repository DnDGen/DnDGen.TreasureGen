using DnDGen.Infrastructure.Mappers.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    [TestFixture]
    public abstract class CollectionsTests : TableTests
    {
        protected CollectionMapper CollectionMapper;
        protected Dictionary<string, IEnumerable<string>> table;

        [SetUp]
        public void CollectionsSetup()
        {
            CollectionMapper = GetNewInstanceOf<CollectionMapper>();
            table = CollectionMapper.Map(Name, tableName);
        }

        protected void AssertCollection(string name, params string[] items)
        {
            Assert.That(table.Keys, Contains.Item(name), tableName);
            AssertCollection(table[name], items, name);
        }

        protected void AssertCollection(IEnumerable<string> actual, IEnumerable<string> expected, string message = null)
        {
            Assert.That(actual, Is.EquivalentTo(expected), message);
        }

        protected void AssertOrderedCollections(string name, params string[] items)
        {
            Assert.That(table.Keys, Contains.Item(name), tableName);
            AssertOrderedCollection(table[name], items);
        }

        protected void AssertOrderedCollection(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            AssertCollection(actual, expected);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}