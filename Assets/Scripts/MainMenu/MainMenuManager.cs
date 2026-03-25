using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        // buttons
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button optionsButton;
        
        // panels
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject savePanel;
        [SerializeField] private GameObject optionsPanel;
        
        // others
        [SerializeField] private TextMeshProUGUI saveNameText;
        [SerializeField] private GameObject saveFilePrefab;
        [SerializeField] private Transform saveFileListContainer;

        private void Awake()
        {
            SaveManager.SetupSaveFolder(); // Ensure the save folder is set up at the start
            CheckSaveFiles();
            
            
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueGame);
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(NewGame);
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(LoadGame);
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitApplication);
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OpenOptionsPanel);
        }

        private void CheckSaveFiles()
        {
            // get player prefs
            var lastUsedSave = PlayerPrefs.GetString(GlobalConstants.lastUsedSaveKey, string.Empty);
            // try to load the last used save file
            var lastSave = SaveManager.LoadSave(lastUsedSave);
            if (lastSave != null)
            {
                DisableAllPanels();
                mainPanel.SetActive(true);
                saveNameText.text = lastSave.SaveName; // Display the last used save name
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }

        private void QuitApplication()
        {
            Application.Quit();
        }

        private void OpenOptionsPanel()
        {
            DisableAllPanels();
            optionsPanel.SetActive(true);
        }

        private void DisableAllPanels()
        {
            mainPanel.SetActive(false);
            savePanel.SetActive(false);
            optionsPanel.SetActive(false);
        }
        
        private void ContinueGame()
        {
            SceneManager.LoadScene("GameScene");
        }
        
        // TODO: create new game scene to setup character and everything.
        private void NewGame()
        {
            SceneManager.LoadScene("CharacterCreationMenu");
        }
        private void LoadGame()
        {
            DisableAllPanels();
            savePanel.SetActive(true);
            SetupSaveScrollView();
        }
        
        private void SetupSaveScrollView()
        {
            // Clear existing save file entries
            foreach (Transform child in saveFileListContainer)
            {
                Destroy(child.gameObject);
            }
            // Get all save files
            var saveFiles = SaveManager.GetAllSaveFiles();
            // Populate the scroll view with save files
            foreach (var saveFile in saveFiles)
            {
                var saveFileObject = Instantiate(saveFilePrefab, saveFileListContainer);
                // set parent
                var saveFileUIHelper = saveFileObject.GetComponent<SaveFileUIHelper>();
                if (saveFileUIHelper != null)
                {
                    saveFileUIHelper.Initialize(saveFile);
                }
            }
        }
    }
}
