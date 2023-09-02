using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.UiClasses;

public class UiSlot
{
    public Rectangle DropRectangle { get; private set; }
    public readonly Vector2 Position;
    private readonly Texture2D _texture;
    public Item currentItem { get; private set; }
    private bool _isLocked;
    private ItemType[] _acceptableItemTypes;
    private MouseState lastMouseState;
    private MouseState currentMouseState;
    private bool _drawToolTip;
    private Game1 _game;

    public UiSlot(Vector2 position, Texture2D texture, Game1 game)
    {
        _game = game;
        Position = position;
        _texture = texture;
        DropRectangle = new Rectangle((int)(Position.X*Game1.ResolutionScale.X), (int)(Position.Y*Game1.ResolutionScale.Y), 
            (int)(_texture.Width*Game1.ResolutionScale.X), (int)(_texture.Height*Game1.ResolutionScale.Y));
    }
    public UiSlot(Vector2 position, Texture2D texture, Game1 game, params ItemType[] acceptableItemTypes)
    {
        _game = game;
        Position = position;
        _texture = texture;
        DropRectangle = new Rectangle((int)(Position.X*Game1.ResolutionScale.X), (int)(Position.Y*Game1.ResolutionScale.Y), 
            (int)(_texture.Width*Game1.ResolutionScale.X), (int)(_texture.Height*Game1.ResolutionScale.Y));
        _acceptableItemTypes = acceptableItemTypes;
    }

    public bool TryToAddItem(Item item)
    {
        if (!_isLocked &&  DropRectangle.Contains(Mouse.GetState().Position) && currentItem is null && _acceptableItemTypes.Contains(item.Type))
        {
            currentItem = item;
            return true;
        }
        return false;
    }

    public void Update(Inventory inventory)
    {
        lastMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        DropRectangle = new Rectangle((int)(Position.X*Game1.ResolutionScale.X), (int)(Position.Y*Game1.ResolutionScale.Y), 
            (int)(_texture.Width*Game1.ResolutionScale.X), (int)(_texture.Height*Game1.ResolutionScale.Y));
        if (DropRectangle.Contains(currentMouseState.Position) && 
            DropRectangle.Contains(lastMouseState.Position) &&
            currentItem is not null)
        {
            if (lastMouseState.LeftButton == ButtonState.Released && 
                currentMouseState.LeftButton == ButtonState.Pressed && 
                !_isLocked)
            {
                inventory.AddItem(currentItem);
                currentItem = null;
            }
            
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                _drawToolTip = true;
            }
        }
        else
        {
            _drawToolTip = false;
        }
        
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (currentItem is not null)
        {
            spriteBatch.Draw(currentItem.Texture, Position*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            if (_drawToolTip)
            {
                _game.ToolTipItem = currentItem;
            }
        }
    }

    public void Clear() => currentItem = null;
    public void SetItem(Item item) => currentItem = item;
    
    public void Lock() => _isLocked = true;
    public void Unlock() => _isLocked = false;
    public bool ContainsItem() => currentItem is not null;
}