using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.Screens;
using Runes_and_Spells.UtilityClasses;

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

    private Game1 _game;
    private MarketScreen _marketScreen;
    private Texture2D _mainCircleTexture;
    private Vector2 _circlePosition;
    private Dictionary<Area, Rectangle> _areas;
    private Rectangle _allRectangle;
    public bool IsRunning { get; private set; }
    private Vector2 _moveDirection;
    private Vector2 _nextPosition;
    public double Score { get; private set; }
    private float _speedModifier = 4;
    private int _startPrice;
    public int MinTrade { get; private set; }
    public int MaxTrade { get; private set; }

    public TradingMiniGame(MarketScreen marketScreen, Game1 game)
    {
        _marketScreen = marketScreen;
        _game = game;
    }
    
    public void LoadContent(ContentManager content)
    {
        _mainCircleTexture = content.Load<Texture2D>("textures/market_screen/mini_game_circle");
        GenerateAreas();
    }

    public void GenerateAreas()
    {
        _allRectangle = new Rectangle((int)(85*Game1.ResolutionScale.X), (int)(550*Game1.ResolutionScale.Y), 
            (int)(1240*Game1.ResolutionScale.X), (int)(396*Game1.ResolutionScale.Y));
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

        if (_game.Energy <= 0.01f)
        {
            _marketScreen.SellItem(_startPrice + (int)(Score / 100));
            Stop();
        }
        _game.SubtractEnergy(0.01f);
        _speedModifier = 4 * Game1.ResolutionScale.X;
        if (Math.Abs(_circlePosition.X - _nextPosition.X) < _speedModifier*2 && Math.Abs(_circlePosition.Y - _nextPosition.Y) < _speedModifier*2)
        {
            _circlePosition = _nextPosition;
            _nextPosition = GenerateNextPosition(_circlePosition);
            _moveDirection = GetMovementDirection(_circlePosition, _nextPosition);
        }
        
        _circlePosition += _moveDirection * _speedModifier;
        
        var mouseState = Mouse.GetState();
        if (
            new Rectangle((int)_circlePosition.X, (int)_circlePosition.Y, 
                _mainCircleTexture.Width, _mainCircleTexture.Height)
            .Contains(mouseState.X, mouseState.Y))
            Score += 6;
        else
            Score -= 5;
        
        var kb = Keyboard.GetState();
        if (Score >= MaxTrade || Score <= MinTrade || kb.IsKeyDown(Keys.Space) || kb.IsKeyDown(Keys.Enter))
        {
            _marketScreen.SellItem(_startPrice + (int)(Score / 100));
            Stop();
        }
    }

    public void Start(int startPrice, int minProfit, int maxProfit)
    {
        MinTrade = minProfit*100;
        MaxTrade = maxProfit*100;
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

    public void Stop()
    {
        IsRunning = false;
        Reset();
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
        spriteBatch.DrawString(font, "Успешность торговли:", new Vector2(499, 482)*Game1.ResolutionScale, 
            new Color(226, 226, 226), 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.DrawString(font, ((int)Score/100).ToString(), new Vector2(715, 520)*Game1.ResolutionScale, 
            scoreColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_mainCircleTexture, _circlePosition, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
    }
}