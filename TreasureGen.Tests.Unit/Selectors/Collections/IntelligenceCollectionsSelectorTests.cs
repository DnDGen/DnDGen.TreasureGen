﻿using Moq;
using NUnit.Framework;
using TreasureGen.Domain.Selectors.Collections;
using TreasureGen.Domain.Tables;

namespace TreasureGen.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class IntelligenceCollectionsSelectorTests
    {
        private IIntelligenceDataSelector selector;
        private Mock<ICollectionsSelector> mockInnerSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionsSelector>();
            selector = new IntelligenceDataSelector(mockInnerSelector.Object);
        }

        [Test]
        public void ReturnIntelligenceResult()
        {
            var data = new string[3];
            data[DataIndexConstants.Intelligence.GreaterPowersCount] = "9266";
            data[DataIndexConstants.Intelligence.LesserPowersCount] = "42";
            data[DataIndexConstants.Intelligence.Senses] = "senses";

            mockInnerSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.IntelligenceData, "name")).Returns(data);

            var selection = selector.SelectFrom("name");
            Assert.That(selection.Senses, Is.EqualTo("senses"));
            Assert.That(selection.LesserPowersCount, Is.EqualTo(42));
            Assert.That(selection.GreaterPowersCount, Is.EqualTo(9266));
        }

        [Test]
        public void ThrowErrorIfFewerThan3Attributes()
        {
            var data = new[] { "senses", "42" };
            mockInnerSelector.Setup(s => s.SelectFrom(It.IsAny<string>(), It.IsAny<string>())).Returns(data);
            Assert.That(() => selector.SelectFrom("name"), Throws.Exception.With.Message.EqualTo("Data is not formatted for intelligence"));
        }
    }
}