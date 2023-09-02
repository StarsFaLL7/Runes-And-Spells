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
    private float _zeroPositionX;
    private float _maxPositionX;
    private float _borderWidth;

    public float Value { get; private set; }
    private Vector2 _holderPosition;
    private bool BeingPressed { get; set; }
    private Rectangle CollisionRectangle { get; set; }

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
        _borderWidth = textureBordersWidth;
        _zeroPositionX = (Position.X + _borderWidth + holderTexture.Width / 2f) * Game1.ResolutionScale.X;
        _maxPositionX = (Position.X + BackTexture.Width - _borderWidth - HolderTexture.Width / 2f) * Game1.ResolutionScale.X;
        _holderPosition = new Vector2(_zeroPositionX + Value / (maxValue - minValue) * (_maxPositionX - _zeroPositionX), position.Y*Game1.ResolutionScale.Y);
        CollisionRectangle = new Rectangle((int)_holderPosition.X, (int)_holderPosition.Y, HolderTexture.Width, HolderTexture.Height);
    }

    private void MoveToMouse(MouseState mouseState)
    {
        if (mouseState.X > _maxPositionX)
            _holderPosition = new Vector2(_maxPositionX, Position.Y);
        else if (mouseState.X < _zeroPositionX)
            _holderPosition = new Vector2(_zeroPositionX, Position.Y);
        else
            _holderPosition = new Vector2(mouseState.X, Position.Y);
        
        
        Value = (_holderPosition.X - _zeroPositionX) * (_maxValue - _minValue) / (_maxPositionX - _zeroPositionX);
    }

    public void Update(MouseState mouseState, ref bool isAnotherObjectFocused)
    {
        _zeroPositionX = (Position.X + _borderWidth + HolderTexture.Width / 2f) * Game1.ResolutionScale.X;
        _maxPositionX = (Position.X + BackTexture.Width - _borderWidth - HolderTexture.Width / 2f) * Game1.ResolutionScale.X;
        _holderPosition = new Vector2(_zeroPositionX + Value / (_maxValue - _minValue) * (_maxPositionX - _zeroPositionX), Position.Y);
        CollisionRectangle = new Rectangle((int)_zeroPositionX, (int)(Position.Y*Game1.ResolutionScale.Y), 
            (int)(_maxPositionX - _zeroPositionX), (int)(HolderTexture.Height*Game1.ResolutionScale.Y));
        
        if (CollisionRectangle.Contains(mouseState.Position) && !isAnotherObjectFocused && mouseState.LeftButton == ButtonState.Pressed)
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
        spriteBatch.Draw(BackTexture, new Vector2(Position.X*Game1.ResolutionScale.X, Position.Y*Game1.ResolutionScale.Y), 
            null, Color.White, 0f, Vector2.Zero,
            Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(HolderTexture, new Vector2(_holderPosition.X - HolderTexture.Width*Game1.ResolutionScale.X / 2f, _holderPosition.Y*Game1.ResolutionScale.Y)
            , null, Color.White, 0f, Vector2.Zero,
            Game1.ResolutionScale, SpriteEffects.None, 1f);
    }

    public void SetValue(float value)
    {
        Value = MathHelper.Clamp(value, _minValue, _maxValue);
        _holderPosition = new Vector2(_zeroPositionX + Value / (_maxValue - _minValue) * (_maxPositionX - _zeroPositionX), Position.Y);
        CollisionRectangle = new Rectangle((int)_holderPosition.X, (int)_holderPosition.Y, HolderTexture.Width, HolderTexture.Height);
    }
}