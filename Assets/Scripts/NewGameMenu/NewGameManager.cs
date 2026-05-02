using System;
using System.Collections.Generic;
using DataModels;
using Enums;
using InGameManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewGameMenu
{
    public class NewGameManager : MonoBehaviour
    {
        #region UIElements

        // hold characterSO for each starting class
        [Header("Starting class data")]
        [SerializeField] private List<ClassDataEntry> startingClassData = new List<ClassDataEntry>();
        
        [Header("Cycle Pickers")]
        [SerializeField] private CyclePicker classPicker;
        [SerializeField] private CyclePicker portraitPicker;
        [SerializeField] private CyclePicker strengthPicker;
        [SerializeField] private CyclePicker dexterityPicker;
        [SerializeField] private CyclePicker agilityPicker;
        [SerializeField] private CyclePicker endurancePicker;
        [SerializeField] private CyclePicker intelligencePicker;
        [SerializeField] private CyclePicker willpowerPicker;
        
        [Header("Other UI elements")]
        [SerializeField] private TMP_Text unspentPointsText;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button startButton;

        #endregion

        
        private CharacterClass _selectedClass;
        private int _selectedPortraitId = 0;
        private int _classStartingPoints = 0;
        private int _unspentPoints = GlobalConstants.startingUnspentPoints;

        private void Awake()
        {
            nameInputField.onValueChanged.AddListener(_ => IsReadyToStart());
            classPicker.OnTextChanged.AddListener(UpdateClass);
            // since sprite db keys start from 0, i can use index directly.
            portraitPicker.OnIndexChanged.AddListener(UpdatePortrait);
            
            // register update unspent points callback for each attribute picker
            // for now, do it lazy and sum up values.
            strengthPicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            dexterityPicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            agilityPicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            endurancePicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            intelligencePicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            willpowerPicker.OnIntChanged.AddListener(_ => UpdateUnspentPoints());
            
            startButton.onClick.AddListener(StartGame);
        }

        private void Start()
        {
            SelectClass(CharacterClass.Warrior);
        }

        private void OnDestroy()
        {
            nameInputField.onValueChanged.RemoveListener(_ => IsReadyToStart());
            classPicker.OnTextChanged.RemoveListener(UpdateClass);
            portraitPicker.OnIndexChanged.RemoveListener(UpdatePortrait);
            
            // unregister callbacks to avoid memory leaks.
            strengthPicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            dexterityPicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            agilityPicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            endurancePicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            intelligencePicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            willpowerPicker.OnIntChanged.RemoveListener(_ => UpdateUnspentPoints());
            
            startButton.onClick.RemoveListener(StartGame);
        }

        public void SelectClass(CharacterClass characterClass)
        {
            _selectedClass = characterClass;
            // todo update attributes and reset unspent points.
            var characterSo = startingClassData.Find(entry => entry.key == characterClass).value;
            _classStartingPoints = characterSo.Attributes.Strength + characterSo.Attributes.Dexterity + characterSo.Attributes.Agility + 
                                   characterSo.Attributes.Endurance + characterSo.Attributes.Intelligence + characterSo.Attributes.Willpower;
            if (characterSo == null)
            {
                Debug.LogError("CharacterSO for class " + characterClass + " not found in startingClassData.");
                return;
            }
            // update attribute limits
            strengthPicker.SetIntLimits(characterSo.Attributes.Strength, GlobalConstants.maxAttributeValue);
            strengthPicker.SetIntValue(characterSo.Attributes.Strength);
            dexterityPicker.SetIntLimits(characterSo.Attributes.Dexterity, GlobalConstants.maxAttributeValue);
            dexterityPicker.SetIntValue(characterSo.Attributes.Dexterity);
            agilityPicker.SetIntLimits(characterSo.Attributes.Agility, GlobalConstants.maxAttributeValue);
            agilityPicker.SetIntValue(characterSo.Attributes.Agility);
            endurancePicker.SetIntLimits(characterSo.Attributes.Endurance, GlobalConstants.maxAttributeValue);
            endurancePicker.SetIntValue(characterSo.Attributes.Endurance);
            intelligencePicker.SetIntLimits(characterSo.Attributes.Intelligence, GlobalConstants.maxAttributeValue);
            intelligencePicker.SetIntValue(characterSo.Attributes.Intelligence);
            willpowerPicker.SetIntLimits(characterSo.Attributes.Willpower, GlobalConstants.maxAttributeValue);
            willpowerPicker.SetIntValue(characterSo.Attributes.Willpower);

            // update unspent points
            _unspentPoints = GlobalConstants.startingUnspentPoints;
            unspentPointsText.text = _unspentPoints.ToString();
        }
        
        // Can also show a warning instead of making the button non-interactable, but for now, just do it simple.
        private void IsReadyToStart()
        {
            // enable start button if name is not empty and unspent points are not negative.
            startButton.interactable = _unspentPoints == 0 && !string.IsNullOrEmpty(nameInputField.text);
        }
        private void UpdateUnspentPoints()
        {
            // sum up attribute values and update unspent points.
            int totalPoints = strengthPicker.CurrentIntValue + dexterityPicker.CurrentIntValue + agilityPicker.CurrentIntValue + 
                              endurancePicker.CurrentIntValue + intelligencePicker.CurrentIntValue + willpowerPicker.CurrentIntValue;
            _unspentPoints = _classStartingPoints + GlobalConstants.startingUnspentPoints - totalPoints;
            unspentPointsText.text = _unspentPoints.ToString();
            // update right button states manually. stupid but whatever.
            strengthPicker.SetRightButtonInteractable(_unspentPoints > 0);
            dexterityPicker.SetRightButtonInteractable(_unspentPoints > 0);
            agilityPicker.SetRightButtonInteractable(_unspentPoints > 0);
            endurancePicker.SetRightButtonInteractable(_unspentPoints > 0);
            intelligencePicker.SetRightButtonInteractable(_unspentPoints > 0);
            willpowerPicker.SetRightButtonInteractable(_unspentPoints > 0);
            IsReadyToStart();
        }

        private void UpdatePortrait(int index)
        {
            _selectedPortraitId = index;
        }
        private void UpdateClass(string className)
        {
            if (Enum.TryParse(className, out CharacterClass characterClass))
            {
                SelectClass(characterClass);
            }
            else
            {
                Debug.LogError("Invalid class name: " + className);
            }
        }
        private void StartGame()
        {
            // create character data and pass it to the next scene using a static class or a singleton.
            CharacterData characterData = new CharacterData
            {
                Name = nameInputField.text,
                Class = _selectedClass,
                Level = new(1),
                Xp = new(0),
                SpriteId = _selectedPortraitId,
                CurrentHp = new(100), // TODO: should be equal to max hp which will be calculated by attributes, but for now just set it to 100.
                Attributes = new DataModels.Attributes
                {
                    Strength = strengthPicker.CurrentIntValue,
                    Dexterity = dexterityPicker.CurrentIntValue,
                    Agility = agilityPicker.CurrentIntValue,
                    Endurance = endurancePicker.CurrentIntValue,
                    Intelligence = intelligencePicker.CurrentIntValue,
                    Willpower = willpowerPicker.CurrentIntValue
                }
            };
            Debug.Log("Character created: " + characterData.Name + ", Class: " + characterData.Class + ", Strength: " + characterData.Attributes.Strength);

            Save newSave = new Save(characterData.Name, new List<CharacterData>() { characterData });
            SaveManager.SaveGame(newSave);
        }
    }
    
    [Serializable]
    public struct ClassDataEntry
    {
        public CharacterClass key;
        public CharacterSo value;
    }
}
