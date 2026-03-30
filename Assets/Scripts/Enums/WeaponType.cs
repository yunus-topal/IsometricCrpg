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
}