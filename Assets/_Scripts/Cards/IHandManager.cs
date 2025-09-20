using _Scripts.Rooms;
using _Scripts.RoomTiles;
using Utilities.Monads;

namespace _Scripts.Cards
{
    public interface IHandManager
    {
        int CardAmount { get; }

        #region Room Tile Cards

        IMaybe<RoomTileCardView> SelectedRoomTileCardView { get; }
        bool SelectRoomTileCard(RoomTileDto tileDto);
        bool DeselectRoomTileCard();
        bool TryPlaySelectRoomTileCard();

        #endregion
        
        #region Room Cards
        
        IMaybe<RoomCardView> SelectedRoomCardView { get;  }
        
        void SelectRoomCard(RoomDto roomDto);
        bool DeselectRoomCard();
        bool TryPlaySelectRoomCard();
        void RefillRoomHand();
        void RedrawRoomHand();
        bool TryUnplayLastCard();
        
        #endregion
    }
}