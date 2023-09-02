using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.TopDownGame.Core.Records;
using Runes_and_Spells.TopDownGame.Core.Utility;
using Runes_and_Spells.TopDownGame.Dialogs;
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
    private Texture2D _compassArrowTexture;
    private TopDownCore _gameCore;
    private SpriteFont _logFont;
    
    private Texture2D _dialogBoxTexture;
    private Dictionary<NPCType, Texture2D> _npcPortraitsTextures;
    private Texture2D _portraitBorderTexure;
    public NPC DialogNPCToDraw;
    
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
        _compassArrowTexture = core.Game.Content.Load<Texture2D>("top-down/compass_arrow");
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
        _dialogBoxTexture = core.Game.Content.Load<Texture2D>("top-down/dialogue/dialog_box");
        _portraitBorderTexure = core.Game.Content.Load<Texture2D>("top-down/dialogue/border");
        _npcPortraitsTextures = new Dictionary<NPCType, Texture2D>()
        {
            { NPCType.Bard, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_bard")},
            { NPCType.FisherMan, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_fisherman")},
            { NPCType.Mage, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_mage")},
            { NPCType.Hunter, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_hunter")},
            { NPCType.Woodman, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_lumberjack")},
            { NPCType.Witch, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_witch")},
            { NPCType.Thief, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_ninja")},
            { NPCType.Trader, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_trader")}
        };
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
            DrawCompassHud(spriteBatch);
        }

        if (_gameCore.PlayerPosition.X < 6014 && !_gameCore.PlayerHasCompass)
        {
            NPC.DrawHint(spriteBatch, "Идти дальше в лес без приборов для навигации опасно,\n" +
                                      "Я могу заблудиться.");
        }

        if (DialogNPCToDraw is not null)
        {
            DrawDialogue(spriteBatch, DialogNPCToDraw);
        }
    }
    
    public void DrawDialogue(SpriteBatch spriteBatch, NPC npc)
    {
        var phrase = npc.CurrentPhrase;
        var npcType = npc.NPCType;
        spriteBatch.Draw(_dialogBoxTexture, new Vector2(15, 762)*Game1.ResolutionScale, null, Color.White, 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_npcPortraitsTextures[npcType], new Vector2(48,789)*Game1.ResolutionScale, null, Color.White, 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_portraitBorderTexure, new Vector2(48,789)*Game1.ResolutionScale, null, Color.White, 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var textColor = new Color(29, 14, 2);
        var font = AllGameItems.Font20Px;
        spriteBatch.DrawString(font, phrase.Text, new Vector2(315, 789)*Game1.ResolutionScale, textColor, 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (npc.IsQuestActive)
        {
            npc.GiveScrollButton.Draw(spriteBatch);
            npc.SlotForScrolls.Draw(spriteBatch);
            return;
        }
        
        var i = 0;
        var y = 774 + font.MeasureString(phrase.Text).Y +
                     (280 - font.MeasureString(phrase.Text).Y - phrase.AnswerVariants.Select(v => font.MeasureString(v.Text).Y+12).Sum()) / 2;
        
        foreach (var variant in phrase.AnswerVariants)
        {
            var text = phrase.SelectedAnswerVariant == variant ? $"> {i+1}. {variant.Text}" : $"{i+1}. {variant.Text}";
            var color = phrase.SelectedAnswerVariant == variant ? textColor : new Color(64, 50, 50);
            var textSize = font.MeasureString(variant.Text);
            var x = 315 + (1032f - textSize.X)/2;
            spriteBatch.DrawString(font, text, new Vector2(x, y)*Game1.ResolutionScale, color, 
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            y += textSize.Y+12;
            i++;
        }
    }

    private void DrawCompassHud(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_compassBGTexture, Vector2.Zero, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var rotation = (float)Math.Atan(
            (10240 - _gameCore.PlayerPosition.X) /
            (_gameCore.PlayerPosition.Y - 512));
        if (_gameCore.PlayerPosition.Y < 512)
        {
            rotation -= (float)Math.PI;
        }
        spriteBatch.Draw(_compassArrowTexture, new Vector2(47, 47.5f)*Game1.ResolutionScale, null, 
            Color.White, rotation, 
            new Vector2((float)(_compassArrowTexture.Width)/2, (float)(_compassArrowTexture.Height)/2), 
            Game1.ResolutionScale, SpriteEffects.None, 1f);

        spriteBatch.DrawString(_gameCore.CompassFont, $"{Game1.GetText("Your position")}:\n" +
                                                      $"X: {Math.Round(_gameCore.PlayerPosition.X/64)}\n" +
                                                      $"Y: {Math.Round(_gameCore.PlayerPosition.Y/64) - 2}", 
            new Vector2(94, 7)*Game1.ResolutionScale, Color.Black,
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
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