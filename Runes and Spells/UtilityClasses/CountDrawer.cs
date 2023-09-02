using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UtilityClasses;

public static class CountDrawer
{
    public static Dictionary<char, Texture2D> Textures { get; private set; }
    public static int TextureWidth { get; private set; }
    public static int TextureHeight { get; private set; }


    public static void DrawNumber(int number, Vector2 rightBottomPosition, SpriteBatch spriteBatch)
    {
        if (Textures.Count == 0)
            throw new InvalidOperationException();
        
        var numberStr = number.ToString();
        var newPosition = new Vector2(rightBottomPosition.X - TextureWidth * numberStr.Length, rightBottomPosition.Y -TextureHeight);
        for (var i = 0; i < numberStr.Length; i++)
        {
            spriteBatch.Draw(Textures[numberStr[i]], new Vector2(newPosition.X + i*TextureWidth, newPosition.Y)*Game1.ResolutionScale, 
                null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
    }
    public static void DrawNumber(int number, Vector2 rightBottomPosition, SpriteBatch spriteBatch, Color color)
    {
        if (Textures.Count == 0)
            throw new InvalidOperationException();
        
        var numberStr = number.ToString();
        var newPosition = new Vector2(rightBottomPosition.X - TextureWidth * numberStr.Length, rightBottomPosition.Y -TextureHeight);
        for (var i = 0; i < numberStr.Length; i++)
        {
            spriteBatch.Draw(Textures[numberStr[i]], new Vector2(newPosition.X + i*TextureWidth, newPosition.Y)*Game1.ResolutionScale, 
                null, color, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
    }

    public static Rectangle MeasureNumber(int number)
    {
        var numberStr = number.ToString();
        return new Rectangle(0, 0, numberStr.Length * TextureWidth, TextureHeight);
    }

    public static void Initialize(ContentManager content)
    {
        Textures = new Dictionary<char, Texture2D>();
        for (var i = 0; i < 10; i++)
            Textures.Add(i.ToString()[0], content.Load<Texture2D>($"textures/Inventory/wood_numbers/{i}"));
        TextureWidth = Textures['0'].Width;
        TextureHeight = Textures['0'].Height;
    }
}