namespace Enums
{
    public enum WeaponType
    {
        Sword,
        Axe,
        Mace,
        Bow,
        Crossbow,
        Staff,
        Wand,
        // ignore these 3 for now.
        Dagger, // can have different special attack
        Spear, // can have different special attack
        Unarmed // should definitely have a different special attack, 
    }
    
    [System.Flags]
    public enum WeaponTypeFlags
    {
        Sword = 1 << 0,
        Axe = 1 << 1,
        Mace = 1 << 2,
        Bow = 1 << 3,
        Crossbow = 1 << 4,
        Staff = 1 << 5,
        Wand = 1 << 6,
        Dagger = 1 << 7,
        Spear = 1 << 8,
        Unarmed = 1 << 9,
    }

    public enum ArmorType
    {
        Light,
        Medium,
        Heavy,
        Robe
    }
    
    // for equipment slots. only for equippable items. not all item types need to have a corresponding slot, e.g. quest items, consumables, throwables don't need slots.
    public enum EquipmentSlot
    {
        None,
        Weapon1, // 2 handed weapons will always be on weapon1 slot, and weapon2 slot will be empty. for 1 handed weapons, they will be on weapon1 slot by default, but can be moved to weapon2 slot if player wants dual wielding.
        Weapon2, // can be a 1 handed weapon or a shield.
        Helmet,
        Armor,
        Gauntlet,
        Boots,
        Talisman1,
        Talisman2,
    }
}