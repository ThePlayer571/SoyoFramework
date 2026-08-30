using System;
using SoyoFramework.Samples.SnakeGame.Backend;
using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    [DefaultExecutionOrder(-1)]
    public class TestManager : MonoBehaviour
    {
        private void Update()
        {
            if (_snakeGame == null) return;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _snakeGame.Map.Snake.AfterBodyChanged.Trigger();
            }
        }

        private TheSnakeGame _snakeGame;

        private void Awake()
        {
            TSG.Init();
            _snakeGame = TSG.Instance.GetAggregateRoot<TheSnakeGame>();
        }
    }
}