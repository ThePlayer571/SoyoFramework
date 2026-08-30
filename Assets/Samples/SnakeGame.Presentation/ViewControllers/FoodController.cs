using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using SoyoFramework.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    public class FoodController : MonoVController<TheSnakeGame>
    {
        [SerializeField] private Tilemap foodTilemap;
        [SerializeField] private TileBase foodTile;

        private Vector2Int _foodPosition;

        protected override void OnAwake(TheSnakeGame theSnakeGame)
        {
            theSnakeGame.Map.Food.Position.RegisterWithInitValue(newPosition =>
            {
                foodTilemap.SetTile(new Vector3Int(_foodPosition.x, _foodPosition.y, 1), null);
                _foodPosition = newPosition;
                foodTilemap.SetTile(new Vector3Int(newPosition.x, newPosition.y, 1), foodTile);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}