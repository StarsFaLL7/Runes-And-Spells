using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class MainHouseScreen : IScreen
{
    private readonly Game1 _game;
    private Texture2D _backgroundTexture;
    private Texture2D _dayPanelTexture;
    private SpriteFont _font40Px;
    private UiButton _buttonBed;
    private UiButton _buttonTableScrolls;
    private UiButton _buttonTableRunes;
    private UiButton _buttonFurnace;
    private List<UiButton> _furnitureButtons;
    private bool _isButtonFocused;
    private UiButton _buttonScreenAltar;
    private UiButton _buttonScreenOutside;
    private UiAnimatedTexture _uiAnimatedClock;
    private UiFadingTexture _sleepingBg;
    private bool _isSleeping;
    private Timer _sleepTimer;
    
    public MainHouseScreen(Game1 game) => _game = game;
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/main_house_screen/background");
        _dayPanelTexture = content.Load<Texture2D>("textures/main_house_screen/day_panel_back");
        _font40Px = content.Load<SpriteFont>("16PixelTimes40px");
        _buttonBed = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/bed_default"),
            content.Load<Texture2D>("textures/main_house_screen/bed_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/bed_hovered"),
            new Vector2(0, 526),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 30)
                {
                    _game.ResetForNewGame();
                    _game.Introduction.Stop();
                }
                _sleepingBg.StartFade();
                _game.NextDay();
            });
        _buttonTableScrolls = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_default"),
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_hovered"),
            new Vector2(978, 523),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 20)
                {
                    _game.Inventory.Clear();
                    _game.Inventory.AddItem(new Item(AllGameItems.Paper));
                    _game.Inventory.AddItem(new Item(AllGameItems.FinishedRunes["rune_finished_water_1_1"]));
                    _game.Inventory.AddItem(new Item(AllGameItems.FinishedRunes["rune_finished_water_1_2"]));
                    _game.Introduction.Step = 21;
                }
                _game.SetScreen(GameScreen.ScrollsCraftingTable);
            });
        _buttonTableRunes = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/table_runes_default"),
            content.Load<Texture2D>("textures/main_house_screen/table_runes_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/table_runes_hovered"),
            new Vector2(565, 527),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 3) _game.Introduction.Step = 4;
                _game.SetScreen(GameScreen.RuneCraftingTable);
            });
        _buttonFurnace = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/furnace_default"),
            content.Load<Texture2D>("textures/main_house_screen/furnace_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/furnace_hovered"),
            new Vector2(1429, 0),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 9) _game.Introduction.Step = 10;
                _game.SetScreen(GameScreen.FurnaceScreen);
            });
        _furnitureButtons = new List<UiButton> { _buttonBed, _buttonFurnace, _buttonTableRunes, _buttonTableScrolls };
        _buttonScreenAltar = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_right_screen_default"),
            content.Load<Texture2D>("textures/buttons/button_right_screen_hovered"),
            content.Load<Texture2D>("textures/buttons/button_right_screen_pressed"),
            new Vector2(1825, 472),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 15) _game.Introduction.Step = 16;
                _game.SetScreen(GameScreen.AltarRoomScreen);
            } );
        _buttonScreenOutside = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_default"),
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_hovered"),
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_pressed"),
            new Vector2(892, 985),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 0) _game.Introduction.Step = 1;
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 24) _game.Introduction.Step = 25;
                _game.IsInTopDownView = true;
                _game.TopDownCore.PlayerLastLookDirection = Direction.Left;
            } );
        _uiAnimatedClock = new UiAnimatedTexture(
            200, 
            content.Load<Texture2D>("textures/animated/clock"),
            new Vector2(192, 192), 
            true);
        _sleepingBg = new UiFadingTexture(content.Load<Texture2D>("textures/backgrounds/sleeping"),
            3,
            UiFadingTexture.Mode.FadeIn,
            () => {
                if (_sleepingBg.FadeMode == UiFadingTexture.Mode.FadeIn)
                    _isSleeping = true;
                else
                    _sleepingBg.Reset(UiFadingTexture.Mode.FadeIn);
                _sleepTimer.StartWithTime(Random.Shared.Next(5000, 8000)); 
            }); 

        _sleepTimer = new Timer(Random.Shared.Next(5000, 8000), () =>
        {
            _isSleeping = false;
            _sleepingBg.Reset(UiFadingTexture.Mode.FadeOut);
            _sleepingBg.StartFade();
            _uiAnimatedClock.SetRandomFrame();
        });
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        if (_isSleeping || _sleepingBg.IsFading) return;
        
        var mouseState = Mouse.GetState();
        if (_game.Introduction.IsPlaying)
        {
            switch (_game.Introduction.Step)
            {
                case 0:
                    _buttonScreenOutside.Update(mouseState, ref _isButtonFocused);
                    break;
                case 3:
                    _buttonTableRunes.Update(mouseState, ref _isButtonFocused);
                    break;
                case 9:
                    _buttonFurnace.Update(mouseState, ref _isButtonFocused);
                    break;
                case 15:
                    _buttonScreenAltar.Update(mouseState, ref _isButtonFocused);
                    break;
                case 20:
                    _buttonTableScrolls.Update(mouseState, ref _isButtonFocused);
                    break;
                case 24:
                    _buttonScreenOutside.Update(mouseState, ref _isButtonFocused);
                    break;
                case 30:
                    _buttonBed.Update(mouseState, ref _isButtonFocused);
                    break;
            }
        }
        else
        {
            foreach (var button in _furnitureButtons)
            {
                button.Update(mouseState, ref _isButtonFocused);
            }
            _buttonScreenAltar.Update(mouseState, ref _isButtonFocused);
            _buttonScreenOutside.Update(mouseState, ref _isButtonFocused);
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);
        spriteBatch.Draw(_dayPanelTexture, Vector2.Zero, Color.White);
        var str = "День: " + _game.DayCount;
        if (_sleepingBg.IsFading && _sleepingBg.FadeMode is UiFadingTexture.Mode.FadeIn)
        {
            str = "День: " + (_game.DayCount - 1);
        }
        var stringSize = _font40Px.MeasureString(str);
        spriteBatch.DrawString(_font40Px, str, 
            new Vector2(
                _dayPanelTexture.Width/2 - stringSize.X / 2, 
                _dayPanelTexture.Height/2 - stringSize.Y/2), new Color(20, 35, 58));
        foreach (var button in _furnitureButtons) button.Draw(spriteBatch);
        _buttonScreenAltar.Draw(spriteBatch);
        _buttonScreenOutside.Draw(spriteBatch);
        _sleepingBg.Draw(Vector2.Zero, spriteBatch);
        if (_isSleeping)
        {
            _uiAnimatedClock.Draw(new Vector2(864, 444), spriteBatch);
            _sleepTimer.Tick();
        }
    }
}