namespace Enums
{
    public enum CharacterClass
    {
        Warrior,
        Ranger,
        Mage,
        Priest
    }

    public static class CharacterClassExtensions
    {
        public static string GetDescription(this CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return "Warriors are strong and resilient fighters, excelling in close combat and defense.";
                case CharacterClass.Ranger:
                    return "Rangers are agile and skilled marksmen, specializing in ranged attacks and stealth.";
                case CharacterClass.Mage:
                    return "Mages are powerful spellcasters, harnessing the forces of magic to deal damage and control the battlefield.";
                case CharacterClass.Priest:
                    return "Priests are devoted healers and support characters, using their divine powers to aid allies and hinder enemies.";
                default:
                    return "Unknown character class.";
            }
        }
    }
}