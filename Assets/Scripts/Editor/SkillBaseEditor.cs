using DataModels;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SkillBase))]
    public class SkillBaseEditor : UnityEditor.Editor
    {
        // Identity
        SerializedProperty skillName;
        SerializedProperty handlerKey;
        SerializedProperty description;
        SerializedProperty icon;

        // Timing
        SerializedProperty cooldown;

        // Range
        SerializedProperty range;
        SerializedProperty useWeaponRange;

        // Targeting
        SerializedProperty targetData;
        SerializedProperty validTargets;
        SerializedProperty canMultiTarget;
        SerializedProperty maxTargets;

        // TargetData children (drawn manually instead of as a foldout block)
        SerializedProperty tdShape;
        SerializedProperty tdRadius;
        SerializedProperty tdWidth;
        SerializedProperty tdLength;
        SerializedProperty tdConeAngle;
        SerializedProperty tdMaterial;

        // Damage / Heal
        SerializedProperty useWeaponDamage;
        SerializedProperty weaponScalingFactor;
        SerializedProperty minValue;
        SerializedProperty maxValue;

        // Prerequisites
        SerializedProperty prerequisites;

        void OnEnable()
        {
            skillName           = serializedObject.FindProperty("skillName");
            handlerKey          = serializedObject.FindProperty("handlerKey");
            description         = serializedObject.FindProperty("description");
            icon                = serializedObject.FindProperty("icon");

            cooldown            = serializedObject.FindProperty("cooldown");
            

            targetData          = serializedObject.FindProperty("targetData");
            
            range               = targetData.FindPropertyRelative("range");
            useWeaponRange      = targetData.FindPropertyRelative("useWeaponRange");
            validTargets        = targetData.FindPropertyRelative("validTargets");
            canMultiTarget      = targetData.FindPropertyRelative("canMultiTarget");
            
            tdShape             = targetData.FindPropertyRelative("shape");
            tdRadius            = targetData.FindPropertyRelative("radius");
            tdWidth             = targetData.FindPropertyRelative("width");
            tdLength            = targetData.FindPropertyRelative("length");
            tdConeAngle         = targetData.FindPropertyRelative("coneAngle");
            maxTargets          = targetData.FindPropertyRelative("maxTargets");
            tdMaterial          = targetData.FindPropertyRelative("indicatorMaterialOverride");

            useWeaponDamage     = serializedObject.FindProperty("useWeaponDamage");
            weaponScalingFactor = serializedObject.FindProperty("weaponScalingFactor");
            minValue            = serializedObject.FindProperty("minValue");
            maxValue            = serializedObject.FindProperty("maxValue");

            prerequisites       = serializedObject.FindProperty("prerequisites");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ── Identity ──────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(skillName);
            EditorGUILayout.PropertyField(handlerKey);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(icon);

            DrawSeparator();
            EditorGUILayout.Space();

            // ── Timing ────────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(cooldown);

            EditorGUILayout.Space();

            // ── Range ─────────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Range", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useWeaponRange);

            if (!useWeaponRange.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(range);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.HelpBox("Range is taken from the equipped weapon.", MessageType.Info);
            }

            EditorGUILayout.Space();

            // ── Targeting ─────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Targeting", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(canMultiTarget);
            EditorGUILayout.PropertyField(validTargets);
            

            var mode = (ValidTargets)validTargets.enumValueIndex;

            // maxTargets only for Multiple... modes
            if (canMultiTarget.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(maxTargets);
                EditorGUI.indentLevel--;
            }

            // TargetData only for AoE modes — hidden for single-target and Self
            if (!canMultiTarget.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Target Shape", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(tdShape);

                var shape = (ShapeType)tdShape.enumValueIndex;

                switch (shape)
                {
                    case ShapeType.Circle:
                        EditorGUILayout.PropertyField(tdRadius);
                        EditorGUILayout.PropertyField(tdMaterial);
                        break;

                    case ShapeType.Rectangle:
                    case ShapeType.Line:
                    case ShapeType.Cross:
                        EditorGUILayout.PropertyField(tdWidth);
                        EditorGUILayout.PropertyField(tdLength);
                        EditorGUILayout.PropertyField(tdMaterial);
                        break;

                    case ShapeType.Cone:
                        EditorGUILayout.PropertyField(tdConeAngle);
                        EditorGUILayout.PropertyField(tdMaterial);
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // ── Damage / Heal Scaling ─────────────────────────────────────────
            EditorGUILayout.LabelField("Damage / Heal Scaling", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useWeaponDamage);

            EditorGUI.indentLevel++;
            if (useWeaponDamage.boolValue)
            {
                // Weapon-based: show scaling factor, hide min/max
                EditorGUILayout.PropertyField(weaponScalingFactor);
                EditorGUILayout.HelpBox("Min/Max values are overridden by weapon damage.", MessageType.Info);
            }
            else
            {
                // Fixed: show min/max, hide scaling factor
                EditorGUILayout.PropertyField(minValue);
                EditorGUILayout.PropertyField(maxValue);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // ── Prerequisites ─────────────────────────────────────────────────
            EditorGUILayout.PropertyField(prerequisites, includeChildren: true);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawSeparator(int thickness = 1, int padding = 6)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness + padding * 2);
            rect.y      += padding;
            rect.height  = thickness;
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
        }
    }

}
