using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.OtherClasses.SaveAndLoad;
using Runes_and_Spells.OtherClasses.SaveAndLoad.Records;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.classes;

public class OverlayMenu
{
    private Game1 _game;
    public OverlayMenu(Game1 game) => _game = game;

    public bool IsVisible;
    private bool _isPopupVisible;
    
    private bool _isSettingsTab;
    private bool _isSaveTab;
    private Texture2D _backTextureDefault;
    private Texture2D _backTextureSettings;
    private Texture2D _backSaveTab;
    private UiButton _buttonContinue;
    private UiButton _buttonSave;
    private UiButton _buttonExitToMenu;
    private UiButton _buttonSettings;
    private UiButton _buttonCloseSettings;
    private UiButton[] _buttonSaveSlots;
    
    private Texture2D _popupBackTexture;
    private UiButton _buttonYesExit;
    private UiButton _buttonNoExit;
    public UiSlider SliderMusicVolume { get; private set; }
    public UiSlider SliderEffectsVolume { get; private set; }
    private UiCheckbox _checkboxFullScreen;
    private UiDropdown _dropdownResolution;
    
    private bool _isButtonFocused;
    private UiButton _buttonCloseSaveTab;
    private SoundEffect _soundEffectPageFlip;
    private SoundEffect _soundSaved;


    public void LoadContent(ContentManager content)
    {
        _soundEffectPageFlip = content.Load<SoundEffect>("sounds/page_flip");
        _soundSaved = content.Load<SoundEffect>("sounds/sound_saved");
        
        _backTextureDefault = content.Load<Texture2D>("textures/overlay_menu/back");
        _backTextureSettings = content.Load<Texture2D>("textures/overlay_menu/back_settings");
        _backSaveTab = content.Load<Texture2D>("textures/overlay_menu/save_tab");
        _popupBackTexture = content.Load<Texture2D>("textures/overlay_menu/popup/popup_back");
        var colorTextButtons = new Color(90, 80, 65);
        var font = AllGameItems.Font24Px;
        var btnTextures = new[]
        {
            content.Load<Texture2D>("textures/overlay_menu/overlay_button_default"),
            content.Load<Texture2D>("textures/overlay_menu/overlay_button_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/overlay_button_pressed")
        };
        _buttonContinue = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 339), "Continue", font, colorTextButtons,
            () =>
            {
                IsVisible = false; 
                _isPopupVisible = false;
                _soundEffectPageFlip.Play();
            }
            );
        _buttonSave = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 434), "Save", font, colorTextButtons,
            () =>
            {
                _isSaveTab = true;
                _isPopupVisible = false;
                _soundEffectPageFlip.Play();
            }
            );
        _buttonSettings = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 529), "Options", font, colorTextButtons,
            () =>
            {
                _isSettingsTab = true; 
                _isPopupVisible = false;
                SliderMusicVolume.SetValue(MediaPlayer.Volume);
                SliderEffectsVolume.SetValue(SoundEffect.MasterVolume);
                _checkboxFullScreen.IsChecked = _game.Graphics.IsFullScreen;
                _dropdownResolution.SelectVariant(
                    _dropdownResolution.Variants
                        .First(v => v.VisibleText == $"{_game.ScreenWidth}x{_game.ScreenHeight}"));
                _soundEffectPageFlip.Play();
            }
            );
        _buttonExitToMenu = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 666), "Exit", font, colorTextButtons,
            () =>
            {
                _isPopupVisible = true;
                _soundEffectPageFlip.Play();
            }
            );
        _buttonCloseSettings = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 666), "Back", font, colorTextButtons,
            () =>
            {
                _isSettingsTab = false;
                _isSaveTab = false;
                _isPopupVisible = false;
                _soundEffectPageFlip.Play();
            }
        );
        _buttonCloseSaveTab = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(828, 681), "Back", font, colorTextButtons,
            () =>
            {
                _isSettingsTab = false;
                _isSaveTab = false;
                _isPopupVisible = false;
                _soundEffectPageFlip.Play();
            }
        );
        SliderMusicVolume = new UiSlider(
            content.Load<Texture2D>("textures/overlay_menu/slider_back"),
            content.Load<Texture2D>("textures/overlay_menu/slider_holder"),
            new Vector2(828, 414),
            21f, 0f, 1f, MediaPlayer.Volume);
        SliderEffectsVolume = new UiSlider(
            content.Load<Texture2D>("textures/overlay_menu/slider_back"),
            content.Load<Texture2D>("textures/overlay_menu/slider_holder"),
            new Vector2(828, 482),
            21f, 0f, 1f, SoundEffect.MasterVolume);

        _buttonNoExit = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(672, 938), "No", font, colorTextButtons,
            () =>
            {
                _isPopupVisible = false; 
                _soundEffectPageFlip.Play();
            } );
        _buttonYesExit = new UiButton(
            btnTextures[0],btnTextures[1],btnTextures[2],
            new Vector2(996, 938), "Yes", font, colorTextButtons,
            () => {
                if (_game.IsInTopDownView)
                {
                    _game.TopDownCore.SoundTheme.Stop();
                }
                _game.SetScreen(GameScreen.Menu);
                IsVisible = false;
                MediaPlayer.Stop();
                MediaPlayer.Play(_game.MusicMenu);
                Reset();
                _game.Introduction.Stop();
                _game.Introduction.Step = 0;
                _game.IsInTopDownView = false;
                _soundEffectPageFlip.Play();
                
            }
        );
        var pos = new Vector2(796, 356);
        _buttonSaveSlots = new UiButton[3];
        for (var i = 0; i < 3; i++)
        {
            var i2 = i + 1;
            _buttonSaveSlots[i] = new UiButton(
                content.Load<Texture2D>("textures/buttons/empty_button_default"),
                content.Load<Texture2D>("textures/buttons/empty_button_hovered"),
                content.Load<Texture2D>("textures/buttons/empty_button_pressed"),
                new Vector2(pos.X, pos.Y + i * 108),
                () =>
                {
                    GameLoader.SaveGame(_game, i2);
                    _soundSaved.Play();
                }
            );
        }
        _checkboxFullScreen = new UiCheckbox("", "", new Color(47, 41, 33),
            UiCheckbox.TextPos.Right,
            content.Load<Texture2D>("textures/ui/checkbox_checked"),
            content.Load<Texture2D>("textures/ui/checkbox_unchecked"),
            new Vector2(1058, 573), b =>
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
            new Rectangle(996, 606, 
                (int)AllGameItems.Font18Px.MeasureString("1920x1080").X + 32, 
                (int)AllGameItems.Font18Px.MeasureString("1920x1080").Y + 12), 
            Game1.DefaultResolutions.Variants.ToArray());
        
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        var textColor = new Color(48, 56, 67);
        if (_isSettingsTab)
        {
            DrawSettingsTab(spriteBatch, textColor);
        }
        else if (_isSaveTab)
        {
            DrawSaveTab(spriteBatch, textColor);
        }
        else
        {
            spriteBatch.Draw(_backTextureDefault, Vector2.Zero, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            _buttonContinue.Draw(spriteBatch);
            _buttonSave.Draw(spriteBatch);
            _buttonSettings.Draw(spriteBatch);
            _buttonExitToMenu.Draw(spriteBatch);
        }
        if (_isPopupVisible)
        {
            DrawPopup(spriteBatch, textColor);
        }
    }

    private void DrawSettingsTab(SpriteBatch spriteBatch, Color textColor)
    {
        spriteBatch.Draw(_backTextureSettings, Vector2.Zero, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var backTextureWidth = 432;
        var texts = new List<(string text, SpriteFont font, float Y, bool fromLeft)>()
        {
            ("Volume", AllGameItems.Font30Px, 330, false),
            ("Music", AllGameItems.Font24Px, 378, false),
            ("Effects", AllGameItems.Font24Px, 446, false),
            ("Video", AllGameItems.Font30Px, 526, false),
            ("Fullscreen", AllGameItems.Font24Px, 573, true),
            ("Resolution", AllGameItems.Font24Px, 615, true)
        };
        foreach (var textInfo in texts)
        {
            var text = Game1.GetText(textInfo.text);
            var textSize = textInfo.font.MeasureString(text);
            var position = new Vector2(744 + (backTextureWidth - textSize.X) / 2, textInfo.Y);
            if (textInfo.fromLeft)
                position = new Vector2(786, textInfo.Y);
            spriteBatch.DrawString(textInfo.font, text,position * Game1.ResolutionScale, 
                textColor,0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
        _buttonCloseSettings.Draw(spriteBatch);
        SliderEffectsVolume.Draw(spriteBatch);
        SliderMusicVolume.Draw(spriteBatch);
        
        _checkboxFullScreen.Draw(spriteBatch);
        _dropdownResolution.Draw(spriteBatch);
    }
    
    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private void DrawSaveTab(SpriteBatch spriteBatch, Color textColor)
    {
        spriteBatch.Draw(_backTextureDefault, Vector2.Zero, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backSaveTab, 
            new Vector2(1920/2 - _backSaveTab.Width/2, 1080/2 - _backSaveTab.Height/2)*Game1.ResolutionScale, 
            null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var text = Game1.GetText("Choose the slot") + ":";
        var textSize = AllGameItems.Font30Px.MeasureString(text);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, 
            new Vector2(1920/2 - textSize.X/2, 1080/2 - _backSaveTab.Height/2 + 11)*Game1.ResolutionScale,
            textColor,0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _buttonCloseSaveTab.Draw(spriteBatch);
        var i = 0;
        foreach (var button in _buttonSaveSlots)
        {
            button.Draw(spriteBatch);
            var infoStr = Game1.GetText("Empty slot");
            if (File.Exists($@"saves\save{i+1}.sav"))
            {
                var str = File.ReadAllLines($@"saves\save{i+1}.sav");
                var gameState = JsonConvert.DeserializeObject<GameStateLoad>(str[9]);
                infoStr = $"{Game1.GetText("Day")}: {gameState.DayCount}. {Game1.GetText("Wallet")}: {gameState.Balance}.\n" +
                    $"{Game1.GetText("Runes unlocked")}: {gameState.RunesUnlocked}.\n" +
                    $"{Game1.GetText("Scrolls unlocked")}: {gameState.ScrollsUnlocked}.";
            }
            var indexStr = (i+1).ToString();
            var infoStrSize = AllGameItems.Font18Px.MeasureString(infoStr);
            var indexStrSize = AllGameItems.Font40Px.MeasureString(indexStr);
            var color = new Color(47, 41, 33);
            spriteBatch.DrawString(AllGameItems.Font40Px, indexStr, 
                new Vector2(_buttonSaveSlots[i].Position.X + 24, 
                _buttonSaveSlots[i].Position.Y + (_buttonSaveSlots[i].ActualTexture().Height - indexStrSize.Y)/2 + 4)*Game1.ResolutionScale, 
                color, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            spriteBatch.DrawString(AllGameItems.Font18Px, infoStr,
                new Vector2(
                _buttonSaveSlots[i].Position.X + 48 + indexStrSize.X, 
                _buttonSaveSlots[i].Position.Y + (_buttonSaveSlots[i].ActualTexture().Height - infoStrSize.Y)/2 + 2)*Game1.ResolutionScale, 
                color, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            i++;
        }
    }

    private void DrawPopup(SpriteBatch spriteBatch,  Color textColor)
    {
        spriteBatch.Draw(_popupBackTexture, new Vector2(645, 818)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var lines = Game1.GetText("Are you sure you want to exit?").Split("\\n");
        var i = 0;
        var font = AllGameItems.Font30Px;
        foreach (var line in lines)
        {
            var lineSize = font.MeasureString(line);
            var position = new Vector2(
                645 + (_popupBackTexture.Width - lineSize.X )/2, 
                818 + (20 + font.LineSpacing*i));
            spriteBatch.DrawString(font, line, position*Game1.ResolutionScale, textColor,
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            i++;
        }
        _buttonNoExit.Draw(spriteBatch);
        _buttonYesExit.Draw(spriteBatch);
    }

    public void Update()
    {
        SliderMusicVolume.SetValue(MediaPlayer.Volume);
        SliderEffectsVolume.SetValue(SoundEffect.MasterVolume);
        var mouseState = Mouse.GetState();
        if (_isPopupVisible)
        {
            _buttonYesExit.Update(mouseState, ref _isButtonFocused);
            _buttonNoExit.Update(mouseState, ref _isButtonFocused);
        }
        
        if (_isSettingsTab)
        {
            _buttonCloseSettings.Update(mouseState, ref _isButtonFocused);
            SliderMusicVolume.Update(mouseState, ref _isButtonFocused);
            SliderEffectsVolume.Update(mouseState, ref _isButtonFocused);
            _checkboxFullScreen.Update(mouseState);
            _dropdownResolution.Update(mouseState);
            _game.SetMusicVolume(SliderMusicVolume.Value);
            _game.SetSoundsVolume(SliderEffectsVolume.Value);
        }
        else if (_isSaveTab)
        {
            _buttonCloseSaveTab.Update(mouseState, ref _isButtonFocused);
            foreach (var button in _buttonSaveSlots)
            {
                button.Update(mouseState, ref _isButtonFocused);
            }
        }
        else
        {
            _buttonContinue.Update(mouseState, ref _isButtonFocused);
            _buttonSave.Update(mouseState, ref _isButtonFocused);
            _buttonSettings.Update(mouseState, ref _isButtonFocused);
            _buttonExitToMenu.Update(mouseState, ref _isButtonFocused);
        }
    }

    public void Reset() 
    {
        _isSettingsTab = false;
        _isSaveTab = false;
        _isPopupVisible = false;
    }
}