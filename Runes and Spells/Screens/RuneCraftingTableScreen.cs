using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;

namespace Runes_and_Spells;

public class RuneCraftingTableScreen : IScreen
{
    private Game1 _game;
    public RuneCraftingTableScreen(Game1 game) => _game = game;

    private Texture2D backgroundTexture;
    private Texture2D _arrowTexture;
    private Texture2D _recipeBookTexture;
    private int _recipeBookPage;
    private UiSlot _dropSlot;
    private UiSlot _outputSlot;
    private RuneCraftingMiniGame miniGame;
    private UiButton _craftButton;
    private UiButton _startCraftButton;
    private UiButton _goBackButton;
    private bool _buttonFocused;
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        backgroundTexture = content.Load<Texture2D>("textures/backgrounds/background_table");
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
        miniGame = new RuneCraftingMiniGame(new Vector2(1008, 350), RuneCraftingMiniGame.Mode.X3, content);
        _craftButton = new UiButton(content.Load<Texture2D>("textures/rune_crafting_table/button_craft_default"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_craft_hovered"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_craft_pressed"),
            new Vector2(1224, 721),
            () => { miniGame.Stop(_dropSlot, _outputSlot); });
        _startCraftButton = new UiButton(content.Load<Texture2D>("textures/rune_crafting_table/button_start_default"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_hovered"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_pressed"),
            new Vector2(1224, 258),
            () => { miniGame.Start(_dropSlot); });
        _goBackButton = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            () => { _game.SetScreen(GameScreen.MainHouseScreen); });
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_dropSlot.currentItem is not null)
            _dropSlot.Update(_game.Inventory);
        miniGame.Update();
        if (!miniGame.IsActive)
            _goBackButton.Update(mouseState, ref _buttonFocused);
        
        if (miniGame.IsActive)
            _craftButton.Update(mouseState, ref _buttonFocused);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Update(mouseState, ref _buttonFocused);
        _outputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _dropSlot);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
        DrawRecipes(spriteBatch, new Vector2(90, 204));
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,239), Color.White);
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,701), Color.White);
        if (miniGame.IsActive)
            _craftButton.Draw(spriteBatch);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Draw(spriteBatch);
        if (!miniGame.IsActive)
            _goBackButton.Draw(spriteBatch);
        miniGame.Draw(spriteBatch);
        _dropSlot.Draw(spriteBatch);
        _outputSlot.Draw(spriteBatch);
        
        _game.Inventory.Draw(graphics, spriteBatch, drawer);
    }

    private void DrawRecipes(SpriteBatch spriteBatch, Vector2 startPosition)
    {
        spriteBatch.Draw(_recipeBookTexture, startPosition, Color.White);
        var index = 0;
        if (_recipeBookPage == 0)
        {
            foreach (var recipe in AllGameItems.KnownCraftRecipes.Values.Where(r => r.Size == 3))
            {
                var textureToDraw = recipe.IsFull ? recipe.FullTexture : recipe.HalfTexture;
                spriteBatch.Draw(textureToDraw, new Vector2(
                    startPosition.X + 6 + 420 * (index / 5), 
                    startPosition.Y + 33 + (textureToDraw.Height + 16) * (index % 5)), Color.White);
                index++;
            }
        }
       
    }
}