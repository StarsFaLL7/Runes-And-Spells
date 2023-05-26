using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;

namespace Runes_and_Spells.Screens;

public class ScrollCraftingScreen : IScreen
{
    private readonly Game1 _game;
    public ScrollCraftingScreen(Game1 game) => _game = game;
    private bool _isButtonFocused;
    
    private Texture2D _backgroundTexture;
    private Texture2D _recipeBookBackTexture;
    private Texture2D _arrowRightTexture;
    private Texture2D _arrowLeftTexture;
    private Texture2D _woodTabletBackTexture;
    private UiButton _buttonGoBack;
    private UiButton _buttonStartCraft;
    private UiButton _buttonOpenNotePad;
    private UiButton _buttonCloseNotePad;
    private UiSlot _inputSlot1;
    private UiSlot _inputSlot2;
    private UiSlot _outputSlot;
    private bool _isNotePadVisible;
    private ScrollCraftingMiniGame _miniGame;
    private int _RecipeBookPage;
    
    public void Initialize()
    {
        
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/backgrounds/background_table");
        _recipeBookBackTexture = content.Load<Texture2D>("textures/scroll_crafting_table/recipe_book_back");
        _woodTabletBackTexture = content.Load<Texture2D>("textures/scroll_crafting_table/tablet_wood_back");
        _arrowRightTexture = content.Load<Texture2D>("textures/Inventory/arrow");
        _arrowLeftTexture = content.Load<Texture2D>("textures/Inventory/arrow_left");
        _buttonGoBack = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 23)
                {
                    _game.Introduction.Step = 24;
                    _outputSlot.Clear();
                }
                _game.SetScreen(GameScreen.MainHouseScreen);
            } );
        _buttonOpenNotePad = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_hovered"),
            new Vector2(111, 442),
            () => _isNotePadVisible = true );
        _buttonCloseNotePad = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_hovered"),
            new Vector2(41, 442),
            () => _isNotePadVisible = false );
        
        var slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg_stone");
        _inputSlot1 = new UiSlot(new Vector2(581, 323), slotTexture, true, ItemType.Rune);
        _inputSlot2 = new UiSlot(new Vector2(1045, 323), slotTexture, true, ItemType.Rune);
        _outputSlot = new UiSlot(new Vector2(813, 323), slotTexture, true, ItemType.Paper);
        
        _miniGame = new ScrollCraftingMiniGame(_outputSlot, _inputSlot1, _inputSlot2, _game);
        _miniGame.Initialize(content);
        _buttonStartCraft = new UiButton(
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_default"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_hovered"),
            content.Load<Texture2D>("textures/rune_crafting_table/button_start_pressed"),
            new Vector2(755, 489),
            () => _miniGame.Start() );
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_isNotePadVisible)
            _buttonCloseNotePad.Update(mouseState, ref _isButtonFocused);
        else
            _buttonOpenNotePad.Update(mouseState, ref _isButtonFocused);
        
        if (!_miniGame.IsActive)
        {
            if (_game.Introduction.IsPlaying && _game.Introduction.Step == 23 || !_game.Introduction.IsPlaying)
                _buttonGoBack.Update(mouseState, ref _isButtonFocused);

            if (_inputSlot1.ContainsItem() && _inputSlot2.ContainsItem() && _outputSlot.ContainsItem() && _outputSlot.currentItem.Type == ItemType.Paper)
                _buttonStartCraft.Update(mouseState, ref _isButtonFocused);
        }
        else _miniGame.Update(mouseState, ref _isButtonFocused);
        
        _inputSlot1.Update(_game.Inventory);
        _inputSlot2.Update(_game.Inventory);
        _outputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _inputSlot1, _inputSlot2, _outputSlot);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);
        spriteBatch.Draw(_woodTabletBackTexture, new Vector2(268, 460), Color.White);
        spriteBatch.Draw(_arrowRightTexture, new Vector2(697, 323), Color.White);
        spriteBatch.Draw(_arrowLeftTexture, new Vector2(929, 323), Color.White);
        _inputSlot1.Draw(spriteBatch);
        _inputSlot2.Draw(spriteBatch);
        _outputSlot.Draw(spriteBatch);
        if (!_miniGame.IsActive)
        {
            _buttonGoBack.Draw(spriteBatch);
            if (_inputSlot1.ContainsItem() && _inputSlot2.ContainsItem() && _outputSlot.ContainsItem() && _outputSlot.currentItem.Type == ItemType.Paper)
                _buttonStartCraft.Draw(spriteBatch);
        }
        else _miniGame.Draw(spriteBatch);

        if (_isNotePadVisible)
        {
            _buttonCloseNotePad.Draw(spriteBatch);
            DrawRecipes(spriteBatch);
        }
        else
            _buttonOpenNotePad.Draw(spriteBatch);
        
        _game.Inventory.Draw(graphics, spriteBatch, drawer);
    }

    private void DrawRecipes(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_recipeBookBackTexture, new Vector2(331, 276), Color.White);
        var x = 337;
        var y = 307;
        foreach (var recipe in AllGameItems.ScrollsRecipes.Where(s => s.Value.isVisible).Take((_RecipeBookPage+1)*14))
        {
            spriteBatch.Draw(recipe.Value.Texture, new Vector2(x, y), Color.White);
            y += recipe.Value.Texture.Height + 12;
            if (y > 860)
            {
                x = 859;
                y = 307;
            }
        }
    }
}