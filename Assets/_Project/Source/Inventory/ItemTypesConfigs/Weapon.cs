using _Project.Source.Data;
using _Project.Source.Player;
using _Project.Source.Player.Shooting;
using UnityEngine;

namespace _Project.Source.Inventory.ItemTypesConfigs
{
    [CreateAssetMenu(menuName = "Configs/Items/New Weapon")]
    public class Weapon : Item, IUsable
    {
        public int AmmoCapacity;
        public int Damage;
        public float ReloadSpeed;
        public float FireRate;
        public GameObject Prefab;
        public Projectile ProjectilePrefab;
        public float ProjectileLifeTime;
        public ConsumableItem AmmoType;
        
        public void Use(PlayerCharacter player)
        {
            player.Equipment.TryEquipWeapon(this);
        }
    }
}