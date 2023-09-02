using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.TopDownGame.NPCData;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Core;

public class TopDownCore
{
    public readonly GameMap Map;
    public readonly GameView View;
    public readonly Controller Controller;
    public readonly Game1 Game;
    public bool IsDialogOpened { get; set; }
    public Vector2 CameraPosition { get; set; }
    public Vector2 PlayerPosition { get; set; }
    public Direction PlayerLastLookDirection { get; set; }
    public bool IsPlayerMoving;
    public bool IsPlayerRunning;
    public bool PlayerHasCompass { get; set; }
    public SpriteFont CompassFont;
    public bool IsFinalQuestPlaying;
    public bool PlayerHasWings;
    public bool PlayerIsOnWings;
    public List<NPC> FinalTeam { get; } = new ();
    public string FinalScroll;
    public Timer TimerStepSound;
    public SoundEffectInstance SoundTheme;

    public TopDownCore(ContentManager content, Game1 game)
    {
        Game = game;
        CameraPosition = new Vector2(
            GameMap.MapWidth * GameMap.TileSize / 2 - Game.ScreenWidth/2, 
            GameMap.MapHeight * GameMap.TileSize / 2 - Game.ScreenHeight/2);
        CameraPosition = new Vector2(0, 0);
        PlayerPosition = new Vector2((GameMap.MapWidth-3)*GameMap.TileSize, 640);
        
        AllMapDynamicObjects.Initialize(content);
        AllDialogs.Initialize();
        Map = new GameMap(this);
        Map.GenerateMap(CameraPosition);
        View = new GameView(this, content.Load<Texture2D>("top-down/spritesheets/tiles"),
            content.Load<Texture2D>("top-down/spritesheets/player_animations"),
            content.Load<Texture2D>("top-down/spritesheets/map_objects"),content.Load<SpriteFont>("logText"));
        Controller = new Controller(this);
        CompassFont = content.Load<SpriteFont>("16PixelTimes20px");
        CameraPosition = new Vector2(GameMap.MapWidth*GameMap.TileSize-Game.ScreenWidth, 0);
        TimerStepSound = new Timer(100, () =>
        {
            AllGameItems.StepSound.Play();
            Game.SubtractEnergy(0.05f);
        });
    }
    
    public void Initialize(ContentManager content)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        View.Draw(Map, CameraPosition, spriteBatch);
        Game.Inventory.Draw(Game.Graphics, spriteBatch);
    }
    
    public void Update()
    {
        TimerStepSound.Tick();
        if (!IsDialogOpened)
            Controller.Update();

        if (new Rectangle((GameMap.MapWidth-1)*GameMap.TileSize + 1, 384, GameMap.TileSize/2, GameMap.TileSize*5).Contains(PlayerPosition))
        {
            if (Game.Introduction.IsPlaying)
            {
                if (Game.Introduction.Step == 2)
                    Game.Introduction.Step = 3;
                else if (Game.Introduction.Step == 29)
                    Game.Introduction.Step = 30;
                else
                    return;
            }
            Game.IsInTopDownView = false;
            Game.SetScreen(GameScreen.MainHouseScreen);
            PlayerPosition = new Vector2(PlayerPosition.X - GameMap.TileSize, PlayerPosition.Y);
            SoundTheme.Stop();
            return;
        }
        if (new Rectangle((GameMap.MapWidth-8)*GameMap.TileSize - 1, 128, GameMap.TileSize*5, GameMap.TileSize/2).Contains(PlayerPosition))
        {
            if (Game.Introduction.IsPlaying)
            {
                if (Game.Introduction.Step == 24)
                {
                    Game.Introduction.Step = 25;
                    Game.AddToBalance(100);
                }
                else
                    return;
            }
            Game.IsInTopDownView = false;
            Game.SetScreen(GameScreen.TradingScreen);
            PlayerPosition = new Vector2(PlayerPosition.X, PlayerPosition.Y + GameMap.TileSize);
            MediaPlayer.Play(AllGameItems.MarketTheme);
            SoundTheme.Stop();
            return;
        }

        if (View.DialogNPCToDraw is not null && View.DialogNPCToDraw.IsQuestActive)
        {
            Game.Inventory.Update(Game.Graphics, View.DialogNPCToDraw.SlotForScrolls);
        }
        else
        {
            Game.Inventory.Update(Game.Graphics);
        }
        
        foreach (var tile in Map.Tiles)
        {
            if (tile.PositionInPixels.X + GameMap.TileSize - CameraPosition.X < -63 || 
                tile.PositionInPixels.Y + GameMap.TileSize - CameraPosition.Y < -63 ||
                tile.PositionInPixels.X > CameraPosition.X + Game.ScreenWidth ||
                tile.PositionInPixels.Y > CameraPosition.Y + Game.ScreenHeight
               )
            {
                tile.IsVisible = false;
            }
            else
            {
                tile.IsVisible = true;
            }
        }
        foreach (var obj in Map.FrontObjects.Concat(Map.MudPuddles).Concat(Map.NPCList))
        {
            if (obj.GetType() == typeof(Chest))
            {
                var chest = (Chest)obj;
                chest.Update();
            }
            if (obj.PositionInPixelsLeftBottom.X + obj.SpriteSheetRectangle.Width < CameraPosition.X ||
                obj.PositionInPixelsLeftBottom.Y - obj.SpriteSheetRectangle.Height >
                CameraPosition.Y + Game.ScreenHeight ||
                obj.PositionInPixelsLeftBottom.X > CameraPosition.X + Game.ScreenWidth ||
                obj.PositionInPixelsLeftBottom.Y < CameraPosition.Y)
            {
                obj.IsVisible = false;
            }
            else obj.IsVisible = true;

            if (!obj.IsVisible) continue;
            if (obj.GetType() == typeof(MudPuddle))
            {
                var puddle = (MudPuddle)obj;
                puddle.Update();
            }
            if (obj.GetType() == typeof(NPC))
            {
                var npc = (NPC)obj;
                npc.Update();
            }
        }
    }
}