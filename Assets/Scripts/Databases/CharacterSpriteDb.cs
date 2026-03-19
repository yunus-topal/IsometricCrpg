using System;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Character Sprites")]
    public sealed class CharacterSpriteDb : ScriptableObject
    {
        [Serializable]
        public struct CharacterSpriteEntry
        {
            public string Id;
            public Sprite Sprite;
        }

        [SerializeField] private List<CharacterSpriteEntry> entries = new();

        public CharacterSpriteEntry TryGet(string id)
        {
            foreach (var entry in entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            // return an empty sprite entry if not found, should be handled on runtime to avoid null reference exception.
            Debug.LogError("Sprite with id " + id + " not found in CharacterSpriteDatabase.");
            return new CharacterSpriteEntry { Id = id, Sprite = null };
        }
    }
}