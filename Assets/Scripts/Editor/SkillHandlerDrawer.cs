using System;
using Attributes;

namespace Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(SkillHandlerKeyAttribute))]
    public class SkillHandlerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string current = property.stringValue;
            string display = string.IsNullOrEmpty(current) ? "— None —" : current;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            if (GUI.Button(position, display, EditorStyles.popup))
            {
                var methods = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.GetCustomAttribute<SkillCollectionAttribute>() != null)
                    .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public).
                        Select(m => $"{t.Name.Replace("Executables", "")}/{m.Name}")) // e.g. "WarriorSkills/Charge"
                    .ToArray();

                SkillSearchPopup.Show(position, methods, current, selected =>
                {
                    // strip the part before the slash to get just the method name, which is what we store in the SkillHandlerKey
                    selected = selected.Split('/').Last();
                    property.stringValue = selected;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            EditorGUI.EndProperty();
        }
    }

    public class SkillSearchPopup : EditorWindow
    {
        private string[] _options;
        private string _search = "";
        private string _current;
        private System.Action<string> _onSelected;
        private Vector2 _scroll;

        // Filtered list recomputed when search changes
        private string[] _filtered;
        private string _lastSearch;

        public static void Show(Rect buttonRect, string[] options, string current, System.Action<string> onSelected)
        {
            var win = CreateInstance<SkillSearchPopup>();
            win._options = options;
            win._current = current;
            win._onSelected = onSelected;
            win.UpdateFilter();

            // Convert button rect to screen space and open just below it
            var screenRect = GUIUtility.GUIToScreenRect(buttonRect);
            win.ShowAsDropDown(screenRect, new Vector2(screenRect.width, 220));
        }

        private void OnGUI()
        {
            // ── Search bar ───────────────────────────────────────────────
            GUI.SetNextControlName("SearchField");
            EditorGUI.BeginChangeCheck();
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
                UpdateFilter();

            // Auto-focus the search field when the window opens
            EditorGUI.FocusTextInControl("SearchField");

            // ── Scrollable results ───────────────────────────────────────
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            if (_filtered.Length == 0)
            {
                EditorGUILayout.LabelField("No results.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var option in _filtered)
                {
                    bool isCurrent = option == _current;

                    // Highlight the currently selected entry
                    var style = new GUIStyle(EditorStyles.label);
                    if (isCurrent)
                    {
                        style.fontStyle = FontStyle.Bold;
                        style.normal.textColor = new Color(0.2f, 0.6f, 1f);
                    }

                    if (GUILayout.Button(option, style))
                    {
                        _onSelected?.Invoke(option);
                        Close();
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            // Close on Escape
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                Close();
        }

        private void UpdateFilter()
        {
            if (_options == null) return;

            _filtered = string.IsNullOrWhiteSpace(_search)
                ? _options
                : _options
                    .Where(o => o.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray();
        }
    }
#endif
}