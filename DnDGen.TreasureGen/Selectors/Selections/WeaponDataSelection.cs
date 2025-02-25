using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Selectors.Selections
{
    internal class WeaponDataSelection : DataSelection<WeaponDataSelection>
    {
        public int ThreatRange { get; set; }
        public string Ammunition { get; set; }
        public string CriticalMultiplier { get; set; }
        public string SecondaryCriticalMultiplier { get; set; }

        public List<DamageDataSelection> Damages { get; set; }
        public List<DamageDataSelection> CriticalDamages { get; set; }

        public WeaponDataSelection()
        {
            Damages = [];
            CriticalDamages = [];
        }

        public override Func<string[], WeaponDataSelection> MapTo => Map;
        public override Func<WeaponDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 4;

        public static WeaponDataSelection Map(string[] splitData)
        {
            var selection = new WeaponDataSelection
            {
                ThreatRange = Convert.ToInt32(splitData[DataIndexConstants.Weapon.ThreatRange]),
                Ammunition = splitData[DataIndexConstants.Weapon.Ammunition],
                CriticalMultiplier = splitData[DataIndexConstants.Weapon.CriticalMultiplier],
                SecondaryCriticalMultiplier = splitData[DataIndexConstants.Weapon.SecondaryCriticalMultiplier]
            };

            return selection;
        }

        public static string[] Map(WeaponDataSelection selection)
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.ThreatRange] = selection.ThreatRange.ToString();
            data[DataIndexConstants.Weapon.Ammunition] = selection.Ammunition;
            data[DataIndexConstants.Weapon.CriticalMultiplier] = selection.CriticalMultiplier;
            data[DataIndexConstants.Weapon.SecondaryCriticalMultiplier] = selection.SecondaryCriticalMultiplier;

            return data;
        }
    }
}
