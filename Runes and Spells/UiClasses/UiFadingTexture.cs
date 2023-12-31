﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UiClasses;

public class UiFadingTexture
{
    public enum Mode
    {
        FadeIn,
        FadeOut
    }
    
    private readonly Texture2D _texture;
    private float _alpha;
    private float _modifier;
    private float _fadingTimeFrames;
    public Mode FadeMode { get; private set; }
    private Action _endAction;
    public bool IsFading { get; private set; }

    public UiFadingTexture(Texture2D texture, float animationTimeSeconds, Mode mode, Action actionOnEnd = null)
    {
        _texture = texture;
        FadeMode = mode;
        _fadingTimeFrames = animationTimeSeconds * 60;
        _endAction = actionOnEnd;
        Reset();
    }

    public void Draw(Vector2 position, SpriteBatch spriteBatch)
    {
        if ((FadeMode == Mode.FadeIn && _alpha >= 1 || FadeMode == Mode.FadeOut && _alpha <= 0) && IsFading)
        {
            if (_endAction is not null)
                _endAction();

            IsFading = false;
        }
        if (_alpha >= 0)
        {
            spriteBatch.Draw(_texture, new Vector2(position.X, position.Y)*Game1.ResolutionScale, 
                null, Color.White * _alpha, 0f, Vector2.Zero, 
                Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
        if (IsFading)
        {
            _alpha += _modifier;
        }
    }

    public void Reset()
    {
        if (FadeMode == Mode.FadeIn)
        {
            _alpha = 0f;
            _modifier = 1f / _fadingTimeFrames;
        }
        else if (FadeMode == Mode.FadeOut)
        {
            _alpha = 1f;
            _modifier = -1f / _fadingTimeFrames;
        }
    }

    public void Reset(Mode newMode)
    {
        FadeMode = newMode;
        Reset();
    }

    public void StartFade() => IsFading = true;
}