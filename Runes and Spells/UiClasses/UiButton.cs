using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.UiClasses;

public class UiButton
{
    private readonly Texture2D _defaultTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    
    public string TextResName { get; private set; }
    public Color TextColor { get; private set; }
    public SpriteFont Font { get; private set; }
    public Rectangle Rectangle { get; private set; }
    public Vector2 Position { get; private set; }
    private readonly Action _action;
    private bool IsHovered { get; set; }
    private bool IsPressed { get; set; }

    public UiButton(Texture2D defaultTexture, Texture2D hoveredTexture, Texture2D pressedTexture, Vector2 position, Action action)
    {
        _defaultTexture = defaultTexture;
        _hoveredTexture = hoveredTexture;
        _pressedTexture = pressedTexture;
        Position = position;
        Rectangle = new Rectangle((int)position.X, (int)position.Y, _defaultTexture.Width, _defaultTexture.Height);
        _action = action;
    }
    
    public UiButton(Texture2D defaultTexture, Texture2D hoveredTexture, Texture2D pressedTexture, Vector2 position, 
        string textResName, SpriteFont font, Color textColor, Action action)
    {
        _defaultTexture = defaultTexture;
        _hoveredTexture = hoveredTexture;
        _pressedTexture = pressedTexture;
        Position = position;
        Rectangle = new Rectangle((int)position.X, (int)position.Y, _defaultTexture.Width, _defaultTexture.Height);
        _action = action;
        TextResName = textResName;
        TextColor = textColor;
        Font = font;
    }

    public void Update(MouseState mouseState,ref bool isAnotherObjectFocused)
    {
        Rectangle = new Rectangle((int)(Position.X * Game1.ResolutionScale.X), (int)(Position.Y * Game1.ResolutionScale.Y), 
            (int)(_defaultTexture.Width * Game1.ResolutionScale.X), (int)(_defaultTexture.Height * Game1.ResolutionScale.X));
        if (mouseState.LeftButton == ButtonState.Released) isAnotherObjectFocused = false;
        
        if (IsPressed && Rectangle.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Released)
        {
            _action();
            ResetStates();
        }
            
        if (!isAnotherObjectFocused)
            IsPressed = false;
        if (isAnotherObjectFocused && IsPressed)
            return;
            
        if (Rectangle.Contains(mouseState.X, mouseState.Y) && !isAnotherObjectFocused)
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
    
    public void SetPosition(Vector2 position)
    {
        Position = position;
        Rectangle = new Rectangle((int)position.X, (int)position.Y, _defaultTexture.Width, _defaultTexture.Height);
    }

    public Texture2D ActualTexture()
    {
        if (IsPressed)
            return _pressedTexture;
        return IsHovered ? _hoveredTexture : _defaultTexture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ActualTexture(), Position*Game1.ResolutionScale, null, Color.White, 0f, Vector2.Zero, 
            Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (TextResName is null or "") return;
        var text = Game1.ResManager.GetString(TextResName);
        
        var txtSize = Font.MeasureString(text);
        spriteBatch.DrawString(Font, text, 
            new Vector2(
                Position.X + (ActualTexture().Width - txtSize.X)/2,
                Position.Y + (ActualTexture().Height - txtSize.Y*0.8f)/2
                )*Game1.ResolutionScale, 
            TextColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }
}