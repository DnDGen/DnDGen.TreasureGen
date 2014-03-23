﻿using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentGen.Mappers.Interfaces;
using EquipmentGen.Tests.Integration.Common;
using EquipmentGen.Tests.Integration.Tables.TestAttributes;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables
{
    [TestFixture]
    public abstract class AttributesTests : IntegrationTests
    {
        [Inject]
        public IAttributesMapper AttributesMapper { get; set; }

        private Dictionary<String, IEnumerable<String>> table;

        [SetUp]
        public void Setup()
        {
            var tableName = GetTableNameFromAttribute();
            table = AttributesMapper.Map(tableName);
        }

        private String GetTableNameFromAttribute()
        {
            var type = GetType();
            var attributes = type.GetCustomAttributes(true);

            if (!attributes.Any(a => a is AttributesTableAttribute))
                throw new ArgumentException("This test class does not have the needed AttributesTableAttribute");

            var attributesFilenameAttribute = attributes.First(a => a is AttributesTableAttribute) as AttributesTableAttribute;
            return attributesFilenameAttribute.TableName;
        }

        protected void AssertContent(String name, IEnumerable<String> expectedAttributes)
        {
            if (!expectedAttributes.Any())
            {
                Assert.That(table.Keys, Is.Not.Contains(name));
                return;
            }

            Assert.That(table.Keys, Contains.Item(name));

            foreach (var expectedAttribute in expectedAttributes)
                Assert.That(table[name], Contains.Item(expectedAttribute));

            var tooMany = table[name].Except(expectedAttributes);
            var tooManyString = String.Join(", ", tooMany);
            var message = String.Format("Should not be in results: {0}", tooManyString);
            Assert.That(table[name].Count(), Is.EqualTo(expectedAttributes.Count()), message);
        }
    }
}