using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;



public class MarketScreen : IScreen
{
    private enum Tab { Sell, Buy }
    private readonly Game1 _game;
    public MarketScreen(Game1 game) => _game = game;
    private Texture2D _background;
    private Texture2D _woodPanel;
    private Texture2D _scrollsPanel;
    private Texture2D _sellTab;
    private Texture2D _buyTab;
    private Texture2D _balanceBackTexture;
    private Texture2D _slotTexture;
    private Texture2D _slotBorderTexture;
    private UiButton _buttonSellTab;
    private UiButton _buttonBuyTab;
    private UiButton _buttonGoBack;
    private UiButton _buttonStartTrade;
    private bool _isButtonFocused;
    private Tab _currentTab = Tab.Buy;
    
    private UiButton _buttonSellItem;
    private UiSlot _inputSellSlot;
    private TradingMiniGame _minigame;
    private SpriteFont _font40Px;
    private SpriteFont _font30Px;
    private int _sellPrice;
    public List<UiSlotForSelling> SellingSlots { get; private set; }
    private readonly Color _darkColor = new Color(17, 32, 55);
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _background = content.Load<Texture2D>("textures/market_screen/background");
        _woodPanel = content.Load<Texture2D>("textures/market_screen/back_wood");
        _scrollsPanel = content.Load<Texture2D>("textures/market_screen/scroll_table");
        _sellTab = content.Load<Texture2D>("textures/market_screen/cat_sell_selected");
        _buyTab = content.Load<Texture2D>("textures/market_screen/cat_buy_selected");
        _balanceBackTexture = content.Load<Texture2D>("textures/market_screen/balance_back");
        _slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg");
        _slotBorderTexture = content.Load<Texture2D>("textures/Inventory/slot_border");
        _buttonSellTab = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_cat_sell_default"),
            content.Load<Texture2D>("textures/market_screen/button_cat_sell_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_cat_sell_pressed"),
            new Vector2(393, 140),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 26) _game.Introduction.Step = 27;
                _currentTab = Tab.Sell;
            } );
        _buttonBuyTab = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_default"),
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_pressed"),
            new Vector2(103, 140),
            () => _currentTab = Tab.Buy);
        _buttonGoBack = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 28) _game.Introduction.Step = 29;
                _game.IsInTopDownView = true;
                _game.TopDownCore.PlayerLastLookDirection = Direction.Down;
            } );
        _buttonSellItem = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_sell_default"),
            content.Load<Texture2D>("textures/market_screen/button_sell_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_sell_pressed"),
            new Vector2(489, 410),
            () =>
            {
                Enum.TryParse<ScrollType>(_inputSellSlot.currentItem.ID.Split('_')[4],true, out var type);
                SellItem(AllGameItems.SellingScrollsPrices[type] + 
                         (int.Parse(_inputSellSlot.currentItem.ID.Split('_')[5])-1)*20);
            } );
        _buttonStartTrade = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_trade_default"),
            content.Load<Texture2D>("textures/market_screen/button_trade_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_trade_pressed"),
            new Vector2(756, 410),
            () =>
            {
                _minigame.Start(_sellPrice);
                _inputSellSlot.Lock();
            } );
        _inputSellSlot = new UiSlot(
            new Vector2(681, 266), 
            content.Load<Texture2D>("textures/Inventory/slot_bg"), 
            true,
            ItemType.Scroll);
        _minigame = new TradingMiniGame();
        _minigame.LoadContent(content);
        _font30Px = content.Load<SpriteFont>("16PixelTimes30px");
        _font40Px = content.Load<SpriteFont>("16PixelTimes40px");
        InitializeSellSlots();
    }
    
    private void InitializeSellSlots()
    {
        SellingSlots = new List<UiSlotForSelling>();
        var x = 96;
        var y = 229;
        for (var i = 0; i < 14; i++)
        {
            SellingSlots.Add(
                new UiSlotForSelling(new Vector2(x, y), 0, _slotTexture, _slotBorderTexture));
            x += _slotTexture.Width + 100;
            if (i == 6)
            {
                x = 96;
                y += _slotTexture.Height + 50;
            }
        }
        FillSellSlots();
    }

    public void FillSellSlots()
    {
        for (var i = 0; i < 5; i++)
        {
            var unknownRunes1 = AllGameItems.UnknownRunes
                .Where(r => 
                    r.Value.ID != "rune_unknown_failed" && 
                    r.Value.ID.Split('_')[3] == "1")
                .ToArray();
            var item = unknownRunes1[Random.Shared.Next(unknownRunes1.Length)];
            SellingSlots[i].SetItem(item.Value, Random.Shared.Next(20, 40));
        }
        for (var i = 5; i < 10; i++)
        {
            var runes1 = AllGameItems.FinishedRunes
                .Where(r => r.Value.ID.Split('_')[3] == "1")
                .ToArray();
            var item = runes1[Random.Shared.Next(runes1.Length)];
            SellingSlots[i].SetItem(item.Value, Random.Shared.Next(40, 60));
        }
        
        SellingSlots[10].SetItem(AllGameItems.Clay, Random.Shared.Next(15, 20), Random.Shared.Next(5, 10));
        SellingSlots[11].SetItem(AllGameItems.Paper, Random.Shared.Next(10, 15), Random.Shared.Next(10, 20));
        SellingSlots[12].SetItem(ItemsDataHolder.OtherItems.KeySilver, Random.Shared.Next(10, 25), Random.Shared.Next(7, 15));
        SellingSlots[13].SetItem(ItemsDataHolder.OtherItems.KeyGold, Random.Shared.Next(30, 50), Random.Shared.Next(3, 7));

    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 28 || !_game.Introduction.IsPlaying)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        
        if (_currentTab == Tab.Buy)
        {
            if (_game.Introduction.IsPlaying && _game.Introduction.Step == 26 || !_game.Introduction.IsPlaying)
                _buttonSellTab.Update(mouseState, ref _isButtonFocused);

            _game.Inventory.Update(graphics);
            foreach (var slot in SellingSlots)
                slot.Update(mouseState, ref _isButtonFocused, _game);
        }
        else
        {
            if (!_game.Introduction.IsPlaying)
                _buttonBuyTab.Update(mouseState, ref _isButtonFocused);
            
            _inputSellSlot.Update(_game.Inventory);
            if (_inputSellSlot.ContainsItem() && !_minigame.IsRunning)
            {
                _buttonSellItem.Update(mouseState, ref _isButtonFocused);
                _buttonStartTrade.Update(mouseState, ref _isButtonFocused);
            }
            _game.Inventory.Update(graphics, _inputSellSlot);
        }
        
        _minigame.Update();
        var kb = Keyboard.GetState();
        if ((_minigame.Score is >= 3500 or <= -2500 || kb.IsKeyDown(Keys.Space) || kb.IsKeyDown(Keys.Enter)) && _minigame.IsRunning)
        {
            Enum.TryParse<ScrollType>(_inputSellSlot.currentItem.ID.Split('_')[4],true, out var type);
            SellItem(AllGameItems.SellingScrollsPrices[type] + 
                     (int.Parse(_inputSellSlot.currentItem.ID.Split('_')[5])-1)*20 + _minigame.Stop()
                     );
        }
    }

    private void SellItem(int price)
    {
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 27) _game.Introduction.Step = 28;
        _minigame.Reset();
        _game.AddToBalance(price);
        _inputSellSlot.Unlock();
        _inputSellSlot.Clear();
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        spriteBatch.Draw(_woodPanel, new Vector2(60, 191), Color.White);
        spriteBatch.Draw(_scrollsPanel, new Vector2(283, 0), Color.White);
        spriteBatch.Draw(_balanceBackTexture, new Vector2(1458, 0), Color.White);
        spriteBatch.DrawString(_font40Px, "Кошель: "+_game.Balance, new Vector2(1482, 10), _darkColor);
        _buttonGoBack.Draw(spriteBatch);
        
        DrawScrollsPrices(spriteBatch);
        
        if (_currentTab == Tab.Buy)
            DrawBuyTab(spriteBatch);
        else
            DrawSellTab(spriteBatch);
        
        _minigame.Draw(spriteBatch, _font30Px);

        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawScrollsPrices(SpriteBatch spriteBatch)
    {
        var x = 301;
        foreach (var scrollPrice in AllGameItems.SellingScrollsPrices.OrderBy(s => s.Value))
        {
            spriteBatch.Draw(AllGameItems.ScrollsTypesInfo[scrollPrice.Key].texture2D, new Vector2(x, 0), Color.White);
            var stringSize = _font30Px.MeasureString(scrollPrice.Value.ToString());
            spriteBatch.DrawString(_font30Px, scrollPrice.Value.ToString(), 
                new Vector2(x+AllGameItems.ScrollsTypesInfo[scrollPrice.Key].texture2D.Width/2 - stringSize.X/2, 69),
                _darkColor);
            x += 123;
        }
    }

    private void DrawSellTab(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sellTab, new Vector2(393, 128), Color.White);
        _buttonBuyTab.Draw(spriteBatch);
        _inputSellSlot.Draw(spriteBatch);
        if (_inputSellSlot.ContainsItem() && !_minigame.IsRunning)
        {
            _buttonSellItem.Draw(spriteBatch);
            _buttonStartTrade.Draw(spriteBatch);
        }
    }

    private void DrawBuyTab(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_buyTab, new Vector2(103, 128), Color.White);
        _buttonSellTab.Draw(spriteBatch);
        foreach (var sellSlot in SellingSlots)
        {
            sellSlot.Draw(spriteBatch, _font30Px, _darkColor);
        }
    }
}