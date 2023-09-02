using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.TopDownGame;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.TopDownGame.Dialogs;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class EndingScreen : IScreen
{
    private Game1 _game;
    public EndingScreen(Game1 game) => _game = game;
    
    private Texture2D _backgroundDarkForest;
    private Texture2D _backgroundCastle;

    private UiButton _buttonUseScroll;
    private UiButton _buttonPeace;
    
    private string[] HealScrolls = new[]
    {
        "Лечения", "Регенерации", "Омоложения", "Буйного роста", "Каменной кожи", 
        "Стальной кожи", "Удачных событий", "Силы", "Теплого пламени", "Магической силы",
        "Решительности", "Ледяной феи"
    };
    
    private string[] NoEffectScrolls = new[]
    {
        "Хождения по воде", "Удачной рыбалки", "Защиты от кислоты", "Снега", "Вдохновения",
        "Опьянения", "Лени", "Ловкости", "Скольжения", "Землекопа", "Быстрой ковки",
        "Дыхания землей", "Замедления", "Неплодородия", "Защиты от огня", "Света",
        "Оттепели", "Левитации", "Ледяного дыхания", "Невидимости", "Легкомыслия",
        "Защиты от бури", "Перышка", "Либидо", "Хладнокровия", "Защиты от мороза",
        "Хождения во сне", "Защиты от магии", "Очарования", "Снятия порчи", "Ярости"
    };
    private string[] DamageScrolls = new[]
    {
        "Водного потока", "Морозной воды", "Тумана", "Прилива", "Склейки",
        "Шипов", "Ледяной ауры", "Молнии", "Сосульки", "Зловония",
        "Слепоты", "Снежного бурана", "Магического льда", "Обездвиживания", "Осколка злобы",
        "Сна", "Галлюцинаций", "Старения", "Некромантии", "Проклятия"
    };
    private string[] KillScrolls = new[]
    {
        "Кипячения",
        "Кислоты",
        "Крист. лавы",
        "Окаменения",
        "Испепеления",
        "Огненного смерча",
        "Призыва огня",
        "Злобного огня",
        "Адского пламени",
        "Взрыва",
        "Ядовитого льда",
        "Ледяного хаоса",
        "Управл. кровью",
        "Гниения",
        "Отравления",
        "Разрыва материи"
    };

    private List<string> Story = new List<string>()
    {
        "С решимостью и надеждой в глазах, вы с Малфиусом начинаете свой путь к логову Веракса.\n" +
        "Вы преодолеваете горы, болота и темные леса, встречая множество монстров и преград\n" +
        "на своем пути. Но ни одно испытание не может остановить ваше стремление к победе.",
        
        "Наконец, вы достигаете мрачной крепости,\n" +
        "где Веракс Мракотворец держит свое злое владычество.\n" +
        "Веракс увидел Малфиуса и вас, его глаза засверкали злобой.",
        
        "Веракс: Наконец-то я вижу своих гостей. Добро пожаловать в мою обитель, Малфиус\n" +
        "и его беззащитный подопечный.\n" +
        "Но знайте, что я не позволю вам покончить с моими планами.",
        
        "Битва начинается. Веракс Мракотворец испускает потоки темной магии,\n" +
        "но герой активирует первый свиток защиты от магии, создавая магический щит,\n" +
        "который отражает колдовство Веракса.",
        
        "Огненные струи обрушиваются на них, но свиток защиты от огня призывает зыбкий щит,\n" +
        "защищающий их от пламени.",
        "Малфиус: Держись, мой юный друг! Теперь используй свиток защиты от мороза,\n" +
        "чтобы замедлить его мощные ледяные заклинания!",
        
        "Главный герой активирует третий свиток, и холодные порывы ветра образуют вокруг\n" +
        "тонкую защитную пелену, которая сопротивляется ледяным атакам Веракса.",
        
        "Бури разразились вокруг, но четвертый свиток защиты от бури создает магический барьер,\n" +
        "спасая героя и Малфиуса от сил стихии.",
        
        "По мере продолжения битвы, Веракс начинает ослабевать.\n" +
        "Тогда Малфиус обрушивает на него огромный поток света.\n" +
        "За несколько миль вокругстановится ярче света солнца.",
        
        "Малфиус продолжает атаку Веракса. Он использует множество неизвестных герою заклинаний.\n" +
        "Искры, свет, звук, - все говорит о том, что это магия невиданной силы.",
        
        "После огромного потока заклинаний Веракс падает на землю практически без сил.",
        
        "Веракс: Да, Кгх... я признаю свое поражение. Я открыл портал в темное измерение,\n" +
        "чтобы захватить королевство, но Кгх-кгх... монстры не поддаются моему властвованию.\n" +
        "Я могу помочь вам закрыть портал, без меня у вас ничего не получится.",
        
        "Малфиус: Возможно, он говорит правду,\n" +
        "для закрытия портала нужен мощный ритуал магии тьмы.",
        
        "Главный герой и Малфиус осознают, что есть выбор.\n" +
        "Они могут использовать финальное заклинание, чтобы полностью покончить с Вераксом,\n" +
        "или дать ему шанс исправить свои ошибки и помочь им закрыть портал."
    };

    private Dictionary<NPCType, string[]> NPCsTexts = new Dictionary<NPCType, string[]>()
    {
        {
            NPCType.Woodman, new[]
            {
                "Неожиданно из портала начинает вылазит огромное растение. \n" +
                "Своими отростками оно прерывает ритуал. \n" +
                "Лесоруб Иван сразу же хватается за топор и отрубает все вылезшие отростки.\n"+
                "Ритуал продолжатся.",
                "Лесоруб: Чего вы растерялись? Мой топор никогда меня не подводит!"
            }
        },
        {
            NPCType.FisherMan, new[]
            {
                "Портал начинает засасывать воздух из мира.\n" +
                "Все успевают схватиться за что-то, кроме вас. Вас начинает уносить в портал.\n" +
                "Вы почти уходите в темное измерение, как вдруг Рыбак Финли обхватывает вас\n" +
                "леской от своей удочки и вытаскивает вас оттуда.",
                "Рыбак: За добро - добром принято платить, мой друг."
            }
        },
        {
            NPCType.Hunter, new[]
            {
                "Из портала выбегает стая темных волков.\n" +
                "Один из них нападает на вас и опрокидывает на землю.\n" +
                "Огромные острые зубу монстра нависают над вами,\n" +
                "как вдруг волк отлетает в сторону от меткого выстрела Охотника Тормунда.",
                "Охотник: Смотри в оба, парень. Либо охотишься ты, либо - на тебя."
            }
        },
        {
            NPCType.Bard, new[]
            {
                "Из портала появляется Сирена, чарующая всех своей мелодичной песней.\n" +
                "Казалось, ритуал прерван, но на Барда Василька ее песня не подействовала.\n" +
                "И он начал петь, чтобы заглушить ее мелодию.\n" +
                "Песня Барда начала резать слух Сирене и она ушла обратно в портал.",
                "Бард: Не надо похвалы, дамы и господа. Я знаю, что я прекрасен."
            }
        },
        {
            NPCType.Witch, new[]
            {
                "Из портала вылетает множество ядовитых шипов. \n" +
                "Малфиус начинает создавать щит от них, но один из шипов попал прямо вам в грудь.\n" +
                "Вы упали без сознания и чуть не умерли, если бы не целительница Вивиана, \n" +
                "которая смогла вылечить вашу рану и нейтрализовать яд.",
                "Целительница: Ничего серьезного, жить будешь. Но будь аккуратней."
            }
        },
        {
            NPCType.Trader, new[]
            {
                "Из портала вылетает стая диких воронов, которые начинают атаковать без разбора\n" +
                "Один из воронов напал на вас и начал царапать ваши руки.\n" +
                "Вы бы потеряли руку, если бы не хитрый поступок Торговца Альфреда:\n" +
                "он кинул открытый кошель с золотом в портал и вороны улетели, увидев блеск золота.",
                "Торговец: Жалко, конечно, но там было не так много, поэтому переживу."
            }
        },
        {
            NPCType.Thief, new[]
            {
                "Из портала неожиданно появляется огромный огр с большой дубиной.\n" +
                "Он начинает бездумно размахивать ей и попадает по вам. Вы отлетаете к стене.\n" +
                "Ваш взгляд улавливает Вора Рейвена, быстро забирающегося на спину великану.\n" +
                "Рейвен молниеносно и тихо пробрался к голове огра и воткнул кинжал ему в горло.",
                "Вор: Пархай как бабочка, жаль, что огр медлительный слишком."
            }
        },
    };

    private string[] VeraxAliveStory = new[]
    {
        "Веракс: Благодарю, что выслушали меня. У меня совсем нет сил, чтобы вам помочь.\n" +
        "Малфиус, ты не мог бы помочь мне?",
        "Малфиус: Сначала клянись магической печатью, что не сбежишь от нас\n" +
        "и поможешь нам закрыть портал.",
        "Веракс: *Что-то бормочет*",
        "Веракс: *Заканчивает бормотать*",
        "Малфиус использовал исцеляющую магию на Вераксе и его силы восстановились.",
        "Веракс: Благодарю. А теперь займемся делом.",
        "Веракс, Малфиус: Et harum quidem rerum facilis est et expedita distinctio.",
        "Веракс и Малфиус начали одновременно произосить какое-то заклинание.\n" +
        "Из портала начали лететь искры.",
        "Все вокруг затряслось от той магической силы, что они использовали.",
        "Веракс, Малфиус: Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil.",
        "Веракс, Малфиус: omnis voluptas assumenda est, omnis dolor repellendus."
    };
    
    private string[] VeraxDeadStory = new[]
    {
        "Малфиус: Ну что же, Веракса с нами больше нет, а портал все еще открыт.",
        "Нужно изучить книгу, которая осталась после него, возможно,\n" +
        "там есть какой-то ритуал.",
        "Малфиус: *Читает книгу*", 
        "Малфиус: Хмм, дела у нас плохи.\n" +
        "Для закрытия портала нужно огромное количество магии, а мои силы на исходе.",
        "С Вераксом все могло бы получится, но теперь есть только один вариант -\n" +
        "нам нужно провести групповой ритуал, \n" +
        "чтобы магия жизни, что течет в каждом из нас помогла нам.",
        "Малфиус: Нужно приступать как можно скорее, давайте встанем в круг.",
        "Вы встали в круг и начали проводить ритуал.",
        "Вы повторяли за Малфиусом непонятные слова." 
    };

    private string[] CurrentStory;

    private Texture2D _dialogBoxTexture;
    private bool VeraxHelps;
    private int _step;
    private SpriteFont _font;
    private KeyboardState _lastKbState;
    private MouseState _lastMouseState;
    private bool _isButtonFocused;
    private string textAfterScroll;
    private List<string> NPCsStories;
    private Timer LockTimer;
    private bool _isLocked;
    
    
    public void Initialize()
    {
        
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        NPCsStories = new List<string>();
        _backgroundDarkForest = content.Load<Texture2D>("textures/backgrounds/dark_forest");
        _backgroundCastle = content.Load<Texture2D>("textures/backgrounds/castle");
        _dialogBoxTexture = content.Load<Texture2D>("textures/backstory_screen/dialog_box_empty");
        _font = content.Load<SpriteFont>("16PixelTimes20px");
        _buttonUseScroll = new UiButton(content.Load<Texture2D>("textures/ending_screen/button_scroll_default"),
            content.Load<Texture2D>("textures/ending_screen/button_scroll_hovered"),
            content.Load<Texture2D>("textures/ending_screen/button_scroll_pressed"),
            new Vector2(554, 495), () =>
            {
                AllGameItems.ClickSound.Play();
                _step++;
                textAfterScroll = $"Вы используете свиток {_game.TopDownCore.FinalScroll}\n" + UseFinalScroll();
                Story.Add(textAfterScroll);
                if (VeraxHelps)
                {
                    Story.AddRange(VeraxAliveStory);
                }
                else
                {
                    Story.AddRange(VeraxDeadStory);
                }
                Story.AddRange(NPCsStories);
                Story.Add("Ритуал продолжается");
                Story.Add("Спустя некоторое время портал захлопывается с оглушающим звуком.");
                Story.Add("Вы теряете сознание от потока магии, который пережили за последние несколько минут.");
                CurrentStory = VeraxDeadStory;
            });
        _buttonPeace = new UiButton(content.Load<Texture2D>("textures/ending_screen/button_peace_default"),
            content.Load<Texture2D>("textures/ending_screen/button_peace_hovered"),
            content.Load<Texture2D>("textures/ending_screen/button_peace_pressed"),
            new Vector2(1033, 495), () =>
            {
                AllGameItems.ClickSound.Play();
                _step++;
                //_globalStep = 1;
                textAfterScroll = "Вы решаете поверить Вераксу, ведь его помощь может быть как нельзя кстати";
                Story.Add(textAfterScroll);
                Story.AddRange(VeraxAliveStory);
                Story.AddRange(NPCsStories);
                Story.Add("Ритуал продолжается");
                Story.Add("Спустя некоторое время портал захлопывается с оглушающим звуком.");
                Story.Add("Вы теряете сознание от потока магии, который пережили за последние несколько минут.");
                CurrentStory = VeraxAliveStory;
            });
        foreach (var npc in _game.TopDownCore.FinalTeam)
        {
            if (npc.GivenScrollPower == 3)
            {
                NPCsStories.Add(NPCsTexts[npc.NPCType][0]);
                NPCsStories.Add(NPCsTexts[npc.NPCType][1]);
            }
        }
        if (NPCsStories.Count == 0 && _game.TopDownCore.FinalTeam.Count > 0)
        {
            NPCsStories.Add(NPCsTexts[_game.TopDownCore.FinalTeam[Random.Shared.Next(_game.TopDownCore.FinalTeam.Count)].NPCType][0]);
            NPCsStories.Add(NPCsTexts[_game.TopDownCore.FinalTeam[Random.Shared.Next(_game.TopDownCore.FinalTeam.Count)].NPCType][1]);
        }

        LockTimer = new Timer(200, () => _isLocked = false);
    }

    private string UseFinalScroll()
    {
        if (HealScrolls.Contains(_game.TopDownCore.FinalScroll))
        {
            VeraxHelps = true;
            return "Силы Веракса неожиданно начинают возвращаться.\nЕго раны затягиваются, а тело исцеляется.\n" +
                   "Он радостно смеется и благодарит вас за помощь.\nТеперь он готов помочь вам закрыть портал.";
        }

        if (DamageScrolls.Contains(_game.TopDownCore.FinalScroll))
        {
            VeraxHelps = false;
            return "Ваше заклинание сильно потрепало Веракса. Он понял, что пощады от вас не будет,\n" +
                   "но и силы его добить у вас нет. Тогда он достал небольшой посох из своего кармана и,\n" +
                   "после нескольких взмахов и бормотания, исчез с ваших глаз.";
        }

        if (KillScrolls.Contains(_game.TopDownCore.FinalScroll))
        {
            VeraxHelps = false;
            return
                "Ваше заклинание убило Веракса... Его смерть выглядела ужасающе, вокруг воцарился леденящий кожу воздух.\n" +
                "Все были в шоке от увиденного, но теперь портал придется закрывать без помощи Веракса.";
        }
        VeraxHelps = false;
        return "Никто не заметил, что произошло. Но, кажется, Веракс понял, что вы подобрали неверное заклинание.\n" +
               "Он начинает заливаться смехом и говорит, что волшебников глупее он еще не видел.\n" +
               "После этого он достает небольшой жезл из своей сумки и, после нескольких взмахов, исчезает в темном тумане.";
    }
    

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        LockTimer.Tick();
        if ((_lastKbState.GetPressedKeyCount() > Keyboard.GetState().GetPressedKeyCount() ||
            _lastMouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed) && !_isLocked && _step != 13 && _step < Story.Count-1)
        {
            _isLocked = true;
            LockTimer.StartAgain();
            _step++;
        }
        else if (_step == 13)
        {
            _buttonPeace.Update(mouseState, ref _isButtonFocused);
            _buttonUseScroll.Update(mouseState, ref _isButtonFocused);
        }
        else if ((_lastKbState.GetPressedKeyCount() > Keyboard.GetState().GetPressedKeyCount() ||
                  _lastMouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed) && !_isLocked && _step != 13 && _step >= Story.Count-1)
        {
            _game.SetScreen(GameScreen.MainHouseScreen);
            _game.TopDownCore.IsFinalQuestPlaying = false;
            var MagePhrase = new DialogPhrase("Что такое, мой юный друг?", "Благодарим вас за прохождение игры!\n" +
                                                                           "В ней все еще присутствует множество недоработок\n" +
                                                                           "Следите за обновлениями, дальше будет больше.", 
                new AnswerVariant("...", null, true));
            var phrase1 = new DialogPhrase(
                "Теперь ты можешь продолжить свое развитие в мире магии,\n" +
                "если ты изучил еще не все возможные руны и заклинания. А я отправляюсь в соседнее королевство,\n" +
                "теперь моя помощь нужна там. Удачи тебе, мой юный друг! И я уверен мы еще увидимся!", null,
                new AnswerVariant("И тебе удачи, Малфиус!", MagePhrase, true));
            var phrase2 = new DialogPhrase(
                "Очень хорошо, что все целы и здоровы.\n" +
                "Я думаю, что будь наша команда меньше, кто-то бы точно пострадал.\n" +
                "Но нам хватило безопасной магии жизни, поэтому все должны чувствовать себя хорошо.", null, 
                new AnswerVariant("Я тоже очень рад, что мы смогли закрыть этот чертов портал.", phrase1, false));
            if (VeraxHelps)
            {
                phrase2.Text =
                    "После ритуала я разговаривал с Вераксом и постарался направить его на путь света и добра.\n" +
                    "Надеюсь, он меня понял.\n" +
                    "Но, в любом случае, он ушел и поклялся больше не заниматься темной магией, чему я очень рад.";
                phrase2.AnswerVariants[0].Text = "Как по мне, Веракс еще может встать на сторону добра.";
            }

            if (!VeraxHelps && _game.TopDownCore.FinalTeam.Count < 4)
            {
                phrase2.Text =
                    "К сожалению, нам не хватило безопасной магии жизни из того малого количества людей,\n" +
                    "которых ты собрал в команду. Поэтому они сейчас себя очень плохо чувствуют.\n" +
                    "Лучше их пока не тревожить, чтобы их организм смог восстановиться.";
                phrase2.AnswerVariants[0].Text = "Очень жаль, что так вышло.";
            }
            var phrase3 = new DialogPhrase(
                "Наконец ты очнулся, мой юный друг.\n" +
                "Я так рад, что все закончилось и мы смогли закрыть этот ужасный портал.\n" +
                "Я тоже долго восстанавливал силы, совсем недавно смог уверенно встать с кровати и прийти сюда.", null, 
                new AnswerVariant("Я тоже очень рад, что мы смогли закрыть этот чертов портал.", phrase2, false));
            var mage = _game.TopDownCore.Map.NPCList.First(n => n.Name == "npc_mage");
            mage.PositionInPixelsLeftBottom = new Vector2(GameMap.MapWidth*GameMap.TileSize-1024, 512);
            mage.SetCurrentPhrase(phrase3);
            _game.TopDownCore.PlayerPosition = new Vector2((GameMap.MapWidth-3)*GameMap.TileSize, 640);
            MediaPlayer.Stop();
            MediaPlayer.Play(mage.GameCore.Game.MusicsMainTheme[mage.GameCore.Game.NextSongIndex]);
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundCastle, Vector2.Zero, Color.White);
        
        if (_step<=2)
            spriteBatch.Draw(_backgroundDarkForest, Vector2.Zero, Color.White);
        
        spriteBatch.Draw(_dialogBoxTexture, new Vector2(294, 756), Color.White);
        
        var y = 756 + _dialogBoxTexture.Height / 2 - _font.MeasureString(Story[_step]).Y / 2;
        spriteBatch.DrawString(_font, Story[_step], new Vector2(332, y), new Color(57, 44,27));
        if(_step == 13)
        {
            _buttonPeace.Draw(spriteBatch);
            _buttonUseScroll.Draw(spriteBatch);
        }
        
    }
}