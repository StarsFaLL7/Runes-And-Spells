﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.classes;

public class ScrollCraftingMiniGame
{
    public bool IsActive { get; private set; }
    private readonly UiSlot _inputSlot1;
    private readonly UiSlot _inputSlot2;
    private readonly UiSlot _outputSlot;
    private UiButton _buttonCancelCraft;
    private UiButton _buttonFinishCraft;
    private string[] _generatedWords;
    private string _generatedString;
    private Writer _writer;
    private StringBuilder _enteredText;
    private bool _isAnyKeyPressed;
    private bool _wasAnyKeyPressed;
    private Timer _backSpaceTimer;
    private bool _isBackSpaceReady = true;
    private Texture2D _writingListTexture;
    private Vector2 _writingListPosition;
    private int _textDrawCharIndex;
    private bool _firstDrawFinished;
    private Timer _firstDrawTimer;

    private Dictionary<string, string> _wordsSpecial = new()
    {
        {"water", "voco aquam"},
        {"ice", "voco glacies"},
        {"grass", "terra auxilo"},
        {"fire", "potestas ignis"},
        {"blood", "sanguis magicae"},
        {"distorted", "corrupto veniam"},
        {"black", "auxo tenebrae"},
        {"air", "aer liberate"},
        {"moon", "lunae magicae"}
    };
    
    private string[] _wordsOther = new[]
    {
        "potento", "librum", "mensa", 
        "runae", "villae", "habito", "charta", "dryma", "essentia", "epistol", "verba", 
        "sente", "numeri", "facino", "colinu", "imponu", "adune",
        "igunt", "res", "sub", "sentam", "opsum"
    };

    public ScrollCraftingMiniGame(UiSlot outputSlot, UiSlot inputSlot1, UiSlot inputSlot2)
    {
        _outputSlot = outputSlot;
        _inputSlot1 = inputSlot1;
        _inputSlot2 = inputSlot2;
    }

    public void Initialize(ContentManager content)
    {
        _writingListTexture = content.Load<Texture2D>("textures/scroll_crafting_table/writing_list_back");
        _writingListPosition = new Vector2(276, 784);
        _buttonFinishCraft = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_pressed"),
            new Vector2(1193, 362),
            Stop);
        _buttonCancelCraft = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_pressed"),
            new Vector2(1193, 264),
            Reset);
        _writer = new Writer(content);
        _enteredText = new StringBuilder();
        _backSpaceTimer = new Timer(200, () => _isBackSpaceReady = true);
        _firstDrawTimer = new Timer(200, () =>
        {
            if (_textDrawCharIndex < _generatedString.Length)
            {
                _textDrawCharIndex++;
                _firstDrawTimer.StartAgain();
            }
            else
                _firstDrawFinished = true;
        });
    }
    
    public void Start()
    {
        IsActive = true;
        _inputSlot1.Lock();
        _inputSlot2.Lock();
        _generatedWords = GenerateWords();
        _firstDrawTimer.Start();
    }

    public void Update(MouseState mouseState, ref bool isButtonFocused)
    {
        if (!IsActive) return;
        _buttonCancelCraft.Update(mouseState, ref isButtonFocused);

        if (CheckEnteredText())
            _buttonFinishCraft.Update(mouseState, ref isButtonFocused);
        
        InputTextUpdate();
    }

    private void InputTextUpdate()
    {
        var kb = Keyboard.GetState();
        _wasAnyKeyPressed = _isAnyKeyPressed;
        _isAnyKeyPressed = kb.GetPressedKeyCount() > 0;
        _backSpaceTimer.Tick();
        if (!_wasAnyKeyPressed && _isAnyKeyPressed || kb.IsKeyDown(Keys.Back))
        {
            var pressedKey = kb.GetPressedKeys()
                .FirstOrDefault(k => k is >= Keys.A and <= Keys.Z or Keys.Back or Keys.Space);
            if (pressedKey is not default(Keys))
            {
                switch (pressedKey)
                {
                    case Keys.Back:
                        if (_enteredText.Length > 0 && _isBackSpaceReady)
                        {
                            _enteredText.Remove(_enteredText.Length - 1, 1);
                            _isBackSpaceReady = false;
                            _backSpaceTimer.StartAgain();
                        }
                        break;
                    case Keys.Space:
                        _enteredText.Append(' ');
                        break;
                    default:
                        _enteredText.Append(pressedKey.ToString().ToLower()[0]);
                        break;
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        spriteBatch.Draw(_writingListTexture, _writingListPosition, Color.White);
        _buttonCancelCraft.Draw(spriteBatch);
        if (CheckEnteredText())
        {
            _buttonFinishCraft.Draw(spriteBatch);
        }

        if (!_firstDrawFinished)
        {
            _firstDrawTimer.Tick();
            _writer.DrawWords(_generatedString[.._textDrawCharIndex].Split(), new Rectangle(292, 465,1082, 128), spriteBatch, Color.Magenta);
        }
        else
            _writer.DrawWords(_generatedWords, new Rectangle(292, 465,1082, 128), spriteBatch, Color.White);
        _writer.DrawWords(_enteredText.ToString().Split(), new Rectangle(292, 790,1082, 128), spriteBatch, Color.White);
    }
    
    private string[] GenerateWords()
    {
        var result = new List<string>(_wordsSpecial[_inputSlot1.currentItem.ID.Split('_')[2]].Split());
        AddRandomWords(result, 1);
        result = result
            .Concat(_wordsSpecial[_inputSlot2.currentItem.ID.Split('_')[2]].Split())
            .ToList();
        AddRandomWords(result, 1);
        _generatedString = String.Join(" ", result);
        return result.ToArray();
    }

    private void AddRandomWords(List<string> list, int count)
    {
        for (var i = 0; i < count; i++)
            list.Add(_wordsOther[Random.Shared.Next(_wordsOther.Length)]);
    }

    private bool CheckEnteredText()
    {
        var enteredWords = _enteredText.ToString().Split().Where(w => w != "").ToArray();
        if (enteredWords.Length == _generatedWords.Length)
        {
            var similar = _generatedWords.Where((t, i) => enteredWords.Length >= i && t == enteredWords[i]).Count();
            if (similar == _generatedWords.Length)
                return true;
        }
        return false;
    }

    private void Stop()
    {
        IsActive = false;
        if (AllGameItems.TryToGetScrollByRunes(out var scroll, _inputSlot1.currentItem, _inputSlot2.currentItem))
        {
            _outputSlot.SetItem(scroll);
            _inputSlot1.Clear();
            _inputSlot2.Clear();
        }
        Reset();
    }
    
    private void Reset()
    {
        IsActive = false;
        _inputSlot1.Unlock();
        _inputSlot2.Unlock();
        _enteredText.Clear();
        _textDrawCharIndex = 0;
        _firstDrawFinished = false;
    }
    
}