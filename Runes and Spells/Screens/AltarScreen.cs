using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class AltarScreen : IScreen
{
    private Game1 _game;
    public AltarScreen(Game1 game) => _game = game;
    
    private Texture2D _backgroundTexture;
    private Texture2D _backPanelTexture;
    private Texture2D _amplificationGui;

    private UiProgressBar _pgUnite;
    private UiProgressBar _pgExtract;

    private UiSlot _slotUnite1;
    private UiSlot _slotUnite2;
    private UiSlot _slotUniteResult;
    private UiSlot _slotExtract;
    private UiSlot _slotExtractResult;
    private UiSlot[] _slotsAmplification;
    private UiSlot _slotAmplificationResult;

    private UiButton _buttonStartUnite;
    private UiButton _buttonStartExtract;
    private UiButton _buttonStartAmplification;

    private UiButton _buttonQuestionUnite;
    private UiButton _buttonQuestionExtract;
    private UiButton _buttonQuestionAmplification;
    private UiButton _buttonClosePopUp;
    private HelperPopUp _showPopUpType = HelperPopUp.None;
    private enum HelperPopUp
    {
        None, Unite, Extract, Amplification
    }

    private Dictionary<HelperPopUp, (Texture2D TextureRu, Texture2D TextureEn)> _popUpsTextures;

    private Timer _timerUnite;
    private Timer _timerExtract;
    private UiButton _buttonGoBack;
    public bool IsUniteInProgress;
    public bool IsExtractInProgress;
    private bool _isButtonFocused;
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/altar_screen/background");
        _backPanelTexture = content.Load<Texture2D>("textures/altar_screen/backPanel");
        _amplificationGui = content.Load<Texture2D>("textures/altar_screen/amplification_gui");
        _popUpsTextures = new Dictionary<HelperPopUp, (Texture2D TextureRu, Texture2D TextureEn)>()
        {
            { HelperPopUp.Amplification, 
                (content.Load<Texture2D>("textures/altar_screen/amplification_popup_ru"), 
                    content.Load<Texture2D>("textures/altar_screen/amplification_popup_en"))
            },
            { HelperPopUp.Extract, 
                (content.Load<Texture2D>("textures/altar_screen/extract_popup_ru"), 
                    content.Load<Texture2D>("textures/altar_screen/extract_popup_en"))
            },
            { HelperPopUp.Unite, 
            (content.Load<Texture2D>("textures/altar_screen/unite_popup_ru"), 
                content.Load<Texture2D>("textures/altar_screen/unite_popup_en"))
            }
        };
        var stoneSlot = content.Load<Texture2D>("textures/Inventory/slot_bg_stone");
        var slotEssenceTexture = content.Load<Texture2D>("textures/altar_screen/essence_slot");
        var buttonStartTexture = new[]
        {
            content.Load<Texture2D>("textures/buttons/small_purple_button_default"),
            content.Load<Texture2D>("textures/buttons/small_purple_button_hovered"),
            content.Load<Texture2D>("textures/buttons/small_purple_button_pressed")
        };
        
        _buttonStartUnite = new UiButton(buttonStartTexture[0], buttonStartTexture[1], buttonStartTexture[2],
            new Vector2(214, 817), 
            "Start", AllGameItems.Font24Px, new Color(85, 56, 100), () =>
            {
                StartUnite();
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 17)
                {
                    _game.Introduction.Step++;
                }
            }
            );
        _slotUnite1 = new UiSlot(new Vector2(141, 414), stoneSlot, _game, ItemType.Rune);
        _slotUnite2 = new UiSlot(new Vector2(405, 414), stoneSlot, _game, ItemType.Rune);
        _slotUniteResult = new UiSlot(new Vector2(273, 639), stoneSlot, _game, ItemType.Rune);
        _pgUnite = new UiProgressBar(
            content.Load<Texture2D>("textures/altar_screen/unite_pg"),
            content.Load<Texture2D>("textures/altar_screen/unite_pg_full"),
            UiProgressBar.ProgressDirection.ToDown, 96, 66, 
            new Vector2(141, 414), 0, 53, 0);
        _timerUnite = new Timer(100, () =>
        {
            _pgUnite.Add(1);
            if (_pgUnite.Value >= _pgUnite.MaxValue)
                FinishUnite();
            else
                _timerUnite.StartAgain();
        });
        
        _buttonStartExtract = new UiButton(buttonStartTexture[0], buttonStartTexture[1], buttonStartTexture[2],
            new Vector2(620, 817), 
            "Start", AllGameItems.Font24Px, new Color(85, 56, 100),
            StartExtract);
        _slotExtract = new UiSlot(new Vector2(678, 321), stoneSlot, _game, ItemType.Rune);
        _slotExtractResult = new UiSlot(new Vector2(678, 669), slotEssenceTexture, _game, ItemType.Essence);
        _pgExtract = new UiProgressBar(content.Load<Texture2D>("textures/altar_screen/extraction_pg"),
            content.Load<Texture2D>("textures/altar_screen/extraction_pg_full"),
            UiProgressBar.ProgressDirection.ToDown, 96, 96, 
            new Vector2(678, 321), 0, 84, 0);
        _timerExtract = new Timer(100, () =>
        {
            _pgExtract.Add(1);
            if (_pgExtract.Value >= _pgExtract.MaxValue)
                FinishExtract();
            else
                _timerExtract.StartAgain();
        });

        _buttonStartAmplification = new UiButton(buttonStartTexture[0], buttonStartTexture[1], buttonStartTexture[2],
            new Vector2(1025, 817), 
            "Start", AllGameItems.Font24Px, new Color(85, 56, 100),
            MakeAmplification);
        _slotsAmplification = new[]
        {
            new UiSlot(new Vector2(1082, 321), slotEssenceTexture, _game, ItemType.Essence),
            new UiSlot(new Vector2(1229, 669), slotEssenceTexture, _game, ItemType.Essence),
            new UiSlot(new Vector2(935, 669), slotEssenceTexture, _game, ItemType.Essence)
        };
        _slotAmplificationResult = new UiSlot(new Vector2(1082, 525), stoneSlot, _game, ItemType.Rune);
        
        _buttonGoBack = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            "Back", AllGameItems.Font30Px, new Color(212, 165, 140),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 18)
                {
                    _game.Introduction.Step = 19;
                    _slotUniteResult.Clear();
                }
                _game.SetScreen(GameScreen.MainHouseScreen);
            } );

        var questionButtonTextures = new[]
        {
            content.Load<Texture2D>("textures/buttons/question_button_default"),
            content.Load<Texture2D>("textures/buttons/question_button_hovered"),
            content.Load<Texture2D>("textures/buttons/question_button_pressed")
        };
        _buttonClosePopUp = new UiButton(content.Load<Texture2D>("textures/buttons/close_button_default"),
            content.Load<Texture2D>("textures/buttons/close_button_hovered"),
            content.Load<Texture2D>("textures/buttons/close_button_pressed"),
            new Vector2(1110, 273), () => _showPopUpType = HelperPopUp.None);
        
        _buttonQuestionAmplification = new UiButton(questionButtonTextures[0],
            questionButtonTextures[1], questionButtonTextures[2],
            new Vector2(1107, 263), () =>
            {
                _showPopUpType = HelperPopUp.Amplification;
                _buttonClosePopUp.SetPosition(new Vector2(1158, 217));
            });
        _buttonQuestionExtract = new UiButton(questionButtonTextures[0],
            questionButtonTextures[1], questionButtonTextures[2],
            new Vector2(702, 263), () =>
            {
                _showPopUpType = HelperPopUp.Extract;
                _buttonClosePopUp.SetPosition(new Vector2(1110, 273));
            });
        _buttonQuestionUnite = new UiButton(questionButtonTextures[0],
            questionButtonTextures[1], questionButtonTextures[2],
            new Vector2(297, 263), () =>
            {
                _showPopUpType = HelperPopUp.Unite;
                _buttonClosePopUp.SetPosition(new Vector2(1110, 273));
            });
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var slotsForDrop = new UiSlot[]
                { _slotExtract, _slotUnite1,_slotUnite2, _slotAmplificationResult }
            .Concat(_slotsAmplification)
            .ToArray();
        if (_game.Introduction.IsPlaying)
        {
            _game.Inventory.Update(graphics, _slotUnite1, _slotUnite2);
        }
        else
        {
            _game.Inventory.Update(graphics, slotsForDrop);
        }
        
        UpdateInBackground();
        
        var mouseState = Mouse.GetState();
        
        if (_showPopUpType != HelperPopUp.None)
        {
            _buttonClosePopUp.Update(mouseState, ref _isButtonFocused);
            return;
        }
        
        _buttonQuestionAmplification.Update(mouseState, ref _isButtonFocused);
        _buttonQuestionExtract.Update(mouseState, ref _isButtonFocused);
        _buttonQuestionUnite.Update(mouseState, ref _isButtonFocused);
        
        var slotsToUpdate = new UiSlot[]
                { _slotExtract, _slotUnite1,_slotUnite2, _slotAmplificationResult, _slotUniteResult, _slotExtractResult}
            .Concat(_slotsAmplification)
            .ToArray();
        foreach (var slot in slotsToUpdate) slot.Update(_game.Inventory);

        if (!IsExtractInProgress && _slotExtract.ContainsItem() && !_slotExtractResult.ContainsItem())
            _buttonStartExtract.Update(mouseState, ref _isButtonFocused);
        
        if (!IsUniteInProgress && _slotUnite1.ContainsItem() && _slotUnite2.ContainsItem() &&
            _slotUnite1.currentItem.ID == _slotUnite2.currentItem.ID && !_slotUniteResult.ContainsItem())
        {
            _buttonStartUnite.Update(mouseState, ref _isButtonFocused);
            if (_game.Introduction.IsPlaying && _game.Introduction.Step == 16)
            {
                _game.Introduction.Step = 17;
            }
        }
        if (_slotsAmplification.All(s => s.ContainsItem()) &&
            _slotAmplificationResult.ContainsItem())
        {
            var power = _slotAmplificationResult.currentItem.ID.Split('_')[3][0];
            if (_slotsAmplification.All(s => s.currentItem.ID[^1] == power))
            {
                _buttonStartAmplification.Update(mouseState, ref _isButtonFocused);
            }
        }

        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 18)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        else if (!_game.Introduction.IsPlaying)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
    }

    public void UpdateInBackground()
    {
        if (IsExtractInProgress) _timerExtract.Tick();
        if (IsUniteInProgress) _timerUnite.Tick();
    }

    private void MakeAmplification()
    {
        foreach (var inSlot in _slotsAmplification) inSlot.Clear();
        var oldId = _slotAmplificationResult.currentItem.ID.Split('_');
        
        var newId = $"rune_finished_{oldId[2]}_{int.Parse(oldId[3]) + 1}_{oldId[4]}";

        var newInfo = default(ItemInfo);
        if (oldId[3] != "3") newInfo = ItemsDataHolder.Runes.FinishedRunes[newId];
        else
        {
            var possibleVariants = ItemsDataHolder.Runes.FinishedRunes
                .Where(p => p.Key.Split('_')[3] == "3")
                .Select(p => p.Value)
                .ToList();
            newInfo = possibleVariants[Random.Shared.Next(possibleVariants.Count)];
        }
        _slotAmplificationResult.SetItem(new Item(newInfo));
        _game.SubtractEnergy(2f);
    }

    private void StartUnite()
    {
        var power = int.Parse(_slotUnite1.currentItem.ID.Split('_')[3]);
        _timerExtract.SetDefaultTime(100*power);
        _pgUnite.SetValue(0);
        _timerUnite.StartAgain();
        _slotUnite1.Lock();
        _slotUnite2.Lock();
        IsUniteInProgress = true;
    }

    private void FinishUnite()
    {
        IsUniteInProgress = false;
        _pgUnite.SetValue(0);
        _timerUnite.Stop();
        
        _slotUnite1.Unlock();
        _slotUnite2.Unlock();
        var mainId = _slotUnite1.currentItem.ID.Split('_');
        var newType = mainId[4][0] == '1' ? "2" : "1";
        var resultRuneId = $"rune_finished_{mainId[2]}_{mainId[3]}_{newType}";
        _slotUniteResult.SetItem(new Item(ItemsDataHolder.Runes.FinishedRunes[resultRuneId]));
        
        _slotUnite1.Clear();
        _slotUnite2.Clear();
    }

    private void StartExtract()
    {
        var power = int.Parse(_slotExtract.currentItem.ID.Split('_')[3]);
        _timerExtract.SetDefaultTime(50*power);
        _pgExtract.SetValue(0);
        _timerExtract.StartAgain();
        _slotExtract.Lock();
        IsExtractInProgress = true;
    }

    private void FinishExtract()
    {
        IsExtractInProgress = false;
        _pgExtract.SetValue(0);
        _timerExtract.Stop();
        _slotExtract.Unlock();
        var power = int.Parse(_slotExtract.currentItem.ID.Split('_')[3]);
        _slotExtractResult.SetItem(new Item(ItemsDataHolder.PowerEssences.GetEssenceInfo(power)));
        _slotExtract.Clear();
    }
    
    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_backPanelTexture, new Vector2(57, 172)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_amplificationGui, new Vector2(890, 312)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _pgUnite.Draw(spriteBatch);
        _pgExtract.Draw(spriteBatch);
        _buttonGoBack.Draw(spriteBatch);
        _buttonQuestionAmplification.Draw(spriteBatch);
        _buttonQuestionExtract.Draw(spriteBatch);
        _buttonQuestionUnite.Draw(spriteBatch);
        var slotsToDraw = new UiSlot[]
                { _slotExtract, _slotUnite1,_slotUnite2, _slotAmplificationResult, _slotUniteResult, _slotExtractResult}
            .Concat(_slotsAmplification)
            .ToArray();
        foreach (var slot in slotsToDraw) slot.Draw(spriteBatch);
        
        if (!IsExtractInProgress && _slotExtract.ContainsItem() && !_slotExtractResult.ContainsItem())
            _buttonStartExtract.Draw(spriteBatch);
        if (!IsUniteInProgress && _slotUnite1.ContainsItem() && _slotUnite2.ContainsItem() &&
            _slotUnite1.currentItem.ID == _slotUnite2.currentItem.ID && !_slotUniteResult.ContainsItem())
            _buttonStartUnite.Draw(spriteBatch);
        
        if (_slotsAmplification.All(s => s.ContainsItem()) &&
            _slotAmplificationResult.ContainsItem())
        {
            var power = _slotAmplificationResult.currentItem.ID.Split('_')[3][0];
            if (_slotsAmplification.All(s => s.currentItem.ID[^1] == power))
            {
                _buttonStartAmplification.Draw(spriteBatch);
            }
        }
        
        DrawStrings(spriteBatch);
        
        if (_showPopUpType != HelperPopUp.None)
        {
            _buttonClosePopUp.Draw(spriteBatch);
            var texture = Game1.CurrentLanguage == Language.Russian ? 
                _popUpsTextures[_showPopUpType].TextureRu : _popUpsTextures[_showPopUpType].TextureEn;
            
            spriteBatch.Draw(texture,
                new Vector2(
                    (_game.ScreenWidth - (462 + texture.Width) * Game1.ResolutionScale.X) / 2,
                    (_game.ScreenHeight - texture.Height * Game1.ResolutionScale.Y) / 2),
                null, Color.White, 0f, Vector2.Zero, 
                Game1.ResolutionScale, SpriteEffects.None, 0f);
            _buttonClosePopUp.Draw(spriteBatch);
        }
        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawStrings(SpriteBatch spriteBatch)
    {
        var strColor = new Color(10 , 10, 10);
        var str = Game1.GetText("Объединение рун");
        var strSize = AllGameItems.Font30Px.MeasureString(str);
        spriteBatch.DrawString(AllGameItems.Font30Px, str, new Vector2(321-strSize.X/2, 227)*Game1.ResolutionScale, strColor,
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        str = Game1.GetText("Извлечение эссенции").Split("\\n")[0];
        strSize = AllGameItems.Font30Px.MeasureString(str);
        spriteBatch.DrawString(AllGameItems.Font30Px, str, new Vector2(726-strSize.X/2, 230 - strSize.Y)*Game1.ResolutionScale, strColor,
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (Game1.GetText("Извлечение эссенции").Split("\\n").Length > 1)
        {
            var str2 = Game1.GetText("Извлечение эссенции").Split("\\n")[1];
            strSize = AllGameItems.Font30Px.MeasureString(str2);
            spriteBatch.DrawString(AllGameItems.Font30Px, str2, new Vector2(726-strSize.X/2, 227)*Game1.ResolutionScale, strColor,
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
        
        str = Game1.GetText("Усиление руны").Split("\\n")[0];
        strSize = AllGameItems.Font30Px.MeasureString(str);
        var y = Game1.GetText("Усиление руны").Split("\\n").Length > 1 ? 230 - strSize.Y : 227;
        spriteBatch.DrawString(AllGameItems.Font30Px, str, new Vector2(1131-strSize.X/2, y)*Game1.ResolutionScale, strColor,
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (Game1.GetText("Усиление руны").Split("\\n").Length > 1)
        {
            var str2 = Game1.GetText("Усиление руны").Split("\\n")[1];
            strSize = AllGameItems.Font30Px.MeasureString(str2);
            spriteBatch.DrawString(AllGameItems.Font30Px, str2, new Vector2(1131-strSize.X/2, 227)*Game1.ResolutionScale, strColor,
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
    }

    public void SleepSkipCrafting()
    {
        if (IsUniteInProgress)
        {
            FinishUnite();
        }
        if (IsExtractInProgress)
        {
            FinishExtract();
        }
    }
}