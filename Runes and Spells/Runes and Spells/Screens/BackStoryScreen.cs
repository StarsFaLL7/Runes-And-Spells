using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.Screens;

public class BackStoryScreen : IScreen
{
    private readonly Game1 _game;
    public BackStoryScreen(Game1 game) => _game = game;
    private Texture2D _dialogBoxEmpty;
    private Texture2D _sceneForestDay;
    
    private UiButton _buttonNextScene;

    private bool _isElementFocused;
    private List<(Texture2D background, Texture2D dialog)> _scenes;
    private int _currentScene;
    public void Initialize()
    {
        _scenes = new List<(Texture2D background, Texture2D dialog)>();
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _buttonNextScene = new UiButton(
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_default"),
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_hovered"),
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_pressed"),
            new Vector2(1560, 963),
            () =>
            {
                if (_scenes.Count - 1 == _currentScene)
                    _game.SetScreen(GameScreen.MainHouseScreen);
                else _currentScene++;
            });
        _dialogBoxEmpty = content.Load<Texture2D>("textures/backstory_screen/dialog_box_empty");
        _sceneForestDay = content.Load<Texture2D>("textures/backstory_screen/bg_forest_day");
        _scenes.Add((_sceneForestDay, _dialogBoxEmpty));
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _isElementFocused = false;
        _buttonNextScene.Update(mouseState, ref _isElementFocused);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        drawer.Draw(spriteBatch, graphics, _scenes[_currentScene].background, Position.TopLeft, new Vector2(0, 0), 0f, 0f);
        drawer.Draw(spriteBatch, graphics, _scenes[_currentScene].dialog, Position.MiddleBottom, new Vector2(0, -42), 0f, 0f);
        spriteBatch.Draw(_buttonNextScene.ActualTexture(), _buttonNextScene.Position, Color.White);
        
        spriteBatch.DrawString(_game.LogText, $"Is object focused: {_isElementFocused}\n" +
                                              $"Button IsPressed: {_buttonNextScene.IsPressed}\n" +
                                              $"Button IsHovered: {_buttonNextScene.IsHovered}", new Vector2(0,0), Color.Black);
        
    }
}