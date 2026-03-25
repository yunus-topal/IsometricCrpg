using DataModels;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class SaveFileUIHelper : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI saveNameText;
        private Save SaveFile { get; set; }
        public void Initialize(Save saveFile)
        {
            SaveFile = saveFile;
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnSaveFileSelected);
            saveNameText.text = saveFile.SaveName;
            // TODO add more fields to prefab and populate here accordingly.

        }

        private void OnSaveFileSelected()
        {
            Debug.Log($"Selected save file: {SaveFile.SaveName}");
            // load save file and then game scene
            SaveManager.CurrentSave = SaveFile;
            SceneManager.LoadScene("GameScene");
        }
    }
}
