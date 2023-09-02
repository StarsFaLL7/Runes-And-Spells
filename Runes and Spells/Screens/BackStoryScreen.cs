using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.UiClasses;

namespace Runes_and_Spells.Screens;

public class BackStoryScreen : IScreen
{
    private readonly Game1 _game;
    public BackStoryScreen(Game1 game) => _game = game;
    private Texture2D _dialogBoxTexture;
    private UiFadingTexture _textureForestDay;
    private UiFadingTexture _textureForestLake;
    private UiFadingTexture _textureForestEvening;
    private UiFadingTexture _textureForestWithHouse;
    private UiFadingTexture _textureInsideHouse;
    private UiButton _buttonNextScene;
    private SpriteFont _font;
    private const float FadeTime = 1f;
    
    private bool _isElementFocused;
    private List<UiFadingTexture> _scenes;
    private int _currentScene;
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _font = content.Load<SpriteFont>("SmallPixelFont36px");
        _buttonNextScene = new UiButton(
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_default"),
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_hovered"),
            content.Load<Texture2D>("textures/backstory_screen/buttons/skip_pressed"),
            new Vector2(1560, 963),
            () =>
            {
                if (_currentScene + 1 < _scenes.Count && _scenes[_currentScene] != _scenes[_currentScene+1])
                {
                    _scenes[_currentScene+1].StartFade();
                }
                
                if (_scenes.Count - 1 == _currentScene)
                {
                    _game.SetScreen(GameScreen.MainHouseScreen);
                    MediaPlayer.Stop();
                    MediaPlayer.Play(_game.MusicsMainTheme[0]);
                    _game.Introduction.StartIntro();
                }
                else _currentScene++;
                
            });
        _dialogBoxTexture = content.Load<Texture2D>("textures/backstory_screen/dialog_box_empty");

        _textureForestLake = new UiFadingTexture(
            content.Load<Texture2D>("textures/backstory_screen/forest_lake"),
            FadeTime, UiFadingTexture.Mode.FadeOut);
        
        _textureForestEvening = new UiFadingTexture(
            content.Load<Texture2D>("textures/backstory_screen/forest_evening"),
            FadeTime, UiFadingTexture.Mode.FadeIn);
        
        _textureForestDay = new UiFadingTexture(
            content.Load<Texture2D>("textures/backstory_screen/bg_forest_day"),
            FadeTime*2, UiFadingTexture.Mode.FadeIn);
        
        _textureForestWithHouse = new UiFadingTexture(
            content.Load<Texture2D>("textures/backstory_screen/house_in_forest"),
            FadeTime, UiFadingTexture.Mode.FadeIn);
        
        _textureInsideHouse = new UiFadingTexture(
            content.Load<Texture2D>("textures/main_house_screen/background"), 
            FadeTime, UiFadingTexture.Mode.FadeIn);
        
        _scenes = new List<UiFadingTexture>()
        {
            _textureForestLake,
            _textureForestEvening,
            _textureForestEvening,
            _textureForestDay,
            _textureForestWithHouse,
            _textureForestWithHouse,
            _textureForestWithHouse,
            _textureForestWithHouse,
            _textureForestWithHouse,
            _textureInsideHouse
        };
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _isElementFocused = false;
        _buttonNextScene.Update(mouseState, ref _isElementFocused);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        if (_currentScene > 0)
        {
            _scenes[_currentScene-1].Draw(new Vector2(0, 0),spriteBatch);
        }
        _scenes[_currentScene].Draw(new Vector2(0, 0),spriteBatch);
        if (_currentScene + 1 < _scenes.Count)
        {
            _scenes[_currentScene+1].Draw(new Vector2(0, 0),spriteBatch);
        }
        
        spriteBatch.Draw(_dialogBoxTexture, new Vector2(294, 756)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, 
            Game1.ResolutionScale, SpriteEffects.None, 1f);

        var text = Game1.GetText($"Backstory{_currentScene + 1}");
        var lines = text.Split("\\n");
        var y = 756 + _dialogBoxTexture.Height / 2 - (_font.LineSpacing*lines.Length) / 2;
        
        foreach (var line in lines)
        {
            spriteBatch.DrawString(_font, line, new Vector2(332, y) * Game1.ResolutionScale, new Color(57, 44,27),
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            y += _font.LineSpacing;
        }
        //spriteBatch.DrawString(_font, _scenes[_currentScene].dialog, new Vector2(332, y) * Game1.ResolutionScale, new Color(57, 44,27),
        //    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _buttonNextScene.Draw(spriteBatch);
    }

    public void Reset() => _currentScene = 0;
}