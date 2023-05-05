using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;

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
    private Texture2D _toolTipBgTexture;
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

    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _toolTipTextColor = Color.FromNonPremultiplied(48, 56, 67, 255);
        _toolTipBgTexture = content.Load<Texture2D>("textures/Inventory/items/ToolTipBg");
        _backgroundTexture = content.Load<Texture2D>("textures/Inventory/inv_back");
        _slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg");
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
        _pageTitleTextures = new List<Texture2D>();
        for (var i = 0; i < 3; i++)
            _pageTitleTextures.Add(content.Load<Texture2D>($"textures/Inventory/title_page{i}"));
        
        _tabTitleTextures = new Dictionary<Tab, Texture2D>()
        {
            {Tab.Other, content.Load<Texture2D>("textures/Inventory/title_other")},
            {Tab.Runes, content.Load<Texture2D>("textures/Inventory/title_runes")},
            {Tab.Scrolls, content.Load<Texture2D>("textures/Inventory/title_scrolls")}
        };
        // Заполнение инвентаря
        foreach (var rune in AllGameItems.FinishedRunes.Where(r => r.Key.Split('_')[3] == "1"))
        {
            Items.Add(new Item(rune.Value.Type, rune.Value.Texture, rune.Key, rune.Value.IsDraggable, 5));
        }
        foreach (var rune in AllGameItems.UnknownRunes
                     .Where(r => r.Key.Split('_').Length > 3 && r.Key.Split('_')[3] == "1"))
        {
            Items.Add(new Item(rune.Value.Type, rune.Value.Texture, rune.Key, rune.Value.IsDraggable, 5));
        }
        Items.Add(new Item(ItemType.Clay, content.Load<Texture2D>("textures/Inventory/items/piece_of_clay"), "clay", true, 25));
    }

    public void Update(GraphicsDeviceManager graphics, params UiSlot[] dropableSlots)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Released)
            _isObjectFocused = false;
        _arrowBigLeftButton.Update(mouseState, ref _isObjectFocused);
        _arrowBigRightButton.Update(mouseState, ref _isObjectFocused);
        _arrowSmallLeftButton.Update(mouseState, ref _isObjectFocused);
        _arrowSmallRightButton.Update(mouseState, ref _isObjectFocused);
        if (_currentTab is Tab.Runes) 
            _itemsToDraw = Items
                .Where(item => item.Type is ItemType.Rune or ItemType.UnknownRune && (item.Count > 0 || item.IsBeingDragged))
                .Skip(_currentPage*24)
                .ToArray();
        else if (_currentTab is Tab.Other)
            _itemsToDraw = Items
                .Where(item => item.Type is ItemType.Catalyst or ItemType.Clay or ItemType.Paper or ItemType.ClaySmall 
                               && (item.Count > 0 || item.IsBeingDragged))
                .Skip(_currentPage*24)
                .ToArray();
        else if (_currentTab is Tab.Scrolls)
            _itemsToDraw = Items
                .Where(item => item.Type is ItemType.Scroll && (item.Count > 0 || item.IsBeingDragged))
                .Skip(_currentPage * 24)
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
            
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        drawer.Draw(spriteBatch, graphics, _backgroundTexture, Position.MiddleRight, new Vector2(0,0), 0, 0);
        spriteBatch.Draw(_arrowSmallLeftButton.ActualTexture(), _arrowSmallLeftButton.Position, Color.White);
        spriteBatch.Draw(_arrowSmallRightButton.ActualTexture(), _arrowSmallRightButton.Position, Color.White);
        spriteBatch.Draw(_arrowBigLeftButton.ActualTexture(), _arrowBigLeftButton.Position, Color.White);
        spriteBatch.Draw(_arrowBigRightButton.ActualTexture(), _arrowBigRightButton.Position, Color.White);
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
                    if (_itemsToDraw[x+4*y].Count > 1)
                        CountDrawer.DrawNumber(_itemsToDraw[x+4*y].Count, 
                            new Vector2(1489 + (_slotTexture.Width+9)*x+_slotTexture.Width+2, 207 + (_slotTexture.Width+9)*y+_slotTexture.Width-3),
                            spriteBatch);
                }
        var toolTipItem = _itemsToDraw.FirstOrDefault(i => i.showToolTip);
        if (draggedItem != default) draggedItem.DrawAtMousePos(spriteBatch);
        if (draggedItem == default && toolTipItem != default) DrawToolTip(toolTipItem, graphics, spriteBatch);
    }

    private void DrawToolTip(Item toolTipItem, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        var texts = toolTipItem.ID.Split('_');
        var posToDraw = new Vector2(Mouse.GetState().X + 10, Mouse.GetState().Y + 10);
        if (Mouse.GetState().X > graphics.PreferredBackBufferWidth - _toolTipBgTexture.Width)
            posToDraw = new Vector2(Mouse.GetState().X - _toolTipBgTexture.Width, Mouse.GetState().Y + 10);
        spriteBatch.Draw(_toolTipBgTexture, posToDraw, Color.White);
        switch (toolTipItem.Type)
        {
            case ItemType.Rune:
                spriteBatch.DrawString(AllGameItems.ToolTipFont, $"Тип: {texts[2]}\nСила: {texts[3]}\nВид: {texts[4]}",
                    new Vector2(posToDraw.X+6, posToDraw.Y), _toolTipTextColor, 0f, Vector2.Zero, 
                    1f,SpriteEffects.None, 0f);
                break;
            case ItemType.Clay:
                spriteBatch.DrawString(AllGameItems.ToolTipFont, "Глина",
                    new Vector2(posToDraw.X+6, posToDraw.Y), _toolTipTextColor, 0f, Vector2.Zero, 
                    1f,SpriteEffects.None, 0f);
                break;
            case ItemType.ClaySmall:
                spriteBatch.DrawString(AllGameItems.ToolTipFont, "Кусочки глины",
                    new Vector2(posToDraw.X+6, posToDraw.Y), _toolTipTextColor, 0f, Vector2.Zero, 
                    1f,SpriteEffects.None, 0f);
                break;
            case ItemType.UnknownRune:
                if (AllGameItems.KnownCraftRecipes.ContainsKey(toolTipItem.ID[13..]) && AllGameItems.KnownCraftRecipes[toolTipItem.ID[13..]].IsFull)
                {
                    spriteBatch.DrawString(AllGameItems.ToolTipFont, $"Тип: {texts[2]}\nСила: {texts[3]}\nВид: {texts[4]}",
                        new Vector2(posToDraw.X+6, posToDraw.Y), _toolTipTextColor, 0f, Vector2.Zero, 
                        1f,SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.DrawString(AllGameItems.ToolTipFont, "Неизвестная\nруна",
                        new Vector2(posToDraw.X+6, posToDraw.Y), _toolTipTextColor, 0f, Vector2.Zero, 
                        1f,SpriteEffects.None, 0f);
                }
                break;
        }
    }

    public void AddItem(Item item)
    {
        if (Items.Contains(item))
            Items[Items.IndexOf(item)].AddOne();
        else
            Items.Add(item);
    }
}