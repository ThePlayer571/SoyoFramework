using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    public class WallFloorController : MonoVController<TheSnakeGame>
    {
        [SerializeField] private Tilemap wallFloorTilemap;
        
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase floorTile;

        protected override void OnAwake(TheSnakeGame theSnakeGame)
        {
            var size = theSnakeGame.Map.Size;
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                var position = new Vector2Int(x, y);
                var isWall = theSnakeGame.Map.Walls.Contains(position);
                wallFloorTilemap.SetTile(new Vector3Int(x, y, 0), isWall ? wallTile : floorTile);
            }
        }
    }
}