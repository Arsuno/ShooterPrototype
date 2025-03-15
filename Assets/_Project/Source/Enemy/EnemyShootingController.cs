using System;
using System.Collections;
using _Project.Source.Enemy.Configs;
using _Project.Source.Inventory.ItemTypesConfigs;
using _Project.Source.Player;
using _Project.Source.Player.Shooting;
using _Project.Source.Pools;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Source.Enemy
{
    public class EnemyShootingController : MonoBehaviour
    {
        [FormerlySerializedAs("unitEquipmentView")] [FormerlySerializedAs("enemyWeaponFactory")] [FormerlySerializedAs("_enemyWeaponSpawner")] [SerializeField] private UnitWeaponView unitWeaponView;
        [SerializeField] private Transform _projectilesParent;

        private Projectile _projectilePrefab;
        private float _fireRate;
        private float _projectileLifeTime;
        private bool _isShooting;
        private Coroutine _shootingCoroutine;
        private ObjectPool<Projectile> _projectilePool = new();
        private Weapon _currentWeapon;
        
        public void Initialize(EnemyConfig config)
        {
            _projectilePrefab = config.Weapon.ProjectilePrefab;
            _fireRate = config.Weapon.FireRate;
            _projectileLifeTime = config.Weapon.ProjectileLifeTime;
            _currentWeapon = config.Weapon;
            
            _projectilePool.Initialize(_projectilePrefab, 15, _projectilesParent);
        }

        public void StartShooting(PlayerCharacter player)
        {
            if (_isShooting) return;
            
            _isShooting = true;
            _shootingCoroutine = StartCoroutine(ShootAtPlayer(player));
        }

        public void StopShooting()
        {
            if (!_isShooting) return;

            if (_shootingCoroutine != null)
            {
                StopCoroutine(_shootingCoroutine);
                _isShooting = false;
            }
        }
        
        private void Shoot(PlayerCharacter player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            Vector3 shootDirection = (player.transform.position - unitWeaponView.CurrentWeaponObject.transform.position).normalized;
            
            Projectile projectile = _projectilePool.GetObject();
            projectile.ReachedTarget += OnProjectileReachedTarget;
            projectile.transform.position = unitWeaponView.CurrentWeaponObject.transform.position;
            projectile.transform.rotation = Quaternion.LookRotation(shootDirection);
            projectile.Initialize(_currentWeapon.Damage, 50f, shootDirection, _projectileLifeTime);
        }

        private void OnProjectileReachedTarget(Projectile projectile)
        {
            _projectilePool.ReturnObject(projectile);
        }

        private IEnumerator ShootAtPlayer(PlayerCharacter player)
        {
            while (true)
            {
                yield return new WaitForSeconds(_fireRate);
                Shoot(player);
            }
        }
    }
}