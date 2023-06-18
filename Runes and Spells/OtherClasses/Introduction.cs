﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.UiClasses;

namespace Runes_and_Spells.classes;

public class Introduction
{
    public bool IsPlaying { get; private set; }
    public int Step { get; set; }
    private List<Vector2> _selectorListPositions;
    private List<string> AllTexts;
    private UiAnimatedTexture _selectorTexture;
    private Texture2D _textBackTexture;
    private SpriteFont _font;
    private UiButton _buttonSkipIntro;
    private Game1 _game;
    private bool _isButtonFocused;
    public Introduction(Game1 game) => _game = game;
    private List<(List<int> steps, TextPos pos, float PositionBackY)> _textsPositions;
    enum TextPos
    {
        Top,
        Middle,
        Bottom
    }
    public void LoadContent(ContentManager content)
    {
        _font = content.Load<SpriteFont>("16PixelTimes24px");
        _textBackTexture = content.Load<Texture2D>("textures/intro/back");
        _selectorTexture = new UiAnimatedTexture(100, 
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
                _game.ResetForNewGame();
            } );
        _textsPositions = new List<(List<int> steps, TextPos pos, float PositionBackY)>()
        {
            (new List<int> {1,2, 3,9,16,20,21,22,24,25,29,30}, TextPos.Top, 0),
            (new List<int> {0,15,23}, TextPos.Middle, (1080 - _textBackTexture.Height)/2),
            (new List<int> {4,5,6,7,8,10,11,12,13,14,17,18,19,26,27,28}, TextPos.Bottom, 1080 - _textBackTexture.Height)
        };
        _selectorListPositions = new List<Vector2>()
        {
            new Vector2(912, 984),
            new Vector2(1188, 537),
            new Vector2(1813, 470),
            new Vector2(659, 540),
            new Vector2(1128, 128),
            new Vector2(1283, 239),
            new Vector2(1128, 470),
            new Vector2(1278, 702),
            new Vector2(94, 8),
            new Vector2(1538, 528),
            new Vector2(912, 444), //10
            new Vector2(912, 580),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(94, 8),
            new Vector2(1813, 492),
            new Vector2(920, 851),
            new Vector2(0, 0),
            new Vector2(94, 8),
            new Vector2(10, 492),
            new Vector2(1090, 529),//20
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(94, 8),
            new Vector2(912, 984),
            new Vector2(1513, 100),//25
            new Vector2(488, 119),
            new Vector2(0, 0),
            new Vector2(94, 8),
            new Vector2(1813, 470),
            new Vector2(170, 568)
        };
        AllTexts = new List<string>
        {
            "Добро пожаловать в обучение молодого волшебника!\nМы посмотрим основные возможности игры. Для начала, стоит сказать, \nчто главная составляющая игры - это руны.\nРуны создаются из глины, которую вы можете собирать рядом со своим домом.",
            "Подойдите к луже глины с помощью WASD и нажмите 'E', чтобы собрать ее.\nНа улице почти все взаимодействия происходят по этой клавише.\nЕсли вам нужно будет передать кому-то свиток,\nто перетяните его из инвентаря прямо на этого человека.",
            "В будущем на улице вы сможете встретить персонажей, с которыми есть\nвозможность взаимодействия. В лесу также разбросаны различные сундуки.\nОни открываются при помощи специальных ключей\n и содержат много полезного.",
            "Вернемся к рунам. Они создаются на специальном столе.\nКстати говоря, каждый открытый серебрянный сундук дает информацию о\nрецепте неизвестной вам руны.",
            "Для того, чтобы создать руну, вам нужно положить глину в слот сверху,\nглина находится в вашем инвентаре в разделе \"Другое\"",
            "Теперь мы можем начать создание руны.",
            "Руна создается путем нанесения на нее особого узора, попробуйте закрасить\nтолько 1 центральную клетку и нажать кнопку \"Создать\"",
            "Чтобы посмотреть информацию о предмете в инвентаре, вы можете нажать\nна него Правой кнопкой мыши или просто навести курсор.",
            "Отлично, у нас получилась неизвестная руна, а ее рецепт добавился\nв блокнот на столе.",
            "Теперь нам нужно обжечь нашу руну.",
            "Чтобы обжечь руну, вам нужно положить неизвестную руну в специальный\nслот и нажать кнопку \"Начать\"",
            "Чтобы обжечь руну, вам нужно положить неизвестную руну в специальный\nслот и нажать кнопку \"Начать\"",
            "Теперь вам нужно нажимать клавишу \"Пробел\", когда стрелка указывают\nна одну из выделенных зон.",
            "В этой мини-игре прогресс будет постепенно убывать и уменьшаться\nза промахи, поэтому нужно следить, чтобы он не опустился до нуля,\nно для обучения мы отключили эту функцию.",
            "Поздравляем! Вы создали свою первую руну. Но это еще далеко не все.",
            "В игре присутствует много различных рун, не все можно создать\nпростым нанесением узора на столе.",
            "В вашем доме есть специальный алтарь, на котором вы можете объединять\nруны одной стихии и уровня силы, чтобы получить более сильные руны.",
            "Заметьте, что здесь от перестановки рун местами, будут получаться\nразные руны. Ой, кажется у вас в кармане завалялась еще одна руна земли.\nДавайте попробуем их объединить!",
            "Вы смогли получить руну более высокого уровня, она позволит вам\nсоздавать более сильные заклинания!",
            "Давайте же попробуем создать заклинание.",
            "Чтобы создать заклинание, вам понадобится волшебный пергамент и две руны \nодинаковой силы.",
            "Положите пергамент в центральный слот, а по бокам две руны,\nздесь их местоположение не влияет на результат.",
            "Теперь вам нужно написать текст заклинания точно так же,\nкак нашептывает вам дух.",
            "Поздравляем! Вы создали заклинание. Важно: Свитки можно создавать если:\n1. Руны одинаковой стихии и разных видов.\n2. Руны разных стихий и одинаковых видов.\nВ обоих вариантах их сила должна быть одинаковой.",
            "Заклинания можно продавать на торговой площади, давайте отправимся туда.\nДля этого нам потребуется выйти на улицу и пройти наверх,\nгде посажены цветы.",
            "Каждый день цены на рынке могут изменяться,\nпоэтому создавать одно и то же заклинание не очень выгодно.",
            "На рынке вы можете купить некоторые товары для себя,\nа можете продать заклинания.",
            "Вы можете продать заклинание по фиксированной цене или торговаться.\nТорговля является мини-игрой, где по экрану перемещается круг.\nПока ваша мышка находится на нем, вы получаете выгоду, и наоборот.\nЧтобы закончить мини-игру нажмите на клавишу \"Пробел\" или \"Enter\".",
            "У вас мог возникнуть вопрос: А как понять, когда закончится день?\nОтвет прост: день заканчивается, когда вы идете спать на своей кровати.\nПосле этого цены на рынке немного меняются, а глина регенерируется.\nКаждый 7-й день цены на рынке могут значительно измениться.",
            "Теперь давайте попробуем вернуться домой и лечь спать.",
            "Ну вот и все, ваше обучение подошло к концу, теперь ложитесь спать,\nа на утро вы можете эксперементировать, создавать новые руны и заклинания!",
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var textPos = _textsPositions.First(l => l.steps.Contains(Step));
        spriteBatch.Draw(_textBackTexture, new Vector2(0, textPos.PositionBackY), Color.White);
        var stringSize = _font.MeasureString(AllTexts[Step]);
        
        spriteBatch.DrawString(_font, AllTexts[Step], 
            new Vector2(20, textPos.PositionBackY + _textBackTexture.Height/2 - stringSize.Y/2), 
            new Color(1, 23, 45));
        
        if (_selectorListPositions[Step].X != 0 || _selectorListPositions[Step].Y != 0)
            _selectorTexture.Draw(_selectorListPositions[Step], spriteBatch);
        _buttonSkipIntro.Draw(spriteBatch);
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