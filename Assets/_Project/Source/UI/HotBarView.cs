using System.Collections.Generic;
using UnityEngine;
using _Project.Source.Inventory;
using Zenject;

namespace _Project.Source.UI
{
    public class HotBarView : MonoBehaviour
    {
        [SerializeField] private Transform _slotsParent;
        [SerializeField] private SlotView _slotPrefab;
        
        private List<SlotView> _spawnedSlots = new();
        private Hotbar _hotbar;
        
        [Inject]
        public void Construct(Hotbar hotbar)
        {
            _hotbar = hotbar;
            
            Refresh(_hotbar.Slots);
        }

        private void Start()
        {
            _hotbar.HotbarSlotsChanged += Refresh;
        }

        private void OnDestroy()
        {
            _hotbar.HotbarSlotsChanged -= Refresh;
        }
        
        private void Refresh(List<HotbarSlot> slots)
        {
            ClearSlots();
            AddSlots(slots);
        }

        private void AddSlots(List<HotbarSlot> slots)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                SlotView slot = Instantiate(_slotPrefab, _slotsParent);
                _spawnedSlots.Add(slot);

                if (slots[i].Item != null)
                {
                    slot.SetIcon(slots[i].Item.Icon);
                    slot.SetAmountTextValue(slots[i].ItemAmount);    
                }
            }
        }

        private void ClearSlots()
        {
            foreach (var slot in _spawnedSlots)
                Destroy(slot.gameObject);
            
            _spawnedSlots.Clear();
        }
    }
}