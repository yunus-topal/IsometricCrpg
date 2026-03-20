using System;
using System.Collections.Generic;

namespace DataModels
{
    [Serializable]
    public class Save
    {
        public string SaveName { get; set; } = "New Save";
        public List<Character> PlayerCharacters { get; set; } = new List<Character>();
        public DateTime SaveTime { get; set; } = DateTime.Now;
        // save quests, inventory, etc. later
    }
}