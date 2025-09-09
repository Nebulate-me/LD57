using System.Collections.Generic;
using _Scripts.Game;
using Signals;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Visuals
{
    public class LevelVisualsController : MonoBehaviour
    {
        [SerializeField] private GameObject levelBuildingPorch;
        
        [SerializeField] private Transform levelPlantParent;
        [SerializeField] private GameObject bushPrefab;
        [SerializeField] private GameObject treePrefab;
        
        [SerializeField] private float levelOffset = 1.5f;

        [Header("Plant generation")]
        [SerializeField] private int generatedPlantAmount = 20;
        [SerializeField] private Vector2 generationRadius = new(10, 10);
        [SerializeField] private float treeSpawnChance = .75f;

        [Inject] private IPrefabPool _prefabPool;
        [Inject] private IRandomService _randomService;
        
        private Level _level;
        private List<GameObject> _plants = new();

        private void OnEnable()
        {
            SignalsHub.AddListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDestroy()
        {
            foreach (var plant in _plants)
            {
                _prefabPool.Despawn(plant);
            }
            _plants.Clear();
        }

        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _level = signal.Level;
            levelBuildingPorch.transform.position = new Vector3(0, -_level.HalfLevelSize.y - levelOffset, 0);
            levelBuildingPorch.SetActive(_level.ShowPorch);
            
            levelPlantParent.DestroyChildren();
            _plants = new List<GameObject>();
            var levelRadiusX = _level.HalfLevelSize.x + levelOffset;
            var levelRadiusY = _level.HalfLevelSize.y + levelOffset;
            for (var i = 0; i < generatedPlantAmount; i++)
            {
                var point = GetRandomRingPoint(levelRadiusX, levelRadiusY);
                var prefab = _randomService.Float(0, 1) <= treeSpawnChance ? treePrefab : bushPrefab;
                prefab.transform.position = point;
                var plant = _prefabPool.Spawn(prefab, levelPlantParent);
                _plants.Add(plant);
            }
        }

        private Vector2 GetRandomRingPoint(float levelRadiusX, float levelRadiusY)
        {
            return GetRandomBool()
                ? new Vector2(
                    GetRandomRingCoordinate(0, generationRadius.x),
                    GetRandomRingCoordinate(levelRadiusY, generationRadius.y)
                )
                : new Vector2(
                    GetRandomRingCoordinate(levelRadiusX, generationRadius.x),
                    GetRandomRingCoordinate(0, generationRadius.y)
                );
        }

        private float GetRandomRingCoordinate(float innerRingRadius, float outerRingRadius)
        {
            return GetRandomBool() 
                ? _randomService.Float(innerRingRadius, outerRingRadius)
                : -_randomService.Float(innerRingRadius, outerRingRadius);
        }

        private bool GetRandomBool()
        {
            return _randomService.Float(0, 1) >= 0.5f;
        }
    }
}