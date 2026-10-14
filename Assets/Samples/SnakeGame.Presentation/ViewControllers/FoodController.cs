using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    public class FoodController : MonoVController<TheSnakeGame>
    {
        [SerializeField] private Tilemap foodTilemap;
        [SerializeField] private TileBase foodTile;

        private Vector2Int _foodPosition;
        private IUnRegister? _foodPositionChanged;

        protected override void OnAvailable(TheSnakeGame theSnakeGame)
        {
            _foodPositionChanged = theSnakeGame.Map.Food.Position.RegisterWithInitValue(newPosition =>
            {
                foodTilemap.SetTile(new Vector3Int(_foodPosition.x, _foodPosition.y, 2), null);
                _foodPosition = newPosition;
                foodTilemap.SetTile(new Vector3Int(newPosition.x, newPosition.y, 2), foodTile);
            });
        }

        protected override void OnUnavailable()
        {
            _foodPositionChanged?.UnRegister();
            _foodPositionChanged = null;
            foodTilemap.ClearAllTiles();
        }
    }
}
