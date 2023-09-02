using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.TopDownGame.Core.Enums;

namespace Runes_and_Spells.TopDownGame.Core;

public class Controller
{
    private TopDownCore _gameCore;
    public const int PlayerFlySpeed = 3;
    public const int PlayerWalkSpeed = 4;
    public const int PlayerRunSpeed = 6;
    private KeyboardState _lastKbState;

    public Controller(TopDownCore core)
    {
        _gameCore = core;
    }
    
    public void Update()
    {
        var lastPlayerPos = _gameCore.PlayerPosition;
        var kbState = Keyboard.GetState();
        var playerSpeed = _gameCore.PlayerIsOnWings ? PlayerFlySpeed : _gameCore.IsPlayerRunning ? PlayerRunSpeed : PlayerWalkSpeed;
        if (kbState.IsKeyDown(Keys.Space) && _lastKbState.IsKeyUp(Keys.Space) && _gameCore.PlayerHasWings)
        {
            if (_gameCore.PlayerIsOnWings && _gameCore.Map.GetTileByPixelCoordinates(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y).Type == GameMap.TileType.Grass &&
                !_gameCore.Map.IsThereSolidObject(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y))
            {
                _gameCore.PlayerIsOnWings = false;
            }
            else if (!_gameCore.PlayerIsOnWings)
            {
                _gameCore.PlayerIsOnWings = true;
            }
        }
        if (kbState.IsKeyDown(Keys.W) && _gameCore.PlayerPosition.Y - GameMap.TileSize*2> 0)
        {
            if ((_gameCore.Map.Tiles[(int)_gameCore.PlayerPosition.X / GameMap.TileSize, 
                    (int)(_gameCore.PlayerPosition.Y-playerSpeed) / GameMap.TileSize].IsPassable || _gameCore.PlayerIsOnWings) &&
                !_gameCore.Map.IsThereSolidObject(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y-playerSpeed))
            {
                _gameCore.PlayerPosition = new Vector2(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y-playerSpeed);
            }
        }
        if (kbState.IsKeyDown(Keys.A) && _gameCore.PlayerPosition.X - GameMap.TileSize/2> 0)
        {
            if ((_gameCore.Map.Tiles[(int)(_gameCore.PlayerPosition.X - playerSpeed)/ GameMap.TileSize,
                    (int)_gameCore.PlayerPosition.Y / GameMap.TileSize].IsPassable || _gameCore.PlayerIsOnWings) &&
                !_gameCore.Map.IsThereSolidObject(_gameCore.PlayerPosition.X - playerSpeed, _gameCore.PlayerPosition.Y) &&
                (_gameCore.PlayerPosition.X >= 6000 || _gameCore.PlayerHasCompass))
            {
                if (_gameCore.Game.Introduction.IsPlaying && _gameCore.PlayerPosition.X > GameMap.MapWidth*GameMap.TileSize - _gameCore.Game.ScreenWidth + 32 || 
                    !_gameCore.Game.Introduction.IsPlaying)
                {
                    _gameCore.PlayerPosition =
                        new Vector2(_gameCore.PlayerPosition.X - playerSpeed, _gameCore.PlayerPosition.Y);
                }
                
            }
        }
        if (kbState.IsKeyDown(Keys.S) && _gameCore.PlayerPosition.Y + GameMap.TileSize< GameMap.MapHeight*GameMap.TileSize)
        {
            
            if ((_gameCore.Map.Tiles.GetLength(1) > (int)(_gameCore.PlayerPosition.Y + playerSpeed) / GameMap.TileSize &&
                _gameCore.Map.Tiles[(int)_gameCore.PlayerPosition.X / GameMap.TileSize,
                    (int)(_gameCore.PlayerPosition.Y + playerSpeed) / GameMap.TileSize].IsPassable || _gameCore.PlayerIsOnWings) &&
                !_gameCore.Map.IsThereSolidObject(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y + playerSpeed))
            {
                if (_gameCore.Game.Introduction.IsPlaying && _gameCore.PlayerPosition.Y < _gameCore.Game.ScreenHeight || 
                    !_gameCore.Game.Introduction.IsPlaying)
                {
                    _gameCore.PlayerPosition =
                        new Vector2(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y + playerSpeed);
                }
               
            }
        }
        if (kbState.IsKeyDown(Keys.D) && _gameCore.PlayerPosition.X + GameMap.TileSize/2 < GameMap.MapWidth*GameMap.TileSize)
        {
            if ((_gameCore.Map.Tiles.GetLength(0) > (int)(_gameCore.PlayerPosition.X + playerSpeed) / GameMap.TileSize  && 
                _gameCore.Map.Tiles[(int)(_gameCore.PlayerPosition.X + playerSpeed) / GameMap.TileSize,
                    (int)_gameCore.PlayerPosition.Y / GameMap.TileSize].IsPassable || _gameCore.PlayerIsOnWings) &&
                !_gameCore.Map.IsThereSolidObject(_gameCore.PlayerPosition.X + playerSpeed, _gameCore.PlayerPosition.Y))
            {
                _gameCore.PlayerPosition =
                    new Vector2(_gameCore.PlayerPosition.X + playerSpeed, _gameCore.PlayerPosition.Y);
            }
        }
        _gameCore.IsPlayerRunning = kbState.IsKeyDown(Keys.LeftShift);
        
        var wasPlayerMoving = _gameCore.IsPlayerMoving;
        _gameCore.IsPlayerMoving = lastPlayerPos != _gameCore.PlayerPosition;
        if (_gameCore.IsPlayerMoving && !_gameCore.TimerStepSound.IsRunning)
        {
            _gameCore.TimerStepSound.StartWithTime(1200/playerSpeed);
        }
        else if (!_gameCore.IsPlayerMoving)
        {
            _gameCore.TimerStepSound.Stop();
        }
        if (lastPlayerPos.Y < _gameCore.PlayerPosition.Y)
            _gameCore.PlayerLastLookDirection = Direction.Down;
        else if (lastPlayerPos.Y > _gameCore.PlayerPosition.Y)
            _gameCore.PlayerLastLookDirection = Direction.Up;
        else if (lastPlayerPos.X > _gameCore.PlayerPosition.X)
            _gameCore.PlayerLastLookDirection = Direction.Left;
        else if (lastPlayerPos.X < _gameCore.PlayerPosition.X)
            _gameCore.PlayerLastLookDirection = Direction.Right;
        
        if (!wasPlayerMoving && _gameCore.IsPlayerMoving)
        {
            _gameCore.View.ResetPlayerAnimationTimer();
        }

        if (!_gameCore.Game.Introduction.IsPlaying)
        {
            _gameCore.CameraPosition = new Vector2(
                Math.Min(Math.Max(_gameCore.PlayerPosition.X - _gameCore.Game.ScreenWidth / 2, 0), GameMap.MapWidth * GameMap.TileSize - _gameCore.Game.ScreenWidth),
                Math.Min(Math.Max(_gameCore.PlayerPosition.Y - _gameCore.Game.ScreenHeight / 2, 0), GameMap.MapHeight * GameMap.TileSize - _gameCore.Game.ScreenHeight)
            );
        }
        
        _lastKbState = kbState;
    }

    
}