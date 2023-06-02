using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.UiClasses;

public class UiSlider
{
    public readonly Texture2D BackTexture;
    public readonly Texture2D HolderTexture;
    public readonly Vector2 Position;
    private readonly float _minValue;
    private readonly float _maxValue;
    private readonly float _zeroPositionX;
    private readonly float _maxPositionX;
    
    public float Value { get; private set; }
    private Vector2 _holderPosition;
    private bool BeingPressed { get; set; }
    private Rectangle HolderRectangle { get; set; }
    public Vector2 GetHolderPosition() => new Vector2(_holderPosition.X - HolderTexture.Width / 2f, _holderPosition.Y);
    
    public UiSlider(Texture2D backTexture, Texture2D holderTexture, Vector2 position, float textureBordersWidth, float minValue, float maxValue, float defaultValue)
    {
        if (maxValue < minValue)
            throw new ArgumentException("Slider: minValue should be smaller than maxValue.");
        
        Value = defaultValue;
        Position = position;
        HolderTexture = holderTexture;
        BackTexture = backTexture;
        _minValue = minValue;
        _maxValue = maxValue;
        _zeroPositionX = Position.X + textureBordersWidth + holderTexture.Width / 2f;
        _maxPositionX = Position.X + BackTexture.Width - textureBordersWidth - HolderTexture.Width / 2f;
        _holderPosition = new Vector2(_zeroPositionX + Value / (maxValue - minValue) * (_maxPositionX - _zeroPositionX), position.Y);
        HolderRectangle = new Rectangle((int)_holderPosition.X, (int)_holderPosition.Y, HolderTexture.Width, HolderTexture.Height);
    }

    private void MoveToMouse(MouseState mouseState)
    {
        if (mouseState.X > _maxPositionX)
            _holderPosition = new Vector2(_maxPositionX, Position.Y);
        else if (mouseState.X < _zeroPositionX)
            _holderPosition = new Vector2(_zeroPositionX, Position.Y);
        else
            _holderPosition = new Vector2(mouseState.X, Position.Y);
        
        HolderRectangle = new Rectangle((int)_holderPosition.X - HolderTexture.Width/2, (int)_holderPosition.Y, HolderTexture.Width, HolderTexture.Height);
        Value = (_holderPosition.X - _zeroPositionX) * (_maxValue - _minValue) / (_maxPositionX - _zeroPositionX);
    }

    public void Update(MouseState mouseState, ref bool isAnotherObjectFocused)
    {
        if (HolderRectangle.Contains(mouseState.Position) && !isAnotherObjectFocused && mouseState.LeftButton == ButtonState.Pressed)
        {
            BeingPressed = true;
            isAnotherObjectFocused = true;
        }
        if (BeingPressed & mouseState.LeftButton == ButtonState.Released)
            BeingPressed = false;
        if (BeingPressed)
            MoveToMouse(mouseState);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(BackTexture, Position, Color.White);
        spriteBatch.Draw(HolderTexture, GetHolderPosition(), Color.White);
    }

    public void SetValue(float value) => Value = MathHelper.Clamp(value, _minValue, _maxValue);
}