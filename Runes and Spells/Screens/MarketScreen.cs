using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;



public class MarketScreen : IScreen
{
    public enum Tab { Sell, Buy }
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
    public Tab CurrentTab { get; private set; } = Tab.Buy;
    
    private UiButton _buttonSellItem;
    private UiSlot _inputSellSlot;
    public TradingMiniGame Minigame { get; private set; }
    private SpriteFont _font40Px;
    private SpriteFont _font30Px;
    private int _sellPrice;
    public List<UiSlotForSelling> SellingSlots { get; private set; }
    private readonly Color _darkColor = new Color(17, 32, 55);
    private SoundEffect _soundSell;
    private SoundEffect _soundBuy;

    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _soundBuy = content.Load<SoundEffect>("sounds/buy_sound");
        _soundSell = content.Load<SoundEffect>("sounds/sell_sound");
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
            "Sell Tab", AllGameItems.Font30Px, new Color(255, 182, 170),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 26) _game.Introduction.Step = 27;
                CurrentTab = Tab.Sell;
                AllGameItems.ClickSound.Play();
            } );
        _buttonBuyTab = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_default"),
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_cat_buy_pressed"),
            new Vector2(103, 140),
            "Buy Tab", AllGameItems.Font30Px, new Color(255, 182, 170),
            () =>
            {
                CurrentTab = Tab.Buy;
                AllGameItems.ClickSound.Play();
            });
        _buttonGoBack = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            "Back", AllGameItems.Font30Px, new Color(212, 165, 140),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 28) _game.Introduction.Step = 29;
                _game.IsInTopDownView = true;
                _game.TopDownCore.PlayerLastLookDirection = Direction.Down;
                _game.TopDownCore.SoundTheme = AllGameItems.OutDoorTheme.CreateInstance();
                _game.TopDownCore.SoundTheme.Play();
                MediaPlayer.Play(_game.MusicsMainTheme[_game.NextSongIndex]);
            } );
        _buttonSellItem = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_sell_default"),
            content.Load<Texture2D>("textures/market_screen/button_sell_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_sell_pressed"),
            new Vector2(489, 410),
            () =>
            {
                SellItem(_sellPrice);
            } );
        _buttonStartTrade = new UiButton(
            content.Load<Texture2D>("textures/market_screen/button_trade_default"),
            content.Load<Texture2D>("textures/market_screen/button_trade_hovered"),
            content.Load<Texture2D>("textures/market_screen/button_trade_pressed"),
            new Vector2(756, 410),
            () =>
            {
                Minigame.Start(_sellPrice, -_sellPrice, Math.Max(5, _sellPrice/2));
                _inputSellSlot.Lock();
            } );
        _inputSellSlot = new UiSlot(
            new Vector2(681, 266), 
            content.Load<Texture2D>("textures/Inventory/slot_bg"), 
            _game,
            ItemType.Scroll, ItemType.Clay, ItemType.Essence, ItemType.Key, ItemType.Paper, ItemType.Rune, ItemType.ClaySmall, ItemType.UnknownRune);
        Minigame = new TradingMiniGame(this, _game);
        Minigame.LoadContent(content);
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
                new UiSlotForSelling(new Vector2(x, y), 0, _slotTexture, _slotBorderTexture, _soundBuy, _game));
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
        /*
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
        */
        for (var i = 0; i < 7; i++)
        {
            var runes1 = AllGameItems.FinishedRunes
                .Where(r => r.Value.ID.Split('_')[3] == "1")
                .ToArray();
            var item = runes1[Random.Shared.Next(runes1.Length)];
            SellingSlots[i].SetItem(item.Value, Random.Shared.Next(40, 60));
        }
        
        SellingSlots[7].SetItem(AllGameItems.Clay, Random.Shared.Next(20, 30), Random.Shared.Next(5, 10));
        SellingSlots[8].SetItem(AllGameItems.Paper, Random.Shared.Next(15, 20), Random.Shared.Next(10, 20));
        SellingSlots[9].SetItem(ItemsDataHolder.OtherItems.KeySilver, Random.Shared.Next(20, 30), Random.Shared.Next(7, 15));
        SellingSlots[10].SetItem(ItemsDataHolder.OtherItems.KeyGold, Random.Shared.Next(30, 50), Random.Shared.Next(3, 7));
    }

    public void LoadInfoToSlot(int index, ItemInfo itemInfo, int price, int count)
    {
        SellingSlots[index].SetItem(itemInfo, price, count);
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 28 || !_game.Introduction.IsPlaying)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        
        _sellPrice = _inputSellSlot.ContainsItem() ? GetItemPrice(_inputSellSlot.currentItem) : 0;

        if (CurrentTab == Tab.Buy)
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
            if (_inputSellSlot.ContainsItem() && !Minigame.IsRunning)
            {
                _buttonSellItem.Update(mouseState, ref _isButtonFocused);
                _buttonStartTrade.Update(mouseState, ref _isButtonFocused);
            }
            _game.Inventory.Update(graphics, _inputSellSlot);
        }
        
        Minigame.Update();
    }

    public void SellItem(int price)
    {
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 27) _game.Introduction.Step = 28;
        Minigame.Reset();
        _game.AddToBalance(price);
        _inputSellSlot.Unlock();
        _inputSellSlot.Clear();
        _soundSell.Play();
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_background, Vector2.Zero, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_woodPanel, new Vector2(60, 191)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_scrollsPanel, new Vector2(283, 0)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_balanceBackTexture, new Vector2(1458, 0)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.DrawString(_font40Px, $"{Game1.GetText("Wallet")}: "+_game.Balance, new Vector2(1482, 10)*Game1.ResolutionScale, 
            _darkColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _buttonGoBack.Draw(spriteBatch);
        
        DrawScrollsPrices(spriteBatch);
        
        if (CurrentTab == Tab.Buy)
            DrawBuyTab(spriteBatch);
        else
            DrawSellTab(spriteBatch);
        
        Minigame.Draw(spriteBatch, _font30Px);

        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawScrollsPrices(SpriteBatch spriteBatch)
    {
        var x = 301;
        foreach (var scrollPrice in AllGameItems.SellingScrollsPrices.OrderBy(s => s.Value))
        {
            spriteBatch.Draw(AllGameItems.ScrollsTypesInfo[scrollPrice.Key].texture2D, new Vector2(x, 0)*Game1.ResolutionScale, 
                null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            var stringSize = _font30Px.MeasureString(scrollPrice.Value.ToString());
            spriteBatch.DrawString(_font30Px, scrollPrice.Value.ToString(), 
                new Vector2(x+(float)AllGameItems.ScrollsTypesInfo[scrollPrice.Key].texture2D.Width/2 - stringSize.X/2, 69)*Game1.ResolutionScale,
                _darkColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            x += 123;
        }
    }

    private void DrawSellTab(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sellTab, new Vector2(393, 128)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        var text = Game1.GetText("Sell Tab");
        var textSize = AllGameItems.Font30Px.MeasureString(text);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, 
            new Vector2(
                393 + (270 - textSize.X)/2,
                138
            )*Game1.ResolutionScale, 
            new Color(255, 210, 200),0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        _buttonBuyTab.Draw(spriteBatch);
        _inputSellSlot.Draw(spriteBatch);
        if (_inputSellSlot.ContainsItem())
        {
            var price = GetItemPrice(_inputSellSlot.currentItem);
            var strSize = _font30Px.MeasureString($"{price}");
            spriteBatch.DrawString(_font30Px, $"{price}", 
                new Vector2(
                    _inputSellSlot.Position.X*Game1.ResolutionScale.X + (_inputSellSlot.DropRectangle.Width - strSize.X*Game1.ResolutionScale.X)/2, 
                    _inputSellSlot.Position.Y*Game1.ResolutionScale.Y + _inputSellSlot.DropRectangle.Height + 9),
                Color.Gold, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            if (!Minigame.IsRunning)
            {
                _buttonSellItem.Draw(spriteBatch);
                _buttonStartTrade.Draw(spriteBatch);
            }
        }
    }

    private int GetItemPrice(Item item)
    {
        var info = _inputSellSlot.currentItem.ID.Split('_');
        if (_inputSellSlot.currentItem.Type == ItemType.Scroll)
        {
            var element = (ScrollType)Enum.Parse(typeof(ScrollType), info[4], true);
            var power = int.Parse(info[5]);
            return AllGameItems.SellingScrollsPrices[element] + 20 * (power - 1);
        }
        if (_inputSellSlot.currentItem.Type == ItemType.Rune)
        {
            var power = int.Parse(info[3]);
            return power == 1 ? 3 : 
                power == 2 ? 5 : 
                10;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.Clay)
        {
            return 2;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.ClaySmall)
        {
            return 0;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.Paper)
        {
            return 10;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.UnknownRune)
        {
            var power = int.Parse(info[3]);
            return power == 1 ? 1 : 
                power == 2 ? 3 : 
                6;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.Key)
        {
            return item.ID.Contains("silver") ? 15 :
                item.ID.Contains("gold") ? 25 :
                100;
        }
        if (_inputSellSlot.currentItem.Type == ItemType.Essence)
        {
            var power = int.Parse(info[1]);
            return power == 1 ? 3 : 
                power == 2 ? 5 : 
                10;
        }
        return 0;
    }

    private void DrawBuyTab(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_buyTab, new Vector2(103, 128)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        var text = Game1.GetText("Buy Tab");
        var textSize = AllGameItems.Font30Px.MeasureString(text);
        spriteBatch.DrawString(AllGameItems.Font30Px, text, 
            new Vector2(
                103 + (270 - textSize.X)/2,
                138
                )*Game1.ResolutionScale, 
            new Color(255, 210, 200),0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        _buttonSellTab.Draw(spriteBatch);
        foreach (var sellSlot in SellingSlots)
        {
            sellSlot.Draw(spriteBatch, _font30Px, _darkColor, new Color(80, 17, 17));
        }

        var toolTipSlot = SellingSlots.FirstOrDefault(s => s.ShowTollTip);
        if (toolTipSlot is not null && toolTipSlot.CurrentItem is not null)
            _game.ToolTipItem = toolTipSlot.CurrentItem;
    }
}