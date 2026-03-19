using System;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Character Sprites")]
    public sealed class CharacterSpriteDb : ScriptableObject
    {
        public Sprite emptySprite; // use this as a placeholder for missing sprites, should be handled on runtime to avoid null reference exception.
        public List<Sprite> Sprites = new List<Sprite>();
        
        private Dictionary<int, Sprite> _spritesDict = new Dictionary<int, Sprite>();
        public Dictionary<int, Sprite> SpritesDict => _spritesDict;
        
        private void OnEnable()
        {
            // populate the dictionary with the sprites from the list, using the index as the id.
            _spritesDict.Clear();
            for (int i = 0; i < Sprites.Count; i++)
            {
                _spritesDict.Add(i, Sprites[i]);
            }
        }
        
        
        public Sprite TryGet(int id)
        {
            // try to get the sprite from the dictionary, if not found, return null and log an error.
            if (_spritesDict.TryGetValue(id, out var sprite))            {
                return sprite;
            }
            // return an empty sprite entry if not found, should be handled on runtime to avoid null reference exception.
            Debug.LogError("Sprite with id " + id + " not found in CharacterSpriteDatabase.");
            return emptySprite;
        }
    }
}