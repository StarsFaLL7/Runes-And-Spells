using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.classes;

public class UiProgressBar
{
    private Vector2 _position;
    public float MinValue { get; }
    public float MaxValue { get; }
    public float Value { get; private set; }
    private readonly Texture2D _backTexture;
    private readonly Texture2D _progressTexture;
    private ProgressDirection _direction;
    private TextureGap _textureGap;
    private record TextureGap(int GapBefore, int GapAfter);
    public enum ProgressDirection
    {
        ToRight,
        ToLeft,
        ToDown,
        ToTop
    }

    public UiProgressBar(Texture2D backTexture, Texture2D progressTexture, ProgressDirection Direction, 
        int textureBeforeGap, int textureAfterGap, Vector2 position, float minValue, float maxValue, float defaultValue)
    {
        if (minValue > maxValue || defaultValue < minValue || defaultValue > maxValue || minValue < 0 || maxValue < 0 || defaultValue < 0)
            throw new ArgumentException("Wrong parameters for progress bar.");
        _backTexture = backTexture;
        _progressTexture = progressTexture;
        _position = position;
        MinValue = minValue;
        MaxValue = maxValue;
        Value = defaultValue;
        _textureGap = new TextureGap(textureBeforeGap, textureAfterGap);
        _direction = Direction;
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backTexture, _position*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);

        var realSize = _progressTexture.Width - _textureGap.GapAfter - _textureGap.GapBefore;
        switch (_direction)
        {
            
            case ProgressDirection.ToRight:
                spriteBatch.Draw(_progressTexture, 
                    new Vector2((_position.X + _textureGap.GapBefore)*Game1.ResolutionScale.X, _position.Y*Game1.ResolutionScale.Y),
                    new Rectangle(
                        _textureGap.GapBefore, 
                        0, 
                        (int)(realSize * Value / MaxValue), 
                        _progressTexture.Height),
                    Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                break;
            case ProgressDirection.ToDown:
                realSize = _progressTexture.Height - _textureGap.GapAfter - _textureGap.GapBefore;
                spriteBatch.Draw(_progressTexture, 
                    new Vector2(_position.X*Game1.ResolutionScale.X, (_position.Y + _textureGap.GapBefore)*Game1.ResolutionScale.Y),
                    new Rectangle(
                        0, 
                        _textureGap.GapBefore, 
                        _progressTexture.Width, 
                        (int)(realSize * Value / MaxValue)),
                    Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                break;
            case ProgressDirection.ToLeft:
                spriteBatch.Draw(_progressTexture, 
                    new Vector2((_position.X + _textureGap.GapBefore)*Game1.ResolutionScale.X, _position.Y*Game1.ResolutionScale.Y),
                    new Rectangle(
                        _textureGap.GapBefore+_progressTexture.Width-(int)(realSize * Value / MaxValue), 
                        0, 
                        (int)(realSize * Value / MaxValue), 
                        _progressTexture.Height),
                    Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                break;
            case ProgressDirection.ToTop:
                realSize = _progressTexture.Height - _textureGap.GapAfter - _textureGap.GapBefore;
                spriteBatch.Draw(_progressTexture, 
                    new Vector2(_position.X*Game1.ResolutionScale.X, (_position.Y + _textureGap.GapBefore)*Game1.ResolutionScale.Y),
                    new Rectangle(
                        0, 
                        _textureGap.GapBefore+_progressTexture.Height-(int)(realSize * Value / MaxValue), 
                        _progressTexture.Width, 
                        (int)(realSize * Value / MaxValue)),
                    Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                break;
        }
            
    }

    public void SetValue(float newValue)
    {
        Value = Math.Max(Math.Min(MaxValue, newValue), MinValue);
    }

    public void Add(float addition)
    {
        if (addition < 0) throw new ArgumentOutOfRangeException(nameof(addition));
        Value = Math.Min(Value + addition, MaxValue);
    }

    public void Subtract(float subtraction)
    {
        if (subtraction < 0) throw new ArgumentOutOfRangeException(nameof(subtraction));
        Value = Math.Max(Value - subtraction, MinValue);
    }

    public void SetPosition(Vector2 newPosition)
    {
        _position = newPosition;
    }
}