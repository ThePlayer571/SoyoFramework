using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using SoyoFramework.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    public class SnakeController : MonoVController<TheSnakeGame>
    {
        [Serializable]
        private struct SnakeTileConfig
        {
            public Vector2Int Direction;
            public TileBase Tile;
        }

        [SerializeField] private Tilemap snakeTilemap;
        [SerializeField] private SnakeTileConfig[] snakeBodyTiles;
        [SerializeField] private SnakeTileConfig[] snakeHeadTiles;
        [SerializeField] private SnakeTileConfig[] snakeTailTiles;


        private Dictionary<Vector2Int, TileBase> _snakeBodyTileDict;
        private Dictionary<Vector2Int, TileBase> _snakeHeadTileDict;
        private Dictionary<Vector2Int, TileBase> _snakeTailTileDict;
        private TheSnakeGame _theSnakeGame;

        private void UpdateSnake()
        {
            var snake = _theSnakeGame.Map.Snake;

            // 尾
            var tailPosition = snake.Body[^1];
            snakeTilemap.SetTile(
                new Vector3Int(tailPosition.x, tailPosition.y, 0),
                _snakeTailTileDict[snake.Body[^2] - tailPosition]);

            // 身子
            for (int index = 1; index < snake.Body.Count - 1; index++)
            {
                var direction1 = snake.Body[index - 1] - snake.Body[index];
                var direction2 = snake.Body[index + 1] - snake.Body[index];
                var direction = direction1 + direction2;
                if (direction == Vector2Int.zero)
                {
                    direction = direction1 - direction2;
                    direction = new Vector2Int(Math.Abs(direction.x), Math.Abs(direction.y));
                }

                var tile = _snakeBodyTileDict[direction];
                var position = snake.Body[index];
                snakeTilemap.SetTile(new Vector3Int(position.x, position.y, 0), tile);
            }

            // 首
            var headPosition = snake.Body[0];
            snakeTilemap.SetTile(
                new Vector3Int(headPosition.x, headPosition.y, 0),
                _snakeHeadTileDict[snake.Body[1] - headPosition]);
        }

        protected override void OnAwake(TheSnakeGame theSnakeGame)
        {
            _theSnakeGame = theSnakeGame;

            _snakeBodyTileDict = snakeBodyTiles
                .Select(config => new KeyValuePair<Vector2Int, TileBase>(config.Direction, config.Tile))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            _snakeHeadTileDict = snakeHeadTiles
                .Select(config => new KeyValuePair<Vector2Int, TileBase>(config.Direction, config.Tile))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            _snakeTailTileDict = snakeTailTiles
                .Select(config => new KeyValuePair<Vector2Int, TileBase>(config.Direction, config.Tile))
                .ToDictionary(kv => kv.Key, kv => kv.Value);


            var snake = theSnakeGame.Map.Snake;

            snake.AfterBodyChanged.RegisterWithInvoke(UpdateSnake)
                .UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}