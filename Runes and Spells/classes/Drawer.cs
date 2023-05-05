using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells;

public class Drawer
{

    public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Texture2D texture, Position position,
        Vector2? offset, float rotation, float layerDepth)
    {
        Vector2 pos;
        Vector2 origin;
        offset ??= new Vector2(0, 0);
        
        if (position == Position.Center)
        {
            pos = new Vector2((graphics.PreferredBackBufferWidth + offset.Value.X) / 2, (graphics.PreferredBackBufferHeight + offset.Value.Y)/2);
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }
        else if (position == Position.BottomLeft)
        {
            pos = new Vector2(offset.Value.X, graphics.PreferredBackBufferHeight + offset.Value.Y);
            origin = new Vector2(0, texture.Height);
        }
        else if (position == Position.BottomRight)
        {
            pos = new Vector2(graphics.PreferredBackBufferWidth + offset.Value.X, graphics.PreferredBackBufferHeight + offset.Value.Y);
            origin = new Vector2(texture.Width, texture.Height);
        }
        else if (position == Position.TopLeft)
        {
            pos = new Vector2(offset.Value.X, offset.Value.Y);
            origin = new Vector2(0, 0);
        }
        else if (position == Position.TopRight)
        {
            pos = new Vector2(graphics.PreferredBackBufferWidth + offset.Value.X,  offset.Value.Y);
            origin = new Vector2(texture.Width, 0);
        }
        else if (position == Position.MiddleTop)
        {
            pos = new Vector2((graphics.PreferredBackBufferWidth + offset.Value.X) / 2, offset.Value.Y);
            origin = new Vector2(texture.Width / 2f, 0);
        }
        else if (position == Position.MiddleBottom)
        {
            pos = new Vector2((graphics.PreferredBackBufferWidth + offset.Value.X) / 2, graphics.PreferredBackBufferHeight + offset.Value.Y);
            origin = new Vector2(texture.Width / 2f, texture.Height);
        }
        else if (position == Position.MiddleLeft)
        {
            pos = new Vector2(offset.Value.X, (graphics.PreferredBackBufferHeight + offset.Value.Y) / 2);
            origin = new Vector2(0, texture.Height / 2f);
        }
        else if (position == Position.MiddleRight)
        {
            pos = new Vector2(graphics.PreferredBackBufferWidth + offset.Value.X, (graphics.PreferredBackBufferHeight + offset.Value.Y) / 2);
            origin = new Vector2(texture.Width, texture.Height / 2f);
        }
        else
        {
            pos = new Vector2(offset.Value.X, offset.Value.Y);
            origin = new Vector2(0, 0);
        }
        
        spriteBatch.Draw(
            texture, 
            pos, 
            null, 
            Color.White, 
            rotation, 
            origin, 
            Vector2.One,
            SpriteEffects.None,
            layerDepth);
    }
}