using DnDGen.Infrastructure.Helpers;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Armors
{
    [TestFixture]
    public class ArmorDataTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.ArmorData;

        [TestCase(ArmorConstants.Buckler, 1, -1, int.MaxValue)]
        [TestCase(ArmorConstants.LightWoodenShield, 1, -1, int.MaxValue)]
        [TestCase(ArmorConstants.LightSteelShield, 1, -1, int.MaxValue)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 2, -2, int.MaxValue)]
        [TestCase(ArmorConstants.HeavySteelShield, 2, -2, int.MaxValue)]
        [TestCase(ArmorConstants.TowerShield, 4, -10, 2)]
        [TestCase(ArmorConstants.PaddedArmor, 1, 0, 8)]
        [TestCase(ArmorConstants.LeatherArmor, 2, 0, 6)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, 3, -1, 5)]
        [TestCase(ArmorConstants.ChainShirt, 4, -2, 4)]
        [TestCase(ArmorConstants.HideArmor, 3, -3, 4)]
        [TestCase(ArmorConstants.ScaleMail, 4, -4, 3)]
        [TestCase(ArmorConstants.Chainmail, 5, -5, 2)]
        [TestCase(ArmorConstants.Breastplate, 5, -4, 3)]
        [TestCase(ArmorConstants.SplintMail, 6, -7, 0)]
        [TestCase(ArmorConstants.BandedMail, 6, -6, 1)]
        [TestCase(ArmorConstants.HalfPlate, 7, -7, 0)]
        [TestCase(ArmorConstants.FullPlate, 8, -6, 1)]
        public void ArmorData(string name, int armorBonus, int armorCheckPenalty, int maxDexterityBonus)
        {
            var data = DataHelper.Parse(new ArmorDataSelection
            {
                ArmorBonus = armorBonus,
                ArmorCheckPenalty = armorCheckPenalty,
                MaxDexterityBonus = maxDexterityBonus,
            });

            AssertOrderedCollections(name, data);
        }
    }
}
