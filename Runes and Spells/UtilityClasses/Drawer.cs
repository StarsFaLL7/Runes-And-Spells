using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UtilityClasses;

public class Drawer
{

    private readonly Dictionary<Position,Func<Texture2D, GraphicsDeviceManager, Vector2, (Vector2 pos, Vector2 origin)>> _dictionary =
        new ()
        {
            {Position.Center, (texture, graphics, offset) =>
                    (new Vector2(
                            (graphics.PreferredBackBufferWidth + offset.X) / 2, (graphics.PreferredBackBufferHeight + offset.Y) / 2), 
                        new Vector2(texture.Width / 2f, texture.Height / 2f))
            },
            {Position.BottomLeft, (texture, graphics, offset) =>
                (new Vector2(offset.X, graphics.PreferredBackBufferHeight + offset.Y), 
                    new Vector2(0, texture.Height))
            },
            {Position.BottomRight, (texture, graphics, offset) =>
                (new Vector2(graphics.PreferredBackBufferWidth + offset.X, graphics.PreferredBackBufferHeight + offset.Y), 
                    new Vector2(texture.Width, texture.Height))
            },
            {Position.TopLeft, (texture, graphics, offset) =>
                (new Vector2(offset.X, offset.Y), 
                    new Vector2(0, 0))
            },
            {Position.TopRight, (texture, graphics, offset) =>
                (new Vector2(graphics.PreferredBackBufferWidth + offset.X,  offset.Y), 
                    new Vector2(texture.Width, 0))
            },
            {Position.MiddleTop, (texture, graphics, offset) =>
                (new Vector2((graphics.PreferredBackBufferWidth + offset.X) / 2, offset.Y), 
                    new Vector2(texture.Width / 2f, 0))
            },
            {Position.MiddleBottom, (texture, graphics, offset) =>
                (new Vector2((graphics.PreferredBackBufferWidth + offset.X) / 2, graphics.PreferredBackBufferHeight + offset.Y), 
                    new Vector2(texture.Width / 2f, texture.Height))
            },
            {Position.MiddleLeft, (texture, graphics, offset) =>
                (new Vector2(offset.X, (graphics.PreferredBackBufferHeight + offset.Y) / 2), 
                    new Vector2(0, texture.Height / 2f))
            },
            {Position.MiddleRight, (texture, graphics, offset) =>
                (new Vector2(graphics.PreferredBackBufferWidth + offset.X, (graphics.PreferredBackBufferHeight + offset.Y) / 2), 
                    new Vector2(texture.Width, texture.Height / 2f))
            },
            {Position.Custom, (texture, graphics, offset) =>
                (new Vector2(offset.X, offset.Y), 
                    new Vector2(0, 0))
            }
        };

    public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Texture2D texture, Position position,
        Vector2? offset, float rotation, float layerDepth)
    {
        offset ??= new Vector2(0, 0);
        var result = _dictionary[position](texture, graphics, (Vector2)offset);
        spriteBatch.Draw(
            texture, 
            result.pos, 
            null, 
            Color.White, 
            rotation, 
            result.origin, 
            Vector2.One,
            SpriteEffects.None,
            layerDepth);
    }
}