using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells;

public interface IScreen
{
    public void Initialize();
    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics);
    public void Update(GraphicsDeviceManager graphics);
    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer);
}