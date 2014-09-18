﻿using System;
using System.Linq;
using EquipmentGen.Common.Coins;
using EquipmentGen.Generators.Interfaces.Coins;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Coins
{
    [TestFixture]
    public class CoinGeneratorTests : StressTests
    {
        [Inject]
        public ICoinGenerator CoinGenerator { get; set; }

        [TestCase("Coin generator")]
        public override void Stress(String thingToStress)
        {
            Stress();
        }

        protected override void MakeAssertions()
        {
            var coin = GenerateCoin();

            Assert.That(coin.Currency, Is.Not.Null);
            Assert.That(coin.Quantity, Is.GreaterThanOrEqualTo(0));
        }

        private Coin GenerateCoin()
        {
            var level = GetNewLevel();
            return CoinGenerator.GenerateAtLevel(level);
        }

        [Test]
        public void CurrencyHappens()
        {
            Coin coin;

            do coin = GenerateCoin();
            while (TestShouldKeepRunning() && String.IsNullOrEmpty(coin.Currency));

            Assert.That(coin.Currency, Is.Not.Empty);
            Assert.That(coin.Quantity, Is.Positive);
            AssertIterations();
        }

        [Test]
        public void CurrencyDoesNotHappen()
        {
            Coin coin;

            do coin = GenerateCoin();
            while (TestShouldKeepRunning() && coin.Currency.Any());

            Assert.That(coin.Currency, Is.Empty);
            Assert.That(coin.Quantity, Is.EqualTo(0));
            AssertIterations();
        }
    }
}