using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public enum MenuTab
{
    Empty,
    NewGame,
    Continue,
    Options
}

public class MainMenuScreen: IScreen
{
    private readonly Game1 _game;
    public MainMenuScreen(Game1 game) => _game = game;

    private Texture2D _bookTexture;
    private Texture2D _backgroundTexture;
    private Texture2D[] _cloudsTextures;
    private List<(Vector2 position, Texture2D texture2D)> _clouds;
    private Texture2D _gameLogo;
    private Texture2D _textOptions;
    private Texture2D _textNewGame;
    
    private UiButton _buttonNewGame;
    private UiButton _buttonContinue;
    private UiButton _buttonOptions;
    private UiButton _buttonExitGame;
    private UiButton[] _buttonsMainMenu;
    private UiButton _buttonStartNewGame;
    private UiSlider _sliderMusicVolume;
    private UiSlider _sliderEffectsVolume;
    private MenuTab _currentTab;
    private SoundEffect _soundEffectPageFlip;
    
    private bool _oneElementIsFocused;
    
    private Timer _timerClouds;

    public void Initialize()
    {
        _clouds = new List<(Vector2 position, Texture2D texture2D)>();
        _timerClouds = new Timer(1, () =>
        {
            _clouds.Add(
                (new Vector2(1920, (1080 - Random.Shared.Next(-200, 200)) / 2f), 
                    _cloudsTextures[Random.Shared.Next(0, 4)])
            );
            _timerClouds.StartWithTime(Random.Shared.Next(10000, 12000));
        });
        _timerClouds.Start();
        MediaPlayer.IsRepeating = true;
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _soundEffectPageFlip = content.Load<SoundEffect>("sounds/page_flip");
        _textOptions = content.Load<Texture2D>("textures/main_menu/ui/text_options");
        _textNewGame = content.Load<Texture2D>("textures/main_menu/ui/text_new_game");
        
        _bookTexture = content.Load<Texture2D>("textures/main_menu/ui_book");
        _backgroundTexture = content.Load<Texture2D>("textures/main_menu/background");
        _gameLogo = content.Load<Texture2D>("textures/main_menu/logo");
        _buttonNewGame = new UiButton(
            content.Load<Texture2D>("textures/main_menu/ui/buttons/new_game_simple"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/new_game_hovered"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/new_game_pressed"),
            new Vector2(570, 204),
            () => 
            {
                if (_currentTab != MenuTab.NewGame)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.NewGame;
            }); 
        _buttonContinue = new UiButton(
            content.Load<Texture2D>("textures/main_menu/ui/buttons/continue_simple"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/continue_hovered"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/continue_pressed"),
            new Vector2(570, 339),
            () => 
            {
                if (_currentTab != MenuTab.Continue)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.Continue;
            });
        _buttonOptions = new UiButton(
            content.Load<Texture2D>("textures/main_menu/ui/buttons/options_simple"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/options_hovered"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/options_pressed"),
            new Vector2(570, 474), 
            () => 
            {
                if (_currentTab != MenuTab.Options)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.Options;
                _sliderMusicVolume.SetValue(MediaPlayer.Volume);
                _sliderEffectsVolume.SetValue(SoundEffect.MasterVolume);
            });
        _buttonExitGame = new UiButton(
            content.Load<Texture2D>("textures/main_menu/ui/buttons/exit_simple"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/exit_hovered"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/exit_pressed"),
            new Vector2(570, 756),
            () => _game.Exit());
        _buttonStartNewGame = new UiButton(
            content.Load<Texture2D>("textures/main_menu/ui/buttons/start_new_game_simple"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/start_new_game_hovered"),
            content.Load<Texture2D>("textures/main_menu/ui/buttons/start_new_game_pressed"),
            new Vector2(1017, 495),
            () =>
            {
                _game.ResetBackStory();
                _game.SetScreen(GameScreen.Backstory);
                MediaPlayer.Stop();
                MediaPlayer.Play(_game.BackstoryMusic);
            });
        _buttonsMainMenu = new[] { _buttonNewGame, _buttonContinue, _buttonOptions, _buttonExitGame };
        _sliderMusicVolume = new UiSlider(
            content.Load<Texture2D>("textures/main_menu/ui/slider_back"), 
            content.Load<Texture2D>("textures/main_menu/ui/slider_holder"),
                    new Vector2(1017, 384), 21f, 0f, 1f, MediaPlayer.Volume);
        _sliderEffectsVolume = new UiSlider(
            content.Load<Texture2D>("textures/main_menu/ui/slider_back"), 
            content.Load<Texture2D>("textures/main_menu/ui/slider_holder"),
            new Vector2(1017, 474), 21f, 0f, 1f,SoundEffect.MasterVolume);
        _cloudsTextures = new[]
        {
            content.Load<Texture2D>("textures/main_menu/cloud1"),
            content.Load<Texture2D>("textures/main_menu/cloud2"),
            content.Load<Texture2D>("textures/main_menu/cloud3"),
            content.Load<Texture2D>("textures/main_menu/cloud4"),
        };
        SoundEffect.MasterVolume = _sliderEffectsVolume.Value;
        MediaPlayer.Volume = _sliderMusicVolume.Value;
    }
    
    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _oneElementIsFocused = false;
        foreach (var button in _buttonsMainMenu)
            button.Update(mouseState, ref _oneElementIsFocused);
        
        switch (_currentTab)
        {
            case MenuTab.Options:
                _sliderEffectsVolume.Update(mouseState, ref _oneElementIsFocused);
                _sliderMusicVolume.Update(mouseState, ref _oneElementIsFocused);
                _game.SetMusicVolume(_sliderMusicVolume.Value);
                _game.SetSoundsVolume(_sliderEffectsVolume.Value);
                break;
            case MenuTab.NewGame:
                _buttonStartNewGame.Update(mouseState, ref _oneElementIsFocused);
                break;
        }
        UpdateClouds(graphics);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0,0), Color.White);
        
        spriteBatch.Draw(_gameLogo, new Vector2((Game1.ScreenWidth-_gameLogo.Width)/2, 0), Color.White);
        
        foreach (var cloudInfo in _clouds)
            spriteBatch.Draw(cloudInfo.texture2D, cloudInfo.position, Color.White);
        spriteBatch.Draw(_bookTexture, new Vector2(
            (Game1.ScreenWidth-_bookTexture.Width)/2,
            (Game1.ScreenHeight-_bookTexture.Height)/2), Color.White);
        foreach (var button in _buttonsMainMenu)
            button.Draw(spriteBatch);
        
        switch (_currentTab)
        {
            case MenuTab.NewGame:
                DrawTabNewGame(spriteBatch);
                break;
            case MenuTab.Options:
                DrawTabOptions(spriteBatch);
                break;
            case MenuTab.Continue:
                DrawTabLoadGame(spriteBatch);
                break;
        }
    }
    
    private void DrawTabOptions(SpriteBatch spriteBatch)
    {
        _sliderMusicVolume.Draw(spriteBatch);
        _sliderEffectsVolume.Draw(spriteBatch);
        spriteBatch.Draw(_textOptions, new Vector2(1017,204), Color.White);
    }

    private void DrawTabLoadGame(SpriteBatch spriteBatch)
    {
        var str = "Данный раздел\nнаходится в разработке :)";
        var size = AllMapDynamicObjects.DialogSpriteFont.MeasureString(str);
        spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, str, 
            new Vector2(Game1.ScreenWidth/2+57, (Game1.ScreenHeight-_bookTexture.Height)/2 + 57), new Color(45, 36, 27));
    }
    
    private void DrawTabNewGame(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_buttonStartNewGame.ActualTexture(), _buttonStartNewGame.Position, Color.White);
        spriteBatch.Draw(_textNewGame, new Vector2(1015, 353), Color.White);
    }

    private void UpdateClouds(GraphicsDeviceManager graphics)
    {
        _timerClouds.Tick();
        for (var i = 0; i < _clouds.Count; i++)
        {
            if (_clouds[i].position.X < -_clouds[i].texture2D.Width)
                _clouds.Remove(_clouds[i]);
            else
                _clouds[i] = (new Vector2(_clouds[i].position.X - 0.6f, _clouds[i].position.Y), _clouds[i].texture2D);
        }
    }
}