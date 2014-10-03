﻿using System;
using EquipmentGen.Common.Goods;
using EquipmentGen.Tables.Interfaces;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Goods.Gems
{
    [TestFixture]
    public class GemDescriptionsTests : AttributesTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Attributes.Formattable.GOODTYPEDescriptions, GoodsConstants.Gem); }
        }

        [TestCase("4d4", "eye agate",
                         "moss agate",
                         "banded agate",
                         "azurite",
                         "blue quartz",
                         "hematite",
                         "lapis lazuli",
                         "malachite",
                         "obsidian",
                         "rhodochrosite",
                         "tiger eye turquoise",
                         "irregular freshwater pearl")]
        [TestCase("2d4*10", "bloodstone",
                            "carnelian",
                            "chalcedony",
                            "chrysoprase",
                            "citrine",
                            "iolite",
                            "jasper",
                            "moonstone",
                            "onyx",
                            "peridot",
                            "clear quartz rock crystal",
                            "sard",
                            "sardonyx",
                            "rose quartz",
                            "smoky rose quartz",
                            "star rose quartz",
                            "zircon")]
        [TestCase("4d4*10", "amber",
                            "amethyst",
                            "chrysoberyl",
                            "coral",
                            "red garnet",
                            "brown-green garnet",
                            "jade",
                            "jet",
                            "white pearl",
                            "golden pearl",
                            "pink pearl",
                            "silver pearl",
                            "red spinel",
                            "red-brown spinel",
                            "deep green spinel",
                            "tourmaline")]
        [TestCase("2d4*100", "alexandrite",
                             "aquamarine",
                             "violet garnet",
                             "black pearl",
                             "deep blue spinel",
                             "golden yellow topaz")]
        [TestCase("4d4*100", "emerald",
                             "white opal",
                             "black opal",
                             "fire opal",
                             "blue sapphire",
                             "fiery yellow corundum",
                             "rich purple corundum",
                             "blue star sapphire",
                             "black star sapphire",
                             "star ruby")]
        [TestCase("2d4*1000", "clearest bright green emerald",
                              "blue-white diamond",
                              "canary diamond",
                              "pink diamond",
                              "brown diamond",
                              "blue diamond",
                              "jacinth")]
        public override void Attributes(String name, params String[] attributes)
        {
            base.Attributes(name, attributes);
        }
    }
}