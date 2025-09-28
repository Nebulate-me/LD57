using _Scripts.Missions.Apartment;
using Signals;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Visuals
{
    public class FireworksController : MonoBehaviour
    {
        [SerializeField] private GameObject fireworkPrefab;
        
        [Inject] private IPrefabPool _prefabPool;
        
        private GameObject _firework;

        private void OnEnable()
        {
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
        }

        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            if (_firework != null)
            {
                _prefabPool.Despawn(_firework);
            }
            
            _firework = _prefabPool.Spawn(fireworkPrefab, transform);
        }
    }
}