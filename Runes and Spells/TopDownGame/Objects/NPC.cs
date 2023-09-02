using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.TopDownGame.Core.Utility;
using Runes_and_Spells.TopDownGame.Dialogs;
using Runes_and_Spells.TopDownGame.NPCData;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Objects;

public class NPC : MapObject
{
    public string VisibleName;
    [JsonIgnore]
    public DialogPhrase CurrentPhrase { get; private set; }
    [JsonIgnore]
    public bool IsDialogOpened;
    public bool IsQuestFinishedGood;
    public bool IsQuestActive;
    public bool IsQuestFinished;
    public int QuestEndDayCount;
    public NPCType NPCType;
    [JsonIgnore]
    public Item GivenScroll;
    public int GivenScrollPower;
    [JsonIgnore]
    public int _animFrame;
    [JsonIgnore]
    public Timer _animTimer;
    [JsonIgnore]
    public int _animFramesCount;
    [JsonIgnore]
    public UiSlot SlotForScrolls;
    [JsonIgnore]
    public UiButton GiveScrollButton;
    
    private bool IsButtonFocused;
    private KeyboardState _lastKBState;
    
    private static TopDownCore _gameCore;
    
    private static Texture2D _dialogBoxTexture;
    private static Dictionary<NPCType, Texture2D> _npcPortraitsTextures;
    private static Texture2D _portraitBorderTexure;
    
    public bool IsFirstFinalQuestActive;
    public bool IsSecondFinalQuestActive;
    public List<string> MageGivenScrollsIds { get; } = new List<string>();

    public NPC(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo fromObjectInfo, string name, 
        string visibleName, NPCType npcType,DialogPhrase startPhrase, TopDownCore core)
        : base(positionInPixelsLeftBottom, fromObjectInfo, name, core)
    {
        _gameCore = core;
        CurrentPhrase = startPhrase;
        NPCType = npcType;
        VisibleName = visibleName;
        _animFramesCount = 6;
        _animTimer = new Timer(400, () =>
        {
            _animFrame++;
            if (_animFrame >= _animFramesCount)
            {
                _animFrame = 0;
            }
            _animTimer.StartAgain();
        });
        _animTimer.StartAgain();
        _dialogBoxTexture = core.Game.Content.Load<Texture2D>("top-down/dialogue/dialog_box");
        _portraitBorderTexure = core.Game.Content.Load<Texture2D>("top-down/dialogue/border");
        _npcPortraitsTextures = new Dictionary<NPCType, Texture2D>()
        {
            { NPCType.Bard, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_bard")},
            { NPCType.FisherMan, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_fisherman")},
            { NPCType.Mage, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_mage")},
            { NPCType.Hunter, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_hunter")},
            { NPCType.Woodman, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_lumberjack")},
            { NPCType.Witch, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_witch")},
            { NPCType.Thief, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_ninja")},
            { NPCType.Trader, core.Game.Content.Load<Texture2D>("top-down/dialogue/portrait_trader")}
        };
        SlotForScrolls = new UiSlot(new Vector2(774, 865), _gameCore.Game.Content.Load<Texture2D>("textures/Inventory/slot_bg"),
            _gameCore.Game, ItemType.Scroll);
        GiveScrollButton = new UiButton(
            _gameCore.Game.Content.Load<Texture2D>("textures/buttons/small_stone_button_default"),
            _gameCore.Game.Content.Load<Texture2D>("textures/buttons/small_stone_button_hovered"),
            _gameCore.Game.Content.Load<Texture2D>("textures/buttons/small_stone_button_pressed"), 
            new Vector2(716, 978),
            "Give", AllGameItems.Font24Px, new Color(35, 35, 35), TryToGiveScroll
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var GapFromTextToAnswers = 32*Game1.ResolutionScale.Y;
        var BorderWidth = 4;
        var InnerBorderGap = 8;
        var variantsGap = 12*Game1.ResolutionScale.Y;
        spriteBatch.Draw(SpriteSheet, new Vector2(PositionInPixelsLeftBottom.X - _gameCore.CameraPosition.X, 
                PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - _gameCore.CameraPosition.Y),
            new Rectangle(SpriteSheetRectangle.X + _animFrame*SpriteSheetRectangle.Width, SpriteSheetRectangle.Y, 
                SpriteSheetRectangle.Width, SpriteSheetRectangle.Height), Color.White);
        if (new Rectangle((int)(PositionInPixelsLeftBottom.X - _gameCore.CameraPosition.X), 
                (int)(PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - _gameCore.CameraPosition.Y), SpriteSheetRectangle.Width,
                SpriteSheetRectangle.Height).Contains(Mouse.GetState().Position))
        {
            var size = AllMapDynamicObjects.DialogSpriteFont.MeasureString(VisibleName);
            spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, VisibleName,  
                new Vector2(Mouse.GetState().X+8,Mouse.GetState().Y+4-size.Y), Color.Black);
        }
        if (IsDialogOpened)
        {
            _gameCore.View.DialogNPCToDraw = this;
            DrawHint(spriteBatch, CurrentPhrase.AlertText);
            /*
            var strSize = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.Text)*Game1.ResolutionScale;
            var answerSize1 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[0].Text + "> 1. ")*Game1.ResolutionScale;
            var answerSize2 = Vector2.Zero;
            if (CurrentPhrase.AnswerVariants.Length > 1)
            {
                answerSize2 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[1].Text + "2. ")*Game1.ResolutionScale;
                if (CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[1])
                {
                    answerSize1 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[0].Text + "1. ")*Game1.ResolutionScale;
                    answerSize2 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[1].Text + "> 2. ")*Game1.ResolutionScale;
                }
            }
            var drawPos = new Vector2(PositionInPixelsLeftBottom.X + SpriteSheetRectangle.Width/2 - _gameCore.CameraPosition.X
                - Math.Max(strSize.X, Math.Max(answerSize1.X,answerSize2.X))/2 - (BorderWidth + InnerBorderGap)*Game1.ResolutionScale.X,

                PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height*3/4 - _gameCore.CameraPosition.Y
                - (AllMapDynamicObjects.DialogTipTexture.Height + 2*BorderWidth + InnerBorderGap*2)*Game1.ResolutionScale.Y - GapFromTextToAnswers
                - answerSize1.Y - strSize.Y);

            var InnerSize = new Vector2(Math.Max(strSize.X, Math.Max(answerSize1.X, answerSize2.X)),
                strSize.Y + GapFromTextToAnswers + answerSize1.Y);
            if (answerSize2 != Vector2.Zero)
            {
                InnerSize.Y += variantsGap + answerSize2.Y;
                drawPos.Y += -answerSize2.Y - variantsGap;
            }
            spriteBatch.Draw(AllMapDynamicObjects.DialogBorderTexture, drawPos,
                new Rectangle(0,0,
                    (int)(InnerSize.X + (2*BorderWidth + 2*InnerBorderGap)*Game1.ResolutionScale.X),
                    (int)(InnerSize.Y + (2*BorderWidth + 2*InnerBorderGap)*Game1.ResolutionScale.Y)),
                Color.White);

            spriteBatch.Draw(AllMapDynamicObjects.DialogBgTexture,
                new Vector2(drawPos.X + BorderWidth*Game1.ResolutionScale.X, drawPos.Y + BorderWidth*Game1.ResolutionScale.Y),
                new Rectangle(0,0,
                    (int)(InnerSize.X + 2*InnerBorderGap*Game1.ResolutionScale.X),
                    (int)(InnerSize.Y + 2*InnerBorderGap*Game1.ResolutionScale.Y)),
                Color.White);

            spriteBatch.Draw(AllMapDynamicObjects.DialogTipTexture,
                new Vector2(drawPos.X + InnerSize.X/2 + (BorderWidth + InnerBorderGap - AllMapDynamicObjects.DialogTipTexture.Width/2)*Game1.ResolutionScale.X,
                    drawPos.Y + InnerSize.Y + (2*BorderWidth + 2*InnerBorderGap - 8)*Game1.ResolutionScale.Y), null,
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);

            spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, CurrentPhrase.Text,
                new Vector2(drawPos.X + (BorderWidth + InnerBorderGap)*Game1.ResolutionScale.X,
                    drawPos.Y + (BorderWidth + InnerBorderGap)*Game1.ResolutionScale.Y), Color.Black,
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);

            if (CurrentPhrase.AnswerVariants.Length == 1)
            {
                spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, $"> 1. {CurrentPhrase.AnswerVariants[0].Text}",
                    new Vector2(drawPos.X + BorderWidth + InnerBorderGap,
                        drawPos.Y + BorderWidth + InnerBorderGap + strSize.Y + GapFromTextToAnswers),
                    Color.Black, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
            if (CurrentPhrase.AnswerVariants.Length == 2)
            {
                var firstVariantPrefix =
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[0] ? "> 1. " : "1. ";
                var secondVariantPrefix =
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[1] ? "> 2. " : "2. ";
                var firstPos = new Vector2(drawPos.X + BorderWidth + InnerBorderGap,
                    drawPos.Y + BorderWidth + InnerBorderGap + strSize.Y + GapFromTextToAnswers);
                var secondPos = new Vector2(drawPos.X + BorderWidth + InnerBorderGap,
                    drawPos.Y + BorderWidth + InnerBorderGap + strSize.Y + GapFromTextToAnswers + answerSize1.Y + variantsGap);
                spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, firstVariantPrefix+CurrentPhrase.AnswerVariants[0].Text,
                    firstPos,
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[0] ? Color.Black : Color.Gray,
                    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, secondVariantPrefix+CurrentPhrase.AnswerVariants[1].Text,
                    secondPos,
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[1] ? Color.Black : Color.Gray,
                    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
            DrawHint(spriteBatch, CurrentPhrase.AlertText);
            */
        }
    }

    public static void DrawHint(SpriteBatch spriteBatch, string text)
    {
        if (text is null || text.Length == 0) return;
        var txtSize = AllMapDynamicObjects.DialogSpriteFont.MeasureString(text);
        var ScreenBorderGap = 24;
        var borderGap = 4;
        var InnerBorderGap = 8;
        var drawPos = new Vector2(1920 - ScreenBorderGap - 2*borderGap - 2*InnerBorderGap - txtSize.X, ScreenBorderGap)*Game1.ResolutionScale;
        
        spriteBatch.Draw(AllMapDynamicObjects.DialogBorderTexture, drawPos, 
            new Rectangle(0,0,
                (int)((txtSize.X + 2*borderGap + 2*InnerBorderGap)*Game1.ResolutionScale.X), 
                (int)((txtSize.Y + 2*borderGap + 2*InnerBorderGap)*Game1.ResolutionScale.Y)),
            Color.White);
        
        spriteBatch.Draw(AllMapDynamicObjects.HintBgTexture, 
            new Vector2(drawPos.X+borderGap*Game1.ResolutionScale.X, drawPos.Y+borderGap*Game1.ResolutionScale.Y), 
            new Rectangle(0,0, 
                (int)((txtSize.X + 2*InnerBorderGap)*Game1.ResolutionScale.X), 
                (int)((txtSize.Y + 2*InnerBorderGap)*Game1.ResolutionScale.Y)),
            Color.White);
        
        spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, text, 
            new Vector2(drawPos.X + (borderGap + InnerBorderGap)*Game1.ResolutionScale.X, 
                drawPos.Y + (borderGap + InnerBorderGap)*Game1.ResolutionScale.Y), 
            new Color(27,29,38), 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }

    public void Update()
    {
        _animTimer.Tick();
        if (NPCType == NPCType.Mage)
        {
            UpdateMage();
        }
        else
        {
            if (IsQuestActive && IsDialogOpened)
            {
                SlotForScrolls.Update(_gameCore.Game.Inventory);
                GiveScrollButton.Update(Mouse.GetState(), ref IsButtonFocused);
            }

            if (IsQuestFinished && CurrentPhrase == AllDialogs.DialogInfo[NPCType].QuestJustFinishedPhrase &&
                QuestEndDayCount < _gameCore.Game.DayCount)
            {
                IsQuestFinishedGood = NPCQuestsConditions.Conditions[NPCType](GivenScroll);
                CurrentPhrase = IsQuestFinishedGood
                    ? AllDialogs.DialogInfo[NPCType].QuestFinishedGood
                    : AllDialogs.DialogInfo[NPCType].QuestFinishedBad;
                GivenScrollPower = int.Parse(GivenScroll.ID.Split('_')[5]);
            }

            if (IsQuestFinished && _gameCore.IsFinalQuestPlaying)
            {
                if (IsQuestFinishedGood && GivenScrollPower >= 2)
                    CurrentPhrase = AllDialogs.DialogInfo[NPCType].FinalPhraseGood;
                else
                    CurrentPhrase = AllDialogs.DialogInfo[NPCType].FinalPhraseBad;
            }
        }

        if (IsDialogOpened)
        {
            if (CurrentPhrase.AnswerVariants.Length > 1)
            {
                if (_lastKBState[Keys.S] == KeyState.Up && Keyboard.GetState()[Keys.S] == KeyState.Down ||
                    _lastKBState[Keys.Down] == KeyState.Up && Keyboard.GetState()[Keys.Down] == KeyState.Down)
                {
                    CurrentPhrase.SelectedAnswerVariant = CurrentPhrase.AnswerVariants[1];
                } 
                if (_lastKBState[Keys.W] == KeyState.Up && Keyboard.GetState()[Keys.W] == KeyState.Down ||
                    _lastKBState[Keys.Up] == KeyState.Up && Keyboard.GetState()[Keys.Up] == KeyState.Down)
                {
                    CurrentPhrase.SelectedAnswerVariant = CurrentPhrase.AnswerVariants[0];
                }
            }
            if (_lastKBState[Keys.Enter] == KeyState.Up && Keyboard.GetState()[Keys.Enter] == KeyState.Down ||
                _lastKBState[Keys.E] == KeyState.Up && Keyboard.GetState()[Keys.E] == KeyState.Down)
            {
                CurrentPhrase.SelectedAnswerVariant.Select(this);
            }
            if (_lastKBState[Keys.Escape] == KeyState.Up && Keyboard.GetState()[Keys.Escape] == KeyState.Down)
            {
                CloseDialog();
            }
        }
        else if ( _lastKBState[Keys.E] == KeyState.Up && Keyboard.GetState()[Keys.E] == KeyState.Down &&
            new Rectangle((int)PositionInPixelsLeftBottom.X - GameMap.TileSize, (int)PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height/2,
                     GameMap.TileSize*3, GameMap.TileSize*2).Contains(_gameCore.PlayerPosition))
        {
            OpenDialog();
        }

        _lastKBState = Keyboard.GetState();
    }

    private void UpdateMage()
    {
        if (IsFirstFinalQuestActive || IsSecondFinalQuestActive)
        {
            IsQuestActive = true;
            SlotForScrolls.Update(_gameCore.Game.Inventory);
            GiveScrollButton.Update(Mouse.GetState(), ref IsButtonFocused);
        }
        
        if (IsFirstFinalQuestActive && MageGivenScrollsIds.Contains("scroll_moon_black_1") &&
            MageGivenScrollsIds.Contains("scroll_fire_black_1") && MageGivenScrollsIds.Contains("scroll_air_black_1") &&
            MageGivenScrollsIds.Contains("scroll_ice_black_1"))
        {
            IsQuestActive = false;
            CurrentPhrase = AllDialogs.MageDialog.FirstQuestFinished;
            IsFirstFinalQuestActive = false;
        }
    }

    private void TryToGiveScroll()
    {
        if (!SlotForScrolls.ContainsItem()) return;
        if (NPCType != NPCType.Mage)
        {
            var scroll = SlotForScrolls.currentItem;
            GivenScroll = scroll;
            QuestEndDayCount = _gameCore.Game.DayCount;
            IsQuestActive = false;
            IsQuestFinished = true;
            CurrentPhrase = AllDialogs.DialogInfo[NPCType].QuestJustFinishedPhrase;
        }
        else if (NPCType == NPCType.Mage)
        {
            var idDetails = SlotForScrolls.currentItem.ID.Split('_');
            if (IsFirstFinalQuestActive)
            {
                MageGivenScrollsIds.Add(String.Join("_", idDetails[..4]));
                var done = " - Готово";
                CurrentPhrase.Text = "Напоминаю, мой юный друг, нам нужны четыре заклинания:\n" +
                    $"1. Защиты от магии{(MageGivenScrollsIds.Contains("scroll_moon_black_1") ? done : "")}\n" +
                    $"2. Защиты от огня{(MageGivenScrollsIds.Contains("scroll_fire_black_1") ? done : "")}\n" +
                    $"3. Защиты от мороза{(MageGivenScrollsIds.Contains("scroll_ice_black_1") ? done : "")}\n" +
                    $"4. Защиты от бури{(MageGivenScrollsIds.Contains("scroll_air_black_1") ? done : "")}";
            }
            if (IsSecondFinalQuestActive)
            {
                GivenScroll = SlotForScrolls.currentItem;
                CurrentPhrase = AllDialogs.MageDialog.SecondQuestFinished;
                IsSecondFinalQuestActive = false;
                _gameCore.IsFinalQuestPlaying = true;
                _gameCore.FinalScroll = ScrollsRecipes.Recipes
                    .First(r => r.Value.id == String.Join("_", idDetails[..4]))
                    .Value.rus;
                IsQuestActive = false;
            }
        }
        SlotForScrolls.Clear();
    }

    public void CloseDialog()
    {
        IsDialogOpened = false;
        _gameCore.IsDialogOpened = false;
        _gameCore.View.DialogNPCToDraw = null;
    }
    
    public void OpenDialog()
    {
        if (CurrentPhrase.AlertText is not null && CurrentPhrase.AlertText != "")
        {
            AllGameItems.AlertSound.Play();
        }
        IsDialogOpened = true;
        _gameCore.IsDialogOpened = true;
        _gameCore.IsPlayerMoving = false;
        _gameCore.IsPlayerRunning = false;
        if (_gameCore.PlayerPosition.Y < PositionInPixelsLeftBottom.Y)
        {
            _gameCore.PlayerLastLookDirection = Direction.Down;
        }
        else if (_gameCore.PlayerPosition.Y > PositionInPixelsLeftBottom.Y)
        {
            _gameCore.PlayerLastLookDirection = Direction.Up;
        }
        else if (_gameCore.PlayerPosition.X > PositionInPixelsLeftBottom.X)
        {
            _gameCore.PlayerLastLookDirection = Direction.Left;
        }
        else if (_gameCore.PlayerPosition.X <= PositionInPixelsLeftBottom.X)
        {
            _gameCore.PlayerLastLookDirection = Direction.Right;
        }
    }

    public void SetCurrentPhrase(DialogPhrase nextPhrase)
    {
        CurrentPhrase = nextPhrase;
    }
}