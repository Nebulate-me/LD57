using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private GameObject roomCardPrefab;
        [SerializeField] private RectTransform roomCardContainer;
        [SerializeField] private int handSize = 5;

        private List<RoomCard> cards = new();

        [Inject] private IDeckManager deckManager;
        [Inject] private IPrefabPool prefabPool;

        private void Start()
        {
            roomCardContainer.DestroyChildren();

            RefillHand();
        }

        private void RefillHand()
        {
            while (cards.Count < handSize)
            {
                if (deckManager.TryDraw(out var card))
                {
                    cards.Add(card);
                    var cardObject = prefabPool.Spawn(roomCardPrefab, roomCardContainer);
                    cardObject.GetComponent<RoomCardView>().SetUp(card);
                }
                else
                {
                    Debug.Log("No cards left in the deck, not drawing!");
                    break;
                }
            }
        }
    }
}