using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
using Signals;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Visuals
{
    public class LevelVisualsController : MonoBehaviour
    {
        [SerializeField] private GameObject levelBuildingPorch;
        
        [SerializeField] private Transform levelPlantParent;
        [SerializeField] private GameObject bushPrefab;
        [SerializeField] private GameObject treePrefab;
        
        [SerializeField] private Vector3 generationOffset = new(0.5f, 0f, 0f);
        [SerializeField] private Vector2 levelOffset = new(1f, 1.5f);

        [Header("Plant generation")]
        [SerializeField] private float treeGenerationInterval = 1;
        [SerializeField] private Vector2 treeGenerationDistance = new(10, 10);

        [Inject] private IPrefabPool _prefabPool;
        
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
            DespawnPlants();
        }

        private void DespawnPlants()
        {
            foreach (var plant in _plants)
            {
                if (plant != null)
                    _prefabPool.Despawn(plant);
            }
            _plants.Clear();
        }

        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _level = signal.Level;
            levelBuildingPorch.transform.position = new Vector3(0, -_level.HalfLevelSize.y - levelOffset.y, 0);
            levelBuildingPorch.SetActive(_level.ShowPorch);

            DespawnPlants();
            levelPlantParent.DestroyChildren();
            _plants = new List<GameObject>();
            
            var treeGenerationBounds = new Vector2(_level.HalfLevelSize.x + levelOffset.x + treeGenerationDistance.x, _level.HalfLevelSize.y + levelOffset.y + treeGenerationDistance.y);
            var isXOdd = false;
            for (var treeX = -treeGenerationBounds.x; treeX < treeGenerationBounds.x; treeX += treeGenerationInterval)
            {
                isXOdd = !isXOdd;
                var isYOdd = false;
                for (var treeY = -treeGenerationBounds.y; treeY < treeGenerationBounds.y; treeY += treeGenerationInterval)
                {
                    isYOdd = !isYOdd;
                    var treePosition =  new Vector3(treeX, treeY, 0) + generationOffset;
                    if (IsPositionWithinLevel(treePosition)) continue;
                    if ((isXOdd && !isYOdd) || (!isXOdd && isYOdd)) continue;
                    
                    var tree = _prefabPool.Spawn(treePrefab);
                    tree.transform.position = treePosition;
                    _plants.Add(tree);
                }
            }
        }

        private bool IsPositionWithinLevel(Vector3 treePosition)
        {
            return treePosition.x >= -_level.HalfLevelSize.x - levelOffset.x 
                   && treePosition.x <= _level.HalfLevelSize.x + levelOffset.x 
                   // && treePosition.y >= -_level.HalfLevelSize.y - levelOffset.y 
                   && treePosition.y <= _level.HalfLevelSize.y + levelOffset.y;
        }
    }
}