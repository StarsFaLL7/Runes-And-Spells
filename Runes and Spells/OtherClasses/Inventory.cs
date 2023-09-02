using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.OtherClasses;

public class Inventory
{
    private readonly Game1 _game;
    public Inventory(Game1 game)
    {
        _game = game;
    }
    
    private enum Tab
    {
        Runes,
        Scrolls,
        Other
    }

    public Rectangle VisibleRectangle { get; private set; }
    
    private Texture2D _backgroundTexture;
    private Texture2D _slotTexture;
    private Texture2D _slotBorderTexture;
    private Texture2D _glowTexture;

    private UiButton _arrowSmallRightButton;
    private UiButton _arrowSmallLeftButton;
    private UiButton _arrowBigRightButton;
    private UiButton _arrowBigLeftButton;
    private bool _isObjectFocused;
    
    private int _currentPage;
    private Tab _currentTab;
    private Item[] _itemsToDraw = {};
    public List<Item> Items { get; set; } = new ();
    private bool IsOpened { get; set; }
    private UiButton _buttonCloseInv;
    private UiButton _buttonOpenInv;
    private SoundEffect _openCloseSound;

    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _openCloseSound = content.Load<SoundEffect>("sounds/inv_sound");
        _backgroundTexture = content.Load<Texture2D>("textures/Inventory/inv_back");
        _glowTexture = content.Load<Texture2D>("textures/Inventory/glow_cell");
        VisibleRectangle = new Rectangle(
            _game.ScreenWidth - _backgroundTexture.Width, (_game.ScreenHeight - _backgroundTexture.Height)/2,
            _backgroundTexture.Width, _backgroundTexture.Height);
        _slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg");
        _slotBorderTexture = content.Load<Texture2D>("textures/Inventory/slot_border");
        _arrowBigRightButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_big_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_right_pressed"),
            new Vector2(1843, 136),
            () =>
            {
                AllGameItems.ClickSound.Play();
                _currentPage = 0;
                if (_currentTab == Tab.Other)
                {
                    _currentTab = Tab.Runes;
                    return;
                }
                _currentTab++;
            }
            );
        _arrowBigLeftButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_big_left_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_left_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_left_pressed"),
            new Vector2(1510, 136),
            () =>
            {
                AllGameItems.ClickSound.Play();
                _currentPage = 0;
                if (_currentTab == Tab.Runes)
                {
                    _currentTab = Tab.Other;
                    return;
                }
                _currentTab--;
            }
        );
        _arrowSmallLeftButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_small_left_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_left_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_left_pressed"),
            new Vector2(1591, 923),
            () =>
            {
                if (_currentPage > 0)
                    _currentPage--;
                AllGameItems.ClickSound.Play();
            }
        );
        _arrowSmallRightButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_small_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_right_pressed"),
            new Vector2(1768, 923),
            () =>
            {
                _currentPage++;
                AllGameItems.ClickSound.Play();
            }
        );
        _buttonCloseInv = new UiButton(content.Load<Texture2D>("textures/Inventory/button_close_inv_default"),
            content.Load<Texture2D>("textures/Inventory/button_close_inv_hovered"),
            content.Load<Texture2D>("textures/Inventory/button_close_inv_pressed"),
            new Vector2(1920 - _backgroundTexture.Width - 42, 1080f/2 - 45),
            () =>
            {
                IsOpened = false;
                _openCloseSound.Play();
            });
        _buttonOpenInv = new UiButton(content.Load<Texture2D>("textures/Inventory/button_open_inv_default"),
            content.Load<Texture2D>("textures/Inventory/button_open_inv_hovered"),
            content.Load<Texture2D>("textures/Inventory/button_open_inv_pressed"),
            new Vector2(1920 - 42, 1080f/2 - 45),
            () =>
            {
                IsOpened = true;
                _openCloseSound.Play();
            });
    }

    public void Update(GraphicsDeviceManager graphics, params UiSlot[] dropableSlots)
    {
        TryToUniteClaySmall();
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _isObjectFocused = false;
        
        if (_game.IsInTopDownView)
        {
            if (IsOpened)
                _buttonCloseInv.Update(mouseState, ref _isObjectFocused);
            else
            {
                _buttonOpenInv.Update(mouseState, ref _isObjectFocused);
                return;
            }
        }
        _arrowBigLeftButton.Update(mouseState, ref _isObjectFocused);
        _arrowBigRightButton.Update(mouseState, ref _isObjectFocused);
        if (_currentPage > 0)
            _arrowSmallLeftButton.Update(mouseState, ref _isObjectFocused);
        if (_itemsToDraw.Length > 24)
            _arrowSmallRightButton.Update(mouseState, ref _isObjectFocused);
        
        if (_currentTab is Tab.Runes) 
            _itemsToDraw = Items
                .Where(item => item.Type is ItemType.Rune or ItemType.UnknownRune && (item.Count > 0 || item.IsBeingDragged))
                .OrderBy(i => i.ID)
                .Skip(_currentPage*24)
                .Take(25)
                .ToArray();
        else if (_currentTab is Tab.Other)
            _itemsToDraw = Items
                .Where(item => item.Type != ItemType.Rune && item.Type != ItemType.UnknownRune && item.Type != ItemType.Scroll 
                               && (item.Count > 0 || item.IsBeingDragged))
                .OrderBy(i => i.ID)
                .Skip(_currentPage*24)
                .Take(25)
                .ToArray();
        else if (_currentTab is Tab.Scrolls)
            _itemsToDraw = Items
                .Where(item => item.Type is ItemType.Scroll && (item.Count > 0 || item.IsBeingDragged))
                .OrderBy(i => i.ID)
                .Skip(_currentPage * 24)
                .Take(25)
                .ToArray();
        if (_itemsToDraw.Any(i => i.IsBeingDragged))
            foreach(var item in _itemsToDraw.Where(i => !i.IsBeingDragged)) item.Lock();
        else
            foreach (var item in _itemsToDraw.Where(i => !i.IsBeingDragged)) item.Unlock();
        
        for (var y = 0; y < 6; y++)
        for (var x = 0; x < 4; x++)
            if (_itemsToDraw.Length > x + 4 * y)
                _itemsToDraw[x + 4 * y].Update(
                    new Vector2(
                    1489 + (_slotTexture.Width + 9) * x,
                    207 + (_slotTexture.Width + 9) * y), dropableSlots.ToList());
        var kb = Keyboard.GetState().GetPressedKeys();
        if (kb.Contains(Keys.H) && kb.Contains(Keys.A) && kb.Contains(Keys.C) && kb.Contains(Keys.K)) AddAllItems();
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        if (_game.IsInTopDownView)
        {
            if (IsOpened)
                _buttonCloseInv.Draw(spriteBatch);
            else
            {
                _buttonOpenInv.Draw(spriteBatch);
                return;
            }
        }
        
        spriteBatch.Draw(_backgroundTexture, 
            new Vector2(_game.ScreenWidth - _backgroundTexture.Width*Game1.ResolutionScale.X,
                (_game.ScreenHeight - _backgroundTexture.Height*Game1.ResolutionScale.Y)/2), null, Color.White, 
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (_currentPage > 0)
            _arrowSmallLeftButton.Draw(spriteBatch);
        if (_itemsToDraw.Length > 24)
            _arrowSmallRightButton.Draw(spriteBatch);
        _arrowBigLeftButton.Draw(spriteBatch);
        _arrowBigRightButton.Draw(spriteBatch);

        var text = Game1.GetText(_currentTab.ToString());
        var textSize = AllGameItems.Font40Px.MeasureString(text);
        var position = new Vector2(1920 - _backgroundTexture.Width + 12 + (_backgroundTexture.Width - 12 - textSize.X) / 2,
            _arrowBigLeftButton.Position.Y + (float)_arrowBigLeftButton.ActualTexture().Height/2 - textSize.Y/2);
        spriteBatch.DrawString(AllGameItems.Font40Px, text, new Vector2(position.X+3, position.Y+3)*Game1.ResolutionScale, new Color(0,0,0,33),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.DrawString(AllGameItems.Font40Px, text, position*Game1.ResolutionScale, new Color(119,48,45),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);

        text = $"{Game1.GetText("Page")} {_currentPage + 1}";
        textSize = AllGameItems.Font30Px.MeasureString(text);
        position = new Vector2(1920 - _backgroundTexture.Width + 12 + (_backgroundTexture.Width - 12 - textSize.X) / 2,
            _arrowSmallLeftButton.Position.Y + (float)_arrowSmallLeftButton.ActualTexture().Height/2 - textSize.Y/2);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, new Vector2(position.X+3, position.Y+3)*Game1.ResolutionScale, new Color(0,0,0,33),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, position*Game1.ResolutionScale, new Color(119,48,45),
            0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        for (var y = 0; y < 6; y++)
            for (var x = 0; x < 4; x++)
                spriteBatch.Draw(_slotTexture, new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y)*Game1.ResolutionScale, 
                    null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, 
                    SpriteEffects.None, 1f);
        
        var draggedItem = _itemsToDraw.FirstOrDefault(i => i.IsBeingDragged);
        var toolTipItem = _itemsToDraw.FirstOrDefault(i => i.ShowToolTip);
        
        for (var y = 0; y < 6; y++)
            for (var x = 0; x < 4; x++)
                if (_itemsToDraw.Length > x + 4 * y)
                {
                    if (_game.CurrentScreen == GameScreen.ScrollsCraftingTable && toolTipItem != default && 
                        toolTipItem.Type == ItemType.Rune)
                    {
                        if (CheckForRuneCompatibility(toolTipItem, _itemsToDraw[x+4*y]))
                        {
                            spriteBatch.Draw(_glowTexture, 
                                new Vector2(
                                    1489 + (_slotTexture.Width+9)*x + 9, 
                                    207 + (_slotTexture.Width+9)*y + 9)*Game1.ResolutionScale, 
                                null, Color.White, 0f, Vector2.Zero, 
                                Game1.ResolutionScale, SpriteEffects.None, 1f); 
                        }
                    }
                    
                    _itemsToDraw[x+4*y].Draw(spriteBatch, 
                        new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y)*Game1.ResolutionScale);
                    spriteBatch.Draw(_slotBorderTexture, 
                        new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y)*Game1.ResolutionScale, 
                        null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                    if (_itemsToDraw[x+4*y].Count > 1)
                        CountDrawer.DrawNumber(_itemsToDraw[x+4*y].Count, 
                            new Vector2(
                                1489 + (_slotTexture.Width+9)*x+_slotTexture.Width+2, 
                                207 + (_slotTexture.Width+9)*y+_slotTexture.Width-3),
                            spriteBatch);
                    
                }
        if (draggedItem != default) draggedItem.DrawAtMousePos(spriteBatch);
        if (draggedItem == default && toolTipItem != default) _game.ToolTipItem = toolTipItem;
    }

    public int GetCountOfItems(Func<Item, bool> predicate)
    {
        var item = Items.FirstOrDefault(predicate);
        if (item is not default(Item))
        {
            return item.Count;
        }
        return -1;
    }

    public bool TryGetItem(Func<Item, bool> predicate, out Item item)
    {
        var found = Items.FirstOrDefault(predicate);
        if (found is not default(Item))
        {
            item = found;
            return true;
        }

        item = null;
        return false;
    }

    private bool CheckForRuneCompatibility(Item rune1, Item rune2)
    {
        var selectedRuneInfo = rune1.ID.Split('_');
        var runeInfo = rune2.ID.Split('_');
        
        if (rune1.Type != ItemType.Rune || rune2.Type != ItemType.Rune ||
            runeInfo[1] != "finished" || runeInfo[3] != selectedRuneInfo[3])
            return false;
        if (runeInfo[2] != selectedRuneInfo[2] && runeInfo[4] == selectedRuneInfo[4] ||
            runeInfo[2] == selectedRuneInfo[2] && runeInfo[4] != selectedRuneInfo[4])
        {
            return true;
        }

        return false;
    }

    public void AddItem(Item item, int count = 1)
    {
        if (Items.Contains(item))
            Items[Items.IndexOf(item)].AddCount(count);
        else
        {
            Items.Add(item);
            Items[Items.IndexOf(item)].AddCount(count - 1);
        }
    }

    private void TryToUniteClaySmall()
    {
        var claySmall = Items.FirstOrDefault(i => i.ID == "clay_small");
        if (claySmall is not null)
        {
            AddItem(new Item(ItemsDataHolder.OtherItems.Clay), claySmall.Count / 5);
            claySmall.SubtractCount(claySmall.Count - claySmall.Count % 5);
        }
    }
    
    private void AddAllItems(int count = 1)
    {
        foreach (var rune in AllGameItems.FinishedRunes)
            AddItem(new Item(rune.Value, count), count);

        foreach (var rune in AllGameItems.UnknownRunes)
            AddItem(new Item(rune.Value, count), count);
        
        foreach (var scroll in AllGameItems.Scrolls)
            AddItem(new Item(scroll.Value, count), count);
        
        AddItem(new Item(AllGameItems.Clay, count*4), count*4);
        AddItem(new Item(AllGameItems.ClaySmall, count*4), count*4);
    }

    public void Clear() => Items.Clear();
}