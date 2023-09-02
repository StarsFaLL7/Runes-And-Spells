using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
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

    private UiButton _buttonOpenHelper;
    private UiButton _buttonCloseHelper;
    private Dictionary<Language, Texture2D> _helperPopupTextures;
    private bool IsHelperPopupVisible;
    
    public ScrollCraftingMiniGame MiniGame { get; private set; }
    
    private Texture2D _recipeBookBackTexture;
    private Texture2D _recipeArrowTexture;
    private Texture2D _recipePlusTexture;
    private Texture2D _recipeQuestionMarkTexture;
    private SpriteFont _recipeSpriteFont;
    private SpriteFont _recipeHeadingSpriteFont;
    private UiButton _buttonRecipeNextPage;
    private UiButton _buttonRecipePreviousPage;
    private Dictionary<ScrollType, Texture2D> _recipeBookColors;
    public bool IsNotePadVisible;
    private UiButton _buttonOpenNotePad;
    private UiButton _buttonCloseNotePad;
    private int _RecipeBookPage;
    private KeyboardState _lastKBState;
    private SoundEffect _soundBookOpen;

    public void Initialize()
    {
        
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _soundBookOpen = content.Load<SoundEffect>("sounds/book_open");
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
            () =>
            {
                _RecipeBookPage++;
                AllGameItems.PageFlipSound.Play();
            });
        _buttonRecipePreviousPage = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_previous_page_default"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            new Vector2(334, 279),
            () =>
            {
                _RecipeBookPage--;
                AllGameItems.PageFlipSound.Play();
            });
        _recipeBookColors = new Dictionary<ScrollType, Texture2D>();
        foreach (var scrollType in ItemsDataHolder.Scrolls.ScrollsTypesInfo.Keys)
            _recipeBookColors.Add(scrollType, content.Load<Texture2D>($"textures/scroll_crafting_table/recipe_book/book_{scrollType.ToString().ToLower()}"));

        _buttonGoBack = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            "Back", AllGameItems.Font30Px, new Color(212, 165, 140),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 22)
                {
                    _game.Introduction.Step = 23;
                    _outputSlot.Clear();
                    _game.Inventory.Clear();
                    _game.Inventory.AddItem(new Item(AllGameItems.Scrolls["scroll_water_grass_2_nature_1"]));
                }
                _game.SetScreen(GameScreen.MainHouseScreen);
            } );
        _buttonOpenNotePad = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_show_book_hovered"),
            new Vector2(111, 442),
            () =>
            {
                IsNotePadVisible = true;
                _soundBookOpen.Play();
            } );
        _buttonCloseNotePad = new UiButton(
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_default"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_hovered"),
            content.Load<Texture2D>("textures/scroll_crafting_table/button_close_book_hovered"),
            new Vector2(41, 442),
            () =>
            {
                IsNotePadVisible = false;
                _soundBookOpen.Play();
            } );
        
        var slotTexture = content.Load<Texture2D>("textures/Inventory/slot_bg_stone");
        _inputSlot1 = new UiSlot(new Vector2(581, 323), slotTexture, _game, ItemType.Rune);
        _inputSlot2 = new UiSlot(new Vector2(1045, 323), slotTexture, _game, ItemType.Rune);
        _outputSlot = new UiSlot(new Vector2(813, 323), slotTexture, _game, ItemType.Paper);
        
        MiniGame = new ScrollCraftingMiniGame(_outputSlot, _inputSlot1, _inputSlot2, _game);
        MiniGame.Initialize(content);
        _buttonStartCraft = new UiButton(
            content.Load<Texture2D>("textures/buttons/small_stone_button_default"),
            content.Load<Texture2D>("textures/buttons/small_stone_button_hovered"),
            content.Load<Texture2D>("textures/buttons/small_stone_button_pressed"), 
            new Vector2(755, 489),
            "Start", AllGameItems.Font24Px, new Color(35, 35, 35),
            () =>
            {
                MiniGame.Start();
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 20) _game.Introduction.Step = 21;
            } );
        _buttonOpenHelper = new UiButton(
            content.Load<Texture2D>("textures/buttons/question_button_default"),
            content.Load<Texture2D>("textures/buttons/question_button_hovered"),
            content.Load<Texture2D>("textures/buttons/question_button_pressed"),
            new Vector2(840, 266), (() => IsHelperPopupVisible = true)
        );
        _buttonCloseHelper = new UiButton(
            content.Load<Texture2D>("textures/buttons/close_button_default"),
            content.Load<Texture2D>("textures/buttons/close_button_hovered"),
            content.Load<Texture2D>("textures/buttons/close_button_pressed"),
            new Vector2(1241, 273), (() => IsHelperPopupVisible = false)
        );
        _helperPopupTextures = new Dictionary<Language, Texture2D>()
        {
            { Language.Russian, content.Load<Texture2D>("textures/scroll_crafting_table/scroll_gui_popup_ru")},
            { Language.English, content.Load<Texture2D>("textures/scroll_crafting_table/scroll_gui_popup_eng")}
        };
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        var kbState = Keyboard.GetState();
        if (IsNotePadVisible)
        {
            _game.Inventory.Update(graphics);
            _buttonCloseNotePad.Update(mouseState, ref _isButtonFocused);
            if (ItemsDataHolder.Scrolls.ScrollsTypesInfo.Count - 1 > _RecipeBookPage)
            {
                _buttonRecipeNextPage.Update(mouseState, ref _isButtonFocused);
                if (!_lastKBState.IsKeyDown(Keys.D) && kbState.IsKeyDown(Keys.D) ||
                    !_lastKBState.IsKeyDown(Keys.Right) && kbState.IsKeyDown(Keys.Right))
                {
                    _RecipeBookPage++;
                    AllGameItems.PageFlipSound.Play();
                }
            }
            if (_RecipeBookPage > 0)
            {
                _buttonRecipePreviousPage.Update(mouseState,ref _isButtonFocused);
                
                if (!_lastKBState.IsKeyDown(Keys.A) && kbState.IsKeyDown(Keys.A) ||
                    !_lastKBState.IsKeyDown(Keys.Left) && kbState.IsKeyDown(Keys.Left))
                {
                    _RecipeBookPage--;
                    AllGameItems.PageFlipSound.Play();
                }
            }
            
            if (!_lastKBState.IsKeyDown(Keys.Escape) && kbState.IsKeyDown(Keys.Escape))
            {
                IsNotePadVisible = false;
                _soundBookOpen.Play();
            }
            _lastKBState = kbState;
            return;
        }
        if (IsHelperPopupVisible)
        {
            _buttonCloseHelper.Update(mouseState, ref _isButtonFocused);
            return;
            
        }
        _buttonOpenHelper.Update(mouseState, ref _isButtonFocused);
        
        _buttonOpenNotePad.Update(mouseState, ref _isButtonFocused);
        
        if (!MiniGame.IsActive)
        {
            if (_game.Introduction.IsPlaying && _game.Introduction.Step == 22 || !_game.Introduction.IsPlaying)
                _buttonGoBack.Update(mouseState, ref _isButtonFocused);

            if (_inputSlot1.ContainsItem() && _inputSlot2.ContainsItem() && _outputSlot.ContainsItem() && _outputSlot.currentItem.Type == ItemType.Paper)
            {
                _buttonStartCraft.Update(mouseState, ref _isButtonFocused);
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 20)
                {
                    _game.Introduction.Step = 21;
                }
            }
        }
        else MiniGame.Update(mouseState, ref _isButtonFocused);
        
        _inputSlot1.Update(_game.Inventory);
        _inputSlot2.Update(_game.Inventory);
        _outputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _inputSlot1, _inputSlot2, _outputSlot);
        _lastKBState = Keyboard.GetState();
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_woodTabletBackTexture, new Vector2(268, 460)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_arrowRightTexture, new Vector2(697, 323)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_arrowLeftTexture, new Vector2(929, 323)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        _inputSlot1.Draw(spriteBatch);
        _inputSlot2.Draw(spriteBatch);
        _outputSlot.Draw(spriteBatch);
        _buttonOpenHelper.Draw(spriteBatch);
        if (!MiniGame.IsActive)
        {
            _buttonGoBack.Draw(spriteBatch);
            if (_inputSlot1.ContainsItem() && _inputSlot2.ContainsItem() && _outputSlot.ContainsItem() && _outputSlot.currentItem.Type == ItemType.Paper)
                _buttonStartCraft.Draw(spriteBatch);
        }
        else MiniGame.Draw(spriteBatch);

        if (IsNotePadVisible)
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
        
        if (IsHelperPopupVisible)
        {
            spriteBatch.Draw(_helperPopupTextures[Game1.CurrentLanguage], new Vector2(425, 255), null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            _buttonCloseHelper.Draw(spriteBatch);
        }
        
        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawRecipes(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_recipeBookColors[(ScrollType)_RecipeBookPage], new Vector2(331, 276)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);

        var position = new Vector2(350, 307);
        var index = 1;
        
        foreach (var recipe in ScrollsRecipes.Recipes
                     .Where(p => p.Value.Type == (ScrollType)_RecipeBookPage))
        {
            if (index == 7) position = new Vector2(872,307);
            if (index == 1)
            {
                var str = Game1.GetText(ItemsDataHolder.Scrolls.ScrollsTypesInfo[recipe.Value.Type].rus);
                var strSize = _recipeHeadingSpriteFont.MeasureString(str);
                
                spriteBatch.DrawString(_recipeHeadingSpriteFont, str, 
                    new Vector2(position.X + (480 - strSize.X) / 2, position.Y - 6)*Game1.ResolutionScale, new Color(48, 56, 67),
                    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                position.Y += 40;
            }

            var isRecipeUnlocked = AllGameItems.KnownScrollsCraftRecipes.Contains(recipe.Value);
            spriteBatch.Draw(
                isRecipeUnlocked ? 
                    ItemsDataHolder.Scrolls.RunesWithNumbersTextures[recipe.Key.runeElementAndVariant1] 
                    : _recipeQuestionMarkTexture, position*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            position.X += 64;
            spriteBatch.Draw(_recipePlusTexture,
                new Vector2(position.X, position.Y + (64 - _recipePlusTexture.Height) / 2)*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            position.X += _recipePlusTexture.Width;
            spriteBatch.Draw(
                isRecipeUnlocked ? ItemsDataHolder.Scrolls.RunesWithNumbersTextures[recipe.Key.runeElementAndVariant2]
                    : _recipeQuestionMarkTexture, position*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            position.X += 64;
            spriteBatch.Draw(_recipeArrowTexture, 
                new Vector2(position.X, position.Y + (64 - _recipeArrowTexture.Height) / 2)*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            position.X += _recipeArrowTexture.Width + 12;
            spriteBatch.Draw(ItemsDataHolder.Scrolls.ScrollTexturesX64[recipe.Value.Type], 
                position*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            position.X = index > 6 ? 872 : 350;
            
            position.Y += 64;
            spriteBatch.DrawString(_recipeSpriteFont, isRecipeUnlocked ? 
                    $"{Game1.GetText("Scroll of")} {Game1.GetText(recipe.Value.rus).ToLower()}" : Game1.GetText("Unknown scroll"), 
                new Vector2(position.X + 12, position.Y)*Game1.ResolutionScale.X, new Color(48, 56, 67),
                0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            position.Y += _recipeSpriteFont.MeasureString(recipe.Value.rus).Y + 6;
            index++;
        }
        
    }
}