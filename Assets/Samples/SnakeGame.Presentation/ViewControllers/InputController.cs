using System;
using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using SoyoFramework.Samples.SnakeGame.Presentation.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    public class InputController : MonoVController
    {
        public Vector2Int Direction { get; private set; } = Vector2Int.right;


        [SerializeField] private float moveInterval = 0.5f;
        private float _timer = 0f;

        private void Update()
        {
            var game = this.GetAggregateRoot<TheSnakeGame>();

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Direction = Vector2Int.up;
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                Direction = Vector2Int.down;
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                Direction = Vector2Int.left;
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                Direction = Vector2Int.right;
            }


            _timer += Time.deltaTime;
            if (_timer >= moveInterval)
            {
                _timer = 0f;
                this.SendCommand(new MoveCommand(Direction));
            }
        }
    }
}