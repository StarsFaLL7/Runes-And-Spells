using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UtilityClasses;

public static class CountDrawer
{
    private static Dictionary<char, Texture2D> _textures;
    private static int _textureWidth;
    private static int _textureHeight;


    public static void DrawNumber(int number, Vector2 rightBottomPosition, SpriteBatch spriteBatch)
    {
        if (_textures.Count == 0)
            throw new InvalidOperationException();
        
        var numberStr = number.ToString();
        var newPosition = new Vector2(rightBottomPosition.X - _textureWidth * numberStr.Length, rightBottomPosition.Y -_textureHeight);
        for (var i = 0; i < numberStr.Length; i++)
        {
            spriteBatch.Draw(_textures[numberStr[i]], new Vector2(newPosition.X + i*_textureWidth, newPosition.Y), Color.White);
        }
    }
    public static void DrawNumber(int number, Vector2 rightBottomPosition, SpriteBatch spriteBatch, Color color)
    {
        if (_textures.Count == 0)
            throw new InvalidOperationException();
        
        var numberStr = number.ToString();
        var newPosition = new Vector2(rightBottomPosition.X - _textureWidth * numberStr.Length, rightBottomPosition.Y -_textureHeight);
        for (var i = 0; i < numberStr.Length; i++)
        {
            spriteBatch.Draw(_textures[numberStr[i]], new Vector2(newPosition.X + i*_textureWidth, newPosition.Y), color);
        }
    }

    public static void Initialize(ContentManager content)
    {
        _textures = new Dictionary<char, Texture2D>();
        for (var i = 0; i < 10; i++)
            _textures.Add(i.ToString()[0], content.Load<Texture2D>($"textures/Inventory/wood_numbers/{i}"));
        _textureWidth = _textures['0'].Width;
        _textureHeight = _textures['0'].Height;
    }
}