using DataModels;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterMainUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI characterHealthText;
        [SerializeField] private Slider characterHealthSlider;

        [SerializeField] private Transform equipmentSkills;
        [SerializeField] private Transform classSkills;
        [SerializeField] private Transform itemSkills;

        [SerializeField] private GameObject skillUIPrefab;

        [CanBeNull] private RuntimeCharData _charData;

        public void Initialize(RuntimeCharData charData)
        {
            if (_charData != null)
            {
                RemoveListeners(_charData);
            }
            _charData = charData;
            _charData!.CharacterData.CurrentHp.OnValueChanged += OnHealthChanged;
            _charData!.MaxHp.OnValueChanged += OnHealthChanged;
            OnHealthChanged(_charData.CharacterData.CurrentHp.Value);

            characterImage.sprite = _charData.Sprite;
            SetupSkills();
        }

        private void SetupSkills()
        {
            if (_charData == null)
            {
                Debug.LogError("[CharacterMainUI] needs to be initialized");
                return; 
            }
            // setup class skills
            foreach (var skill in _charData.Skills)
            {
                var skillUI = Instantiate(skillUIPrefab, classSkills).GetComponent<SkillUI>();
                skillUI.Initialize(skill);
            }
            
            // setup equipment skills
            foreach (var equipment in _charData.Equipments)
            {
                foreach (var skill in equipment.item.grantedSkills)
                {
                    var skillUI = Instantiate(skillUIPrefab, equipmentSkills).GetComponent<SkillUI>();
                    skillUI.Initialize(skill);
                }
            }
            
            // todo: setup inventory skills.
        }

        private void RemoveListeners(RuntimeCharData charData)
        {
            charData.CharacterData.CurrentHp.OnValueChanged -= OnHealthChanged;
        }


        private void OnHealthChanged(int _)
        {
            int? currentHp = _charData?.CharacterData.CurrentHp.Value;
            int? maxHp = _charData?.MaxHp.Value;
            if (maxHp == null || currentHp == null)
            {
                Debug.LogError("[CharacterMainUI] charData is missing!");
                return;
            }
            characterHealthText.text = $"{currentHp}/{maxHp}";
            characterHealthSlider.value = (float) currentHp / maxHp.Value;
        }
        
        private void OnDestroy()
        {
            // 3. THE CRITICAL STEP: 
            // If the UI is destroyed, we MUST tell the persistent data to let go.
            if (_charData != null)
            {
                RemoveListeners(_charData);
            }
        }
    }
}
