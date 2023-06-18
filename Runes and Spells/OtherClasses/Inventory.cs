using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.classes;

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
    
    private Texture2D _backgroundTexture;
    private Texture2D _slotTexture;
    private Texture2D _slotBorderTexture;
    private Texture2D _toolTipBgTexture;
    private Texture2D _toolTipBgOutlineTexture;

    private UiButton _arrowSmallRightButton;
    private UiButton _arrowSmallLeftButton;
    private UiButton _arrowBigRightButton;
    private UiButton _arrowBigLeftButton;
    private Dictionary<Tab, Texture2D> _tabTitleTextures;
    private List<Texture2D> _pageTitleTextures;
    private bool _isObjectFocused;
    private Color _toolTipTextColor;
    
    private int _currentPage;
    private Tab _currentTab;
    private Item[] _itemsToDraw = {};
    public List<Item> Items { get; } = new ();
    public bool IsOpened { get; private set; }
    private UiButton _buttonCloseInv;
    private UiButton _buttonOpenInv;

    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _toolTipTextColor = Color.FromNonPremultiplied(48, 56, 67, 255);
        _toolTipBgTexture = content.Load<Texture2D>("textures/Inventory/items/ToolTipBg");
        _toolTipBgOutlineTexture = content.Load<Texture2D>("textures/Inventory/items/ToolTipBgOutline");
        _backgroundTexture = content.Load<Texture2D>("textures/Inventory/inv_back");
        _slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg");
        _slotBorderTexture = content.Load<Texture2D>("textures/Inventory/slot_border");
        _arrowBigRightButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_big_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_big_right_pressed"),
            new Vector2(1843, 136),
            () =>
            {
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
            }
        );
        _arrowSmallRightButton = new UiButton(content.Load<Texture2D>("textures/Inventory/arrow_small_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_right_default"),
            content.Load<Texture2D>("textures/Inventory/arrow_small_right_pressed"),
            new Vector2(1768, 923),
            () =>
            {
                if (_currentPage < _pageTitleTextures.Count - 1)
                    _currentPage++;
            }
        );
        _buttonCloseInv = new UiButton(content.Load<Texture2D>("textures/Inventory/button_close_inv_default"),
            content.Load<Texture2D>("textures/Inventory/button_close_inv_hovered"),
            content.Load<Texture2D>("textures/Inventory/button_close_inv_pressed"),
            new Vector2(Game1.ScreenWidth - _backgroundTexture.Width - 42, Game1.ScreenHeight/2 - 45),
            () => IsOpened = false);
        _buttonOpenInv = new UiButton(content.Load<Texture2D>("textures/Inventory/button_open_inv_default"),
            content.Load<Texture2D>("textures/Inventory/button_open_inv_hovered"),
            content.Load<Texture2D>("textures/Inventory/button_open_inv_pressed"),
            new Vector2(Game1.ScreenWidth - 42, Game1.ScreenHeight/2 - 45),
            () => IsOpened = true);

        
        _pageTitleTextures = new List<Texture2D>();
        for (var i = 0; i < 8; i++)
            _pageTitleTextures.Add(content.Load<Texture2D>($"textures/Inventory/title_page{i}"));
        
        _tabTitleTextures = new Dictionary<Tab, Texture2D>()
        {
            {Tab.Other, content.Load<Texture2D>("textures/Inventory/title_other")},
            {Tab.Runes, content.Load<Texture2D>("textures/Inventory/title_runes")},
            {Tab.Scrolls, content.Load<Texture2D>("textures/Inventory/title_scrolls")}
        };
        //AddAllItems(5);
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
        // var kb = Keyboard.GetState().GetPressedKeys();
        // if (kb.Contains(Keys.H) && kb.Contains(Keys.A) && kb.Contains(Keys.C) && kb.Contains(Keys.K)) AddAllItems();
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
            new Vector2(Game1.ScreenWidth - _backgroundTexture.Width,
                (Game1.ScreenHeight - _backgroundTexture.Height)/2), Color.White);
        
        if (_currentPage > 0)
            _arrowSmallLeftButton.Draw(spriteBatch);
        if (_itemsToDraw.Length > 24)
            _arrowSmallRightButton.Draw(spriteBatch);
        _arrowBigLeftButton.Draw(spriteBatch);
        _arrowBigRightButton.Draw(spriteBatch);
        spriteBatch.Draw(_tabTitleTextures[_currentTab], new Vector2(1545, 132), Color.White);
        spriteBatch.Draw(_pageTitleTextures[_currentPage], new Vector2(1620, 924), Color.White);
        for (var y = 0; y < 6; y++)
            for (var x = 0; x < 4; x++)
                spriteBatch.Draw(_slotTexture, new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y), Color.White);
        
        var draggedItem = _itemsToDraw.FirstOrDefault(i => i.IsBeingDragged);
        for (var y = 0; y < 6; y++)
            for (var x = 0; x < 4; x++)
                if (_itemsToDraw.Length > x + 4 * y)
                {
                    _itemsToDraw[x+4*y].Draw(spriteBatch, new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y));
                    spriteBatch.Draw(_slotBorderTexture, new Vector2(1489 + (_slotTexture.Width+9)*x, 207 + (_slotTexture.Width+9)*y), Color.White);
                    if (_itemsToDraw[x+4*y].Count > 1)
                        CountDrawer.DrawNumber(_itemsToDraw[x+4*y].Count, 
                            new Vector2(1489 + (_slotTexture.Width+9)*x+_slotTexture.Width+2, 207 + (_slotTexture.Width+9)*y+_slotTexture.Width-3),
                            spriteBatch);
                    
                }
        var toolTipItem = _itemsToDraw.FirstOrDefault(i => i.ShowToolTip);
        if (draggedItem != default) draggedItem.DrawAtMousePos(spriteBatch);
        if (draggedItem == default && toolTipItem != default) DrawToolTip(toolTipItem, graphics, spriteBatch);
    }

    private void DrawToolTip(Item toolTipItem, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        var strSize = AllGameItems.ToolTipFont.MeasureString(toolTipItem.ToolTipText);
        var rect = new Rectangle(
            (_toolTipBgTexture.Width - (int)strSize.X) / 2 - 16, 
            (_toolTipBgTexture.Height - (int)strSize.Y) / 2 - 16,
            (int)strSize.X + 16, (int)strSize.Y + 6);
        
        var posToDraw = new Vector2(Mouse.GetState().X + 12, Mouse.GetState().Y + 12);
        if (Mouse.GetState().X > graphics.PreferredBackBufferWidth - rect.Width - 20)
            posToDraw = new Vector2(Mouse.GetState().X - rect.Width, Mouse.GetState().Y + 12);
        
        spriteBatch.Draw(_toolTipBgOutlineTexture, new Vector2(posToDraw.X - 3, posToDraw.Y - 3), 
            new Rectangle(rect.X-6, rect.Y-6, rect.Width + 6, rect.Height + 6), Color.White);
        spriteBatch.Draw(_toolTipBgTexture, posToDraw, rect, Color.White);

        spriteBatch.DrawString(AllGameItems.ToolTipFont, toolTipItem.ToolTipText,
            new Vector2(posToDraw.X+9, posToDraw.Y+3), _toolTipTextColor, 0f, Vector2.Zero, 
            1f,SpriteEffects.None, 0f);
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