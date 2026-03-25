using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalConstants;

namespace MainMenu
{
    public class SliderConfig : MonoBehaviour
    {
        private enum SliderType
        {
            Music,
            Sound,
            // TODO: sound effects etc...
        }
        
        [SerializeField] private TextMeshProUGUI sliderText;
        [SerializeField] private Slider slider;
        [SerializeField] private SliderType sliderType;
        
        private string _sliderKey = String.Empty;
        public void Start()
        {
            _sliderKey = sliderType switch
            {
                SliderType.Music => musicKey,
                SliderType.Sound => soundKey,
                // SliderType.SoundEffects => soundEffectsKey,
                _ => string.Empty
            };
            
            sliderText.text = sliderType switch
            {
                SliderType.Music => musicText,
                SliderType.Sound => soundText,
                _ => string.Empty
            };
            slider.onValueChanged.AddListener(UpdateValue);
            
            // get slider value from player prefs
            if (PlayerPrefs.HasKey(_sliderKey))
            {
                slider.value = PlayerPrefs.GetFloat(_sliderKey);
            }
            else
            {
                // set default value
                slider.value = 1f;
                PlayerPrefs.SetFloat(_sliderKey, 1f);
            }
        }

        private void UpdateValue(float f)
        {
            PlayerPrefs.SetFloat(_sliderKey, f);
        }
    }
}
