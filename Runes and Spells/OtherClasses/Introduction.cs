using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.classes;

public class Introduction
{
    public bool IsPlaying { get; private set; }
    public int Step { get; set; }
    
    private Game1 _game;
    public Introduction(Game1 game) => _game = game;
    
    private UiAnimatedTexture _selectorTexture;
    private UiAnimatedTexture _selectorTexture2;
    private UiButton _buttonSkipIntro;
    private bool _isButtonFocused;
    private List<(Vector2 TextPosition, Vector2 SelectorPosition)> _stepsPositions;
    private Dictionary<BackDetail, Texture2D> _backTextures;
    
    private const int BackBorder = 21;
    private List<string> AllTexts;
    
    enum BackDetail
    {
        CornerTopLeft,
        CornetTopRight,
        CornerBottomLeft,
        CornerBottomRight,
        SideLeft,
        SideRight,
        SideTop,
        SideBottom,
        Middle
    }
    public void LoadContent(ContentManager content)
    {
        _selectorTexture = new UiAnimatedTexture(200, 
            content.Load<Texture2D>("textures/intro/selector"), 
            new Vector2(96, 96), true);
        _selectorTexture2 = new UiAnimatedTexture(200, 
            content.Load<Texture2D>("textures/intro/selector"), 
            new Vector2(96, 96), true);
        _buttonSkipIntro = new UiButton(
            content.Load<Texture2D>("textures/intro/button_skip_into_default"),
            content.Load<Texture2D>("textures/intro/button_skip_into_hovered"),
            content.Load<Texture2D>("textures/intro/button_skip_into_pressed"),
            new Vector2(1365, 0),
            () =>
            {
                Stop();
                _game.ResetAfterIntroduction();
                AllGameItems.ClickSound.Play();
            } );
        _backTextures = new Dictionary<BackDetail, Texture2D>()
        {
            { BackDetail.SideBottom, content.Load<Texture2D>("textures/intro/back/side_bottom")},
            { BackDetail.SideLeft, content.Load<Texture2D>("textures/intro/back/side_left")},
            { BackDetail.SideRight, content.Load<Texture2D>("textures/intro/back/side_right")},
            { BackDetail.SideTop, content.Load<Texture2D>("textures/intro/back/side_top")},
            { BackDetail.Middle, content.Load<Texture2D>("textures/intro/back/middle")},
            { BackDetail.CornerBottomLeft, content.Load<Texture2D>("textures/intro/back/corner_left_bottom")},
            { BackDetail.CornerBottomRight, content.Load<Texture2D>("textures/intro/back/corner_right_bottom")},
            { BackDetail.CornerTopLeft, content.Load<Texture2D>("textures/intro/back/corner_left_top")},
            { BackDetail.CornetTopRight, content.Load<Texture2D>("textures/intro/back/corner_right_top")},
        };
        _stepsPositions = new List<(Vector2 TextPosition, Vector2 SelectorPosition)>()
        {
            (new Vector2(9, 100), new Vector2(912, 984)), //0
            (new Vector2(9, 100), new Vector2(_game.ScreenWidth-750, 540)),
            (new Vector2(9, 100), new Vector2(_game.ScreenWidth-100, 470)),
            (new Vector2(9, 100), new Vector2(659, 540)),
            (new Vector2(9, 100), new Vector2(1128, 128)),
            (new Vector2(9, 100), new Vector2(1283, 239)),
            (new Vector2(9, 100), new Vector2(1128, 470)),
            (new Vector2(9, 100), new Vector2(1278, 702)),
            (new Vector2(9, 100), new Vector2(94, 8)),
            (new Vector2(9, 100), new Vector2(1538, 528)),
            (new Vector2(9, 100), new Vector2(912, 444)), //10
            (new Vector2(9, 100), new Vector2(912, 580)),
            (new Vector2(9, 100), new Vector2(0, 0)),
            (new Vector2(9, 100), new Vector2(0, 0)),
            (new Vector2(9, 100), new Vector2(94, 8)),
            (new Vector2(9, 100), new Vector2(1813, 492)),
            (new Vector2(9, 739), new Vector2(141, 414)),//16
            (new Vector2(9, 921), new Vector2(270, 798)),//17
            (new Vector2(9, 921), new Vector2(94, 8)),
            (new Vector2(9, 100), new Vector2(1090, 529)),
            (new Vector2(9, 768), new Vector2(0, 0)),//20
            (new Vector2(9, 100), new Vector2(0, 0)),
            (new Vector2(9, 768), new Vector2(94, 8)),
            (new Vector2(9, 100), new Vector2(912, 984)), //23
            (new Vector2(9, 100), new Vector2(_game.ScreenWidth-407, 100)), // 24
            (new Vector2(9, 540), new Vector2(488, 375)),//25 Поставить на покупку
            (new Vector2(9, 540), new Vector2(488, 119)), //26
            (new Vector2(9, 540), new Vector2(681, 266)),//27
            (new Vector2(9, 540), new Vector2(94, 8)),
            (new Vector2(9, 100), new Vector2(_game.ScreenWidth-100, 470)),
            (new Vector2(9, 100), new Vector2(170, 568))
        };
        AllTexts = new List<string>
        {
            "Добро пожаловать в обучение молодого волшебника!\n" +
            "Мы посмотрим основные возможности игры.\n" +
            "Начнем с того, что главный элемент игры - это руны.\n" +
            "Руны создаются из глины, которую вы можете\n" +
            "собирать рядом со своим домом.",
            
            "Подойдите к луже глины с помощью WASD и нажмите 'E', чтобы собрать ее.\n" +
            "В лесу вы будете встречать различных персонажей, взаимодействие\n" +
            "с которыми происходит так же при помощи клавиши 'E'.",
            
            "Находясь на улице, вы можете бежать, держа клавишу 'Shift'.\n" +
            "В лесу вы можете найти различные сундуки.\n" +
            "Они открываются при помощи специальных ключей\n" +
            "и содержат много полезного.",
            
            "Вернемся к рунам. Они создаются на специальном столе.\n" +
            "Кстати говоря, каждый открытый серебрянный сундук\n" +
            "отрывает рецепт случайной неизвестной вам руны.",
            
            "Для того, чтобы создать руну, вам нужно положить глину\n" +
            "в слот сверху, глина находится в вашем инвентаре\n" +
            "в разделе \"Другое\"",
            
            "Теперь мы можем начать создание руны.\n" +
            "На данном столе вы можете создавать любые руны 1-го и 2-го уровня силы", //5
            
            "Руна создается путем нанесения на нее особого узора,\n" +
            "попробуйте закрасить только 1 центральную клетку\n" +
            "и нажать кнопку \"Создать\"",
            
            "Чтобы посмотреть информацию о предмете в инвентаре,\n" +
            "вы можете нажать на него Правой кнопкой мыши или\n" +
            "просто навести курсор и подождать.\n" +
            "У каждой руны есть три характеристики: Стихия, Сила и Вид.\n" +
            "В игре присутствуют 9 различных стихий, три уровня силы рун.\n" +
            "А также каждая руна может быть 1-го или 2-го вида.",
            
            "Вы смогли создать пока неизвестную руну, а ее рецепт добавился в блокнот\n" +
            "на столе. Заметим, когда у вас будет полный рецепт руны, вы сможете нажать\n" +
            "на нее в блокноте, чтобы быстро создать ее, без нанесения узора.",
            
            "После создания слепка руны, вам потребуется магическая печь,\n" +
            "на которой вы можете обжечь слепок и создать полноценную руну.",
            
            "Чтобы обжечь руну, вам нужно положить неизвестный слепок\n" +
            "в слот по центру и нажать кнопку \"Начать\"", //10
            
            "Чтобы обжечь руну, вам нужно положить неизвестный слепок\n" +
            "в слот по центру и нажать кнопку \"Начать\"",
            
            "Теперь вам нужно нажимать клавишу \"Пробел\",\n" +
            "когда стрелка указывают на одну из выделенных зон.",
            
            "В этой мини-игре прогресс будет постепенно убывать\n" +
            "и уменьшаться за промахи, поэтому нужно следить,\n" +
            "чтобы он не опустился до нуля,\n" +
            "но для обучения мы отключили эту функцию.",
            
            "Поздравляем! Вы создали свою первую руну.\n" +
            "Теперь вы можете посмотреть ее характеристики.\n" +
            "Но это еще далеко не все.",
            
            "В игре присутствует много различных рун,\n" +
            "но их рецепты вы будете открывать постепенно,\n" +
            "однако есть способ создать руну,\n" +
            "рецепт слепка которой вы еще не знаете.", //15
            
            "В вашем доме есть алтарь, на котором вы можете\n" +
            "извлекать эссенцию рун, объединять и усиливать руны и.\n" +
            "1. При объединии рун вы получаете руну такой же\n" +
            "стихии и силы, но другого вида.\n" +
            "2. При извлечении эссенции вы получаете эссенцию силы руны,\n" +
            "которая никак не связана с видом и стихией\n" +
            "3. Чтобы усилить руну вам потребуются три эссенции силы\n" +
            "и любая руна с такой же силой, тогда ее сила увеличиться на 1.\n" +
            "Давайте попробуем объединить две руны! Для этого положите\n" +
            "две одинаковые руны в прибор для объединения рун", //16
            
            "Теперь вы можете нажать кнопку для объединения рун.\n" +
            "После этого придется немного подождать", //17
            
            "Вы создали новую руну, рецепт которой вам неизвестен.\n" +
            "На алтаре вы можете создать множество разных рун\n" +
            "при помощи объединения и усиления. В любой момент вы можете нажать\n" +
            "на кнопки с вопросительным знаком, чтобы вспомнить принцип\n" +
            "работы каждого прибора", //18
            
            "Давайте же попробуем создать заклинание.\n" +
            "Заклинания создаются на специальном столе", //19
            
            "Чтобы создать свиток, вам понадобится волшебный пергамент\n" +
            "и две руны одинаковой силы. Чтобы создать свиток руны\n" +
            "должны подходить друг другу. Вы можете узнать о правилах,\n" +
            "нажав на кнопку с вопросительным знаком.\n" +
            "Находясь за этим столом, зажав Правую кнопку мыши на руне\n" +
            "в инвентаре, будут подсвечены зеленым цветом все руны,\n" +
            "с которыми вы можете объединить данную руну.\n" +
            "Положите пергамент в центральный слот,\n" +
            "а по бокам две подходящие руны.",//20
            
            "Теперь вам нужно написать текст заклинания точно\n" +
            "так же, как нашептывает вам дух. Если заклинание введено верно,\n" +
            "появится кнопка завершения создания свитка.",//21
            
            "Поздравляем! Вы создали заклинание.\n" +
            "Условия, когда из двух рун можно создать свиток - Если руны:\n" +
            "1. Одинаковой стихии и разных видов.\n" +
            "2. Разных стихий и одинаковых видов.\n" +
            "В обоих вариантах их сила должна быть одинаковой.",//22
            
            "Заклинания и другие предметы можно продавать на\n" +
            "торговой площади, давайте отправимся туда.\n" +
            "Для этого нам потребуется выйти на улицу\n" +
            "и пройти наверх, где посажены цветы.",//23
            
            "Каждый день цены на рынке немного изменяются.\n" +
            "Каждые 7 дней они сбрасываются.\n" +
            "Цена свитка зависит от цены на рынке и от его уровня силы.\n" +
            "Каждый уровень силы добавляет 20 монет к ценности.",//24
            
            "На рынке вы также можете купить некоторые товары для себя.",//25
            
            "Вы можете продать предмет по фиксированной цене или торговаться.\n" +
            "Торговля является мини-игрой, где по экрану перемещается круг.\n" +
            "Пока ваш курсор находится на нем, вы получаете выгоду, и наоборот.\n" +
            "Чтобы закончить мини-игру нажмите на клавишу \"Пробел\" или \"Enter\".", //26
                
            "Вы могли заметить, что у вас есть ваша энергия,\n" +
            "которая тратится за каждое действие в игре.\n" +
            "Энергия отображается шкалой в виде звездочек.\n" +
            "На всех экранах, кроме текущего, она отображается слева сверху.\n" +
            "Если ваша энергия закончится, вы отправляетесь домой и засыпаете.\n" +
            "А сейчас продайте свиток, который вам удалось создать.", //27 Добавить пояснение, что на этом экране не слева сверху, добавить что надо продать свиток
            
            "Теперь давайте вернемся домой. Кстати, вот,\n" +
            "что вы можете найти в сундуках:\n" +
            "1. Серебряный - 1 рецепт руны и 1 пергамент.\n" +
            "2. Золотой - 1 рецепт свитка и две руны 3-го уровня силы\n" +
            "3. Изумрудный - 1 рецепт руны, 1 рецепт свитка,\n" +
            "2 руны 1-й силы, 2 руны 2-й силы и 1 руна 3-й силы.", //Отсюда переход, когда GoBack
            
            "Когда вы просыпаетесь, наступает новый день.\n" +
            "Все персонажи, которых вы встретите в лесу,\n" +
            "скажут результат выполнения их просьбы на следующий день.", //29
            
            "На этом ваше обучение подошло к концу,\n" +
            "теперь ложитесь спать, а на утро вы начинайте\n" +
            "эксперементировать, создавать новые руны и заклинания!\n" +
            "Мы советуем начать изучения с путешествия в лес.",
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var font = AllGameItems.Font20Px;
        
        var positions = _stepsPositions[Step];
        if (_game.MarketScreen.Minigame.IsRunning)
        {
            return;
        }
        DrawBack(font.MeasureString(AllTexts[Step]), positions.TextPosition, spriteBatch);
        
        spriteBatch.DrawString(font, AllTexts[Step], 
            new Vector2(positions.TextPosition.X + BackBorder, positions.TextPosition.Y + 21)*Game1.ResolutionScale, 
            new Color(1, 23, 45), 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        _buttonSkipIntro.Draw(spriteBatch);

        if (positions.SelectorPosition is { X: 0, Y: 0 }) return;
        if (_game.IsInTopDownView)
        {
            _selectorTexture.Draw(positions.SelectorPosition, spriteBatch);
        }
        else
        {
            _selectorTexture.Draw(positions.SelectorPosition*Game1.ResolutionScale, spriteBatch);
        }
        if (Step == 16)
        {
            _selectorTexture2.Draw(new Vector2(405, 414)*Game1.ResolutionScale, spriteBatch);
        }
    }

    private void DrawBack(Vector2 textSize, Vector2 position, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backTextures[BackDetail.CornerTopLeft], 
            position*Game1.ResolutionScale, 
            null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.SideTop], 
            new Vector2(position.X + BackBorder, position.Y)*Game1.ResolutionScale, 
            new Rectangle(0,0,(int)textSize.X, BackBorder),
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.CornetTopRight], 
            new Vector2(position.X + BackBorder + (int)textSize.X, position.Y)*Game1.ResolutionScale, 
           null,
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.SideRight], 
            new Vector2(position.X + BackBorder + (int)textSize.X, position.Y+BackBorder)*Game1.ResolutionScale, 
            new Rectangle(0,0,BackBorder, (int)textSize.Y),
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.CornerBottomRight], 
            new Vector2(position.X + BackBorder + (int)textSize.X, position.Y+BackBorder+(int)textSize.Y)*Game1.ResolutionScale, 
            null,
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.SideBottom], 
            new Vector2(position.X + BackBorder, position.Y+BackBorder+(int)textSize.Y)*Game1.ResolutionScale, 
            new Rectangle(0,0,(int)textSize.X, BackBorder),
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.CornerBottomLeft], 
            new Vector2(position.X, position.Y+BackBorder+(int)textSize.Y)*Game1.ResolutionScale, 
            null,
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.SideLeft], 
            new Vector2(position.X, position.Y+BackBorder)*Game1.ResolutionScale, 
            new Rectangle(0,0,BackBorder, (int)textSize.Y),
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backTextures[BackDetail.Middle], 
            new Vector2(position.X + BackBorder, position.Y+BackBorder)*Game1.ResolutionScale, 
            new Rectangle(0,0,(int)textSize.X, (int)textSize.Y),
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }

    public void Update()
    {
        _buttonSkipIntro.Update(Mouse.GetState(), ref _isButtonFocused);
    }

    public void StartIntro()
    {
        Step = 0;
        IsPlaying = true;
    }

    public void Stop()
    {
        IsPlaying = false;
        Step = 0;
    }
}