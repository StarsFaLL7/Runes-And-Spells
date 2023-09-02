using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.OtherClasses.SaveAndLoad;
using Runes_and_Spells.OtherClasses.SaveAndLoad.Records;
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

    private UiButton _buttonNewGame;
    private UiButton _buttonContinue;
    private UiButton _buttonOptions;
    private UiButton _buttonExitGame;
    private UiButton[] _buttonsMainMenu;
    private UiButton[] _buttonsLoadGame;
    private UiButton _buttonStartNewGame;
    private UiButton _buttonLangRussian;
    private UiButton _buttonLangEnglish;
    private Texture2D _langSelected;
    private UiSlider _sliderMusicVolume;
    private UiSlider _sliderEffectsVolume;
    private UiCheckbox _checkboxFullScreen;
    private UiDropdown _dropdownResolution;
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

        _bookTexture = content.Load<Texture2D>("textures/main_menu/ui_book");
        _backgroundTexture = content.Load<Texture2D>("textures/main_menu/background");
        _gameLogo = content.Load<Texture2D>("textures/main_menu/logo");
        var paperButtonsTextures = new[]
        {
            content.Load<Texture2D>("textures/buttons/paper_button/paper_menu_button_default"),
            content.Load<Texture2D>("textures/buttons/paper_button/paper_menu_button_hovered"),
            content.Load<Texture2D>("textures/buttons/paper_button/paper_menu_button_pressed")
        };
        var colorTextButtons = new Color(45, 36, 27);
        _buttonNewGame = new UiButton(
            paperButtonsTextures[0],
            paperButtonsTextures[1],
            paperButtonsTextures[2],
            new Vector2(570, 204), 
            "New game", AllGameItems.Font30Px, colorTextButtons,
            () => 
            {
                if (_currentTab != MenuTab.NewGame)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.NewGame;
            }); 
        _buttonContinue = new UiButton(
            paperButtonsTextures[0],
            paperButtonsTextures[1],
            paperButtonsTextures[2],
            new Vector2(570, 339),
            "Continue", AllGameItems.Font30Px, colorTextButtons,
            () => 
            {
                if (_currentTab != MenuTab.Continue)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.Continue;
            });
        _buttonOptions = new UiButton(
            paperButtonsTextures[0],
            paperButtonsTextures[1],
            paperButtonsTextures[2],
            new Vector2(570, 474),
            "Options", AllGameItems.Font30Px, colorTextButtons,
            () => 
            {
                if (_currentTab != MenuTab.Options)
                    _soundEffectPageFlip.Play();
                _currentTab = MenuTab.Options;
                _sliderMusicVolume.SetValue(MediaPlayer.Volume);
                _sliderEffectsVolume.SetValue(SoundEffect.MasterVolume);
                _checkboxFullScreen.IsChecked = _game.Graphics.IsFullScreen;
                _dropdownResolution.SelectVariant(
                    _dropdownResolution.Variants
                        .First(v => v.VisibleText == $"{_game.ScreenWidth}x{_game.ScreenHeight}"));
            });
        _buttonExitGame = new UiButton(
            paperButtonsTextures[0],
            paperButtonsTextures[1],
            paperButtonsTextures[2],
            new Vector2(570, 756),
            "Exit", AllGameItems.Font30Px, colorTextButtons,
            () => _game.Exit());
        _buttonStartNewGame = new UiButton(
            paperButtonsTextures[0],
            paperButtonsTextures[1],
            paperButtonsTextures[2],
            new Vector2(1017, 495),
            "Let's go!", AllGameItems.Font30Px, colorTextButtons,
            () =>
            {
                _game.StartNewGame();
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
        var pos = new Vector2(1017, 339);
        _buttonsLoadGame = new UiButton[3];
        for (var i = 0; i < 3; i++)
        {
            var i1 = i + 1;
            _buttonsLoadGame[i] = new UiButton(
                content.Load<Texture2D>("textures/buttons/empty_button_default"),
                content.Load<Texture2D>("textures/buttons/empty_button_hovered"),
                content.Load<Texture2D>("textures/buttons/empty_button_pressed"),
                new Vector2(pos.X, pos.Y + i * 135), 
                () => {GameLoader.LoadGame(_game, i1); }
            );
        }
        SoundEffect.MasterVolume = _sliderEffectsVolume.Value;
        MediaPlayer.Volume = _sliderMusicVolume.Value;

        _checkboxFullScreen = new UiCheckbox("", "", new Color(47, 41, 33),
            UiCheckbox.TextPos.Right,
            content.Load<Texture2D>("textures/ui/checkbox_checked"),
            content.Load<Texture2D>("textures/ui/checkbox_unchecked"),
            new Vector2(1262, 593), b =>
            {
                _game.Graphics.IsFullScreen = b;
                _game.Graphics.ApplyChanges();
                _game.SaveSettings();
            }, 
            _game, AllGameItems.Font20Px);
        _dropdownResolution = new UiDropdown(AllGameItems.Font18Px, new Color(47, 41, 33), 
            content.Load<Texture2D>("textures/ui/dd_border"),
            content.Load<Texture2D>("textures/ui/dd_back"),
            _game, 
            new Rectangle(1200, 654, 
                (int)AllGameItems.Font18Px.MeasureString("1920x1080").X + 32, 
                (int)AllGameItems.Font18Px.MeasureString("1920x1080").Y + 11),
            Game1.DefaultResolutions.Variants.ToArray());

        
        _buttonLangRussian = new UiButton(
            content.Load<Texture2D>("textures/buttons/lang/russian_button_default"),
            content.Load<Texture2D>("textures/buttons/lang/russian_button_hovered"),
            content.Load<Texture2D>("textures/buttons/lang/russian_button_pressed"),
            new Vector2(1742, 986), () => {_game.SetLanguage(Language.Russian);});
        _buttonLangEnglish = new UiButton(
            content.Load<Texture2D>("textures/buttons/lang/english_button_default"),
            content.Load<Texture2D>("textures/buttons/lang/english_button_hovered"),
            content.Load<Texture2D>("textures/buttons/lang/english_button_pressed"),
            new Vector2(1826, 986), () => {_game.SetLanguage(Language.English);});
        _langSelected = content.Load<Texture2D>("textures/buttons/lang/button_outline");
    }
    
    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _oneElementIsFocused = false;
        foreach (var button in _buttonsMainMenu)
            button.Update(mouseState, ref _oneElementIsFocused);
        _buttonLangEnglish.Update(mouseState, ref _oneElementIsFocused);
        _buttonLangRussian.Update(mouseState, ref _oneElementIsFocused);
        switch (_currentTab)
        {
            case MenuTab.Options:
                _sliderEffectsVolume.Update(mouseState, ref _oneElementIsFocused);
                _sliderMusicVolume.Update(mouseState, ref _oneElementIsFocused);
                _game.SetMusicVolume(_sliderMusicVolume.Value);
                _game.SetSoundsVolume(_sliderEffectsVolume.Value);
                _checkboxFullScreen.Update(mouseState);
                _dropdownResolution.Update(mouseState);
                break;
            case MenuTab.NewGame:
                _buttonStartNewGame.Update(mouseState, ref _oneElementIsFocused);
                break;
            case MenuTab.Continue:
                for (var i = 0; i < _game.SavesFilesPaths.Length; i++)
                {
                    _buttonsLoadGame[i].Update(mouseState, ref _oneElementIsFocused);
                }
                break;
        }
        UpdateClouds(graphics);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), null, Color.White, 0f,
            Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        spriteBatch.Draw(_gameLogo, new Vector2((_game.ScreenWidth-_gameLogo.Width*Game1.ResolutionScale.X)/2f, 0),
            null, Color.White, 0f, Vector2.Zero, 
            Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        foreach (var cloudInfo in _clouds)
            spriteBatch.Draw(cloudInfo.texture2D, 
                new Vector2(cloudInfo.position.X*Game1.ResolutionScale.X, cloudInfo.position.Y*Game1.ResolutionScale.Y), 
                null, Color.White, 0f, Vector2.Zero, 
                Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        spriteBatch.Draw(_bookTexture, new Vector2(
            (_game.ScreenWidth-_bookTexture.Width*Game1.ResolutionScale.X)/2f, (_game.ScreenHeight-_bookTexture.Height*Game1.ResolutionScale.Y)/2f), 
            null, Color.White, 0f, Vector2.Zero, 
            Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        foreach (var button in _buttonsMainMenu)
            button.Draw(spriteBatch);
        
        DrawLanguageMenu(spriteBatch);
        
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

    private void DrawLanguageMenu(SpriteBatch spriteBatch)
    {
        var text = $"{Game1.GetText("Language")}:";
        var txtSize = AllGameItems.Font30Px.MeasureString(text);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, 
            new Vector2(
                _buttonLangEnglish.Position.X + _buttonLangEnglish.ActualTexture().Width - txtSize.X, 
                _buttonLangEnglish.Position.Y - txtSize.Y - 9) * Game1.ResolutionScale, 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _buttonLangEnglish.Draw(spriteBatch);
        _buttonLangRussian.Draw(spriteBatch);
        var pos = Game1.CurrentLanguage == Language.English ? _buttonLangEnglish.Position : _buttonLangRussian.Position;
        spriteBatch.Draw(_langSelected, pos*Game1.ResolutionScale, null, Color.White,
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }
    
    private void DrawTabOptions(SpriteBatch spriteBatch)
    {
        _sliderMusicVolume.Draw(spriteBatch);
        _sliderEffectsVolume.Draw(spriteBatch);
        _checkboxFullScreen.Draw(spriteBatch);
        _dropdownResolution.Draw(spriteBatch);
        
        var y = 200f;
        var title = Game1.GetText("Options");
        var titleSize = AllGameItems.Font40Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font40Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        y += titleSize.Y + 31;
        title = Game1.GetText("Volume");
        titleSize = AllGameItems.Font30Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font30Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        y += titleSize.Y + 17;
        title = Game1.GetText("Music");
        titleSize = AllGameItems.Font20Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font20Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        y += titleSize.Y + 61;
        title = Game1.GetText("Effects");
        titleSize = AllGameItems.Font20Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font20Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        y += titleSize.Y + 60;
        title = Game1.GetText("Video");
        titleSize = AllGameItems.Font30Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font30Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        y += titleSize.Y + 19;
        title = Game1.GetText("Fullscreen");
        titleSize = AllGameItems.Font20Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font20Px, title, 
            new Vector2(1016*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        y += titleSize.Y + 43;
        title = Game1.GetText("Resolution");
        titleSize = AllGameItems.Font20Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font20Px, title, 
            new Vector2(1016*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }

    private void DrawTabLoadGame(SpriteBatch spriteBatch)
    {
        var title = Game1.GetText("Load");
        var titleSize = AllGameItems.Font40Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font40Px, title, 
           new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, 235*Game1.ResolutionScale.Y), 
           new Color(45, 36, 27),
           0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        var i = 0;
        foreach (var fp in _game.SavesFilesPaths)
        {
            _buttonsLoadGame[i].Draw(spriteBatch);
            var str = File.ReadAllLines($@"saves\save{i+1}.sav");
            var gameState = JsonConvert.DeserializeObject<GameStateLoad>(str[9]);
            
            var infoStr = $"{Game1.GetText("Day")}: {gameState.DayCount}. {Game1.GetText("Wallet")}: {gameState.Balance}.\n" +
                          $"{Game1.GetText("Runes unlocked")}: {gameState.RunesUnlocked}.\n" +
                          $"{Game1.GetText("Scrolls unlocked")}: {gameState.ScrollsUnlocked}.";
            var indexStr = (i+1).ToString();
            
            var infoStrSize = AllGameItems.Font18Px.MeasureString(infoStr);
            var indexStrSize = AllGameItems.Font40Px.MeasureString(indexStr);
            
            var color = new Color(47, 41, 33);
            spriteBatch.DrawString(AllGameItems.Font40Px, indexStr, 
                new Vector2(_buttonsLoadGame[i].Position.X + 24, 
                    _buttonsLoadGame[i].Position.Y + (_buttonsLoadGame[i].ActualTexture().Height - indexStrSize.Y)/2 + 4)*Game1.ResolutionScale, 
                color, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            spriteBatch.DrawString(AllGameItems.Font18Px, infoStr, 
                new Vector2(
                    _buttonsLoadGame[i].Position.X + 48 + indexStrSize.X, 
                    _buttonsLoadGame[i].Position.Y + (_buttonsLoadGame[i].ActualTexture().Height - infoStrSize.Y)/2 + 2)*Game1.ResolutionScale, 
                color, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            i++;
        }
    }
    
    private void DrawTabNewGame(SpriteBatch spriteBatch)
    {
        _buttonStartNewGame.Draw(spriteBatch);

        var y = 320f;
        var title = Game1.GetText("Ready to start");
        var titleSize = AllGameItems.Font30Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font30Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        y += titleSize.Y;
        title = Game1.GetText("a new");
        titleSize = AllGameItems.Font30Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font30Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        y += titleSize.Y;
        title = Game1.GetText("adventure?");
        titleSize = AllGameItems.Font30Px.MeasureString(title);
        spriteBatch.DrawString(AllGameItems.Font30Px, title, 
            new Vector2(_game.ScreenWidth/2+(_bookTexture.Width/2 - titleSize.X)/2*Game1.ResolutionScale.X, y*Game1.ResolutionScale.Y), 
            new Color(45, 36, 27),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
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