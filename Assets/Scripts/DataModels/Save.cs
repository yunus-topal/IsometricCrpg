using System;
using System.Collections.Generic;

namespace DataModels
{
    [Serializable]
    public class Save
    {
        public string SaveName;
        public List<CharacterData> PlayerCharacters;

        public string SaveTime;
        // save quests, inventory, etc. later

        public Save()
        {
            SaveName = "New Save";
            PlayerCharacters = new List<CharacterData>();
            SaveTime = DateTime.Now.ToString("o");
        }
        public Save (string saveName, List<CharacterData> playerCharacters)
        {
            SaveName = saveName;
            PlayerCharacters = playerCharacters;
            SaveTime = DateTime.Now.ToString("o");
        }
    }
}