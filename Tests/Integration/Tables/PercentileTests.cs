﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EquipmentGen.Mappers.Interfaces;
using EquipmentGen.Selectors.Decorators;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables
{
    [TestFixture]
    public abstract class PercentileTests : TableTests
    {
        [Inject]
        public IPercentileMapper PercentileMapper { get; set; }

        protected const String EmptyContent = "";

        private Dictionary<Int32, String> table;
        private Regex allCapsRegex;
        private readonly IEnumerable<String> replacementStrings;

        public PercentileTests()
        {
            allCapsRegex = new Regex("[A-Z]{4}[A-Z]+");
            replacementStrings = ReplacementStringConstants.GetAll();
        }

        [SetUp]
        public void Setup()
        {
            table = PercentileMapper.Map(tableName);
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

        private void AssertValidReplacementStrings(String value)
        {
            var matches = allCapsRegex.Matches(value);
            foreach (var match in matches)
                Assert.That(replacementStrings, Contains.Item(match.ToString()));
        }

        public virtual void Percentile(String content, Int32 lower, Int32 upper)
        {
            for (var roll = lower; roll <= upper; roll++)
                Percentile(content, roll);
        }

        public virtual void Percentile(String content, Int32 roll)
        {
            Assert.That(table.Keys, Contains.Item(roll), tableName);

            var message = String.Format("Roll: {0}", roll);
            Assert.That(table[roll], Is.EqualTo(content), message);
        }
    }
}