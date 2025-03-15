using _Project.Source.Data;
using _Project.Source.Factories;
using _Project.Source.Inventory;
using _Project.Source.SaveLoadSystems;
using UnityEngine;
using Zenject;

namespace _Project.Source.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Hotbar _hotbar;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<JSONSaveLoadSystem>().AsSingle();
            Container.Bind<Hotbar>().FromInstance(_hotbar).AsSingle().NonLazy();
            
            Container.BindFactory<GameData, GameDataFactory>().AsCached();
            Container.Bind<GameData>().FromFactory<GameDataFactory>().AsSingle();
        }
    }
}