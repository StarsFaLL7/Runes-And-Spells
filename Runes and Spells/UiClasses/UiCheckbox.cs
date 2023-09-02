using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.UiClasses;

public class UiCheckbox
{
    public enum TextPos
    {
        Left,
        Right
    }
    
    private readonly Game1 _game;
    public Texture2D TextureChecked { get; }
    public Texture2D TextureUnchecked { get; }
    public string RusText { get; }
    public string EngText { get; }
    public Action<bool> Action { get; }
    public SpriteFont Font { get; }
    public Color TextColor { get; }
    public Vector2 Position { get; }
    public bool IsChecked { get; set; }

    private MouseState _lastMouseState;
    private Rectangle _clickRectangle;
    private Vector2 _textSize = Vector2.Zero;
    private readonly TextPos _textPos;

    public UiCheckbox(string rusText, string engText, Color textColor, TextPos textPosition,Texture2D textureChecked, Texture2D textureUnchecked, 
        Vector2 position, Action<bool> action, Game1 game, SpriteFont font)
    {
        _game = game;
        TextureChecked = textureChecked;
        TextureUnchecked = textureUnchecked;
        RusText = rusText;
        EngText = engText;
        TextColor = textColor;
        Action = action;
        Font = font;
        Position = position;
        _clickRectangle =
            new Rectangle((int)Position.X, (int)Position.Y, TextureUnchecked.Width, TextureUnchecked.Height);
        _textPos = textPosition;
    }

    public void Update(MouseState mouseState)
    {
        var text = Game1.CurrentLanguage == Language.Russian ? RusText : EngText;
        if (text is not null && text != "")
            _textSize = Font.MeasureString(text);
        
        _clickRectangle =
            new Rectangle((int)(Position.X*Game1.ResolutionScale.X), (int)(Position.Y*Game1.ResolutionScale.Y),
                (int)(TextureUnchecked.Width*Game1.ResolutionScale.X), (int)(TextureUnchecked.Height*Game1.ResolutionScale.Y));
        
        if (_clickRectangle.Contains(_lastMouseState.Position) && _clickRectangle.Contains(mouseState.Position) &&
            _lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
        {
            IsChecked = !IsChecked;
            Action?.Invoke(IsChecked);
        }

        _lastMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(IsChecked? TextureChecked : TextureUnchecked, 
            new Vector2(Position.X * Game1.ResolutionScale.X, Position.Y*Game1.ResolutionScale.Y), 
            null, Color.White, 0f, Vector2.Zero,
            Game1.ResolutionScale, SpriteEffects.None, 1f);
        var text = Game1.CurrentLanguage == Language.Russian ? RusText : EngText;
        if (text is not null && text != "")
        {
            _textSize = Font.MeasureString(text)*Game1.ResolutionScale;
            if (_textPos == TextPos.Right)
            {
                spriteBatch.DrawString(Font, text, new Vector2((Position.X + TextureChecked.Width + 16)*Game1.ResolutionScale.X, 
                        Position.Y*Game1.ResolutionScale.Y + (TextureChecked.Height*Game1.ResolutionScale.Y - _textSize.Y*0.8f)/2), 
                    TextColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
            else
            {
                var textSize = Font.MeasureString(text);
                spriteBatch.DrawString(Font, text, new Vector2((Position.X - textSize.X - 16)*Game1.ResolutionScale.X, 
                        Position.Y*Game1.ResolutionScale.Y + (TextureChecked.Height*Game1.ResolutionScale.Y - _textSize.Y*0.8f)/2), 
                    TextColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
        }
    }
}