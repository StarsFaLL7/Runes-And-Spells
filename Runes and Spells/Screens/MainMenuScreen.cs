using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
    private Song _musicMenu;
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

        _musicMenu = content.Load<Song>("music/MainMenuTheme");
        MediaPlayer.Play(_musicMenu);
        
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
            () => _game.SetScreen(GameScreen.Backstory));
        _buttonsMainMenu = new[] { _buttonNewGame, _buttonContinue, _buttonOptions, _buttonExitGame };
        _sliderMusicVolume = new UiSlider(
            content.Load<Texture2D>("textures/main_menu/ui/slider_back"), 
            content.Load<Texture2D>("textures/main_menu/ui/slider_holder"),
                    new Vector2(1017, 384), 21f, 0f, 1f, 0.1f);
        _sliderEffectsVolume = new UiSlider(
            content.Load<Texture2D>("textures/main_menu/ui/slider_back"), 
            content.Load<Texture2D>("textures/main_menu/ui/slider_holder"),
            new Vector2(1017, 474), 21f, 0f, 1f,0.5f);
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
                SoundEffect.MasterVolume = _sliderEffectsVolume.Value;
                MediaPlayer.Volume = _sliderMusicVolume.Value;
                _sliderEffectsVolume.Update(mouseState, ref _oneElementIsFocused);
                _sliderMusicVolume.Update(mouseState, ref _oneElementIsFocused);
                break;
            case MenuTab.NewGame:
                _buttonStartNewGame.Update(mouseState, ref _oneElementIsFocused);
                break;
        }
        UpdateClouds(graphics);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        drawer.Draw(spriteBatch, graphics, _backgroundTexture, Position.TopLeft, null, 0f, 0f);
        drawer.Draw(spriteBatch, graphics, _gameLogo, Position.MiddleTop, null, 0f, 0f);
        foreach (var cloudInfo in _clouds)
            drawer.Draw(spriteBatch, graphics, cloudInfo.texture2D, Position.Custom, cloudInfo.position, 0f, 0f);
        drawer.Draw(spriteBatch, graphics, _bookTexture, Position.Center, null, 0f, 0f);
        foreach (var button in _buttonsMainMenu)
            spriteBatch.Draw(button.ActualTexture(), button.Position, Color.White);
        
        switch (_currentTab)
        {
            case MenuTab.NewGame:
                DrawTabNewGame(spriteBatch);
                break;
            case MenuTab.Options:
                DrawTabOptions(spriteBatch);
                break;
        }
        
        spriteBatch.DrawString(_game.LogText, $"Music volume: {_sliderMusicVolume.Value}\n" +
                                              $"Effects volume: {_sliderEffectsVolume.Value}\n" +
                                              $"Menu tab: {_currentTab}\n" +
                                              $"Is object focused: {_oneElementIsFocused}\n", new Vector2(0,0), Color.White);
    }
    
    private void DrawTabOptions(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sliderMusicVolume.BackTexture, _sliderMusicVolume.Position, Color.White);
        spriteBatch.Draw(_sliderMusicVolume.HolderTexture, _sliderMusicVolume.GetHolderPosition(), Color.White);
        spriteBatch.Draw(_sliderEffectsVolume.BackTexture, _sliderEffectsVolume.Position, Color.White);
        spriteBatch.Draw(_sliderEffectsVolume.HolderTexture, _sliderEffectsVolume.GetHolderPosition(), Color.White);
        spriteBatch.Draw(_textOptions, new Vector2(1017,204), Color.White);
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