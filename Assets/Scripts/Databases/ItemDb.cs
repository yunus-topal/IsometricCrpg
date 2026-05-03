using System.Collections.Generic;
using System.Linq;
using DataModels;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Items")]
    public class ItemDb : DatabaseBase
    {
        public Sprite placeholder;
        public List<ItemBase> Items = new List<ItemBase>();
        
        private Dictionary<string, ItemBase> _itemsDict = new();
        
        private void OnEnable()
        {
            _itemsDict.Clear();
            foreach (var item in Items)
            {
                if (item == null) continue;
                if (item.icon == null) item.icon = placeholder;
        
                if (string.IsNullOrEmpty(item.ItemId))
                    Debug.LogError($"Item '{item.itemName}' is missing an ItemId.");
                else
                    _itemsDict[item.ItemId] = item;
            }
            
            Debug.Log($"[ItemDb] Loaded {_itemsDict.Count} items into the database.");
        }
        
        public List<ItemBase> GetAllItems()
        {
            return Items;
        }
        public ItemBase GetItemById(string id)
        {
            if (_itemsDict.TryGetValue(id, out var item))
                return item;

            Debug.LogError($"Item with id '{id}' not found in ItemDatabase.");
            return null;
        }
        
        public List<ItemBase> GetItemsByIds(List<string> ids)
        {
            List<ItemBase> items = new List<ItemBase>();
            foreach (var id in ids)
            {
                var item = GetItemById(id);
                if (item != null)
                    items.Add(item);
            }
            return items;
        }
        
    }
}
