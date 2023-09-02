using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private readonly Texture2D _slotBorderTexture;
    public int Count { get; set; }
    public bool ShowTollTip { get; private set; }
    private SoundEffect _soundBuy;
    private readonly Game1 _game;

    public UiSlotForSelling(Vector2 position, int price, Texture2D slotTexture, Texture2D slotBorderTexture, SoundEffect buySound, Game1 game)
    {
        _soundBuy = buySound;
        Position = position;
        Price = price;
        _slotTexture = slotTexture;
        _rectangle = new Rectangle((int)position.X, (int)position.Y, slotTexture.Width, slotTexture.Height);
        _slotBorderTexture = slotBorderTexture;
        _game = game;
    }

    public void Update(MouseState mouseState, ref bool isAnotherObjectFocused, Game1 game)
    {
        if (mouseState.LeftButton == ButtonState.Released) isAnotherObjectFocused = false;
        _rectangle = new Rectangle((int)(Position.X*Game1.ResolutionScale.X), (int)(Position.Y*Game1.ResolutionScale.Y), 
            (int)(_slotTexture.Width*Game1.ResolutionScale.X), (int)(_slotTexture.Height*Game1.ResolutionScale.Y));
        if (IsPressed && _rectangle.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Released)
        {
            if (game.Balance >= Price && CurrentItem is not null)
            {
                _soundBuy.Play();
                game.SubtractFromBalance(Price);
                game.Inventory.AddItem(CurrentItem);
                Count--;
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 25) _game.Introduction.Step = 26;
                if (Count <= 0)
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
        if (mouseState.RightButton == ButtonState.Pressed && CurrentItem is not null && _rectangle.Contains(mouseState.X, mouseState.Y))
        {
            ShowTollTip = true;
        }
        else
        {
            ShowTollTip = false;
        }
    }

    private void Clear()
    {
        Price = 0;
        CurrentItem = null;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Color colorСanBuy, Color colorCantBuy)
    {
        spriteBatch.Draw(_slotTexture, Position*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (CurrentItem is null) return;
        
        spriteBatch.Draw(CurrentItem.Texture, Position*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        var stringSize = font.MeasureString(Price.ToString());
        
        var strColor = Price <= _game.Balance ? colorСanBuy : colorCantBuy;
        spriteBatch.DrawString(font, Price.ToString(), 
            new Vector2(Position.X + _slotTexture.Width/2 - stringSize.X/2, Position.Y + _slotTexture.Height + 9)*Game1.ResolutionScale,
            strColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        spriteBatch.Draw(_slotBorderTexture, Position*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (Count > 1)
        {
            CountDrawer.DrawNumber(Count,
                new Vector2(Position.X + _slotTexture.Width - 9,
                    Position.Y + _slotTexture.Height - 9),
                spriteBatch);
        }
    }

    public void SetItem(ItemInfo itemInfo, int price, int count = 1)
    {
        Count = count;
        CurrentItem = new Item(itemInfo);
        Price = price;
    }
}