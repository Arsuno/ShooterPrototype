using System;
using _Project.Source.Inventory.ItemTypesConfigs;
using UnityEngine;

namespace _Project.Source.Player.Equipment
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private ItemUsageHandler _itemUsageHandler;
        
        private Weapon _equippedWeapon;
        
        public event Action<Weapon> WeaponEquipped;
        public event Action WeaponUnequipped;

        public void TryEquipWeapon(Weapon weapon)
        {
            if (_equippedWeapon == weapon)
            {
                UnequipWeapon();
                return;
            }

            _equippedWeapon = weapon;
            WeaponEquipped?.Invoke(weapon);
        }

        private void UnequipWeapon()
        {
            if (_equippedWeapon == null) return;
            
            _equippedWeapon = null;
            WeaponUnequipped?.Invoke();
        }
    }
}