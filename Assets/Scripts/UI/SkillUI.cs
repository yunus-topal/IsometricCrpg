using DataModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        
        private SkillBase _skill;

        public void Initialize(SkillBase skill)
        {
            _skill = skill;
            skillIcon.sprite = skill.icon;
        }
        
        // todo: handle hover event to show skill details, handle click event to use skill, etc.
    }
}
