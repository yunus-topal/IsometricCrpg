using System;
using System.IO;
using System.Linq;
using DataModels;
using JetBrains.Annotations;
using UnityEngine;

namespace Managers
{
    // TODO: think about putting an internal id to save files to avoid issues with renaming save files.
    // TODO: handle autosave and quicksave too.
    public static class SaveManager
    {
        [CanBeNull] private static Save currentSave;
        public static Save CurrentSave
        {
            get
            {
                if (currentSave == null)
                {
                    var lastUsedSave = PlayerPrefs.GetString(GlobalConstants.lastUsedSaveKey, string.Empty);
                    currentSave = LoadSave(lastUsedSave);
                }
                return currentSave;
            }
            set
            {
                currentSave = value;
                if (currentSave != null)
                {
                    PlayerPrefs.SetString(GlobalConstants.lastUsedSaveKey, currentSave.SaveName);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey(GlobalConstants.lastUsedSaveKey);
                }
            }
        }    
        
        public static bool OverwriteSave(Save newSave, Save oldSave)
        {
            DeleteSave(oldSave);
            return SaveGame(newSave);
        }
        public static bool SaveGame(Save save)
        {
            if (save == null)
            {
                Debug.LogError("Cannot save a null SaveFile.");
                return false;
            }
            if (string.IsNullOrEmpty(save.SaveName))
            {
                Debug.LogError("SaveFile must have a valid SaveName.");
                return false;
            }
            save.SaveTime = DateTime.Now.ToString("o"); // Update the save date to current time
            return WriteToDisk(save);
        }

        public static bool DeleteSave(Save save)
        {
            if (save == null)
            {
                Debug.LogError("Cannot delete a null SaveFile.");
                return false;
            }
            var filePath = Path.Combine(GlobalConstants.saveFileLocation, $"{save.SaveName}.json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"Save file '{save.SaveName}' deleted successfully.");
                    return true;
                }
                Debug.LogWarning($"Save file '{save.SaveName}' does not exist.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete the save file: {ex.Message}");
                return false;
            }
        }

        public static Save LoadSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogWarning("No save name provided to load.");
                return null;
            }
            var filePath = Path.Combine(GlobalConstants.saveFileLocation, $"{saveName}.json");
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var saveFile = JsonUtility.FromJson<Save>(json);
                    Debug.Log($"Save file '{saveName}' loaded successfully.");
                    return saveFile;
                }
                Debug.LogWarning($"Save file '{saveName}' does not exist.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load the save file: {ex.Message}");
                return null;
            }
            
        }
        
        public static Save[] GetAllSaveFiles()
        {
            var saveFiles = Directory.GetFiles(GlobalConstants.saveFileLocation, "*.json")
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .Select(fileName => LoadSave(fileName))
                .Where(saveFile => saveFile != null)
                .ToArray();
            return saveFiles;
        }
        
        public static void SetupSaveFolder()
        {
            if (!Directory.Exists(GlobalConstants.saveFileLocation))
            {
                try
                {
                    Directory.CreateDirectory(GlobalConstants.saveFileLocation);
                    Debug.Log($"Save file directory created at {GlobalConstants.saveFileLocation}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create save file directory: {ex.Message}");
                }
            }
            else
            {
                Debug.Log($"Save file directory already exists at {GlobalConstants.saveFileLocation}");
            }
        }
        
        private static bool WriteToDisk(Save saveFile)
        {
            var filePath = Path.Combine(GlobalConstants.saveFileLocation, $"{saveFile.SaveName}.json");
            try
            {
                Debug.Log($"save name: {saveFile.SaveName}");
                // write newtonsoft json to file
                string json =  JsonUtility.ToJson(saveFile);

                Debug.Log($"Saving save file to {filePath} with content: {json}");
                File.WriteAllText(filePath, json);
                Debug.Log($"Save file '{saveFile.SaveName}' saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save the save file: {ex.Message}");
                return false;
            }
        }
    }
}