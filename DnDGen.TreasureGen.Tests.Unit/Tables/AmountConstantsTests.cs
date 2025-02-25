using DnDGen.RollGen;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Tables
{
    [TestFixture]
    public class AmountConstantsTests
    {
        [TestCase(AmountConstants.Range0, 0)]
        [TestCase(AmountConstants.Range1, 1)]
        [TestCase(AmountConstants.Range1d3, 1, 3)]
        [TestCase(AmountConstants.Range1d3Plus1, 1, 3, 1, 1)]
        [TestCase(AmountConstants.Range1d3Plus2, 1, 3, 1, 2)]
        [TestCase(AmountConstants.Range1d3Plus4, 1, 3, 1, 4)]
        [TestCase(AmountConstants.Range1d3Plus6, 1, 3, 1, 6)]
        [TestCase(AmountConstants.Range1d3Plus9, 1, 3, 1, 9)]
        [TestCase(AmountConstants.Range1d3Plus12, 1, 3, 1, 12)]
        [TestCase(AmountConstants.Range1d3Plus17, 1, 3, 1, 17)]
        [TestCase(AmountConstants.Range1d3Plus23, 1, 3, 1, 23)]
        [TestCase(AmountConstants.Range1d3Plus31, 1, 3, 1, 31)]
        [TestCase(AmountConstants.Range1d3Plus42, 1, 3, 1, 42)]
        [TestCase(AmountConstants.Range1d4, 1, 4)]
        [TestCase(AmountConstants.Range1d4x10, 1, 4, 10)]
        [TestCase(AmountConstants.Range1d4x100, 1, 4, 100)]
        [TestCase(AmountConstants.Range1d4x1000, 1, 4, 1000)]
        [TestCase(AmountConstants.Range1d4x10000, 1, 4, 10000)]
        [TestCase(AmountConstants.Range1d6, 1, 6)]
        [TestCase(AmountConstants.Range1d6x100, 1, 6, 100)]
        [TestCase(AmountConstants.Range1d6x1000, 1, 6, 1000)]
        [TestCase(AmountConstants.Range1d6x10000, 1, 6, 10000)]
        [TestCase(AmountConstants.Range1d8, 1, 8)]
        [TestCase(AmountConstants.Range1d8x10, 1, 8, 10)]
        [TestCase(AmountConstants.Range1d8x100, 1, 8, 100)]
        [TestCase(AmountConstants.Range1d8x1000, 1, 8, 1000)]
        [TestCase(AmountConstants.Range1d10, 1, 10)]
        [TestCase(AmountConstants.Range1d10x10, 1, 10, 10)]
        [TestCase(AmountConstants.Range1d10x100, 1, 10, 100)]
        [TestCase(AmountConstants.Range1d10x1000, 1, 10, 1000)]
        [TestCase(AmountConstants.Range1d10x10000, 1, 10, 10000)]
        [TestCase(AmountConstants.Range1d12, 1, 12)]
        [TestCase(AmountConstants.Range1d12x10, 1, 12, 10)]
        [TestCase(AmountConstants.Range1d12x100, 1, 12, 100)]
        [TestCase(AmountConstants.Range1d12x1000, 1, 12, 1000)]
        [TestCase(AmountConstants.Range1d12x10000, 1, 12, 10000)]
        [TestCase(AmountConstants.Range1d20, 1, 20)]
        [TestCase(AmountConstants.Range1d100, 1, 100)]
        [TestCase(AmountConstants.Range2d3, 2, 3)]
        [TestCase(AmountConstants.Range2d4, 2, 4)]
        [TestCase(AmountConstants.Range2d4x10, 2, 4, 10)]
        [TestCase(AmountConstants.Range2d4x100, 2, 4, 100)]
        [TestCase(AmountConstants.Range2d4x1000, 2, 4, 1000)]
        [TestCase(AmountConstants.Range2d6, 2, 6)]
        [TestCase(AmountConstants.Range2d6x100, 2, 6, 100)]
        [TestCase(AmountConstants.Range2d6x1000, 2, 6, 1000)]
        [TestCase(AmountConstants.Range2d6x10000, 2, 6, 10000)]
        [TestCase(AmountConstants.Range2d8, 2, 8)]
        [TestCase(AmountConstants.Range2d8x10, 2, 8, 10)]
        [TestCase(AmountConstants.Range2d8x100, 2, 8, 100)]
        [TestCase(AmountConstants.Range2d8x1000, 2, 8, 1000)]
        [TestCase(AmountConstants.Range2d10, 2, 10)]
        [TestCase(AmountConstants.Range2d10x100, 2, 10, 100)]
        [TestCase(AmountConstants.Range2d10x1000, 2, 10, 1000)]
        [TestCase(AmountConstants.Range2d12, 2, 12)]
        [TestCase(AmountConstants.Range2d12x10, 2, 12, 10)]
        [TestCase(AmountConstants.Range2d100, 2, 100)]
        [TestCase(AmountConstants.Range3d3, 3, 3)]
        [TestCase(AmountConstants.Range3d4, 3, 4)]
        [TestCase(AmountConstants.Range3d4x10, 3, 4, 10)]
        [TestCase(AmountConstants.Range3d4x100, 3, 4, 100)]
        [TestCase(AmountConstants.Range3d4x1000, 3, 4, 1000)]
        [TestCase(AmountConstants.Range3d6, 3, 6)]
        [TestCase(AmountConstants.Range3d6x10, 3, 6, 10)]
        [TestCase(AmountConstants.Range3d6x100, 3, 6, 100)]
        [TestCase(AmountConstants.Range3d6x1000, 3, 6, 1000)]
        [TestCase(AmountConstants.Range3d8, 3, 8)]
        [TestCase(AmountConstants.Range3d8x1000, 3, 8, 1000)]
        [TestCase(AmountConstants.Range3d10, 3, 10)]
        [TestCase(AmountConstants.Range3d10x100, 3, 10, 100)]
        [TestCase(AmountConstants.Range3d10x1000, 3, 10, 1000)]
        [TestCase(AmountConstants.Range3d12, 3, 12)]
        [TestCase(AmountConstants.Range3d12x1000, 3, 12, 1000)]
        [TestCase(AmountConstants.Range3d20, 3, 20)]
        [TestCase(AmountConstants.Range4d4, 4, 4)]
        [TestCase(AmountConstants.Range4d4x10, 4, 4, 10)]
        [TestCase(AmountConstants.Range4d4x100, 4, 4, 100)]
        [TestCase(AmountConstants.Range4d6, 4, 6)]
        [TestCase(AmountConstants.Range4d6x100, 4, 6, 100)]
        [TestCase(AmountConstants.Range4d8, 4, 8)]
        [TestCase(AmountConstants.Range4d8x100, 4, 8, 100)]
        [TestCase(AmountConstants.Range4d8x1000, 4, 8, 1000)]
        [TestCase(AmountConstants.Range4d10, 4, 10)]
        [TestCase(AmountConstants.Range4d10x10, 4, 10, 10)]
        [TestCase(AmountConstants.Range4d10x100, 4, 10, 100)]
        [TestCase(AmountConstants.Range4d12, 4, 12)]
        [TestCase(AmountConstants.Range4d12x1000, 4, 12, 1000)]
        [TestCase(AmountConstants.Range4d20, 4, 20)]
        [TestCase(AmountConstants.Range5d4x100, 5, 4, 100)]
        [TestCase(AmountConstants.Range5d6x10, 5, 6, 10)]
        [TestCase(AmountConstants.Range5d6x100, 5, 6, 100)]
        [TestCase(AmountConstants.Range6d4x100, 6, 4, 100)]
        [TestCase(AmountConstants.Range6d6, 6, 6)]
        [TestCase(AmountConstants.Range7d6, 7, 6)]
        public void RangeConstant(string constant, int quantity, int die = 1, int multiplier = 1, int bonus = 0)
        {
            var roll = $"{quantity}d{die}";

            if (bonus > 0)
                roll += $"+{bonus}";

            if (die == 1)
                roll = quantity.ToString();

            if (multiplier > 1)
            {
                var lower = quantity * multiplier + bonus;
                var upper = quantity * die * multiplier + bonus;
                roll = RollHelper.GetRollWithFewestDice(lower, upper);
            }

            Assert.That(constant, Is.EqualTo(roll));
        }
    }
}
