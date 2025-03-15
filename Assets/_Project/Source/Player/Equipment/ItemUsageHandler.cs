using System;
using _Project.Source.Data;
using _Project.Source.Inventory;
using _Project.Source.Inventory.ItemTypesConfigs;
using UnityEngine;
using Zenject;

namespace _Project.Source.Player.Equipment
{
    public class ItemUsageHandler : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerCharacter;
        
        private Hotbar _hotbar;

        [Inject]
        public void Construct(Hotbar hotbar)
        {
            _hotbar = hotbar;
        }
        
        private void Start()
        {
            if (_hotbar != null)
                _hotbar.ItemSelected += UseItem;
        }

        private void OnDestroy()
        {
            if (_hotbar != null)
                _hotbar.ItemSelected -= UseItem;
        }
    
        private void UseItem(Item item)
        {
            if (item is IUsable usableItem)
            {
                Debug.Log("Player character " + _playerCharacter);
                usableItem.Use(_playerCharacter);
            }
        }
    }
}