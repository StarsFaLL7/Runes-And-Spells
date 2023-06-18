using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class AltarRoomScreen : IScreen
{
    private UiButton _buttonGoBack;
    private UiButton _buttonGoToAltar;
    private Texture2D _backgroundTexture;
    private Game1 _game;
    public AltarRoomScreen(Game1 game) => _game = game;
    private bool _isButtonFocused;
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _buttonGoToAltar = new UiButton(
            content.Load<Texture2D>("textures/altar_room_screen/altar_button_default"),
            content.Load<Texture2D>("textures/altar_room_screen/altar_button_hovered"),
            content.Load<Texture2D>("textures/altar_room_screen/altar_button_hovered"),
            new Vector2(770, 769),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 16)
                {
                    _game.Inventory.Clear();
                    _game.Inventory.AddItem(new Item(AllGameItems.FinishedRunes["rune_finished_grass_1_1"]));
                    _game.Inventory.AddItem(new Item(AllGameItems.FinishedRunes["rune_finished_grass_1_2"]));
                    _game.Introduction.Step = 17;
                }
                _game.SetScreen(GameScreen.AltarScreen);
            } );
        _buttonGoBack = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_left_screen_default"),
            content.Load<Texture2D>("textures/buttons/button_left_screen_hovered"),
            content.Load<Texture2D>("textures/buttons/button_left_screen_pressed"),
            new Vector2(0, 472),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 19) _game.Introduction.Step = 20;
                _game.SetScreen(GameScreen.MainHouseScreen);
            } );
        _backgroundTexture = content.Load<Texture2D>("textures/altar_room_screen/room_with_altar");
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_game.Introduction.IsPlaying)
        {
            switch (_game.Introduction.Step)
            {
                case 16:
                    _buttonGoToAltar.Update(mouseState, ref _isButtonFocused);
                    break;
                case 19:
                    _buttonGoBack.Update(mouseState, ref _isButtonFocused);
                    break;
            }
        }
        else
        {
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
            _buttonGoToAltar.Update(mouseState, ref _isButtonFocused);
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);
        _buttonGoBack.Draw(spriteBatch);
        _buttonGoToAltar.Draw(spriteBatch);
    }
}