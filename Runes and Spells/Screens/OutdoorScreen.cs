using System;
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

public class OutdoorScreen : IScreen
{
    private Game1 _game;
    public OutdoorScreen(Game1 game) => _game = game;
    
    private Texture2D _background;
    private UiButton _buttonGoHome;
    private UiButton _buttonGoToMarket;
    private UiButton _buttonPuddle;
    private bool _isButtonFocused;
    private float _alphaValue = 1f;
    private float _fadeDecrement = -0.02f;
    private Timer _animationTimer;
    private Vector2 _animStartPos;
    private int _animClayCount;
    private bool _isPlayingAnimation;
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _background = content.Load<Texture2D>("textures/backgrounds/outdoor");
        _buttonGoHome = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_default"),
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_hovered"),
            content.Load<Texture2D>("textures/buttons/button_bottom_screen_pressed"),
            new Vector2(892, 985),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 2) _game.Introduction.Step = 3;
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 29) _game.Introduction.Step = 30;
                _game.SetScreen(GameScreen.MainHouseScreen);
            } );
        _buttonGoToMarket = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_right_screen_default"),
            content.Load<Texture2D>("textures/buttons/button_right_screen_hovered"),
            content.Load<Texture2D>("textures/buttons/button_right_screen_pressed"),
            new Vector2(1825, 472),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 25)
                {
                    _game.Introduction.Step = 26;
                    _game.Inventory.Clear();
                    _game.Inventory.AddItem(new Item(AllGameItems.Scrolls["scroll_ocean_flow_1"]));
                }
                _game.SetScreen(GameScreen.TradingScreen);
            } );
        
        _buttonPuddle = new UiButton(
            content.Load<Texture2D>("textures/buttons/clay_puddle"),
            content.Load<Texture2D>("textures/buttons/clay_puddle"),
            content.Load<Texture2D>("textures/buttons/clay_puddle"),
            new Vector2(441, 333),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 1) _game.Introduction.Step = 2;
                _game.ClayClaimed = true;
                _animClayCount = Random.Shared.Next(5, 8);
                var random = Random.Shared.Next(0, 100);
                if (random < 10)
                    _animClayCount += Random.Shared.Next(5, 8);
                else if (random < 30)
                    _animClayCount += Random.Shared.Next(2, 4);
                
                _animStartPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                _isPlayingAnimation = true;
                _game.Inventory.AddItem(
                new Item(AllGameItems.Clay.Type, AllGameItems.Clay.Texture, AllGameItems.Clay.ID, AllGameItems.Clay.ToolTip), _animClayCount);
                _animationTimer.StartAgain();
            } );
        _animationTimer = new Timer(20, () => {
            _alphaValue += _fadeDecrement;
            _animStartPos.Y -= 0.5f;
            if (_alphaValue > 0)
                _animationTimer.StartAgain();
            else
            {
                _isPlayingAnimation = false;
                _alphaValue = 1f;
            }
        });
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (!_game.ClayClaimed)
            _buttonPuddle.Update(mouseState, ref _isButtonFocused);
        if (_game.Introduction.IsPlaying)
        {
            switch (_game.Introduction.Step)
            {
                case 2:
                    _buttonGoHome.Update(mouseState, ref _isButtonFocused);
                    break;
                case 25:
                    _buttonGoToMarket.Update(mouseState, ref _isButtonFocused);
                    break;
                case 29:
                    _buttonGoHome.Update(mouseState, ref _isButtonFocused);
                    break;
            }
        }
        else
        {
            _buttonGoHome.Update(mouseState, ref _isButtonFocused);
            _buttonGoToMarket.Update(mouseState, ref _isButtonFocused);
        }
        
        if (_isPlayingAnimation)
        {
            _animationTimer.Tick();
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        if (!_game.ClayClaimed)
            _buttonPuddle.Draw(spriteBatch);
        _buttonGoHome.Draw(spriteBatch);
        _buttonGoToMarket.Draw(spriteBatch);
        if (_isPlayingAnimation)
        {
            CountDrawer.DrawNumber(_animClayCount, _animStartPos, spriteBatch, 
                Color.White * _alphaValue);
        }
    }
    
}