using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.classes;

public class TradingMiniGame
{
    enum Area
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    
    private Texture2D _mainCircleTexture;
    private Vector2 _circlePosition;
    private Dictionary<Area, Rectangle> _areas;
    private Rectangle _allRectangle;
    public bool IsRunning { get; private set; }
    private Vector2 _moveDirection;
    private Vector2 _nextPosition;
    public double Score { get; private set; }
    private const int SpeedModifier = 4;
    private int _startPrice;
    
    public void LoadContent(ContentManager content)
    {
        _mainCircleTexture = content.Load<Texture2D>("textures/market_screen/mini_game_circle");
        
        _allRectangle = new Rectangle(85, 550, 1240, 396);
        _areas = new Dictionary<Area, Rectangle>
        {
            {
                Area.TopLeft,
                new Rectangle(_allRectangle.Left, _allRectangle.Top, _allRectangle.Width / 2, _allRectangle.Height / 2)
            },
            {
                Area.TopRight,
                new Rectangle(_allRectangle.Center.X, _allRectangle.Top, _allRectangle.Width / 2,
                    _allRectangle.Height / 2)
            },
            {
                Area.BottomLeft,
                new Rectangle(_allRectangle.Left, _allRectangle.Center.Y, _allRectangle.Width / 2,
                    _allRectangle.Height / 2)
            },
            {
                Area.BottomRight,
                new Rectangle(_allRectangle.Center.X, _allRectangle.Center.Y, _allRectangle.Width / 2,
                    _allRectangle.Height / 2)
            }
        };
    }

    public void Update()
    {
        if (!IsRunning) return;
        
        if (Math.Abs(_circlePosition.X - _nextPosition.X) < SpeedModifier && Math.Abs(_circlePosition.Y - _nextPosition.Y) < SpeedModifier)
        {
            _circlePosition = _nextPosition;
            _nextPosition = GenerateNextPosition(_circlePosition);
            _moveDirection = GetMovementDirection(_circlePosition, _nextPosition);
        }
        
        _circlePosition += _moveDirection * SpeedModifier;
        
        var mouseState = Mouse.GetState();
        if (
            new Rectangle((int)_circlePosition.X, (int)_circlePosition.Y, 
                _mainCircleTexture.Width, _mainCircleTexture.Height)
            .Contains(mouseState.X, mouseState.Y))
            Score += 6;
        else
            Score -= 5;
    }

    public void Start(int startPrice)
    {
        Reset();
        _startPrice = startPrice;
        IsRunning = true;
    }

    public void Reset()
    {
        _circlePosition = new Vector2(
            Random.Shared.Next(_allRectangle.Left, _allRectangle.Right),
            Random.Shared.Next(_allRectangle.Top, _allRectangle.Bottom));
        _nextPosition = GenerateNextPosition(_circlePosition);
        _moveDirection = GetMovementDirection(_circlePosition, _nextPosition);
        Score = 0;
    }

    public int Stop()
    {
        IsRunning = false;
        return _startPrice + (int)Score / 100;
    }

    private Vector2 GetMovementDirection(Vector2 arrivePos, Vector2 destinationPos)
    {
        var resultVector = destinationPos - arrivePos;
        resultVector.Normalize();
        return resultVector;
    }

    private Vector2 GenerateNextPosition(Vector2 position)
    {
        var possibleAreas = _areas.Where(a => !a.Value.Contains(position)).ToArray();
        var nextAreaInfo = possibleAreas[Random.Shared.Next(possibleAreas.Length)];
        var nextPos = new Vector2(
            Random.Shared.Next(nextAreaInfo.Value.Left, nextAreaInfo.Value.Right),
            Random.Shared.Next(nextAreaInfo.Value.Top, nextAreaInfo.Value.Bottom)
            );
        return nextPos;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!IsRunning) return;
        
        var scoreColor = Score switch
        {
            >= 0 and < 1000 => Color.Orange,
            >= 1000 and < 2000 => Color.LightGreen,
            >= 2000 => Color.Gold,
            _ => Color.OrangeRed
        };
        spriteBatch.DrawString(font, "Успешность торговли:", new Vector2(499, 482), new Color(226, 226, 226));
        spriteBatch.DrawString(font, ((int)Score/100).ToString(), new Vector2(715, 520), scoreColor);
        spriteBatch.Draw(_mainCircleTexture, _circlePosition, Color.White);
    }
}