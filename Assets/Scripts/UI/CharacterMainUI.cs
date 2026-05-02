using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterMainUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshPro characterHealthText;
        [SerializeField] private Slider characterHealthSlider;

        [SerializeField] private Transform equipmentSkills;
        [SerializeField] private Transform classSkills;
        [SerializeField] private Transform itemSkills;

        [SerializeField] private GameObject skillUIPrefab;
    }
}
