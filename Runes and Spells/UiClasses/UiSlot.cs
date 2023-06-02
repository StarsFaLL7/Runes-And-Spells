using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.OtherClasses;

namespace Runes_and_Spells.UiClasses;

public class UiSlot
{
    public Rectangle DropRectangle { get; private set; }
    private readonly Vector2 _position;
    private readonly Texture2D _texture;
    public Item currentItem { get; private set; }
    private bool _isLocked;
    private ItemType[] _acceptableItemTypes;
    private MouseState lastMouseState;
    private MouseState currentMouseState;

    public UiSlot(Vector2 position, Texture2D texture, bool isDropArea)
    {
        _position = position;
        _texture = texture;
        DropRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
    }
    public UiSlot(Vector2 position, Texture2D texture, bool isDropArea, params ItemType[] acceptableItemTypes)
    {
        _position = position;
        _texture = texture;
        DropRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
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
        if (DropRectangle.Contains(currentMouseState.Position) && 
            DropRectangle.Contains(lastMouseState.Position) &&
            lastMouseState.LeftButton == ButtonState.Released && 
            currentMouseState.LeftButton == ButtonState.Pressed && 
            !_isLocked && currentItem is not null)
        {
            inventory.AddItem(currentItem);
            currentItem = null;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, Color.White);
        if (currentItem is not null)
        {
            spriteBatch.Draw(currentItem.Texture, _position, Color.White);
        }
    }

    public void Clear() => currentItem = null;
    public void SetItem(Item item) => currentItem = item;
    
    public void Lock() => _isLocked = true;
    public void Unlock() => _isLocked = false;
    public bool ContainsItem() => currentItem is not null;
}