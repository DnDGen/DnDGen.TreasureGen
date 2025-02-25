using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Art
{
    [TestFixture]
    public class ArtDescriptionsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.GOODTYPEDescriptions(GoodsConstants.Art); }
        }

        [TestCase("1d10*10", "silver ewer",
                             "carved bone statuette",
                             "carved ivory statuette",
                             "finely wrought small gold bracelet")]
        [TestCase("3d6*10", "cloth of gold vestments",
                            "black velvet mask with numerous citrines",
                            "silver chalice with lapis lazuli gems")]
        [TestCase("1d6*100", "large well-done wool tapestry",
                             "brass mug with jade inlays")]
        [TestCase("1d10*100", "silver comb with moonstones",
                              "silver-plated steel longsword with jet jewel in hilt")]
        [TestCase("2d6*100", "carved harp of exotic wood with ivory inlay and zircon gems",
                             "10 lb. solid gold idol")]
        [TestCase("3d6*100", "gold dragon comb with red garnet eye",
                             "gold and topaz bottle stopper cork",
                             "ceremonial electrum dagger with a star ruby in the pommel")]
        [TestCase("4d6*100", "eyepatch with mock eye of sapphire and moonstone",
                             "fire opal pendant on a fine gold chain",
                             "old masterpiece painting")]
        [TestCase("5d6*100", "embroidered silk and velvet mantle with numerous moonstones",
                             "sapphire pendant on gold chain")]
        [TestCase("1d4*1000", "embroidered and bejeweled glove",
                              "jeweled anklet",
                              "gold music box")]
        [TestCase("1d6*1000", "golden circlet with four aquamarines",
                              "a necklace of small pink pearls")]
        [TestCase("2d4*1000", "jeweled gold crown",
                              "jeweled electrum ring")]
        [TestCase("2d6*1000", "gold and ruby ring",
                              "gold cup set with emeralds")]
        public void Collection(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}