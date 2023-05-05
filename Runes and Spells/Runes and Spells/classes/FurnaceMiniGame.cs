using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.classes;

public class FurnaceMiniGame
{
    private class GuiButton
    {
        public GuiButton(Vector2 position, Texture2D defaultTexture, Texture2D pressedTexture)
        {
            _position = position;
            _defaultTexture = defaultTexture;
            _pressedTexture = pressedTexture;
        }

        private readonly Vector2 _position;
        private readonly Texture2D _defaultTexture;
        private readonly Texture2D _pressedTexture;
        public bool IsPressed { get; set; }

        private Texture2D ActualTexture() => IsPressed ? _pressedTexture : _defaultTexture;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ActualTexture(), _position, Color.White);
        }
    }

    public bool IsActive { get; set; }
    private readonly UiSlot _inputSlot;
    private readonly UiProgressBar _progressBar;
    private readonly float _minPosition;
    private readonly float _maxPosition;
    private readonly Vector2 _position;
    private readonly Texture2D _pointerTexture;
    private readonly Texture2D _leftAreaEndTexture;
    private readonly Texture2D _rightAreaEndTexture;
    private readonly Texture2D _fullAreaTexture;
    private readonly Texture2D _backTexture;
    
    private List<(int start, int end, int width)> _successAreas;
    private Vector2 _pointerPosition;
    private bool _isMovingRight;
    private int _difficult;
    private bool _wasSpacePressed;
    private bool _isSpacePressed;
    private readonly Timer _clickTimer;
    private readonly GuiButton _spaceButton;
    
    public FurnaceMiniGame(UiProgressBar progressBar, UiSlot inputSlot, Vector2 position, ContentManager content)
    {
        _progressBar = progressBar;
        _inputSlot = inputSlot;
        _position = position;
        _pointerTexture = content.Load<Texture2D>("textures/furnace_screen/mini_game/arrow_indicator");
        _successAreas = new List<(int start, int end, int width)>();
        _backTexture = content.Load<Texture2D>("textures/furnace_screen/mini_game/bar_bg");
        _fullAreaTexture = content.Load<Texture2D>("textures/furnace_screen/mini_game/bar_full");
        _rightAreaEndTexture = content.Load<Texture2D>("textures/furnace_screen/mini_game/area_right_end");
        _leftAreaEndTexture = content.Load<Texture2D>("textures/furnace_screen/mini_game/area_left_end");
        _minPosition = _position.X;
        _maxPosition = _minPosition + _backTexture.Width;
        _pointerPosition = new Vector2((_minPosition + _maxPosition) / 2, _position.Y + _backTexture.Height + 4);
        _spaceButton = new GuiButton(new Vector2(840, 777), 
            content.Load<Texture2D>("textures/furnace_screen/mini_game/spacebar_default"), 
            content.Load<Texture2D>("textures/furnace_screen/mini_game/spacebar_pressed"));
        _clickTimer = new Timer(100, () => 
        {
            _pointerPosition.Y += 12;
            _spaceButton.IsPressed = false;
        });
    }

    public void Update()
    {
        if (!IsActive) return;
        if (_pointerPosition.X >= _maxPosition - _pointerTexture.Width/2) _isMovingRight = false;
        if (_pointerPosition.X <= _minPosition - _pointerTexture.Width / 2) _isMovingRight = true;

        if (_progressBar.Value >= _progressBar.MaxValue) Stop(true);
        if (_progressBar.Value <= _progressBar.MinValue) Stop(false);
        if (_clickTimer.IsRunning)
        {
            _clickTimer.Tick();
            return;
        }
        
        _progressBar.Subtract((float)(_difficult+3)/8);
        _wasSpacePressed = _isSpacePressed;
        _isSpacePressed = Keyboard.GetState()[Keys.Space] == KeyState.Down;
        var spaceClicked = !_wasSpacePressed && _isSpacePressed;
        if (_isMovingRight && !spaceClicked) _pointerPosition.X += 2 + (float)_difficult/2;
        else if (!spaceClicked) _pointerPosition.X -= 2 + (float)_difficult / 2;
        else
        {
            _spaceButton.IsPressed = true;
            _pointerPosition.Y -= 12;
            _clickTimer.StartAgain();
            var selectedArea = _successAreas
                    .FirstOrDefault(a => 
                        a.start <= _pointerPosition.X + _pointerTexture.Width/2 && 
                        a.end >= _pointerPosition.X + _pointerTexture.Width/2);
            if (selectedArea.width != 0) 
            {
                _progressBar.Add((_progressBar.MaxValue - _progressBar.MinValue) / (6 + _difficult*2));
                _successAreas.Remove(selectedArea);
                while(_successAreas.Count < 3)
                    if (TryToGenerateArea(out var newArea, _successAreas)) _successAreas.Add(newArea);
            }
            else
            {
                _progressBar.Subtract((_progressBar.MaxValue - _progressBar.MinValue) / (14 - _difficult*2));
            }
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        spriteBatch.Draw(_backTexture, _position, Color.White);
        foreach (var area in _successAreas)
        {
            spriteBatch.Draw(_leftAreaEndTexture, new Vector2(area.start, _position.Y), Color.White);
            spriteBatch.Draw(_fullAreaTexture, new Vector2(area.start, _position.Y), 
                new Rectangle(0,0, area.width - _rightAreaEndTexture.Width, _fullAreaTexture.Height), Color.White);
            spriteBatch.Draw(_rightAreaEndTexture, new Vector2(area.end - _rightAreaEndTexture.Width, _position.Y), Color.White);
        }

        _spaceButton.Draw(spriteBatch);
        spriteBatch.Draw(_pointerTexture, _pointerPosition, Color.White);
    }

    private List<(int start, int end, int width)> GenerateAreas()
    {
        var result = new List<(int start, int end, int width)>();
        while (result.Count < 3)
        {
            if (TryToGenerateArea(out var newArea, result))
            {
                result.Add(newArea);
            }
        }
        return result;
    }

    private bool TryToGenerateArea(out (int start, int end, int width) area, List<(int start, int end, int width)> allAreas)
    {
        var start = Random.Shared.Next((int)_minPosition, (int)_maxPosition - 300 + _difficult * 50);
        var end = (int)Math.Min(start + Random.Shared.Next(105 - _difficult * 15, 300 - _difficult * 50), _maxPosition);
        if (!allAreas.Any(a => a.Item1 <= end && a.Item2 >= start))
        {
            area = (start, end, end - start);
            return true;
        }
        area = default;
        return false;
    }
    
    public void Start(int difficult)
    {
        _difficult = difficult;
        _successAreas = GenerateAreas();
        IsActive = true;
        _inputSlot.Lock();
    }

    private void Stop(bool win)
    {
        if (!win)
        {
            _inputSlot.Clear();
            Reset();
            return;
        }
        if (!_inputSlot.currentItem.ID.Contains("failed"))
        {
            AllGameItems.SetRecipeFull(_inputSlot.currentItem.ID);
            var newId = _inputSlot.currentItem.ID.Replace("unknown", "finished");
            var newItem = AllGameItems.FinishedRunes[newId];
            _inputSlot.SetItem(new Item(newItem.Type, newItem.Texture, newId, newItem.IsDraggable));
        }
        else
        {
            var newItem = AllGameItems.ClaySmall;
            _inputSlot.SetItem(new Item(newItem.Type, newItem.Texture, newItem.ID, newItem.isDraggable));
        }
        Reset();
    }

    private void Reset()
    {
        IsActive = false;
        _progressBar.SetValue((_progressBar.MaxValue + _progressBar.MinValue)*0.4f);
        _inputSlot.Unlock();
    }
}