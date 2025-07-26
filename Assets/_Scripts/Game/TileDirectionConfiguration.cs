using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using _Scripts.RoomTiles;

namespace _Scripts.Game
{
    public struct TileDirectionConfiguration
    {
        public List<RoomDirection> OpenDirections { get; }
        public List<RoomDirection> ClosedDirections { get; }
        public List<RoomDirection> AnyDirections { get; }
        
        public TileDirectionConfiguration(RoomTileCellDto cell)
        {
            OpenDirections = cell.Tile.OpenDirections.Rotate(cell.Direction);
            AnyDirections =  cell.Tile.DoorDirections.Rotate(cell.Direction);
            ClosedDirections = OpenDirections.Concat(AnyDirections).InvertList();
        }

        public TileDirectionConfiguration(List<RoomDirection> adjacentOpenDirections, List<RoomDirection> adjacentAnyDirections, List<RoomDirection> adjacentClosedDirections)
        {
            OpenDirections = adjacentOpenDirections;
            AnyDirections = adjacentAnyDirections;
            ClosedDirections = adjacentClosedDirections;
        }
    }
}