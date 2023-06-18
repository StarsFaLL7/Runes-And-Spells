using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public DialogPhrase CurrentPhrase { get; private set; }
    public bool IsDialogOpened;
    public bool IsQuestFinishedGood;
    public bool IsQuestActive;
    public bool IsQuestFinished;
    public int QuestEndDayCount;
    public NPCType NPCType;
    public Item GivenScroll;
    public int GivenScrollPower;
    public int _animFrame;
    public Timer _animTimer;
    public int _animFramesCount;
    private KeyboardState _lastKBState;

    public bool IsFirstFinalQuestActive;
    public bool IsSecondFinalQuestActive;
    private List<string> MageGivenScrollsIds = new List<string>();

    public NPC(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo fromObjectInfo, string name, 
        string visibleName, NPCType npcType,DialogPhrase startPhrase, TopDownCore core)
        : base(positionInPixelsLeftBottom, fromObjectInfo, name, core)
    {
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
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var GapFromTextToAnswers = 32;
        var BorderWidth = 4;
        var InnerBorderGap = 8;
        var variantsGap = 12;
        spriteBatch.Draw(SpriteSheet, new Vector2(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X, 
                PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - GameCore.CameraPosition.Y),
            new Rectangle(SpriteSheetRectangle.X + _animFrame*SpriteSheetRectangle.Width, SpriteSheetRectangle.Y, 
                SpriteSheetRectangle.Width, SpriteSheetRectangle.Height), Color.White);
        if (new Rectangle((int)(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X), 
                (int)(PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - GameCore.CameraPosition.Y), SpriteSheetRectangle.Width,
                SpriteSheetRectangle.Height).Contains(Mouse.GetState().Position))
        {
            var size = AllMapDynamicObjects.DialogSpriteFont.MeasureString(VisibleName);
            spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, VisibleName,  
                new Vector2(Mouse.GetState().X+8,Mouse.GetState().Y+4-size.Y), Color.Black);
        }
        if (IsDialogOpened)
        {
            var strSize = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.Text);
            var answerSize1 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[0].Text + "> 1. ");
            var answerSize2 = Vector2.Zero;
            if (CurrentPhrase.AnswerVariants.Length > 1)
            {
                answerSize2 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[1].Text + "2. ");
                if (CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[1])
                {
                    answerSize1 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[0].Text + "1. ");
                    answerSize2 = AllMapDynamicObjects.DialogSpriteFont.MeasureString(CurrentPhrase.AnswerVariants[1].Text + "> 2. ");
                }
            }
            var drawPos = new Vector2(PositionInPixelsLeftBottom.X + SpriteSheetRectangle.Width/2 - GameCore.CameraPosition.X
                - Math.Max(strSize.X, Math.Max(answerSize1.X,answerSize2.X))/2 - BorderWidth - InnerBorderGap,
                
                PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height*3/4 - GameCore.CameraPosition.Y
                - AllMapDynamicObjects.DialogTipTexture.Height - 2*BorderWidth - InnerBorderGap*2 - GapFromTextToAnswers
                - answerSize1.Y - strSize.Y);
            
            var InnerSize = new Vector2(Math.Max(strSize.X, Math.Max(answerSize1.X, answerSize2.X)), 
                strSize.Y + GapFromTextToAnswers + answerSize1.Y);
            if (answerSize2 != Vector2.Zero)
            {
                InnerSize.Y += variantsGap + answerSize2.Y;
                drawPos.Y += -answerSize2.Y - variantsGap;
            }
            
            
            spriteBatch.Draw(AllMapDynamicObjects.DialogBorderTexture, drawPos, 
                new Rectangle(0,0, (int)InnerSize.X + 2*BorderWidth + 2*InnerBorderGap, (int)InnerSize.Y + 2*BorderWidth + 2*InnerBorderGap), Color.White);
            spriteBatch.Draw(AllMapDynamicObjects.DialogBgTexture, new Vector2(drawPos.X + BorderWidth, drawPos.Y + BorderWidth), 
                new Rectangle(0,0, (int)InnerSize.X + 2*InnerBorderGap, (int)InnerSize.Y + 2*InnerBorderGap), Color.White);
            spriteBatch.Draw(AllMapDynamicObjects.DialogTipTexture, 
                new Vector2(drawPos.X + BorderWidth + InnerBorderGap + InnerSize.X/2 - AllMapDynamicObjects.DialogTipTexture.Width/2,
                    drawPos.Y + InnerSize.Y + 2*BorderWidth + 2*InnerBorderGap - 8), Color.White);
            
            spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, CurrentPhrase.Text, 
                new Vector2(drawPos.X + BorderWidth + InnerBorderGap, drawPos.Y + BorderWidth + InnerBorderGap), Color.Black);
            if (CurrentPhrase.AnswerVariants.Length == 1)
            {
                spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, $"> 1. {CurrentPhrase.AnswerVariants[0].Text}", 
                    new Vector2(drawPos.X + BorderWidth + InnerBorderGap, drawPos.Y + BorderWidth + InnerBorderGap + strSize.Y + GapFromTextToAnswers), 
                    Color.Black);
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
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[0] ? Color.Black : Color.Gray);
                spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, secondVariantPrefix+CurrentPhrase.AnswerVariants[1].Text, 
                    secondPos, 
                    CurrentPhrase.SelectedAnswerVariant == CurrentPhrase.AnswerVariants[1] ? Color.Black : Color.Gray);
            }
            DrawHint(spriteBatch, CurrentPhrase.AlertText);
        }
    }

    public static void DrawHint(SpriteBatch spriteBatch, string text)
    {
        if (text is null || text.Length == 0) return;
        
        var txtSize = AllMapDynamicObjects.DialogSpriteFont.MeasureString(text);
        var ScreenBorderGap = 24;
        var borderGap = 4;
        var InnerBorderGap = 8;
        var drawPos = new Vector2(Game1.ScreenWidth - ScreenBorderGap - 2*borderGap - 2*InnerBorderGap - txtSize.X, ScreenBorderGap);
        spriteBatch.Draw(AllMapDynamicObjects.DialogBorderTexture, drawPos, 
            new Rectangle(0,0,
                (int)txtSize.X + 2*borderGap + 2*InnerBorderGap, (int)txtSize.Y + 2*borderGap + 2*InnerBorderGap), Color.White);
        spriteBatch.Draw(AllMapDynamicObjects.HintBgTexture, new Vector2(drawPos.X+borderGap, drawPos.Y+borderGap), 
            new Rectangle(0,0, 
                (int)txtSize.X + 2*InnerBorderGap, (int)txtSize.Y + 2*InnerBorderGap), Color.White);
        spriteBatch.DrawString(AllMapDynamicObjects.DialogSpriteFont, text, 
            new Vector2(drawPos.X + borderGap + InnerBorderGap, drawPos.Y+borderGap+InnerBorderGap), new Color(27,29,38));
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
            if (IsQuestActive)
            {
                var draggedItem = GameCore.Game.Inventory.Items.FirstOrDefault(i => i.IsBeingDragged);
                if (draggedItem is not null && GivenScroll is null &&
                    draggedItem.Type == ItemType.Scroll &&
                    new Rectangle((int)(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X),
                        (int)(PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height -
                              GameCore.CameraPosition.Y),
                        SpriteSheetRectangle.Width, SpriteSheetRectangle.Height).Contains(Mouse.GetState().Position))
                {
                    GivenScroll = draggedItem;
                    draggedItem.IsBeingDragged = false;
                    QuestEndDayCount = GameCore.Game.DayCount;
                    IsQuestActive = false;
                    IsQuestFinished = true;
                    CurrentPhrase = AllDialogs.DialogInfo[NPCType].QuestJustFinishedPhrase;
                }
            }

            if (IsQuestFinished && CurrentPhrase == AllDialogs.DialogInfo[NPCType].QuestJustFinishedPhrase &&
                QuestEndDayCount < GameCore.Game.DayCount)
            {
                IsQuestFinishedGood = NPCQuestsConditions.Conditions[NPCType](GivenScroll);
                CurrentPhrase = IsQuestFinishedGood
                    ? AllDialogs.DialogInfo[NPCType].QuestFinishedGood
                    : AllDialogs.DialogInfo[NPCType].QuestFinishedBad;
                GivenScrollPower = int.Parse(GivenScroll.ID.Split('_')[5]);
            }

            if (IsQuestFinished && GameCore.IsFinalQuestPlaying)
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
                if (_lastKBState[Keys.A] == KeyState.Up && Keyboard.GetState()[Keys.A] == KeyState.Down ||
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
                     GameMap.TileSize*5/2, GameMap.TileSize*2).Contains(GameCore.PlayerPosition))
        {
            OpenDialog();
        }

        _lastKBState = Keyboard.GetState();
    }

    private void UpdateMage()
    {
        var draggedItem = GameCore.Game.Inventory.Items.FirstOrDefault(i => i.IsBeingDragged);
        if (draggedItem is not null && GivenScroll is null &&
            draggedItem.Type == ItemType.Scroll &&
            new Rectangle((int)(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X),
                (int)(PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height / 2 -
                      GameCore.CameraPosition.Y),
                SpriteSheetRectangle.Width, SpriteSheetRectangle.Height).Contains(Mouse.GetState().Position) &&
            Mouse.GetState().LeftButton == ButtonState.Released)
        {
            GivenScroll = draggedItem;
            draggedItem.IsBeingDragged = false;
            if (MageCheckInputScroll(draggedItem))
            {
                var idDetails = draggedItem.ID.Split('_');
                MageGivenScrollsIds.Add(String.Join("_", idDetails[..3]));
                draggedItem.SubtractCount(1);
                if (IsSecondFinalQuestActive)
                {
                    CurrentPhrase = AllDialogs.MageDialog.SecondQuestFinished;
                    IsSecondFinalQuestActive = false;
                    GameCore.IsFinalQuestPlaying = true;
                    GameCore.FinalScroll = ScrollsRecipes.Recipes
                        .First(r => r.Value.id == String.Join("_", idDetails[..3]))
                        .Value.rus;
                }
            }
            
            if (IsFirstFinalQuestActive && MageGivenScrollsIds.Contains("scroll_moon_black_1") &&
                MageGivenScrollsIds.Contains("scroll_fire_black_1") && MageGivenScrollsIds.Contains("scroll_air_black_1") &&
                MageGivenScrollsIds.Contains("scroll_ice_black_1"))
            {
                CurrentPhrase = AllDialogs.MageDialog.FirstQuestFinished;
                IsFirstFinalQuestActive = false;
                IsSecondFinalQuestActive = true;
            }
            
        }
    }

    private bool MageCheckInputScroll(Item scroll)
    {
        if (IsFirstFinalQuestActive && (scroll.ID.Contains("scroll_moon_black_1") || scroll.ID.Contains("scroll_fire_black_1") || 
            scroll.ID.Contains("scroll_air_black_1") || scroll.ID.Contains("scroll_ice_black_1")))
        {
            return true;
        }
        if (IsSecondFinalQuestActive)
        {
            return true;
        }

        return false;
    }
    
    public void CloseDialog()
    {
        IsDialogOpened = false;
        GameCore.IsDialogOpened = false;
    }
    
    public void OpenDialog()
    {
        IsDialogOpened = true;
        GameCore.IsDialogOpened = true;
        GameCore.IsPlayerMoving = false;
        GameCore.IsPlayerRunning = false;
        if (GameCore.PlayerPosition.Y < PositionInPixelsLeftBottom.Y)
        {
            GameCore.PlayerLastLookDirection = Direction.Down;
        }
        else if (GameCore.PlayerPosition.Y > PositionInPixelsLeftBottom.Y)
        {
            GameCore.PlayerLastLookDirection = Direction.Up;
        }
        else if (GameCore.PlayerPosition.X > PositionInPixelsLeftBottom.X)
        {
            GameCore.PlayerLastLookDirection = Direction.Left;
        }
        else if (GameCore.PlayerPosition.X <= PositionInPixelsLeftBottom.X)
        {
            GameCore.PlayerLastLookDirection = Direction.Right;
        }
    }

    public void SetCurrentPhrase(DialogPhrase nextPhrase)
    {
        CurrentPhrase = nextPhrase;
    }
}