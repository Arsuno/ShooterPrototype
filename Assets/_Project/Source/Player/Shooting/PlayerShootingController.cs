using System;
using System.Collections;
using _Project.Source.Inventory;
using _Project.Source.Inventory.ItemTypesConfigs;
using _Project.Source.Player.Equipment;
using _Project.Source.Player.Input;
using _Project.Source.Pools;
using UnityEngine;
using Zenject;

namespace _Project.Source.Player.Shooting
{
    public class PlayerShootingController : MonoBehaviour
    {
        private const float MAX_SHOOT_DISTANCE = 100f;
        private const float DEBUG_RAY_DURATION = 2f;
        private const float DEBUG_RAY_LENGTH = 100f;
        private const int PROJECTILES_BASE_POOL_SIZE = 15;
        
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private PlayerEquipmentView equipmentView;
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerEquipment _playerEquipment;
        [SerializeField] private Transform _projectilesParent;
        
        private Hotbar _hotbar;
        private ObjectPool<Projectile> _projectilePool = new();
        private Weapon _currentWeapon;
        private int _ammoInCurrentWeapon;
        private bool _reloading;
        
        public event Action<int, int> AmmoAmountChanged; 
        public event Action ReloadingStarted;
        public event Action WeaponUnequipped;

        [Inject]
        public void Construct(Hotbar hotbar)
        {
            _hotbar = hotbar;
        }
        
        private void Start()
        {
            _playerInputHandler.ShootButtonPressed += TryShoot;
            _playerInputHandler.ReloadButtonPressed += TryReload;
            
            _playerEquipment.WeaponEquipped += OnWeaponEquipped;
            _playerEquipment.WeaponUnequipped += OnWeaponUnequipped;
        }

        private void OnDestroy()
        {
            _playerInputHandler.ShootButtonPressed -= TryShoot;
            _playerInputHandler.ReloadButtonPressed -= TryReload;
            
            _playerEquipment.WeaponEquipped -= OnWeaponEquipped;
            _playerEquipment.WeaponUnequipped -= OnWeaponUnequipped;
        }

        private void TryShoot()
        {
            if (_currentWeapon == null) return;
            if (_camera == null) return;
            if (_ammoInCurrentWeapon <= 0) return;

            Shoot();
        }
        
        private void OnWeaponEquipped(Weapon weapon)
        {
            _currentWeapon = weapon;
            _ammoInCurrentWeapon = _currentWeapon.AmmoCapacity;
            
            AmmoAmountChanged?.Invoke(_ammoInCurrentWeapon, _currentWeapon.AmmoCapacity);
            _projectilePool.Initialize(_currentWeapon.ProjectilePrefab, PROJECTILES_BASE_POOL_SIZE, _projectilesParent);
        }
        
        private void OnWeaponUnequipped()
        {
            _currentWeapon = null;
            WeaponUnequipped?.Invoke();
        }

        private void Shoot()
        {
            var bulletSpawnTransform = equipmentView.CurrentWeaponObject.transform;
            
            Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            Vector3 shootDirection;

            if (Physics.Raycast(ray, out RaycastHit hit, MAX_SHOOT_DISTANCE))
            {
                shootDirection = (hit.point - bulletSpawnTransform.position).normalized;
                Debug.DrawRay(bulletSpawnTransform.position, shootDirection * DEBUG_RAY_LENGTH, Color.green, DEBUG_RAY_DURATION);
            }
            else
            {
                shootDirection = _camera.transform.forward;
                Debug.DrawRay(bulletSpawnTransform.position, shootDirection * DEBUG_RAY_LENGTH, Color.red, DEBUG_RAY_DURATION);
            }
            
            Projectile projectile = _projectilePool.GetObject();
            projectile.ReachedTarget += OnProjectileReachedTarget;
            projectile.transform.position = equipmentView.CurrentWeaponObject.transform.position;
            projectile.transform.rotation = Quaternion.LookRotation(shootDirection);
            projectile.Initialize(_currentWeapon.Damage, 100f, shootDirection, _currentWeapon.ProjectileLifeTime);

            _ammoInCurrentWeapon--;
            AmmoAmountChanged?.Invoke(_ammoInCurrentWeapon, _currentWeapon.AmmoCapacity);
        }

        private void TryReload()
        {
            Debug.Log("Try reloading");
            
            var ammoInHotbar = _hotbar.GetItemAmount(_currentWeapon.AmmoType);
            
            if (ammoInHotbar <= 0) return;

            var needAmmoForFullMagazine = _currentWeapon.AmmoCapacity - _ammoInCurrentWeapon;

            if (ammoInHotbar >= needAmmoForFullMagazine) // В хотбаре 45, а нужно 15
            {
                StartCoroutine(Reload(needAmmoForFullMagazine));
            }
            else
            {
                StartCoroutine(Reload(ammoInHotbar));
            }
        }

        private IEnumerator Reload(int ammo)
        {
            _reloading = true;
            var ammoBeforeReload = 0;
            ReloadingStarted?.Invoke();
            
            yield return new WaitForSeconds(_currentWeapon.ReloadSpeed);

            if (_ammoInCurrentWeapon > 0)
                ammoBeforeReload = _ammoInCurrentWeapon;

            _ammoInCurrentWeapon = ammoBeforeReload + ammo;
            _hotbar.RemoveItem(_currentWeapon.AmmoType, ammo);
            AmmoAmountChanged?.Invoke(_ammoInCurrentWeapon, _currentWeapon.AmmoCapacity);
            _reloading = false;
        }
        
        private void OnProjectileReachedTarget(Projectile projectile)
        {
            Debug.Log("Пуля достигла цели");
            _projectilePool.ReturnObject(projectile);
        }
    }
}