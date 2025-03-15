using _Project.Source.Enemy.Configs;
using _Project.Source.Player;
using UnityEngine;

namespace _Project.Source.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyConfig _config;
        [SerializeField] private EnemyMovement _enemyMovement;
        [SerializeField] private UnitWeaponView unitWeaponView;
        [SerializeField] private EnemyShootingController _enemyShootingController;
        
        private PlayerCharacter _player;
        private float _detectionRadius;

        private void Start()
        {
            _detectionRadius = _config.DetectionRadius;
            
            _enemyMovement.Initialize(_config);
            unitWeaponView.Display(_config.Weapon);
            _enemyShootingController.Initialize(_config);
        }

        private void Update()
        {
            if (DetectPlayer())
            {
                _enemyShootingController.StartShooting(_player);
            }
            else
            {
                _enemyShootingController.StopShooting();
                _enemyMovement.GoPatrolling();
            }
        }

        private bool DetectPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRadius);
            
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out PlayerCharacter player))
                {
                    _player = player;
                    
                    Vector3 direction = _player.transform.position - gameObject.transform.position;
                    direction.y = 0;
                    
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime);
                    }
                    
                    return true;
                }
            }

            _player = null;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}