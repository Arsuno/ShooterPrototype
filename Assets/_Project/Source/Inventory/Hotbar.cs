using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Source.Data;
using _Project.Source.Factories;
using _Project.Source.Inventory.Configs;
using _Project.Source.Inventory.ItemTypesConfigs;
using _Project.Source.SaveLoadSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Source.Inventory
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField] private HotbarConfig _config;
        
        private Dictionary<Item, HotbarSlot> _itemToSlot = new();
        private readonly List<HotbarSlot> _slots = new();
        
        private int _slotsCount;
        private GameData _loadedData;
        private ISaveLoadSystem _saveLoadSystem;
        
        public List<HotbarSlot> Slots => _slots;
        
        public event Action<List<HotbarSlot>> HotbarSlotsChanged;
        public event Action<Item> ItemSelected;

        [Inject]
        public void Construct(GameDataFactory gameDataFactory, ISaveLoadSystem saveLoadSystem)
        {
            _loadedData = gameDataFactory.Create();
            _saveLoadSystem = saveLoadSystem;
        }
        
        private void Start()
        {
            Initialize(_loadedData);
        }
    
        private void Update()
        {
            for (int i = 0; i < _slotsCount; i++)
            {
                Key key = (Key)((int)Key.Digit1 + i);
                
                if (Keyboard.current[key].wasPressedThisFrame)
                    SelectSlot(i);
            }
        }

        private List<HotbarSlotData> GetSlotsData()
        {
            return _slots.Select(
                t => new HotbarSlotData
                {
                    Item = t.Item, ItemAmount = t.ItemAmount
                }).ToList();
        }
        
        public void AddItem(Item item, int amount)
        {
            if (_itemToSlot.TryGetValue(item, out var existingSlot))
            {
                existingSlot.AddItemAmount(amount);
                HotbarSlotsChanged?.Invoke(_slots);
                Save();
                return;
            }

            foreach (var slot in _slots.Where(slot => slot.Item == null))
            {
                slot.AssignItem(item, amount);
                _itemToSlot[item] = slot;
                HotbarSlotsChanged?.Invoke(_slots);
                Save();
                return;
            }

            Debug.Log("Hotbar is full!");
        }

        public void RemoveItem(Item item, int amount)
        {
            if (!_itemToSlot.TryGetValue(item, out var slot)) 
                return;
            
            slot.TryRemoveItemAmount(amount);

            if (slot.Item == null)
                _itemToSlot.Remove(item);

            HotbarSlotsChanged?.Invoke(_slots);
            Save();
        }

        public int GetItemAmount(Item item)
        {
            if (_itemToSlot.TryGetValue(item, out var slot))
                return slot.ItemAmount;
            
            return 0;
        }
        
        private void Initialize(GameData data)
        {
            ResetHotbar();

            if (data == null)
                InitializeWithDefaultItems();
            else
                InitializeFromSaveData(data);

            HotbarSlotsChanged?.Invoke(_slots);
        }

        private void ResetHotbar()
        {
            _slots.Clear();
            _itemToSlot.Clear();
    
            _slotsCount = _config.SlotsAmount;
    
            for (int i = 0; i < _slotsCount; i++)
                _slots.Add(new HotbarSlot());
        }

        private void InitializeWithDefaultItems()
        {
            Debug.Log("No save data, using default items.");
    
            for (var i = 0; i < _slotsCount; i++)
            {
                if (i < _config.StartItems.Length)
                {
                    var item = _config.StartItems[i];
                    _slots[i].AssignItem(item, item.StartAmount);
                    _itemToSlot[item] = _slots[i];
                }
            }
        }

        private void InitializeFromSaveData(GameData data)
        {
            Debug.Log($"Loading {data.HotbarSlots.Count} slots from save.");

            for (var i = 0; i < _slotsCount; i++)
            {
                if (i < data.HotbarSlots.Count)
                {
                    var slotData = data.HotbarSlots[i];
                    var item = slotData.Item;
                    var amount = slotData.ItemAmount;

                    _slots[i].AssignItem(item, amount);

                    if (item != null)
                        _itemToSlot[item] = _slots[i];
                }
            }
        }

        private void Save()
        {
            var data = new GameData()
            {
                HotbarSlots = GetSlotsData()
            };
                    
            _saveLoadSystem.Save(data);
        }
    
        private void SelectSlot(int index)
        {
            if (index < 0 || index >= _slots.Count)
                return;

            var slot = _slots[index];
            
            if (slot.Item != null)
                ItemSelected?.Invoke(slot.Item);
        }
    }
}