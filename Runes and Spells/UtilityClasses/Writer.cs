using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UtilityClasses;

public class Writer
{
    private Dictionary<char, Texture2D> _textures;
    private readonly int _symbWidth;
    private readonly int _symbHeight;
    
    public Writer(ContentManager content)
    {
        _textures = new Dictionary<char, Texture2D>();
        for (var i = 97; i < 123; i++)
        {
            _textures.Add((char)i, content.Load<Texture2D>($"textures/font/{(char)i}"));
        }
        _symbHeight = _textures['a'].Height;
        _symbWidth = _textures['a'].Width;
    }
    
    public void DrawString(string text, Rectangle box, SpriteBatch spriteBatch)
    {
        var currentPosition = new Vector2(box.Left, box.Top);
        var words = text.Split();
        foreach (var word in words)
        {
            foreach (var symb in word)
            {
                if (_textures.ContainsKey(symb))
                    spriteBatch.Draw(_textures[symb], currentPosition, Color.White);
                currentPosition.X += _symbWidth;
            }
            currentPosition.X += _symbWidth;
            if (currentPosition.X + _symbWidth * (1 + word.Length) > box.Right)
            {
                currentPosition.Y += _symbHeight;
                currentPosition.X = box.Left;
            }
        }
    }
    
    public void DrawWords(string[] words, Rectangle box, SpriteBatch spriteBatch, Color color)
    {
        var currentPosition = new Vector2(box.Left, box.Top);
        foreach (var word in words)
        {
            if (currentPosition.X + _symbWidth * word.Length > box.Right)
            {
                currentPosition.Y += _symbHeight;
                currentPosition.X = box.Left;
            }
            
            foreach (var symb in word)
            {
                if (_textures.ContainsKey(symb))
                    spriteBatch.Draw(_textures[symb], currentPosition, color);
                currentPosition.X += _symbWidth;
            }
            currentPosition.X += _symbWidth;
        }
    }
}