using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Mundane
{
    public static class AlchemicalItemConstants
    {
        public const string AlchemistsFire = "Alchemist's Fire";
        public const string Acid = "Acid";
        public const string Smokestick = "Smokestick";
        public const string HolyWater = "Holy Water";
        public const string Antitoxin = "Antitoxin";
        public const string EverburningTorch = "Everburning Torch";
        public const string TanglefootBag = "Tanglefoot Bag";
        public const string Thunderstone = "Thunderstone";

        public static IEnumerable<string> GetAllAlchemicalItems() =>
            [
                AlchemistsFire,
                Acid,
                Smokestick,
                HolyWater,
                Antitoxin,
                EverburningTorch,
                TanglefootBag,
                Thunderstone
            ];
    }
}
