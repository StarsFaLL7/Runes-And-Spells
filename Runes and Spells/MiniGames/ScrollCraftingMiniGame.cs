using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

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
    private KeyboardState _lastKbState;
    private Timer _backSpaceTimer;
    private bool _isBackSpaceReady = true;
    private Texture2D _writingListTexture;
    private Vector2 _writingListPosition;
    private int _textDrawCharIndex;
    private bool _firstDrawFinished;
    private Timer _firstDrawTimer;
    private Game1 _game;
    
    
    private Dictionary<string, string> _wordsSpecial = new()
    {
        {"water", "aquam"},
        {"ice", "glace"},
        {"grass", "terra"},
        {"fire", "ignis"},
        {"blood", "sangus"},
        {"distorted", "corrupto"},
        {"black", "tenebrae"},
        {"air", "aer"},
        {"moon", "lunae"}
    };
    
    private string[] _wordsOther = new[]
    {
        "potento", "librum", "mensa", 
        "runae", "villae", "habito", "charta", "dryma", "essentia", "epistol", "verba", 
        "sente", "numeri", "facino", "colinu", "imponu", "adune",
        "igunt", "res", "sub", "sentam", "opsum", "mensa", "pluma", "atrox",
        "drake", "timeo"
    };
    
    private SoundEffect _soundScrollCompleted;
    private SoundEffect _soundScrollFailed;

    public ScrollCraftingMiniGame(UiSlot outputSlot, UiSlot inputSlot1, UiSlot inputSlot2, Game1 game)
    {
        _outputSlot = outputSlot;
        _inputSlot1 = inputSlot1;
        _inputSlot2 = inputSlot2;
        _game = game;
    }

    public void Initialize(ContentManager content)
    {
        _soundScrollFailed = content.Load<SoundEffect>("sounds/scroll_failed");
        _soundScrollCompleted = content.Load<SoundEffect>("sounds/scroll_completed");
        _writingListTexture = content.Load<Texture2D>("textures/scroll_crafting_table/writing_list_back");
        _writingListPosition = new Vector2(276, 784);
        _buttonFinishCraft = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/finish_button_pressed"),
            new Vector2(1193, 362),
            "Finish", AllGameItems.Font24Px, new Color(37, 43, 32),
            Stop);
        _buttonCancelCraft = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/cancel_button_pressed"),
            new Vector2(1193, 264),
            "Cancel", AllGameItems.Font24Px, new Color(68, 28, 28),
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
        _outputSlot.Lock();
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
        var kbState = Keyboard.GetState();
        _isAnyKeyPressed = kbState.GetPressedKeyCount() > 0;
        _backSpaceTimer.Tick();
        var newKeys = kbState.GetPressedKeys()
            .Where(k => _lastKbState.IsKeyUp(k))
            .Where(k => k is >= Keys.A and <= Keys.Z or Keys.Back or Keys.Space);

        foreach (var pressedKey in newKeys)
        {
            if (pressedKey is not default(Keys))
            {
                switch (pressedKey)
                {
                    case Keys.Back:
                        if (_enteredText.Length > 0)
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
                        if (_enteredText.ToString().Split().All(s => s.Length < 24) && _enteredText.ToString().Length < 40)
                            _enteredText.Append(pressedKey.ToString().ToLower()[0]);
                        break;
                }
            }
        }
        _lastKbState = kbState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        spriteBatch.Draw(_writingListTexture, _writingListPosition*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
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
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 21) _game.Introduction.Step = 22;
        IsActive = false;
        if (AllGameItems.TryToGetScrollByRunes(out var scroll, out var scrollInfo, _inputSlot1.currentItem, _inputSlot2.currentItem))
        {
            _outputSlot.SetItem(scroll);
            _inputSlot1.Clear();
            _inputSlot2.Clear();
            _soundScrollCompleted.Play();
            AllGameItems.TryToUnlockScrollRecipe(scrollInfo);
            _game.SubtractEnergy(3f);
        }
        else
        {
            _soundScrollFailed.Play();
        }
        Reset();
    }
    
    private void Reset()
    {
        IsActive = false;
        _inputSlot1.Unlock();
        _inputSlot2.Unlock();
        _outputSlot.Unlock();
        _enteredText.Clear();
        _textDrawCharIndex = 0;
        _firstDrawFinished = false;
    }
    
}