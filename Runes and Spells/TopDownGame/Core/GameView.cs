using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.TopDownGame.Core.Records;
using Runes_and_Spells.TopDownGame.Core.Utility;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Core;

public class GameView
{
    private Texture2D _tileSpriteSheet;
    private Texture2D _playerSpriteSheet;
    private Texture2D _playerOnWingsSpritesheet;
    private Texture2D _mapObjectsTileSheet;
    private Texture2D _compassBGTexture;
    private TopDownCore _gameCore;
    private SpriteFont _logFont;
    public Timer PlayerAnimationTimer { get; private set; }
    public int PlayerAnimationFrameIndex;
    public AnimationInfo PlayerAnimationInfo { get; private set; }

    private Dictionary<GameMap.TileType, Vector2> _tileSpriteSheetPositions = new()
    {
        { GameMap.TileType.Grass, new Vector2(0,0)},
        { GameMap.TileType.Road, new Vector2(1,0)},
        { GameMap.TileType.Water, new Vector2(2,0)},
    };

    private Vector2[] _neighboursMoves = new[]
    {
        new Vector2(-1, -1),
        new Vector2(-1, 0),
        new Vector2(-1, 1),
        new Vector2(0, -1),
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, -1),
        new Vector2(1, 0),
        new Vector2(1, 1)
    };

    

    public GameView(TopDownCore core,Texture2D tileSpriteSheet, Texture2D playerAnimationsSpriteSheet, Texture2D mapObjectsTileSheet,SpriteFont logFont)
    {
        _tileSpriteSheet = tileSpriteSheet;
        _playerSpriteSheet = playerAnimationsSpriteSheet;
        _mapObjectsTileSheet = mapObjectsTileSheet;
        _compassBGTexture = core.Game.Content.Load<Texture2D>("top-down/compass_bg");
        _playerOnWingsSpritesheet = core.Game.Content.Load<Texture2D>("top-down/spritesheets/player_wings_animations");
        _gameCore = core;
        _logFont = logFont;
        PlayerAnimationInfo = PlayerAnimation._playerStandAnimations[core.PlayerLastLookDirection];
        PlayerAnimationTimer = new Timer(PlayerAnimationInfo.FrameDuration, () =>
        {
            PlayerAnimationFrameIndex = PlayerAnimationFrameIndex + 1 == PlayerAnimationInfo.FramesCount ? 0 : PlayerAnimationFrameIndex + 1;
            PlayerAnimationTimer.StartWithTime(PlayerAnimationInfo.FrameDuration - (_gameCore.IsPlayerRunning ? 50 : 0));
        });
        PlayerAnimationTimer.Start();
    }

    public void Draw(GameMap map, Vector2 cameraPosition, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < map.Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < map.Tiles.GetLength(1); j++)
            {
                var tile = map.Tiles[i, j];
                if (!tile.IsVisible) continue;
                
                if (tile.Type == GameMap.TileType.Water)
                {
                    var tileSheetPosition = new Vector2(2, 0);
                    var neighboursGrass = _neighboursMoves
                        .Select(v => map.Tiles[
                            Math.Min(Math.Max(i + (int)v.Y, 0), GameMap.MapWidth-1), 
                            Math.Min(Math.Max(j + (int)v.X, 0), GameMap.MapHeight-1)]
                            .Type == GameMap.TileType.Grass)
                        .ToArray();
                    var grassCount = neighboursGrass.Count(b => b);
                    if (grassCount > 0)
                    {
                        if (neighboursGrass[7]) tileSheetPosition = new Vector2(1 ,3);
                        else if (neighboursGrass[3]) tileSheetPosition = new Vector2(2 ,4);
                        else if (neighboursGrass[5]) tileSheetPosition = new Vector2(0 ,4);
                        else if (neighboursGrass[1]) tileSheetPosition = new Vector2(1 ,5);
                        else if (neighboursGrass[0]) tileSheetPosition = new Vector2(2 ,5);
                        else if (neighboursGrass[2]) tileSheetPosition = new Vector2(0 ,5);
                        else if (neighboursGrass[6]) tileSheetPosition = new Vector2(2 ,3);
                        else if (neighboursGrass[8]) tileSheetPosition = new Vector2(0 ,3);
                    }

                    if (grassCount > 1)
                    {
                        if (neighboursGrass[1] && neighboursGrass[3]) tileSheetPosition = new Vector2(1 ,2);
                        else if (neighboursGrass[1] && neighboursGrass[5]) tileSheetPosition = new Vector2(0 ,2);
                        else if (neighboursGrass[7] && neighboursGrass[3]) tileSheetPosition = new Vector2(1 ,1);
                        else if (neighboursGrass[7] && neighboursGrass[5]) tileSheetPosition = new Vector2(0 ,1);
                    }
                    spriteBatch.Draw(_tileSpriteSheet,
                        new Vector2(tile.PositionInPixels.X - cameraPosition.X, tile.PositionInPixels.Y - cameraPosition.Y), 
                        new Rectangle((int)tileSheetPosition.X * GameMap.TileSize, 
                            (int)tileSheetPosition.Y * GameMap.TileSize, 
                            GameMap.TileSize, GameMap.TileSize),
                        Color.White);
                }
                else if (tile.Type == GameMap.TileType.Grass)
                {
                    spriteBatch.Draw(_tileSpriteSheet,
                        new Vector2(tile.PositionInPixels.X - cameraPosition.X, tile.PositionInPixels.Y - cameraPosition.Y), 
                        new Rectangle((int)_tileSpriteSheetPositions[tile.Type].X * GameMap.TileSize, 
                            (int)_tileSpriteSheetPositions[tile.Type].Y * GameMap.TileSize, 
                            GameMap.TileSize, GameMap.TileSize),
                        Color.White);
                    if (tile.DecorationObject is not null)
                    {
                        spriteBatch.Draw(_mapObjectsTileSheet,
                            new Vector2(tile.PositionInPixels.X - cameraPosition.X, tile.PositionInPixels.Y - cameraPosition.Y), 
                            tile.DecorationObject.RectangleSpriteSheet,
                            Color.White);
                    }
                }
            }
        }
        DrawMapPuddles(spriteBatch);
        DrawMapSolidObjects(spriteBatch, false);
        DrawPlayer(spriteBatch);
        DrawMapSolidObjects(spriteBatch, true);
        if (_gameCore.PlayerHasCompass)
        {
            spriteBatch.Draw(_compassBGTexture, Vector2.Zero, Color.White);
            spriteBatch.DrawString(_gameCore.CompassFont, "Ваша позиция:\n" +
                                                          $"X: {Math.Round(_gameCore.PlayerPosition.X/64)}\n" +
                                                          $"Y: {Math.Round(_gameCore.PlayerPosition.Y/64) - 2}", 
                new Vector2(94, 7), Color.Black);
        }

        if (_gameCore.PlayerPosition.X < 6014 && !_gameCore.PlayerHasCompass)
        {
            NPC.DrawHint(spriteBatch, "Идти дальше в лес без приборов для навигации опасно,\n" +
                                      "Я могу заблудиться.");
        }
    }

    private void DrawMapPuddles(SpriteBatch spriteBatch)
    {
        foreach (var puddle in _gameCore.Map.MudPuddles)
        {
            puddle.Draw(spriteBatch);
        }
    }

    private void DrawMapSolidObjects(SpriteBatch spriteBatch, bool abovePlayer)
    {
        var objList = abovePlayer
            ? _gameCore.Map.FrontObjects.Concat(_gameCore.Map.NPCList).Where(o => o.PositionInPixelsLeftBottom.Y > _gameCore.PlayerPosition.Y)
            : _gameCore.Map.FrontObjects.Concat(_gameCore.Map.NPCList).Where(o => o.PositionInPixelsLeftBottom.Y <= _gameCore.PlayerPosition.Y);
        foreach (var obj in objList.Where(obj => obj.IsVisible).OrderBy(o => o.PositionInPixelsLeftBottom.Y))
        {
            if (obj.GetType() == typeof(Chest))
            {
                var chest = (Chest)obj;
                chest.Draw(spriteBatch);
                continue;
            }
            if (obj.GetType() == typeof(NPC))
            {
                var npc = (NPC)obj;
                npc.Draw(spriteBatch);
                continue;
            }
            spriteBatch.Draw(_mapObjectsTileSheet,
                    new Vector2(obj.PositionInPixelsLeftBottom.X - _gameCore.CameraPosition.X, 
                        obj.PositionInPixelsLeftBottom.Y - _gameCore.CameraPosition.Y - obj.SpriteSheetRectangle.Height), 
                    obj.SpriteSheetRectangle,
                    Color.White);
            
        }
        
    }
    
    private void DrawPlayer(SpriteBatch spriteBatch)
    {
        PlayerAnimationTimer.Tick();
        if (_gameCore.IsPlayerMoving)
            PlayerAnimationInfo = PlayerAnimation._playerWalkAnimations[_gameCore.PlayerLastLookDirection];
        else
            PlayerAnimationInfo = PlayerAnimation._playerStandAnimations[_gameCore.PlayerLastLookDirection];
        
        
        var rect = new Rectangle((int)(PlayerAnimationInfo.StartIndex.X + PlayerAnimationFrameIndex) * GameMap.TileSize, 
            (int)PlayerAnimationInfo.StartIndex.Y * 2 * GameMap.TileSize,
            GameMap.TileSize, GameMap.TileSize*2);
        
        if (_gameCore.PlayerIsOnWings)
            spriteBatch.Draw(_playerOnWingsSpritesheet, new Vector2(
                    _gameCore.PlayerPosition.X - _gameCore.CameraPosition.X - GameMap.TileSize/2,
                    _gameCore.PlayerPosition.Y - _gameCore.CameraPosition.Y - GameMap.TileSize*2 - 8), 
                rect, Color.White);
        else
            spriteBatch.Draw(_playerSpriteSheet, new Vector2(
                    _gameCore.PlayerPosition.X - _gameCore.CameraPosition.X - GameMap.TileSize/2,
                    _gameCore.PlayerPosition.Y - _gameCore.CameraPosition.Y - GameMap.TileSize*2), 
                rect, Color.White);
    }

    public void ResetPlayerAnimationTimer()
    {
        if (_gameCore.IsPlayerMoving)
            PlayerAnimationInfo = PlayerAnimation._playerWalkAnimations[_gameCore.PlayerLastLookDirection];
        else
            PlayerAnimationInfo = PlayerAnimation._playerStandAnimations[_gameCore.PlayerLastLookDirection];
        var runAddition = 0;
        if (_gameCore.IsPlayerRunning) runAddition = -50;
        PlayerAnimationTimer.Stop();
        PlayerAnimationTimer.StartWithTime(PlayerAnimationInfo.FrameDuration + runAddition);
    }
}