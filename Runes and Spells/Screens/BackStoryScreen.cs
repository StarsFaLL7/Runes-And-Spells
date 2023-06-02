using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

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
    private List<(UiFadingTexture background, string dialog)> _scenes;
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
                if (_currentScene + 1 < _scenes.Count && _scenes[_currentScene].background != _scenes[_currentScene+1].background)
                {
                    _scenes[_currentScene+1].background.StartFade();
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
        
        _scenes = new List<(UiFadingTexture background, string dialog)>()
        {
            (_textureForestLake, texts[0]),
            (_textureForestEvening, texts[1]),
            (_textureForestEvening, texts[2]),
            (_textureForestDay, texts[3]),
            (_textureForestWithHouse, texts[4]),
            (_textureForestWithHouse, texts[5]),
            (_textureForestWithHouse, texts[6]),
            (_textureForestWithHouse, texts[7]),
            (_textureForestWithHouse, texts[8]),
            (_textureInsideHouse, texts[9]),
        };
        //_scenes[0].background.StartFade();
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
        if (_currentScene > 0)
        {
            _scenes[_currentScene-1].background.Draw(new Vector2(0, 0),spriteBatch);
        }
        _scenes[_currentScene].background.Draw(new Vector2(0, 0),spriteBatch);
        if (_currentScene + 1 < _scenes.Count)
        {
            _scenes[_currentScene+1].background.Draw(new Vector2(0, 0),spriteBatch);
        }
        //_scenes[_currentScene].background.Draw(new Vector2(0, 0),spriteBatch);
        spriteBatch.Draw(_dialogBoxTexture, new Vector2(294, 756), Color.White);
        var y = 756 + _dialogBoxTexture.Height / 2 - _font.MeasureString(_scenes[_currentScene].dialog).Y / 2;
        spriteBatch.DrawString(_font, _scenes[_currentScene].dialog, new Vector2(332, y), new Color(57, 44,27));
        _buttonNextScene.Draw(spriteBatch);
    }

    private List<string> texts = new ()
    {
        "В далеком краю странствовал один путешественник, \nпреследующий свою главную цель в жизни. Он долго скитался\nиз одного края мира в другой, изучая обряды и традиции,\nчтобы постичь столь хаотичную силу - магию.",
        "В один из дней своего путешествия он забрел очень далеко \nв лес и заблудился. Усталость накатывала все сильнее,\nи уже начинало темнеть. Остаться спать в открытом\nместе означало бы гибель, ведь звери в этом лесу\nпросыпаются именно ночью.",
        "Путешественнику пришлось идти всю ночь в поисках укрытия.\nВокруг то и дело были слышны ужасные вои и пугающие звуки.",
        "И вот, начало светать. Путник из последних сил забрался \nна высокий склон и, пристально оглядев округу, \nзаметил небольшую избушку.",
        "Это был заброшенный дом, в котором уже давно никого не \nбыло. Обследуя свое новое укрытие он был очень \nудивлен - в доме находились вещи, про которые он как-то раз\nчитал в старых книгах: алтарь и магическая печь.",
        "Путник решил остаться в этом доме на несколько дней, \nчтобы передохнуть. На улице рядом с входной дверью \nнаходилась большая лужа грязи, которую было решено убрать.",
        "На следующий день после уничтожения грязи,\nстранник впал в ступор - лужа снова была полная,\nкак будто он ее и не убирал.",
        "Внимательно изучив эту необъяснимую на первый взгляд \nлужу грязи, путешественник пришел к выводу:\nэто вовсе не грязь, а магическая глина, \nспособная на регенерацию.",
        "Тогда к нему и пришло осознание: прошлый владелец дома \nзанимался созданием рун - древним способом хранить \nмагию в глине путем зачарования.",
        "Не упуская такой возможности, путник решает продолжить дело\nпрошлого хозяина и изучить этот удивительный мир магии.\nНо первым делом ему надо сделать свою первую\nв жизни руну, а потом и свой первый свиток магии."
    };
}