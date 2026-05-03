using System.Collections.Generic;
using System.Linq;
using Databases;
using DataModels;
using UnityEditor;
using UnityEngine;

namespace Editor
{
public enum SyncableType
    {
        Items,
        Skills,
        StatusEffects,
    }

    public class DbSyncWindow : EditorWindow
    {
        private SyncableType _selectedType = SyncableType.Items;

        [MenuItem("RPG/Sync Database")]
        public static void Open() => GetWindow<DbSyncWindow>("DB Sync");

        private void OnGUI()
        {
            GUILayout.Label("Database Sync", EditorStyles.boldLabel);
            GUILayout.Space(6);

            _selectedType = (SyncableType)EditorGUILayout.EnumPopup("Type", _selectedType);
            GUILayout.Space(10);

            if (GUILayout.Button("Sync", GUILayout.Height(30)))
                Sync(_selectedType);
            
            if (GUILayout.Button("SyncAll", GUILayout.Height(30)))
                SyncAll();
        }

        private static void Sync(SyncableType type)
        {
            switch (type)
            {
                case SyncableType.Items:
                    SyncDb<ItemBase, ItemDb>(db => db.Items);
                    break;
                case SyncableType.Skills:
                    SyncDb<SkillBase, SkillDb>(db => db.Skills);
                    break;
                case SyncableType.StatusEffects:
                    SyncDb<StatusEffectBase, StatusEffectDb>(db => db.StatusEffects);
                    break;
            }
        }
        
        private static void SyncAll()
        {
            SyncDb<ItemBase, ItemDb>(db => db.Items);
            SyncDb<SkillBase, SkillDb>(db => db.Skills);
            SyncDb<StatusEffectBase, StatusEffectDb>(db => db.StatusEffects);
        }

        private static void SyncDb<TAsset, TDb>(System.Func<TDb, List<TAsset>> getList)
            where TAsset : ScriptableObject
            where TDb : ScriptableObject
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}")
                .Select(g => AssetDatabase.LoadAssetAtPath<TAsset>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(a => a != null)
                .OrderBy(a => a.name)
                .ToList();

            var dbGuids = AssetDatabase.FindAssets($"t:{typeof(TDb).Name}");
            if (dbGuids.Length == 0)
            {
                Debug.LogWarning($"[DbSync] No {typeof(TDb).Name} found in project.");
                return;
            }

            foreach (var guid in dbGuids)
            {
                var db = AssetDatabase.LoadAssetAtPath<TDb>(AssetDatabase.GUIDToAssetPath(guid));
                getList(db).Clear();
                getList(db).AddRange(assets);
                EditorUtility.SetDirty(db);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[DbSync] Synced {assets.Count} {typeof(TAsset).Name}(s) into {dbGuids.Length} {typeof(TDb).Name}(s).");
        }
    }
}