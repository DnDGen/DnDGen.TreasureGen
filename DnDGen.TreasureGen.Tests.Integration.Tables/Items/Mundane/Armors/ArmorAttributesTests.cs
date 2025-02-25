using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Armors
{
    [TestFixture]
    public class ArmorAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor); }
        }

        [TestCase(ArmorConstants.Buckler,
            AttributeConstants.Wood,
            AttributeConstants.Shield)]
        [TestCase(ArmorConstants.LightWoodenShield,
            AttributeConstants.Wood,
            AttributeConstants.Shield)]
        [TestCase(ArmorConstants.LightSteelShield,
            AttributeConstants.Metal,
            AttributeConstants.Shield)]
        [TestCase(ArmorConstants.HeavyWoodenShield,
            AttributeConstants.Wood,
            AttributeConstants.Shield)]
        [TestCase(ArmorConstants.HeavySteelShield,
            AttributeConstants.Metal,
            AttributeConstants.Shield)]
        [TestCase(ArmorConstants.TowerShield,
            AttributeConstants.Wood,
            AttributeConstants.Shield,
            AttributeConstants.TowerShield)]
        [TestCase(ArmorConstants.PaddedArmor)]
        [TestCase(ArmorConstants.LeatherArmor)]
        [TestCase(ArmorConstants.StuddedLeatherArmor, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.ChainShirt, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.HideArmor)]
        [TestCase(ArmorConstants.ScaleMail, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.Chainmail, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.Breastplate, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.SplintMail, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.BandedMail, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.HalfPlate, AttributeConstants.Metal)]
        [TestCase(ArmorConstants.FullPlate, AttributeConstants.Metal)]
        public void ArmorAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}