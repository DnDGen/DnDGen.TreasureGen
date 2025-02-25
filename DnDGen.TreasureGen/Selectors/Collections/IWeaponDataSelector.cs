using DnDGen.TreasureGen.Selectors.Selections;

namespace DnDGen.TreasureGen.Selectors.Collections
{
    internal interface IWeaponDataSelector
    {
        WeaponDataSelection Select(string name, string size);
    }
}
