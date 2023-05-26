using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.classes;

public class AnimatedTexture
{
    private Texture2D _spritesheet;
    private Timer _animTimer;
    private int _currentFrame;
    private Vector2 _frameSize;
    private int _framesCount;
    private bool _isLoop;
    
    public AnimatedTexture(int msBetweenFrames, Texture2D spriteSheet, Vector2 frameSize, bool isLoop)
    {
        _isLoop = isLoop;
        _spritesheet = spriteSheet;
        _frameSize = frameSize;
        _framesCount = spriteSheet.Width / (int)frameSize.X;
        _animTimer = new Timer(msBetweenFrames, () =>
        {
            if (_currentFrame + 1 >= _framesCount && !_isLoop)
                _animTimer.Stop();
            else if (_currentFrame + 1 >= _framesCount && _isLoop)
            {
                _currentFrame = 0;
                _animTimer.StartAgain();
            }
            else
                _currentFrame++;
        });
    }

    public void Draw(Vector2 position, SpriteBatch spriteBatch)
    {
        if (!_animTimer.IsRunning)
            _animTimer.StartAgain();
        spriteBatch.Draw(_spritesheet, position, 
            new Rectangle(_currentFrame * (int)_frameSize.X, 0, (int)_frameSize.X, (int)_frameSize.Y),
            Color.White);
        _animTimer.Tick();
    }

    public void SetRandomFrame() => _currentFrame = Random.Shared.Next(0, _framesCount);
}