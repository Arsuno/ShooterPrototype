using TMPro;
using UnityEngine;

namespace _Project.Source.UI
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private GameObject _fillObject;
        
        [SerializeField] private float _healthDivider = 100f;
        [SerializeField] private float _fixedScaleY = 1f;
        [SerializeField] private float _fixedScaleZ = 1f;

        private void OnEnable()
        {
            _health.Changed += OnHealthChanged;
        }

        private void OnDisable()
        {
            _health.Changed -= OnHealthChanged;   
        }
        
        private void OnHealthChanged()
        { 
            var scaleValue = _health.CurrentValue / _healthDivider;
            _fillObject.transform.localScale = new Vector3(scaleValue, _fixedScaleY, _fixedScaleZ);
            
            if (_label != null)
                _label.text = $"Health: {_health.CurrentValue}/{_health.MaxValue}";
        }
    }
}