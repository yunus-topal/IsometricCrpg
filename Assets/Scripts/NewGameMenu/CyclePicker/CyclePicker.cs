using Databases;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NewGameMenu.CyclePicker
{
    /// <summary>
    /// Generic cycle-picker prefab component.
    /// Configure the mode and data directly in the Inspector (or at runtime via code).
    ///
    /// Modes:
    ///   Image   — cycles through a Sprite array displayed in an Image component.
    ///   Text    — cycles through a string array displayed in a TMP_Text component.
    ///   Integer — increments / decrements an int within [min, max] by step.
    ///             Limits and step can be changed at any time at runtime.
    ///
    /// Wire OnIndexChanged / OnTextChanged / OnIntChanged in the Inspector.
    /// Call ResetPicker() to snap back to the start state.
    /// </summary>
    public class CyclePicker : MonoBehaviour
    {
        public enum PickerMode { Image, Text, Integer, SpriteDb }

        // ── Inspector ─────────────────────────────────────────────────────────────

        [Header("Mode")]
        [SerializeField] private PickerMode mode = PickerMode.Text;

        [Header("Image mode")]
        [SerializeField] private Sprite[] sprites;

        [Header("Text mode")]
        [SerializeField] private string[] options;

        [Header("Integer mode")]
        [SerializeField] private int intMin     = 0;
        [SerializeField] private int intMax     = 10;
        [SerializeField] private int intStep    = 1;
        [SerializeField] private int intDefault = 0;
        
        [Header("Sprite db mode")]
        public CharacterSpriteDb spriteDb;

        [Header("Settings")]
        [Tooltip("Image and Text modes only. Wraps around at the ends instead of stopping.")]
        [SerializeField] private bool wrap = true;

        [Header("UI References")]
        [SerializeField] private Button   btnLeft;
        [SerializeField] private Button   btnRight;
        [SerializeField] private Image    displayImage;
        [SerializeField] private TMP_Text displayText;

        [Header("Events")]
        public UnityEvent<int>    OnIndexChanged;  // Image / Text: fires with current index
        public UnityEvent<string> OnTextChanged;   // Text only: fires with the current string
        public UnityEvent<int>    OnIntChanged;    // Integer only: fires with current int value

        // ── State ─────────────────────────────────────────────────────────────────

        private int _index;     // Image / Text modes
        private int _intValue;  // Integer mode

        // ── Public read-only accessors ────────────────────────────────────────────

        public PickerMode Mode            => mode;
        public int        CurrentIndex    => _index;
        public string     CurrentText     => (options != null && _index < options.Length) ? options[_index] : string.Empty;
        public int        CurrentIntValue => _intValue;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        private void Awake()
        {
            btnLeft .onClick.AddListener(Previous);
            btnRight.onClick.AddListener(Next);
        }

        private void Start()
        {
            _index    = 0;
            _intValue = SnapToStep(Mathf.Clamp(intDefault, intMin, intMax));
            Refresh();
        }

        private void OnDestroy()
        {
            if (btnLeft)  btnLeft .onClick.RemoveListener(Previous);
            if (btnRight) btnRight.onClick.RemoveListener(Next);
        }

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>Advance one step forward.</summary>
        public void Next()
        {
            switch (mode)
            {
                case PickerMode.SpriteDb:
                case PickerMode.Image:
                case PickerMode.Text:
                    StepIndex(+1);
                    break;

                case PickerMode.Integer:
                    StepInt(+intStep);
                    break;
            }
        }

        /// <summary>Go back one step.</summary>
        public void Previous()
        {
            switch (mode)
            { 
                case PickerMode.SpriteDb:
                case PickerMode.Image:
                case PickerMode.Text:
                    StepIndex(-1);
                    break;

                case PickerMode.Integer:
                    StepInt(-intStep);
                    break;
            }
        }

        /// <summary>
        /// Reset to initial state:
        ///   Image / Text  → index 0
        ///   Integer       → intMin
        /// Fires the relevant event after resetting.
        /// </summary>
        public void ResetPicker()
        {
            switch (mode)
            {
                case PickerMode.SpriteDb:
                case PickerMode.Image:
                case PickerMode.Text:
                    _index = 0;
                    Refresh();
                    FireIndexEvent();
                    break;

                case PickerMode.Integer:
                    _intValue = SnapToStep(intMin);
                    Refresh();
                    FireIntEvent();
                    break;
            }
        }

        // ── Runtime setters ───────────────────────────────────────────────────────

        /// <summary>
        /// Update integer limits at runtime. The current value is clamped and
        /// re-snapped immediately. Pass notify:true to fire OnIntChanged.
        /// </summary>
        public void SetIntLimits(int min, int max, int step = 1, bool notify = false)
        {
            intMin  = min;
            intMax  = max;
            intStep = Mathf.Max(1, step);
            _intValue = SnapToStep(Mathf.Clamp(_intValue, intMin, intMax));
            Refresh();
            if (notify) FireIntEvent();
        }

        /// <summary>Jump directly to an index (Image / Text mode only).</summary>
        public void SetIndex(int index)
        {
            if (mode == PickerMode.Integer)
            {
                Debug.LogWarning("[CyclePicker] SetIndex is not valid in Integer mode. Use SetIntValue.");
                return;
            }
            int count = ItemCount();
            if (count == 0) return;
            _index = wrap
                ? ((index % count) + count) % count
                : Mathf.Clamp(index, 0, count - 1);
            Refresh();
            FireIndexEvent();
        }

        /// <summary>Set the integer value directly. Snapped to step grid and clamped.</summary>
        public void SetIntValue(int value)
        {
            if (mode != PickerMode.Integer)
            {
                Debug.LogWarning("[CyclePicker] SetIntValue is not valid outside of Integer mode.");
                return;
            }
            _intValue = SnapToStep(Mathf.Clamp(value, intMin, intMax));
            Refresh();
            FireIntEvent();
        }

        /// <summary>Replace the sprite list at runtime and switch to Image mode.</summary>
        public void SetSprites(Sprite[] newSprites, bool resetIndex = true)
        {
            mode    = PickerMode.Image;
            sprites = newSprites;
            _index  = resetIndex ? 0 : Mathf.Clamp(_index, 0, Mathf.Max(0, sprites.Length - 1));
            Refresh();
            FireIndexEvent();
        }

        /// <summary>Replace the options list at runtime and switch to Text mode.</summary>
        public void SetOptions(string[] newOptions, bool resetIndex = true)
        {
            mode    = PickerMode.Text;
            options = newOptions;
            _index  = resetIndex ? 0 : Mathf.Clamp(_index, 0, Mathf.Max(0, options.Length - 1));
            Refresh();
            FireIndexEvent();
        }

        // ── Step logic (separated per mode) ──────────────────────────────────────

        private void StepIndex(int direction)
        {
            int count = ItemCount();
            if (count == 0) return;

            int next = _index + direction;
            _index = wrap
                ? ((next % count) + count) % count
                : Mathf.Clamp(next, 0, count - 1);

            Refresh();
            FireIndexEvent();
        }

        private void StepInt(int delta)
        {
            // Clamp directly — no wrapping for integers
            _intValue = SnapToStep(Mathf.Clamp(_intValue + delta, intMin, intMax));
            Refresh();
            FireIntEvent();
        }

        // ── Display refresh ───────────────────────────────────────────────────────

        private void Refresh()
        {
            switch (mode)
            {
                case PickerMode.SpriteDb:
                    SetDisplayMode(showImage: true);
                    RefreshButtonStates(ItemCount());
                    if (ItemCount() > 0 && displayImage != null)
                        displayImage.sprite = spriteDb.TryGet(_index);
                    break;
                case PickerMode.Image:
                    SetDisplayMode(showImage: true);
                    RefreshButtonStates(ItemCount());
                    if (ItemCount() > 0 && displayImage != null)
                        displayImage.sprite = sprites[_index];
                    break;

                case PickerMode.Text:
                    SetDisplayMode(showImage: false);
                    RefreshButtonStates(ItemCount());
                    if (ItemCount() > 0 && displayText != null)
                        displayText.text = options[_index];
                    break;

                case PickerMode.Integer:
                    SetDisplayMode(showImage: false);
                    if (displayText != null)
                        displayText.text = _intValue.ToString();
                    // Buttons disabled at the hard limits
                    if (btnLeft)  btnLeft .interactable = _intValue > intMin;
                    if (btnRight) btnRight.interactable = _intValue < intMax;
                    break;
            }
        }

        private void RefreshButtonStates(int count)
        {
            bool hasContent = count > 0;
            if (wrap)
            {
                // Always interactable when wrapping (as long as there is content)
                if (btnLeft)  btnLeft .interactable = hasContent;
                if (btnRight) btnRight.interactable = hasContent;
            }
            else
            {
                if (btnLeft)  btnLeft .interactable = hasContent && _index > 0;
                if (btnRight) btnRight.interactable = hasContent && _index < count - 1;
            }
        }

        // ── Event helpers ─────────────────────────────────────────────────────────

        private void FireIndexEvent()
        {
            OnIndexChanged?.Invoke(_index);
            if (mode == PickerMode.Text)
                OnTextChanged?.Invoke(CurrentText);
        }

        private void FireIntEvent()
        {
            OnIntChanged?.Invoke(_intValue);
        }

        // ── Utilities ─────────────────────────────────────────────────────────────

        private int ItemCount()
        {
            switch (mode)
            {
                case PickerMode.SpriteDb: return spriteDb != null ? spriteDb.Sprites.Count : 0;
                case PickerMode.Image: return sprites != null ? sprites.Length : 0;
                case PickerMode.Text:  return options != null ? options.Length  : 0;
                default:               return 0; // integers don't use an item count
            }
        }

        /// <summary>Rounds value down to the nearest valid step from intMin.</summary>
        private int SnapToStep(int value)
        {
            if (intStep <= 1) return value;
            int offset = value - intMin;
            return intMin + (offset / intStep) * intStep;
        }

        private void SetDisplayMode(bool showImage)
        {
            if (displayImage != null) displayImage.gameObject.SetActive(showImage);
            if (displayText  != null) displayText .gameObject.SetActive(!showImage);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            intStep = Mathf.Max(1, intStep);
            intDefault = SnapToStep(Mathf.Clamp(intDefault, intMin, intMax));
            if (!Application.isPlaying && displayText != null) Refresh();
        }
#endif
    }
}
