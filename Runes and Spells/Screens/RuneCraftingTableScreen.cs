using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.MiniGames;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class RuneCraftingTableScreen : IScreen
{
    private Game1 _game;
    public RuneCraftingTableScreen(Game1 game) => _game = game;

    private Texture2D _backgroundTexture;
    private Texture2D _arrowTexture;
    private Texture2D _recipeBookTexture;
    private int _recipeBookPage;
    private UiSlot _dropSlot;
    private UiSlot _outputSlot;
    private RuneCraftingMiniGame _miniGame;
    private UiButton _craftButton;
    private UiButton _startCraftButton;
    private UiButton _goBackButton;
    private UiButton _recipePrevPage;
    private UiButton _recipeNextPage;
    private bool _buttonFocused;
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/backgrounds/background_table");
        _recipeBookTexture = content.Load<Texture2D>("textures/rune_crafting_table/recipes_book");
        _arrowTexture = content.Load<Texture2D>("textures/Inventory/arrow_down");
        _dropSlot = new UiSlot(
            new Vector2(1128, 128), 
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"), 
            true,
            ItemType.Clay);
        _outputSlot = new UiSlot(
            new Vector2(1128, 812), 
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"), 
            false);
        _miniGame = new RuneCraftingMiniGame(new Vector2(1008, 350), RuneCraftingMiniGame.Mode.X3, content, _game);
        _craftButton = new UiButton(content.Load<Texture2D>("textures/rune_crafting_table/button_craft_default"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_craft_hovered"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_craft_pressed"),
            new Vector2(1224, 721),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 7) _game.Introduction.Step = 8;
                _miniGame.Stop(_dropSlot, _outputSlot);
            });
        _startCraftButton = new UiButton(content.Load<Texture2D>("textures/rune_crafting_table/button_start_default"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_hovered"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_pressed"),
            new Vector2(1224, 258),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 5) _game.Introduction.Step = 6;
                _miniGame.Start(_dropSlot);
            });
        _goBackButton = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            () =>
            {
                _game.SetScreen(GameScreen.MainHouseScreen);
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 8)
                {
                    _game.Introduction.Step = 9;
                    _outputSlot.Clear();
                    _game.Inventory.Clear();
                    _game.Inventory.AddItem(new Item(AllGameItems.UnknownRunes["rune_unknown_grass_1_2"]));
                }
            });
        _recipeNextPage = new UiButton(content.Load<Texture2D>("textures/buttons/button_next_page_default"),
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"),
            new Vector2(861, 207),
            () => _recipeBookPage++);
        _recipePrevPage = new UiButton(content.Load<Texture2D>("textures/buttons/button_previous_page_default"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            new Vector2(93, 207),
            () => _recipeBookPage--);
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_dropSlot.currentItem is not null)
            _dropSlot.Update(_game.Inventory);
        _miniGame.Update();
        
        if (_miniGame.IsActive)
            _craftButton.Update(mouseState, ref _buttonFocused);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Update(mouseState, ref _buttonFocused);
        
        _outputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _dropSlot);
        if (_recipeBookPage > 0)
            _recipePrevPage.Update(mouseState, ref _buttonFocused);
        if (AllGameItems.KnownRunesCraftRecipes.Count > 10 * (1 + _recipeBookPage))
            _recipeNextPage.Update(mouseState, ref _buttonFocused);
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 4 && _dropSlot.ContainsItem())
        {
            _game.Introduction.Step = 5;
        }
        
        if (_game.Introduction.IsPlaying)
        {
            if (_game.Introduction.Step == 8)
                _goBackButton.Update(mouseState, ref _buttonFocused);
            if (_game.Introduction.Step == 6 && 
                !_miniGame.GetCurrentScheme()[0] && !_miniGame.GetCurrentScheme()[1] && !_miniGame.GetCurrentScheme()[2] &&
                !_miniGame.GetCurrentScheme()[3] && _miniGame.GetCurrentScheme()[4] && !_miniGame.GetCurrentScheme()[5] &&
                !_miniGame.GetCurrentScheme()[6] && !_miniGame.GetCurrentScheme()[7] && !_miniGame.GetCurrentScheme()[8])
            {
                _game.Introduction.Step = 7;
            }
        }
        else if (!_miniGame.IsActive)
            _goBackButton.Update(mouseState, ref _buttonFocused);
        
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 6 && 
            !_miniGame.GetCurrentScheme()[0] && !_miniGame.GetCurrentScheme()[1] && !_miniGame.GetCurrentScheme()[2] &&
            !_miniGame.GetCurrentScheme()[3] && _miniGame.GetCurrentScheme()[4] && !_miniGame.GetCurrentScheme()[5] &&
            !_miniGame.GetCurrentScheme()[6] && !_miniGame.GetCurrentScheme()[7] && !_miniGame.GetCurrentScheme()[8])
        {
            _game.Introduction.Step = 7;
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);
        DrawRecipes(spriteBatch, new Vector2(90, 204));
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,239), Color.White);
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,701), Color.White);
        if (_miniGame.IsActive)
            _craftButton.Draw(spriteBatch);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Draw(spriteBatch);
        if (!_miniGame.IsActive)
            _goBackButton.Draw(spriteBatch);
        _miniGame.Draw(spriteBatch);
        _dropSlot.Draw(spriteBatch);
        _outputSlot.Draw(spriteBatch);
        _game.Inventory.Draw(graphics, spriteBatch, drawer);
    }

    private void DrawRecipes(SpriteBatch spriteBatch, Vector2 startPosition)
    {
        spriteBatch.Draw(_recipeBookTexture, startPosition, Color.White);
        if (_recipeBookPage > 0)
            _recipePrevPage.Draw(spriteBatch);
        if (AllGameItems.KnownRunesCraftRecipes.Count > 10 * (1 + _recipeBookPage))
            _recipeNextPage.Draw(spriteBatch);
        var index = 0;
        foreach (var recipe in AllGameItems.KnownRunesCraftRecipes.Values.Where(r => r.Size == 3).Skip(10*_recipeBookPage).Take(10))
        {
            var textureToDraw = recipe.IsFull ? recipe.FullTexture : recipe.HalfTexture;
            spriteBatch.Draw(textureToDraw, new Vector2(
                startPosition.X + 6 + 420 * (index / 5), 
                startPosition.Y + 33 + (textureToDraw.Height + 16) * (index % 5)), Color.White);
            index++;
        }
    }
}