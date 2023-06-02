using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.UiClasses;

public class UiSlotForSelling
{
    public Item CurrentItem { get; private set; }
    private Rectangle _rectangle;
    private Vector2 Position { get; set; }
    public int Price { get; private set; }
    private bool IsPressed { get; set; }
    private readonly Texture2D _slotTexture;
    private Texture2D _slotBorderTexture;
    private int _count;

    public UiSlotForSelling(Vector2 position, int price, Texture2D slotTexture, Texture2D slotBorderTexture)
    {
        Position = position;
        Price = price;
        _slotTexture = slotTexture;
        _rectangle = new Rectangle((int)position.X, (int)position.Y, slotTexture.Width, slotTexture.Height);
        _slotBorderTexture = slotBorderTexture;
    }

    public void Update(MouseState mouseState, ref bool isAnotherObjectFocused, Game1 game)
    {
        if (mouseState.LeftButton == ButtonState.Released) isAnotherObjectFocused = false;
        
        if (IsPressed && _rectangle.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Released)
        {
            if (game.Balance >= Price && CurrentItem is not null)
            {
                game.SubtractFromBalance(Price);
                game.Inventory.AddItem(CurrentItem);
                _count--;
                if (_count <= 0)
                    Clear();
            }
            IsPressed = false;
        }
            
        if (!isAnotherObjectFocused)
            IsPressed = false;
        if (isAnotherObjectFocused && IsPressed)
            return;
            
        if (_rectangle.Contains(mouseState.X, mouseState.Y) && !isAnotherObjectFocused)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsPressed = true;
                isAnotherObjectFocused = true;
            }
        }
    }

    private void Clear()
    {
        Price = 0;
        CurrentItem = null;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Color color)
    {
        spriteBatch.Draw(_slotTexture, Position, Color.White);
        
        if (CurrentItem is null) return;
        
        spriteBatch.Draw(CurrentItem.Texture, Position, Color.White);
        var stringSize = font.MeasureString(Price.ToString());
        spriteBatch.DrawString(font, Price.ToString(), 
            new Vector2(Position.X + _slotTexture.Width/2 - stringSize.X/2, Position.Y + _slotTexture.Height + 9), color);
        
        spriteBatch.Draw(_slotBorderTexture, Position, Color.White);
        if (_count > 1)
        {
            CountDrawer.DrawNumber(_count,
                new Vector2(Position.X + _slotTexture.Width - 9,
                    Position.Y + _slotTexture.Height - 9),
                spriteBatch);
        }
    }

    public void SetItem(ItemInfo itemInfo, int price, int count = 1)
    {
        _count = count;
        CurrentItem = new Item(itemInfo);
        Price = price;
    }
}