using System;
using System.Collections.Generic;
using _Scripts.Game;
using _Scripts.Mascot;
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
        
        [Header("Level Background")]
        [SerializeField] private SpriteRenderer levelBuildingBackground;
        [SerializeField] private SpriteRenderer levelBuildingBackgroundHighlight;
        [SerializeField] private Sprite levelBuildingBackgroundHighlightFloorSprite;
        [SerializeField] private Sprite levelBuildingBackgroundHighlightAllWindowsSprite;
        [SerializeField] private GameObject levelBuildingSharedRoomsHighlight;
        [SerializeField] private GameObject levelBuildingDoorsHighlight;
        
        [Space]
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
            SignalsHub.AddListener<HighlightBuildingSignal>(OnHighlightFloor);
            SignalsHub.AddListener<UnhighlightBuildingSignal>(OnUnhighlightFloor);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
            SignalsHub.RemoveListener<HighlightBuildingSignal>(OnHighlightFloor);
            SignalsHub.RemoveListener<UnhighlightBuildingSignal>(OnUnhighlightFloor);
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
            
            levelBuildingBackground.size = _level.LevelSize;
            var highlightScaleMultiplier = levelBuildingBackgroundHighlight.transform.localScale.x > 0
                ? (1f / levelBuildingBackgroundHighlight.transform.localScale.x)
                : 1f;
            levelBuildingBackgroundHighlight.size = new Vector2(_level.LevelSize.x * highlightScaleMultiplier + 0.1f, _level.LevelSize.y * highlightScaleMultiplier + 0.1f);
            levelBuildingBackgroundHighlight.gameObject.SetActive(false);
            levelBuildingSharedRoomsHighlight.SetActive(false);
            levelBuildingDoorsHighlight.SetActive(false);
            
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
                    
                    var tree = _prefabPool.Spawn(treePrefab, levelPlantParent);
                    tree.transform.position = treePosition;
                    _plants.Add(tree);
                }
            }
        }

        private void OnHighlightFloor(HighlightBuildingSignal signal)
        {
            switch (signal.TargetType)
            {
                case MascotTutorialBuildingTargetType.Floor:
                    levelBuildingBackgroundHighlight.sprite = levelBuildingBackgroundHighlightFloorSprite;
                    levelBuildingBackgroundHighlight.gameObject.SetActive(true);
                    break;
                case MascotTutorialBuildingTargetType.SharedRooms:
                    levelBuildingSharedRoomsHighlight.SetActive(true);
                    break;
                case MascotTutorialBuildingTargetType.AllWindows:
                    levelBuildingBackgroundHighlight.sprite = levelBuildingBackgroundHighlightAllWindowsSprite;
                    levelBuildingBackgroundHighlight.gameObject.SetActive(true);
                    break;
                case MascotTutorialBuildingTargetType.Doors:
                    levelBuildingDoorsHighlight.SetActive(true);
                    break;
                case MascotTutorialBuildingTargetType.IndividualWindows:
                    break;
            }
        }
        
        private void OnUnhighlightFloor(UnhighlightBuildingSignal signal)
        {
            levelBuildingBackgroundHighlight.gameObject.SetActive(false);
            levelBuildingSharedRoomsHighlight.SetActive(false);
            levelBuildingDoorsHighlight.SetActive(false);
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