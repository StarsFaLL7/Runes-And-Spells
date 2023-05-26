using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Runes_and_Spells.classes;

public class OverlayMenu
{
    private Game1 _game;
    public OverlayMenu(Game1 game) => _game = game;

    public bool IsVisible;
    private bool _isPopupVisible;
    
    private bool _isSettingsTab;
    private Texture2D _backTextureDefault;
    private Texture2D _backTextureSettings;
    private UiButton _buttonContinue;
    private UiButton _buttonSave;
    private UiButton _buttonExitToMenu;
    private UiButton _buttonSettings;
    private UiButton _buttonCloseSettings;

    private Texture2D _popupBackTexture;
    private UiButton _buttonYesExit;
    private UiButton _buttonNoExit;
    public UiSlider SliderMusicVolume { get; private set; }
    public UiSlider SliderEffectsVolume { get; private set; }
    private bool _isButtonFocused;
    
    public void LoadContent(ContentManager content)
    {
        _backTextureDefault = content.Load<Texture2D>("textures/overlay_menu/back");
        _backTextureSettings = content.Load<Texture2D>("textures/overlay_menu/back_settings");
        _popupBackTexture = content.Load<Texture2D>("textures/overlay_menu/popup/popup_back");
        _buttonContinue = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/button_continue_default"),
            content.Load<Texture2D>("textures/overlay_menu/button_continue_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/button_continue_pressed"),
            new Vector2(828, 339),
            () =>
            {
                IsVisible = false; 
                _isPopupVisible = false;
            }
            );
        _buttonSave = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/button_save_default"),
            content.Load<Texture2D>("textures/overlay_menu/button_save_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/button_save_pressed"),
            new Vector2(828, 434),
            () => { _isPopupVisible = false; }
            );
        _buttonSettings = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/button_settings_default"),
            content.Load<Texture2D>("textures/overlay_menu/button_settings_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/button_settings_pressed"),
            new Vector2(828, 529),
            () =>
            {
                _isSettingsTab = true; 
                _isPopupVisible = false;
            }
            );
        _buttonExitToMenu = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/button_exit_default"),
            content.Load<Texture2D>("textures/overlay_menu/button_exit_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/button_exit_pressed"),
            new Vector2(828, 666),
            () =>
            {
                _isPopupVisible = true;
            }
            );
        _buttonCloseSettings = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/button_back_default"),
            content.Load<Texture2D>("textures/overlay_menu/button_back_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/button_back_pressed"),
            new Vector2(828, 666),
            () =>
            {
                _isSettingsTab = false;
                _isPopupVisible = false;
            }
        );
        SliderMusicVolume = new UiSlider(
            content.Load<Texture2D>("textures/overlay_menu/slider_back"),
            content.Load<Texture2D>("textures/overlay_menu/slider_holder"),
            new Vector2(828, 470),
            21f, 0f, 1f, MediaPlayer.Volume);
        SliderEffectsVolume = new UiSlider(
            content.Load<Texture2D>("textures/overlay_menu/slider_back"),
            content.Load<Texture2D>("textures/overlay_menu/slider_holder"),
            new Vector2(828, 571),
            21f, 0f, 1f, SoundEffect.MasterVolume);

        _buttonNoExit = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/popup/button_no_default"),
            content.Load<Texture2D>("textures/overlay_menu/popup/button_no_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/popup/button_no_pressed"),
            new Vector2(672, 938),
            () => { _isPopupVisible = false; } );
        _buttonYesExit = new UiButton(
            content.Load<Texture2D>("textures/overlay_menu/popup/button_yes_default"),
            content.Load<Texture2D>("textures/overlay_menu/popup/button_yes_hovered"),
            content.Load<Texture2D>("textures/overlay_menu/popup/button_yes_pressed"),
            new Vector2(996, 938),
            () => { 
                _game.SetScreen(GameScreen.Menu);
                IsVisible = false;
                MediaPlayer.Stop();
                MediaPlayer.Play(_game.MusicMenu);
                Reset();
            }
        );
        
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_isSettingsTab)
        {
            spriteBatch.Draw(_backTextureSettings, Vector2.Zero, Color.White);
            _buttonCloseSettings.Draw(spriteBatch);
            SliderEffectsVolume.Draw(spriteBatch);
            SliderMusicVolume.Draw(spriteBatch);
        }
        else
        {
            spriteBatch.Draw(_backTextureDefault, Vector2.Zero, Color.White);
            _buttonContinue.Draw(spriteBatch);
            _buttonSave.Draw(spriteBatch);
            _buttonSettings.Draw(spriteBatch);
            _buttonExitToMenu.Draw(spriteBatch);
        }

        if (_isPopupVisible)
        {
            spriteBatch.Draw(_popupBackTexture, new Vector2(645, 818), Color.White);
            _buttonNoExit.Draw(spriteBatch);
            _buttonYesExit.Draw(spriteBatch);
        }
    }

    public void Update()
    {
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
            _game.SetMusicVolume(SliderMusicVolume.Value);
            _game.SetSoundsVolume(SliderEffectsVolume.Value);
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
        _isPopupVisible = false;
    }
}