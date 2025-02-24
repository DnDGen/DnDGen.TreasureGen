using DnDGen.TreasureGen.Selectors.Selections;

namespace DnDGen.TreasureGen.Items
{
    public class Damage
    {
        public string Roll { get; set; }
        public string Type { get; set; }
        public string Summary
        {
            get
            {
                var summary = $"{Roll} {Type}".Trim();

                if (IsConditional)
                {
                    summary += $" ({Condition})";
                }

                return summary;
            }

        }
        public string Condition { get; set; }
        public bool IsConditional => !string.IsNullOrEmpty(Condition);

        public Damage()
        {
            Roll = string.Empty;
            Type = string.Empty;
            Condition = string.Empty;
        }

        public Damage Clone()
        {
            return new Damage
            {
                Roll = Roll,
                Type = Type,
                Condition = Condition
            };
        }

        internal static Damage From(DamageDataSelection selection)
        {
            return new Damage
            {
                Roll = selection.Roll,
                Type = selection.Type,
                Condition = selection.Condition,
            };
        }
    }
}
