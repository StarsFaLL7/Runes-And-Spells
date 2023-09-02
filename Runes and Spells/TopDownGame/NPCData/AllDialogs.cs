using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Dialogs;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.NPCData;

public static class AllDialogs
{
    public static Dictionary<NPCType, (DialogPhrase FirstPhrase, DialogPhrase QuestInProgressPhrase, DialogPhrase
        QuestJustFinishedPhrase, DialogPhrase QuestFinishedGood, DialogPhrase QuestFinishedBad, DialogPhrase FinalPhraseGood, DialogPhrase FinalPhraseBad)> 
        DialogInfo { get; private set; }

    public static void Initialize()
    {
        DialogInfo = new();
        WoodmanDialog.Initialize();
        FishermanDialog.Initialize();
        HunterDialog.Initialize();
        BardDialog.Initialize();
        WitchDialog.Initialize();
        TraderDialog.Initialize();
        Thief.Initialize();
        MageDialog.Initialize();
    }
    
    public static class WoodmanDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Что говоришь, колдун какой-то распоясался? \n" +
            "Да, я помогу тебе разобраться с ним", 
            "Дровосек присоединился к вашей команде.", 
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Извини, брат, у меня полно дел, нужно много проблем здесь решить.", null, 
            new AnswerVariant("Хорошо, тогда помогай людям тут.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Волки все еще в лесу!\n" +
                "Нам нужно что-то огненное, чтобы справиться с ними.",
                null, new AnswerVariant("Я скоро вернусь", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Интересная штуковина, что ж, попробуем ее в деле!",
                null, new AnswerVariant("...", null, true));

            var endAnswer = new AnswerVariant("...", null, true);
            var endPhrase = new DialogPhrase(
                "Охх, пойду дровишек что ли нарублю.", null, endAnswer);

            var phrase1 = new DialogPhrase(
                "Только слюни не распускай там!\n" +
                "И парню тому советую не верить, а то подозрительный он больно.\n" +
                "И, кстати, вот тебе компас, чтобы не потерялся", null,
                new AnswerVariant("...", endPhrase, true, npc => npc.GameCore.PlayerHasCompass = true));
            var phrase2 = new DialogPhrase(
                "Я начал замечать каких-то странных животных далеко в западной\n" +
                "части леса. Это какие-то монстры, на местных не похожи, а я,\n" +
                "поверь, всех местных животных знаю! А по пути туда я несколько\n" +
                "раз видел какого-то подозрительного парня. Советую тебе посмотреть,\n" +
                "что там такое, точно чертовщина какая-то!",
                null,
                new AnswerVariant("Хорошо, я посмотрю.", phrase1, false));
            var phrase3 = new DialogPhrase(
                "У меня есть еще кое-что, что тебя должно заинтересовать!", null,
                new AnswerVariant("О чем ты говоришь?", phrase2, false));
            var endQuestAnswer = new AnswerVariant("...", phrase3, false, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Твоя магическая фигня не сработала.\n" +
                "Печальный из тебя волшебник, что сказать.\n" +
                "А отдувался за твою ошибку я!\n" +
                "Повезло тебе, что мой топор всегда со мной!", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Вот это я задал этим волкам! Неплохой огонек ты мне подкинул.\n" +
                "Дай мне еще пару таких свитков потом, я ими печь буду топить, ха!\n" +
                "Я пока волков прогонял, у них в логове нашел штуку странную,\n" +
                "деревья не рубит - бесполезная выходит, но я подумал, \n" +
                "может тебе нужна будет?",
                "Вы получили мистический изумрудный ключ", endQuestAnswer);
            var questCanceledPhrase = new DialogPhrase(
                "Ха! Мне пофиг на твои принципы.\n" +
                "Если мы не справимся с этими волками, то они справятся с нами.\n" +
                "А я на твоей могиле плакать не буду, Ха!", "Вы отклонили задание персонажа.", endQuestAnswer);
            var questAcceptedPhrase = new DialogPhrase(
                "Ну, хоть что-то полезное из тебя выйдет.\n" +
                "Слушай внимательно:\n" +
                "Эти волки сильно боятся огня, принеси мне что-нибудь,\n" +
                "что поможет выгнать их!",
                "Подсказка: У вас появилось задание отдать свиток магии персонажу.\n" +
                "Обратите внимание, что вы можете отдать ему абсолютно любой свиток.\n" +
                "Чем больше будет его сила, тем влиятельней будет ваша помощь.",
                new AnswerVariant("Я скоро вернусь.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));
            var phrase4 = new DialogPhrase(
                "Я собираюсь найти и уничтожить их логово. Они создают хаос\n" +
                "в этой местности, и я не могу продолжать работу с этими\n" +
                "бродягами рядом. Если ты и правда магией занимаешься,\n" +
                "может предложишь способ расправиться с ними?", null,
                new AnswerVariant("Да, я что-нибудь придумаю.", questAcceptedPhrase, false),
                new AnswerVariant("Я сожалею, но убивать животных - не по моей части.", questCanceledPhrase,
                    false));

            var phrase5 = new DialogPhrase(
                "Меня зовут Иван, я местный дровосек. Я заметил, что в лесу\n" +
                "появилась стая диких волков. Они становятся все более\n" +
                "агрессивными и нападают на людей, проходящих мимо.", null,
                new AnswerVariant("...", phrase4, false));
            var phrase6 = new DialogPhrase(
                "Дровосек задумчиво хлопает по своему топору и продолжает.", null,
                new AnswerVariant("...", phrase5, false));
            var phrase7 = new DialogPhrase(
                "Мне нужна твоя помощь, и я плачу неплохо.",
                null,
                new AnswerVariant("Я занимаюсь изучением рун и заклинаний. Чем я могу тебе помочь?", phrase6, false));

            FirstPhrase = new DialogPhrase(
                "Эй, ты! Ты тот странноватый тип, который поселился в лесу, верно?", null,
                new AnswerVariant("Здравствуй, да, я не так давно живу тут.", phrase7, false));
            
            DialogInfo.Add(NPCType.Woodman, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, 
                QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }
    
    public static class FishermanDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Да, несомненно, этот хаос нужно остановить, я помогу тебе, друг.", "Рыбак присоединился к вашей команде.", 
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Борьба со злом - очень хорошее дело, но не стоит забывать о людях,\n" +
            "для которых я ловлю рыбу. Без меня им придется туго, поэтому извини,\n" +
            "но этот путь ты должен преодолеть без меня.", null, 
            new AnswerVariant("Хорошо, тогда помогай людям тут.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Что такое, юный волшебник? Улова пока что все так же нет.\n" +
                "Магия океана должна помочь решить эту проблему.",
                null, new AnswerVariant("Я скоро вернусь", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Я чувствую силу этого свитка. Посмотрим, поможет ли он мне.",
                null, new AnswerVariant("...", null, true));

            var endAnswer = new AnswerVariant("...", null, true);
            var endPhrase = new DialogPhrase(
                "Эхх, рыба-рыба. Как прекрасно заниматься любимым делом.", null, endAnswer);
            var endQuestAnswer = new AnswerVariant("...", endPhrase, false, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "К сожалению, твой свиток не сработал. Кажется, нужно что-то другое.\n" +
                "Что-ж, буду искать другие решения.", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Спасибо тебе за помощь. Я уже вижу, сколько рыбы плывет сюда.\n" +
                "И смотри, что я выловил со дна, пока рыбы не было. Отдаю это тебе,\n" +
                "надеюсь, ты найдешь ему применение. \n" +
                "Как говорится: \"Делай добро и жди добра\"",
                "Вы получили мокрый изумрудный ключ", endQuestAnswer);
            var questCanceledPhrase = new DialogPhrase(
                "Что ж, не печалься, мой друг. Мы найдем другое решение.\n" +
                "Как говорит Малфиус: \"Безвыходных ситуаций не бывает\"", null, endQuestAnswer);
            var questAcceptedPhrase = new DialogPhrase(
                "Отправляйся с мудрыми словами, юный волшебник. \n" +
                "\"Там, где вода встречается с луной, скрыто могущество,\n" +
                "ожидающее откровения\".\n" +
                "Пусть эти слова вдохновят тебя в твоем путешествии.", null,
                new AnswerVariant("Хорошо! Я вернусь как можно раньше.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));
            var phrase4 = new DialogPhrase(
                "Да, умный ты парень, но вот не в моих это силах.\n" +
                "Малфиус смог бы с этим помочь. Моя проблема более низменная:\n" +
                "рыба перестает водится в реке. Малфиус помогал мне ее ловить\n" +
                "при помощи магии океана. Может, у тебя получится создать для\n" +
                "меня свиток магии Океана, который помог бы мне увеличить улов?", null,
                new AnswerVariant("Да, я постараюсь создать для тебя подходящий свиток.", questAcceptedPhrase, false),
                new AnswerVariant("К сожалению, я не смогу помочь. Мне еще предстоит изучить все аспекты магии океана.", questCanceledPhrase,
                    false));

            var phrase5 = new DialogPhrase(
                "В последнее время что-то странное происходит в наших водах.\n" +
                "Лесоруб Иван еще говорит, что появились какие-то злобные монстры...\n" +
                "Я не знаю, кто или что стоит за всем этим, но чувствую,\n" +
                "что власть зла усиливается.", null,
                new AnswerVariant("Тогда нам нужно противостоять злу, верно?", phrase4, false));
            
            var phrase6 = new DialogPhrase(
                "Хм, раз ты занимаешься магией, у меня есть для тебя просьба,\n" +
                "юный волшебник.", null,
                new AnswerVariant("Слушаю", phrase5, false));
            var phrase7 = new DialogPhrase(
                "Хм, я даже и не знаю.\n" +
                "Но ты можешь встретиться с нашей целительницей или ведьмой,\n" +
                "как ее некоторые называют. Она живет в западной части леса.\n" +
                "Я думаю, что она подскажет тебе способ перебраться через реку.",
                null,
                new AnswerVariant("Спасибо, попробую ее найти.", phrase6, false));
            var phrase8 = new DialogPhrase(
                "Его зовут Малфиус, хороший он человек, всегда всем помогает,\n" +
                "но в последнее время куда-то пропал. Занят, наверное.\n" +
                "Его обычно можно встретить на другом берегу реки,\n" +
                "но вот как тебе туда попасть...", null,
                new AnswerVariant("А какие есть варианты?", phrase7, false));
            var phrase9 = new DialogPhrase(
                "В далекие времена, когда тьма и свет сражались за владычество,\n" +
                "великие маги создали особые свитки - это такой способ хранить магию\n" +
                "на пергаменте. И до сих пор некоторые изучают и создают свитки,\n" +
                "подобные тем, что были созданы первыми. У меня есть знакомый\n" +
                "волшебник, который часто помогает мне с рыбалкой при\n" +
                "помощи этих свитков.", null,
                new AnswerVariant("Что за волшебник? Я могу с ним встретиться?", phrase8, false));
            var phrase10 = new DialogPhrase(
                "Хех, что уж тут понимать, видно по тебе, а у меня глаз в\n" +
                "мои года уже хорошо наметан. Хочешь расскажу тебе одну\n" +
                "интересную историю из мира магии?", null,
                new AnswerVariant("Да, я с удовольствием послушаю.", phrase9, false));
            FirstPhrase = new DialogPhrase(
                "Ах, здравствуй, юный волшебник!\n" +
                "Рыбак Финли рад видеть тебя на берегу океана.\n" +
                "Пришел сюда в поисках чего-то особенного, а не просто поймать\n" +
                "рыбку, да?", null,
                new AnswerVariant("Здравствуй, старый рыбак, как ты понял, что я занимаюсь магией?", phrase10, false));
            
            DialogInfo.Add(NPCType.FisherMan, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }
    
    public static class HunterDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Мы победим. Победим любой ценой. Я иду с тобой.", "Охотник присоединился к вашей команде.", 
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Я все еще не поправился после сражения с монстрами,\n" +
            "вряд ли я смогу вам помочь, извини.", null, 
            new AnswerVariant("Хорошо, тогда поправляйся.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Да где же мне найти что-то холодное... \n" +
                "О, это снова ты. Принес свиток, который создаст холод?",
                null, new AnswerVariant("Секунду...", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Не уверен я, что этот свиток поможет, но попробовать,\n" +
                "в любом случае, стоит.",
                null, new AnswerVariant("...", null, true));

            var endAnswer = new AnswerVariant("...", null, true);
            var endPhrase = new DialogPhrase(
                "Мы разбили этих страшных тварей!\n" +
                "Мы заглянули в их синие глаза, и выжили!", null, endAnswer);
            var endQuestAnswer = new AnswerVariant("...", endPhrase, false, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Что ты мне такое принес? Твой свиток не создал достаточно холода,\n" +
                "чтобы монстры испугались. Пришлось разбираться самому, хорошо,\n" +
                "что обычными ранами отделался. Но дело сделано.", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Ох, это было хорошо. Твой свиток отлично сработал.\n" +
                "Все монстры ушли, теперь ты можешь спокойно пройти на запад.\n" +
                "Спасибо тебе за помощь!",
                "На земле, откуда убежали монстры вы нашли испачканный кровью изумрудный ключ", endQuestAnswer);
            
            var questAcceptedPhrase = new DialogPhrase(
                "Отлично! Удачи тебе, волшебник.\n" +
                "Если захочешь послушать истории бывалого охотника, обращайся.", null,
                new AnswerVariant("И тебе удачи! Я пойду создавать нужный свиток.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));
            var phrase4 = new DialogPhrase(
                "Вот и отлично! Нам нужно мощное заклинание холода,\n" +
                "которое отпугнет этих существ обратно в темное измерение.", null,
                new AnswerVariant("Хорошо, скоро вернусь с заклинанием льда.", questAcceptedPhrase, false));

            var phrase5 = new DialogPhrase(
                "Знаешь, я ловил множество зверей в своей жизни,\n" +
                "но таких тварей еще не видывал. Они выглядят ужасающе.\n" +
                "И что самое интересное, они боятся холода!", null,
                new AnswerVariant("Хм, это интересно.\n" +
                                  "Я могу использовать свои знания рун и заклинаний,\n" +
                                  "чтобы создать что-то, что отпугнет этих монстров.", phrase4, false));
            
            var phrase6 = new DialogPhrase(
                "Монстры, которых мы никогда раньше не видели. \n" +
                "Они приходят из какого-то другого измерения, и,\n" +
                "мне кажется, только ты, волшебник, можешь нам помочь.", null,
                new AnswerVariant(
                    "Понимаю. Расскажите мне больше об этих существах.", phrase5, false));
            var phrase7 = new DialogPhrase(
                "Меня зовут Тормунд, обычно я занимаюсь охотой в других лесах,\n" +
                "но в последнее время в наших лесах появились странные существа,\n" +
                "их след привел меня сюда.", null,
                new AnswerVariant("...", phrase6, false));
            FirstPhrase = new DialogPhrase(
                "Хмм, да что же сними делать. Эй, ты! Да, именно ты!\n" +
                "Ты, я погляжу, волшебник? Мне нужна твоя помощь.", null,
                new AnswerVariant(
                    "Здравствуйте, да, я начал заниматься магией.\n" +
                    "Чем я могу быть полезен?", phrase7, false));
            
            DialogInfo.Add(NPCType.Hunter, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
        
        
    }
    
    public static class BardDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Ничего себе! После нашего похода я смогу написать такую балладу,\n" +
            "чтобы во всем королевстве узнали, кто такой Бард Василек!\n" +
            "Конечно я иду с вами!", "Бард присоединился к вашей команде.", 
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Куда мне до подвигов, когда я две строчки срифмовать не могу.\n" +
            "Знаешь, жизнь - боль.\n" +
            "Оставьте меня.", null, 
            new AnswerVariant("Хорошо, я понял.", null, true));

        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Вот бы со мной случилось какое-нибудь удачное событие:\n" +
                "мог бы я вернуться в молодость, найти вдохновение или\n" +
                "вообще хорошенько поспать...",
                null, new AnswerVariant("Секунду...", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Интересный кусок пергамента! Надеюсь, он мне поможет.",
                null, new AnswerVariant("...", null, true));

            var endAnswer = new AnswerVariant("...", null, true);
            var endPhrase = new DialogPhrase(
                "Барду вы заплатите чеканной... Кхм-кхм...", null, endAnswer);
            
            var songEnd = new DialogPhrase("Давай-давай, удачи тебе!", null,
                new AnswerVariant("И тебе удачи!", endPhrase, true));
            var song1 = new DialogPhrase("Ну что? Все понял?", null,
                new AnswerVariant(
                    "Надо еще поразмыслить, но спасибо! Пойду искать Малфиуса", songEnd, false));
            var song2 = new DialogPhrase("Иди скорей, беги, лети\n" +
                                         "Быстрей ты реку перейди,\n" +
                                         "И Малфиус тебя заждался,\n" +
                                         "В тебе он вряд ли ошибался", null,
                new AnswerVariant("...", song1, false));
            var song3 = new DialogPhrase("И Малфиус пропал, пошел он разбираться\n" +
                                         "Как нам Веракса победить и как живым остаться.\n" +
                                         "И знал волшебник, наш герой, придет скоро один\n" +
                                         "Спаситель новый, новичок, но с ним Веракса победим.", null,
                new AnswerVariant("...", song2, false));
            var song4 = new DialogPhrase("А времена настали ужасные для всех,\n" +
                                         "И в страхе все живут, такой вот сделал грех\n" +
                                         "Один колдун известный, Вераксом звать его\n" +
                                         "Какие же мотивы поступка своего?", null,
                new AnswerVariant("...", song3, false));
            var song5 = new DialogPhrase("О, Малфиус Великий, известен он везде.\n" +
                                         "И всем он помогает, в случившейся беде!\n" +
                                         "Волшебник он известный, о подвигах его\n" +
                                         "Разносятся истории и очень далеко!", null,
                new AnswerVariant("...", song4, false));
            var song6 = new DialogPhrase("И так, начнем.", null,
                new AnswerVariant("Я внимательно слушаю.", song5, false));
            var endQuestAnswer = new AnswerVariant("...", song6, false, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Привет! Спасибо тебе за помощь!\n" +
                "Не уверен, что твой свиток сработал, но я много тренировался на бумаге\n" +
                "и теперь смогу рассказать тебе, кто такой Малфиус!", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Привет! Спасибо тебе за помощь! Я уже чувствую, как рифма льется из\n" +
                "моих уст! Теперь я смогу рассказать тебе, кто такой Малфиус!\n" +
                "И, вот, держи этот ключ, его попросил передать достойному волшебнику \n" +
                "сам Малфиус.", "Вы получили загадочный изумрудный ключ.", endQuestAnswer);
            
            var questAcceptedPhrase = new DialogPhrase(
                "Отлично! Я так рад! так рад! Я буду ждать и верить,\n" +
                "что ты мне сможешь помочь! Мне бы какую-нибудь магию, как у фей,\n" +
                "они такие легкие и так изящно парят в воздухе!", null,
                new AnswerVariant("Постараюсь найти что-нибудь подходящее.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));

            var questInDeny = new DialogPhrase(
                "Ты вернулся, чтобы помочь мне?", null,
                new AnswerVariant("Можешь напомнить, с чем тебе нужно помочь?", questAcceptedPhrase, false));
            
            var questDeniedPhrase = new DialogPhrase(
                "Ладно, буду пробовать писать на бумаге.\n" +
                "Если все-таки решишь мне помочь, возвращайся", null,
                new AnswerVariant("...", questInDeny, true));
            var phrase1 = new DialogPhrase(
                "Я совсем разучился создавать рифму. Не знаю, что такое,\n" +
                "но это ужасно. Помоги мне и я расскажу тебе о волшебнике Малфиусе\n" +
                "в стихах!", null,
                new AnswerVariant(
                    "Я занимаюсь магией и могу создать для тебя волшебный свиток,\n" +
                    "который тебе поможет.", questAcceptedPhrase, false),
                new AnswerVariant("Извини, но я не знаю, чем тебе помочь.\n" +
                                  "Попробуй больше практиковаться.", questDeniedPhrase, false)
                );
            questInDeny.AnswerVariants[0].NextPhrase = phrase1;
            
            var phrase2 = new DialogPhrase(
                "О, дорогой друг, я знаю всех в округе! Кхм, кхм. \n" +
                "О, Малфиус великий, известен он везде,\n" +
                "И всем он помогает, кто оказался в беде!\n" +
                "Нет! Нет! Это ужас! Все пропало! Все пропало!", null,
                new AnswerVariant("Что такое?", phrase1, false));
            
            FirstPhrase = new DialogPhrase(
                "... А воздух свежий как роса!\n" +
                "Куда пойду - идет слеза...\n" +
                "Ой, привет. Ты пришел послушать самого великого Барда\n" +
                "этого королевства, Василька?", null,
                new AnswerVariant(
                    "Здравствуй, скорее нет. Я ищу некого волшебника Малфиуса,\n" +
                    "знаешь такого?", phrase2, false));

            DialogInfo.Add(NPCType.Bard, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }
    
    public static class WitchDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Я ценю то, что ты решился мне помочь. Теперь я отплачу тебе тем же.\n" +
            "Мне нужно собрать несколько своих зелий, и я буду готова.", "Целительница присоединилась к вашей команде.",
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Я ценю, что ты пытался мне помочь, но у меня сейчас совсем все тяжело.\n" +
            "Если вы перенесете свой поход на месяц-другой, я с радостью пойду с вами", null, 
            new AnswerVariant("Извини, действовать нужно сейчас.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Здравствуй, путник. Напоминаю: моему клиенту нужна магия жизни,\n" +
                "которая позволит влюбить в него девушку, чем эффективнее, тем лучше.",
                null, new AnswerVariant("Да, хорошо, скоро принесу свиток.", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "У тебя получилось хорошее заклинание. Надеюсь, моему клиенту оно подойдет.",
                null, new AnswerVariant("...", null, true));
            
            var endPhrase = new DialogPhrase(
                "Бедные зайчики... Нужно пойти накосить особую траву,\n" +
                "чтобы они не боялись выходить из своих норок.", null, 
                new AnswerVariant("...", null, true));
            
            var phraseQuestFinish = new DialogPhrase(
                "Да, конечно, вот, держи эти крылья.\n" +
                "Высоко летать на них не получится, но реку должны помочь перелететь.", 
                "Чтобы использовать крылья, нажмите на пробел.\n" +
                "Пока вы летаете, вы можете перейти на другой берег реки.",
                new AnswerVariant(
                    "Спасибо большое! Я уверен, что мы сможем победить зло\n" +
                    "в этих землях.", endPhrase, true, 
                    npc => npc.GameCore.PlayerHasWings = true));
            
            var endQuestAnswer = new AnswerVariant(
                "Нам все еще нужно победить зло.\n" +
                "Ты поможешь мне перебраться на другой берег реки?", phraseQuestFinish, false, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Спасибо за твою помощь, но, кажется, твое заклинание не сработало\n" +
                "нужным образом. Клиент почти ничего не заплатил, и теперь мне\n" +
                "снова нужно где-то искать деньги на жизнь", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Огромное спасибо тебе, путник. Твое заклинание сработало идеально.\n" +
                "Теперь у меня снова есть деньги на жизнь.\n" +
                "Вот, держи этот ключ. Клиент отдал его вместе с деньгами, он остался\n" +
                "очень доволен.", "Вы получили блестящий изумрудный ключ.", endQuestAnswer);
            
            var questAcceptedPhrase = new DialogPhrase(
                "Отлично, я думаю, что Магия Жизни сможет помочь моему клиенту.\n" +
                "Ему нужно что-то, что позволит влюбить в себя девушку.", null,
                new AnswerVariant("Постараюсь найти что-нибудь подходящее.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));

            var phrase1 = new DialogPhrase(
                "Значит, ты ищешь Малфиуса? Ты можешь с ним поговорить на другом\n" +
                "берегу реки. Я могу помочь тебе переправиться через реку,\n" +
                "если ты поможешь мне.", null,
                new AnswerVariant("Хорошо, я согласен. А что конкретно от меня требуется?.", questAcceptedPhrase, false));

            var phrase2 = new DialogPhrase(
                "Я не уверена, но я уже начала беспокоиться об этом.\n" +
                "Часто я нахожу раненых и растерзанных животных вокруг своей избушки.\n" +
                "Я чувствую, что это связано с темными силами, но больше ничего не знаю.\n" +
                "Я догадываюсь, что есть кто-то, кто стоит за всем этим.", null,
                new AnswerVariant("Да, все заметили, что происходит что-то странное.\n" +
                                  "Я собираюсь найти местного волшебника, чтобы узнать больше.", phrase1, false));
            var phrase3 = new DialogPhrase(
                "Этот клиент обещал заплатить мне огромную сумму, если я помогу ему\n" +
                "влюбить в себя девушку. Ко мне приходят все меньше людей, все боятся\n" +
                "заходить в лес после того, как началось что-то плохое.\n" +
                "Однако, я не знаю, как помочь этому человеку.", null,
                new AnswerVariant("Я занимаюсь изучением магии и рун, поэтому думаю, что смогу тебе помочь.\n" +
                                  "А что ты знаешь о том, что происходит вокруг?", phrase2, false));
            var phrase4 = new DialogPhrase(
                "Ты знаешь, я принимаю у себя жителей из города и помогаю им\n" +
                "излечиться от болезней и травм. Я живу на эти деньги, чтобы продолжать\n" +
                "свою работу. Но сегодня ко мне пришел клиент с необычной просьбой.",
                null,
                new AnswerVariant("Что за просьба такая необычная?", phrase3, false));

            FirstPhrase = new DialogPhrase(
                "Добро пожаловать, путник. Меня зовут Вивиана.\n" +
                "Я рада, что ты нашел меня. Я видела, как ты помогал моим знакомым в\n" +
                "этом лесу, и мне тоже нужна твоя помощь.", null,
                new AnswerVariant("Здравствуй. Я всегда готов помочь. В чем заключается твоя просьба?", phrase4, false));

            DialogInfo.Add(NPCType.Witch, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }
    
    public static class TraderDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Ты спас самого близкого для меня человека.\n" +
            "Никакая шкатулка этого не окупит. Да, я пойду с вами, чтобы хоть немного\n" +
            "оплатить свой долг перед тобой.", "Торговец присоединился к вашей команде", 
            new AnswerVariant("Отлично, спасибо большое!", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Ты не помнишь?! Моей жене очень плохо, мне не с кем ее оставить.\n" +
            "Да и не хочу я идти с тобой.", null, 
            new AnswerVariant("Хорошо, я понял.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Моей жене все хуже. Ну что-то вроде лечения или регенерации.",
                null, new AnswerVariant(
                    "Да, стараюсь как можно быстрее.", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Спасибо за этот свиток! Сегодня попробую вылечить свою жену.",
                null, new AnswerVariant("Всегда пожалуйста.", null, true));
            var endPhrase = new DialogPhrase(
                "Эх, где бы мне найти того, кто согласится это купить...", null, 
                new AnswerVariant("...", null, true));

            var endQuestAnswer = new AnswerVariant("...", endPhrase, true, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Что ты мне дал? Это не помогло!\n" +
                "Иди своей дорогой, а я буду искать нормальных покупателей.", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Благодарю тебе, великий волшебник! Моя жена теперь чувствует себя\n" +
                "прекрасно! Как и обещал, держи эту шкатулку. Я уверен, ты достоен ее!", 
                "Вы берете мистическую шкатулку в руки, как вдруг она издает громкие скрипы и расхлопывается.\n" +
                "В ней вы находите древний изумрудный ключ", endQuestAnswer);
            
            var questAcceptedPhrase = new DialogPhrase(
                "Отлично! Я очень надеюсь, что ты сможешь помочь!\n" +
                "Я не знаю, какие у тебя есть свитки, но мне нужно что-то вроде лечения\n" +
                "или регенерации.", null,
                new AnswerVariant("Постараюсь найти что-нибудь подходящее.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true));

            var questDenied = new DialogPhrase(
                "Не мешай мне продавать товары.", null, endQuestAnswer);
            
            var questOnDeny = new DialogPhrase(
                "Сомневаюсь, что у тебя хватит золота на эту шкатулку.\n" +
                "Иди своей дорогой, а я буду искать нормальных покупателей.", null,
                new AnswerVariant("Ладно, извини, что потревожил.", questDenied, true),
                new AnswerVariant("Подожди, хорошо, я постараюсь создать свиток для твоей жены.", questAcceptedPhrase, false));
            
            
            var phrase1 = new DialogPhrase(
                "Знаешь, дело в том, что я и не торговец вовсе. Меня зовут Альфред.\n" +
                "Я продаю товары потому что у моей жены сильный кашель,\n" +
                "она не может встать с кровати, а лекарства очень дорогие.\n" +
                "Но я тут подумал, может, ты сможешь помочь ее исцелить?", null,
                new AnswerVariant(
                    "Хорошо, я постараюсь создать свиток,\n" +
                    "который поможет твоей жене", questAcceptedPhrase, false),
                new AnswerVariant("Может все-таки я куплю ее за золото?", questOnDeny, false));

            var phrase2 = new DialogPhrase(
                "Это древняя реликвия, она откроется только если ее в руки возьмет\n" +
                "достойный волшебник. Не желаешь проверить, достоен ли ты?\n" +
                "Но за бесплатно я тебе ее не отдам.", null,
                new AnswerVariant("Хорошо, сколько золота тебе нужно?", phrase1, false));
            var phrase3 = new DialogPhrase(
                "Безделушки? Так значит ты занимаешься магией?\n" +
                "Хм... А что ты скажешь про эту древнюю магическую шкатулку?", null,
                new AnswerVariant("Что это такое?", phrase2, false));

            FirstPhrase = new DialogPhrase(
                "Здравствуй, путник! Не желаешь купить один из моих товаров?\n" +
                "Только лучшее качество.", null,
                new AnswerVariant(
                    "Здравствуй, у меня совсем нет времени на твои безделушки.\n" +
                    "Мне нужно победить зло, которое появилось в этом лесу.", phrase3, false));

            DialogInfo.Add(NPCType.Trader, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }
    
    public static class Thief
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase QuestInProgressPhrase;
        public static DialogPhrase QuestJustFinishedPhrase;
        public static DialogPhrase QuestFinishedGood;
        public static DialogPhrase QuestFinishedBad;
        public static DialogPhrase FinalPhraseGood = new DialogPhrase(
            "Сначала поговорим об оплате?... Ладно-ладно, шучу,\n" +
            "просто сделаешь мне еще одно заклинание невидимости... Пожалуйста.\n" +
            "И я помогу тебе победить этого Веракса!", "Вор присоединился к вашей команде", 
            new AnswerVariant("Хорошо, после сражения, обязательно сделаю тебе свиток.", null, true, 
                npc =>
                {
                    if (!npc.GameCore.FinalTeam.Contains(npc)) 
                        npc.GameCore.FinalTeam.Add(npc);
                }));
        public static DialogPhrase FinalPhraseBad = new DialogPhrase(
            "Слушай, у меня тут огромный заказ, мне нужно хорошо подготовиться.\n" +
            "Там будет столько золота, сколько никто из нас не видел.\n" +
            "Извини, но я не смогу вам помочь.", null, 
            new AnswerVariant("Хорошо, я понял.", null, true));
        public static void Initialize()
        {
            QuestInProgressPhrase = new DialogPhrase(
                "Я жду твоего волшебства, чтобы я мог стать невидимым.",
                null, new AnswerVariant("Осталось немного.", null, true));
            QuestJustFinishedPhrase = new DialogPhrase(
                "Что ж посмотрим, получится ли у твоего свитка обмануть зрение людей.",
                null, new AnswerVariant("...", null, true));
            var endPhrase = new DialogPhrase(
                "Порхай как бабочка, жаль как пчела...\n" +
                "А? Это ты, извини, я занят, тренируюсь.", null, 
                new AnswerVariant("...", null, true));

            var endQuestAnswer = new AnswerVariant("...", endPhrase, true, (npc =>
            {
                if (npc.IsQuestFinishedGood)
                {
                    npc.GameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                }
            }));
            QuestFinishedBad = new DialogPhrase(
                "Я не стал невидимым! Как и говорил, это нереально.\n" +
                "Не получится ни у кого...", null, endQuestAnswer);
            QuestFinishedGood = new DialogPhrase(
                "Это было что-то! Меня вообще никто не видел!\n" +
                "Я был как обычный воздух! Жаль только, что дело пустышкой оказалось,\n" +
                "в доме совсем не было ничего ценного.\n" +
                "Вот, наверное, самое интересное, что удалось унести.", 
                "Вы получили новый изумрудный ключ", endQuestAnswer);
            
            var questDenied1 = new DialogPhrase(
                "Вот и я говорю, что быть полностью невидимым нереально.", null, 
                new AnswerVariant("...", null, true));
            
            var questAcceptedPhrase = new DialogPhrase(
                "Хорошо, предлагаю сделку: ты делаешь меня невидимым,\n" +
                "а я отдаю тебе самую ценную вещь, которую получится унести.", null,
                new AnswerVariant("Договорились.", QuestInProgressPhrase, true,
                    npc => npc.IsQuestActive = true), 
                new AnswerVariant("Извини, мне нужно заниматься более важными делами.", questDenied1, false));

            var questDenied2 = new DialogPhrase(
                "Ты меня не видел, я тебя не видел. Договорились?", null, 
                new AnswerVariant("...", null, true));
            var answerToDeny =
                new AnswerVariant("Хорошо, можешь продолжать свою тренировку", questDenied2, true);
            
            var interestingPhrase = new DialogPhrase(
                "О чем ты говоришь? Это нереально. Если бы я был невидимым,\n" +
                "дела бы явно пошли в гору.", null, 
                new AnswerVariant(
                    "Дело в том, что я занимаюсь изучением магии\n" +
                    "и могу помочь тебе стать невидимым.", questAcceptedPhrase, false),
                new AnswerVariant("А, ладно, ничего.", questDenied1, false));
            var answerToInterestingPhrase =
                new AnswerVariant("Невидимым? Это звучит осуществимо.", interestingPhrase, false);
            
            var dontDisturb = new DialogPhrase(
                "Ну не мешай. Я стараюсь быть как тень, стараюсь быть невидимым...", null, 
                answerToInterestingPhrase, answerToDeny);
            questDenied2.AnswerVariants[0].NextPhrase = dontDisturb;
            questDenied1.AnswerVariants[0].NextPhrase = dontDisturb;
            
            var phrase1 = new DialogPhrase(
                "Да, но я ворую только на заказ, себе беру небольшую часть. \n" +
                "Намечается большое дело и мне нужно быть подготовленным.\n" +
                "Эх, вот бы я был невидимым.", null,
                answerToDeny, answerToInterestingPhrase);

            var phrase2 = new DialogPhrase(
                "Тссс!!! Я тренируюсь перед крупной кражей.", null,
                new AnswerVariant("Ты занимаешься воровством?", phrase1, false));
            FirstPhrase = new DialogPhrase(
                "Тсс... Тихо и тактично...", null,
                new AnswerVariant("Что ты делаешь?", phrase2, false));

            DialogInfo.Add(NPCType.Thief, (FirstPhrase, QuestInProgressPhrase, QuestJustFinishedPhrase, QuestFinishedGood, QuestFinishedBad, FinalPhraseGood, FinalPhraseBad));
        }
    }

    public static class MageDialog
    {
        public static DialogPhrase FirstPhrase;
        public static DialogPhrase FirstQuestInProgressPhrase;
        public static DialogPhrase SecondQuestInProgressPhrase;
        public static DialogPhrase FirstQuestFinished;
        public static DialogPhrase SecondQuestFinished { get; set; }

        public static void Initialize()
        {
            FirstQuestInProgressPhrase = new DialogPhrase(
                "Напоминаю, мой юный друг, нам нужны четыре заклинания:\n" +
                "1. Защиты от магии\n" +
                "2. Защиты от огня\n" +
                "3. Защиты от мороза\n" +
                "4. Защиты от бури",
                null, new AnswerVariant("Осталось немного.", null, true));
            SecondQuestInProgressPhrase = new DialogPhrase(
                "Юный друг, ты принес мощное финальное заклинание,\n" +
                "которое добьет Веракса? У нас не так много времени.", null, new AnswerVariant("Да, уже совсем скоро будет готово.", null, true)
            );
            var secondQuest1 = new DialogPhrase(
                "Я оставляю выбор финального свитка тебе, но ты должен понимать,\n" +
                "что это должна быть сильная магия.",
                null, new AnswerVariant(
                    "Хорошо, я придумаю, какое заклинание нам стоит применить\n" +
                    "в конце, чтобы победить Веракса.", SecondQuestInProgressPhrase, true, npc =>
                    {
                        npc.IsSecondFinalQuestActive = true;
                        npc.IsQuestActive = true;
                    })
                );
            FirstQuestFinished = new DialogPhrase(
                "У тебя получились отличные свитки.\n" +
                "С их помощью мы сможем защититься от Веракса.\n" +
                "Но теперь нам нужен свиток, который мы используем\n" +
                "в самый последний момент, чтобы поставить точку в нашей битве.",
                null, new AnswerVariant("...", secondQuest1, false,npc => npc.IsFirstFinalQuestActive = false));
            
            var startFirstQuest = new DialogPhrase(
                "Спасибо, мой юный друг. Я верю в тебя и в твои способности.\n" +
                "Время поджимает, и наши судьбы связаны судьбой всего королевства.\n" +
                "Поторопись, и пусть свет и добро будут с тобой.", null,
                new AnswerVariant("...", FirstQuestInProgressPhrase, true, npc => npc.IsFirstFinalQuestActive = true)
            );
            var phrase1 = new DialogPhrase(
                "Первый должен защищать от магии, чтобы отразить колдовство Веракса.\n" +
                "Второй - оберегать от огня, потому что его огненные заклинания ужасны.\n" +
                "Третий - спасать от мороза, ибо Веракс использует ледяные чары.\n" +
                "А четвертый должен защищать от бури, чтобы укрыться от его ярости.", null,
                new AnswerVariant("Я понял, Малфиус.\n" +
                                  "Я приму эту задачу на себя и создам необходимые свитки.",
                    startFirstQuest, false));
            var phrase2 = new DialogPhrase(
                "Я знаю, что ты уже уверенно занимаешься созданием волшебных свитков.\n" +
                "Мне нужно, чтобы ты создал четыре защитных свитка.", null,
                new AnswerVariant("Нужны какие-то специальные свитки?", phrase1, false));
            var phrase3 = new DialogPhrase(
                "Я заметил, что его аура искажена и источает зло,\n" +
                "а сам себя он прозвал Мракотворцем.\n" +
                "Я не знаю его точных мотивов, но нам нужно его остановить.\n" +
                "С твоей помощью у нас должно все получится", null,
                new AnswerVariant("Что я могу сделать, чтобы помочь?", phrase2, false));
            var phrase4 = new DialogPhrase(
                "Веракс когда-то давно был одним из моих учеников,\n" +
                "но потом темные силы захватили его и он ушел скитаться по миру.\n" +
                "Он изучал темные и запретные искусства в течение многих лет.\n" +
                "Он обладает невероятной магической силой и способностью\n" +
                "манипулировать темными энергиями.", null,
                new AnswerVariant("...", phrase3, false));
            var phrase5 = new DialogPhrase(
                "О, мой юный друг, в этом мире появился злой колдун\n" +
                "по имени Веракс Мракотворец. Он открыл портал в темное измерение,\n" +
                "из которого появляются монстры и разносят зло на своем пути.\n" +
                "Люди и животные живут в страхе, их мир погрузился во мрак.\n" +
                "Но один я не смогу победить Веракса.", null,
                new AnswerVariant("Это звучит страшно, Малфиус. Зачем он это сделал?", phrase4, false));
            var phrase6 = new DialogPhrase(
                "Ах, вот и ты, мой юный друг!\n" +
                "Я знал, что ты придешь ко мне.\n" +
                "В твоих глазах я вижу стремление к знаниям и силе.", null,
                new AnswerVariant(
                    "Добрый день, Малфиус. Я заметил, что в этих землях происходит\n" +
                    "что-то странное и искал ответы на все свои вопросы.", phrase5, false));

            FirstPhrase = new DialogPhrase(
                "*Старый волшебник погружен в чтение древних летописей*", null,
                new AnswerVariant("Кхм...", phrase6, false));
            
            var finalPhrase = new DialogPhrase(
                "Будь осторожен, юный волшебник.\n" +
                "Хоть я и буду рядом, не надейся только на мою помощь.\n" +
                "Я верю в нашу силу и вижу, что ты достойный человек,\n" +
                "и пусть свет и добро будут с нами.", null,
                new AnswerVariant("...", null, true,
                    npc =>
                        {
                            npc.GameCore.Game.SetScreen(GameScreen.EndingScreen);
                            npc.GameCore.Game.IsInTopDownView = false;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(npc.GameCore.Game.FinalSong);
                        }
                    ));
            var prefinalInProgressPhrase = new DialogPhrase(
                "Добрый день, юный друг. Ты уже собрал достаточно людей в команду?\n" +
                "Если так, то мы можем отправляться на финальную битву с Вераксом!", null,
                new AnswerVariant("Да, я готов. Пора отправляться в путь.", finalPhrase, false),
                new AnswerVariant("Мне нужно еще немного времени.", null, true));
            
            SecondQuestFinished = new DialogPhrase(
                "Хорошо, я доверюсь твоему выбору.\n" +
                "Теперь нам нужно собрать команду.\n" +
                "Может, у тебя есть знакомые в этом королевстве,\n" +
                "кто не отказался бы нам помочь в борьбе со злом?", null,
                new AnswerVariant("Я думаю, найдется несколько.", prefinalInProgressPhrase, true, npc =>
                {
                    npc.GameCore.IsFinalQuestPlaying = true;
                    npc.IsSecondFinalQuestActive = false;
                }));
            
        }
    }
}