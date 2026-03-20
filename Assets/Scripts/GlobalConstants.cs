// use this static class to store any global data that needs to be accessed on runtime.

using UnityEngine;

public static class GlobalConstants
{
    public static readonly string saveFileLocation = Application.persistentDataPath + "/Saves/";
    // check player prefs using this
    public static readonly string lastUsedSaveKey = "lastUsedSave";
}