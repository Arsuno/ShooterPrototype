using _Project.Source.Data;
using _Project.Source.SaveLoadSystems;
using UnityEngine;
using Zenject;

namespace _Project.Source.Factories
{
    public class GameDataFactory : PlaceholderFactory<GameData>
    {
        private ISaveLoadSystem _saveLoadSystem;

        [Inject]
        public void Construct(ISaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
        }
        
        public override GameData Create()
        {
            var data = _saveLoadSystem.Load<GameData>();
            if (data == null)
            {
                Debug.Log("GameData not found, creating new one...");
                data = new GameData();
                _saveLoadSystem.Save(data);
            }
            return data;
        }
    }
}