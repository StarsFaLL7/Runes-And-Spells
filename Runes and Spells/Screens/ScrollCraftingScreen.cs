using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class ScrollCraftingScreen : IScreen
{
    private readonly Game1 _game;
    public ScrollCraftingScreen(Game1 game) => _game = game;
    private bool _isButtonFocused;
    
    private Texture2D _backgroundTexture;
    private Texture2D _arrowRightTexture;
    private Texture2D _arrowLeftTexture;
    private Texture2D _woodTabletBackTexture;
    private UiButton _buttonGoBack;
    private UiButton _buttonStartCraft;
    private UiSlot _inputSlot1;
    private UiSlot _inputSlot2;
    private UiSlot _outputSlot;
    private ScrollCraftingMiniGame _miniGame;
    
    private Texture2D _recipeBookBackTexture;
    private Texture2D _recipeArrowTexture;
    private Texture2D _recipePlusTexture;
    private Texture2D _recipeQuestionMarkTexture;
    private SpriteFont _recipeSpriteFont;
    private SpriteFont _recipeHeadingSpriteFont;
    private UiButton _buttonRecipeNextPage;
    private UiButton _buttonRecipePreviousPage;
    private Dictionary<ScrollType, Texture2D> _recipeBookColors;
    private bool _isNotePadVisible;
    private UiButton _buttonOpenNotePad;
    private UiButton _buttonCloseNotePad;
    private int _RecipeBookPage;
    
    
    public void Initialize()
    {
        
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/backgrounds/background_table");
        _woodTabletBackTexture = content.Load<Texture2D>("textures/scroll_crafting_table/tablet_wood_back");
        _arrowRightTexture = content.Load<Texture2D>("textures/Inventory/arrow");
        _arrowLeftTexture = content.Load<Texture2D>("textures/Inventory/arrow_left");
        
        _recipeBookBackTexture = content.Load<Texture2D>("textures/scroll_crafting_table/recipe_book_back");
        _recipeArrowTexture = content.Load<Texture2D>("textures/scroll_crafting_table/recipes/arrow");
        _recipePlusTexture = content.Load<Texture2D>("textures/scroll_crafting_table/recipes/plus");
        _recipeQuestionMarkTexture = content.Load<Texture2D>("textures/scroll_crafting_table/recipes/question_mark");
        _recipeSpriteFont = content.Load<SpriteFont>("16PixelTimes20px");
        _recipeHeadingSpriteFont = content.Load<SpriteFont>("16PixelTimes30px");
        _buttonRecipeNextPage = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_next_page_default"), 
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"), 
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"), 
            new Vector2(1318, 279),
            () => _RecipeBookPage++);
        _buttonRecipePreviousPage = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_previous_page_default"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            new Vector2(334, 279),
            () => _RecipeBookPage--);
        _recipeBookColors = new Dictionary<ScrollType, Texture2D>();
        foreach (var scrollType in ItemsDataHolder.Scrolls.ScrollsTypesInfo.Keys)
            _recipeBookColors.Add(scrollType, content.Load<Texture2D>($"textures/scroll_crafting_table/recipe_book/book_{scrollType.ToString().ToLower()}"));

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
        {
            _buttonCloseNotePad.Update(mouseState, ref _isButtonFocused);
            if (ItemsDataHolder.Scrolls.ScrollsTypesInfo.Count - 1 > _RecipeBookPage)
            {
                _buttonRecipeNextPage.Update(mouseState, ref _isButtonFocused);
            }

            if (_RecipeBookPage > 0)
            {
                _buttonRecipePreviousPage.Update(mouseState,ref _isButtonFocused);
            }
        }
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

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
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
            
            if (ItemsDataHolder.Scrolls.ScrollsTypesInfo.Count - 1 > _RecipeBookPage)
                _buttonRecipeNextPage.Draw(spriteBatch);
            if (_RecipeBookPage > 0)
                _buttonRecipePreviousPage.Draw(spriteBatch);
        }
        else
            _buttonOpenNotePad.Draw(spriteBatch);
        
        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawRecipes(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_recipeBookColors[(ScrollType)_RecipeBookPage], new Vector2(331, 276), Color.White);

        var position = new Vector2(350, 307);
        var index = 1;
        
        foreach (var recipe in ScrollsRecipes.Recipes
                     .Where(p => p.Value.Type == (ScrollType)_RecipeBookPage))
        {
            if (index == 7) position = new Vector2(872,307);
            if (index == 1)
            {
                var strSize = _recipeHeadingSpriteFont.MeasureString(
                        ItemsDataHolder.Scrolls.ScrollsTypesInfo[recipe.Value.Type].rus);
                
                spriteBatch.DrawString(_recipeHeadingSpriteFont, ItemsDataHolder.Scrolls.ScrollsTypesInfo[recipe.Value.Type].rus, 
                    new Vector2(position.X + (480 - strSize.X) / 2, position.Y - 6), new Color(48, 56, 67));
                position.Y += 40;
            }

            var isRecipeUnlocked = AllGameItems.KnownScrollsCraftRecipes.Contains(recipe.Value);
            spriteBatch.Draw(
                isRecipeUnlocked ? 
                    ItemsDataHolder.Scrolls.RunesWithNumbersTextures[recipe.Key.runeElementAndVariant1] 
                    : _recipeQuestionMarkTexture, position, Color.White);
            position.X += 64;
            spriteBatch.Draw(_recipePlusTexture, new Vector2(position.X, position.Y + (64 - _recipePlusTexture.Height) / 2), Color.White);
            position.X += _recipePlusTexture.Width;
            spriteBatch.Draw(
                isRecipeUnlocked ? ItemsDataHolder.Scrolls.RunesWithNumbersTextures[recipe.Key.runeElementAndVariant2]
                    : _recipeQuestionMarkTexture, position, Color.White);
            position.X += 64;
            spriteBatch.Draw(_recipeArrowTexture, new Vector2(position.X, position.Y + (64 - _recipeArrowTexture.Height) / 2), Color.White);
            position.X += _recipeArrowTexture.Width + 12;
            spriteBatch.Draw(ItemsDataHolder.Scrolls.ScrollTexturesX64[recipe.Value.Type], position, Color.White);
            
            position.X = index > 6 ? 872 : 350;
            
            position.Y += 64;
            spriteBatch.DrawString(_recipeSpriteFont, isRecipeUnlocked ? $"Свиток {recipe.Value.rus.ToLower()}" : "Неизвестный свиток", 
                new Vector2(position.X + 12, position.Y), new Color(48, 56, 67));
            position.Y += _recipeSpriteFont.MeasureString(recipe.Value.rus).Y + 6;
            index++;
        }
        
    }
}