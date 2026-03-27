// use this static class to store any global data that needs to be accessed on runtime.

using UnityEngine;

public static class GlobalConstants
{
    // player pref keys
    public static readonly string musicKey = "music";
    public static readonly string soundKey = "sound";
    // ui texts
    public static readonly string musicText = "Music";
    public static readonly string soundText = "Sound";
    
    // save file location
    public static string saveFileLocation => Application.persistentDataPath + "/Saves/";    
    
    // check player prefs using this
    public static readonly string lastUsedSaveKey = "lastUsedSave";
    
    // game related constants
    public static readonly int maxAttributeValue = 18;
    public static readonly int startingUnspentPoints = 10;
}