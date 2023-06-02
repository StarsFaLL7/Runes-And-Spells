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
    private Texture2D _backTexture;
    private Texture2D _progressTexture;

    public UiProgressBar(Texture2D backTexture, Texture2D progressTexture, Vector2 position, float minValue, float maxValue, float defaultValue)
    {
        if (minValue > maxValue || defaultValue < minValue || defaultValue > maxValue || minValue < 0 || maxValue < 0 || defaultValue < 0)
            throw new ArgumentException("Wrong parameters for progress bar.");
        _backTexture = backTexture;
        _progressTexture = progressTexture;
        _position = position;
        MinValue = minValue;
        MaxValue = maxValue;
        Value = defaultValue;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backTexture, _position, Color.White);
        spriteBatch.Draw(_progressTexture, _position, new Rectangle(0, 0, (int)(_progressTexture.Width * Value / MaxValue) ,_progressTexture.Height), Color.White);
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
}