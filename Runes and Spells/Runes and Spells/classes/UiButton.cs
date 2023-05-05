using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells;

public class UiButton
{
    private readonly Texture2D _defaultTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    private Rectangle _rectangle;
    public Vector2 Position { get; private set; }
    private readonly Action _action;
    
    public bool IsHovered { get; set; }
    public bool IsPressed { get; set; }

    public UiButton(Texture2D defaultTexture, Texture2D hoveredTexture, Texture2D pressedTexture, Vector2 position, Action action)
    {
        _defaultTexture = defaultTexture;
        _hoveredTexture = hoveredTexture;
        _pressedTexture = pressedTexture;
        Position = position;
        _rectangle = new Rectangle((int)position.X, (int)position.Y, _defaultTexture.Width, _defaultTexture.Height);
        _action = action;
    }

    public void Update(MouseState mouseState,ref bool isAnotherObjectFocused)
    {
        if (mouseState.LeftButton == ButtonState.Released) isAnotherObjectFocused = false;
        
        if (IsPressed && _rectangle.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Released)
        {
            _action();
            ResetStates();
        }
            
        if (!isAnotherObjectFocused)
            IsPressed = false;
        if (isAnotherObjectFocused && IsPressed)
            return;
            
        if (_rectangle.Contains(mouseState.X, mouseState.Y) && !isAnotherObjectFocused)
        {
            IsHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsPressed = true;
                isAnotherObjectFocused = true;
            }
        }
        else
        {
            IsHovered = false;
        }
    }

    private void ResetStates()
    {
        IsHovered = false;
        IsPressed = false;
    }
    /*
    public void SetPosition(Vector2 position)
    {
        Position = position;
        _rectangle = new Rectangle((int)position.X, (int)position.Y, _defaultTexture.Width, _defaultTexture.Height);
    }*/

    public Texture2D ActualTexture()
    {
        if (IsPressed)
            return _pressedTexture;
        
        return IsHovered ? _hoveredTexture : _defaultTexture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ActualTexture(), Position, Color.White);
    }
}