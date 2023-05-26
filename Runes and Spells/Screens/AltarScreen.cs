using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;

namespace Runes_and_Spells.Screens;

public class AltarScreen : IScreen
{
    private Game1 _game;
    public AltarScreen(Game1 game) => _game = game;
    
    private Texture2D _backgroundTexture;
    private Texture2D _recipeBookTexture;
    private Texture2D _smallArrowTexture;
    private Texture2D _smallEqualitySignTexture;
    private Texture2D _smallPurpleSlotTexture;
    private UiButton _buttonGoBack;
    private UiSlot _inputSlotMain;
    private UiSlot _inputSlotSecondary;
    private Texture2D _arrowDownTexture;
    private UiProgressBar _progressBar;
    private UiButton _buttonStartCraft;
    public bool IsCraftingInProgress { get; private set; }
    private Timer CreationTimer { get; set; }
    private bool _isButtonFocused;
    private int _recipeBookPage;
    private UiButton _buttonPrevRecipePage;
    private UiButton _buttonNextRecipePage;
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/altar_screen/background");
        _arrowDownTexture = content.Load<Texture2D>("textures/Inventory/arrow_down");
        _recipeBookTexture = content.Load<Texture2D>("textures/altar_screen/recipe_book");
        _smallArrowTexture = content.Load<Texture2D>("textures/altar_screen/small_arrow_sign");
        _smallEqualitySignTexture = content.Load<Texture2D>("textures/altar_screen/small_equality_sign");
        _smallPurpleSlotTexture = content.Load<Texture2D>("textures/altar_screen/small_purple_slot");
        _buttonGoBack = new UiButton(
            content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 18) _game.Introduction.Step = 19;
                _game.SetScreen(GameScreen.AltarRoomScreen);
            } );
        _inputSlotMain = new UiSlot(new Vector2(1093, 425),
            content.Load<Texture2D>("textures/altar_screen/purple_slot"), true, ItemType.Rune);
        _inputSlotSecondary = new UiSlot(new Vector2(1093, 193),
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"), true, ItemType.Rune);
        _progressBar = new UiProgressBar(
            content.Load<Texture2D>("textures/altar_screen/progress_bar_back"),
            content.Load<Texture2D>("textures/altar_screen/progress_bar_progress"),
            new Vector2(912, 666),
            0, 950, 0);
        _buttonStartCraft = new UiButton(
            content.Load<Texture2D>("textures/altar_screen/button_start_default"),
            content.Load<Texture2D>("textures/altar_screen/button_start_hovered"),
            content.Load<Texture2D>("textures/altar_screen/button_start_pressed"),
            new Vector2(1031, 567),
            StartCraft );
        _buttonPrevRecipePage = new UiButton(content.Load<Texture2D>("textures/buttons/button_previous_page_default"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            new Vector2(23, 195),
            () => _recipeBookPage-- );
        _buttonNextRecipePage = new UiButton(content.Load<Texture2D>("textures/buttons/button_next_page_default"),
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_next_page_hovered"),
            new Vector2(791, 195),
            () => _recipeBookPage++ );
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        _game.Inventory.Update(graphics, _inputSlotMain, _inputSlotSecondary);
        var mouseState = Mouse.GetState();
        
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 18)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        else if (!_game.Introduction.IsPlaying)
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);

        _inputSlotMain.Update(_game.Inventory);
        _inputSlotSecondary.Update(_game.Inventory);
        if (_inputSlotMain.ContainsItem() &&
            _inputSlotSecondary.ContainsItem() && 
            !IsCraftingInProgress &&
            _inputSlotMain.currentItem.ID.Split('_')[3] != "3" &&
            _inputSlotSecondary.currentItem.ID.Split('_')[3] != "3")
        {
            _buttonStartCraft.Update(mouseState, ref _isButtonFocused);
        }
        if (AllGameItems.RuneUniteRecipes.Count > 14 * (_recipeBookPage+1))
            _buttonNextRecipePage.Update(mouseState, ref _isButtonFocused);
        if (_recipeBookPage > 0)
            _buttonPrevRecipePage.Update(mouseState, ref _isButtonFocused);
        UpdateInBackground();
    }

    public void UpdateInBackground()
    {
        if (IsCraftingInProgress)
            CreationTimer.Tick();
        
        if (IsCraftingInProgress && _progressBar.Value >= _progressBar.MaxValue)
            FinishCraft();
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);
        spriteBatch.Draw(_arrowDownTexture, new Vector2(1112, 309), Color.White);
        _inputSlotMain.Draw(spriteBatch);
        _inputSlotSecondary.Draw(spriteBatch);
        _buttonGoBack.Draw(spriteBatch);
        _progressBar.Draw(spriteBatch);
        if (_inputSlotMain.ContainsItem() && _inputSlotSecondary.ContainsItem() && !IsCraftingInProgress &&
            _inputSlotMain.currentItem.ID.Split('_')[3] != "3" &&
            _inputSlotSecondary.currentItem.ID.Split('_')[3] != "3")
        {
            _buttonStartCraft.Draw(spriteBatch);
        }
        
        DrawRecipes(spriteBatch);
        
        _game.Inventory.Draw(graphics, spriteBatch, drawer);
    }

    private void StartCraft()
    {
        IsCraftingInProgress = true;
        _inputSlotMain.Lock();
        _inputSlotSecondary.Lock();
        CreationTimer = new Timer(
            1000 * int.Parse(_inputSlotMain.currentItem.ID.Split('_')[3]),
            () =>
            {
                if (_progressBar.Value < _progressBar.MaxValue)
                {
                    _progressBar.Add(100);
                    CreationTimer.StartAgain();
                }
            });
        CreationTimer.StartAgain();
    }
    
    private void FinishCraft()
    {
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 17) _game.Introduction.Step = 18;
        IsCraftingInProgress = false;
        _progressBar.SetValue(0);
        _inputSlotMain.Unlock();
        _inputSlotSecondary.Unlock();
        if (AllGameItems.TryToUniteRunes(
                _inputSlotMain.currentItem.ID, 
                _inputSlotSecondary.currentItem.ID, out var result))
        {
            var info = AllGameItems.FinishedRunes[result];
            _inputSlotMain.SetItem(new Item(info));
            _inputSlotSecondary.Clear();
        }
        else
        {
            _inputSlotMain.Clear();
        }
    }

    public void SleepSkipCrafting()
    {
        if (IsCraftingInProgress)
            FinishCraft();
    }

    private void DrawRecipes(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_recipeBookTexture, new Vector2(20, 192), Color.White);
        if (AllGameItems.RuneUniteRecipes.Count > 14 * (_recipeBookPage+1))
            _buttonNextRecipePage.Draw(spriteBatch);
        if (_recipeBookPage > 0)
            _buttonPrevRecipePage.Draw(spriteBatch);
        var x = 20 + 20;
        var y = 192 + 20;
        var item = 1;
        if (AllGameItems.RuneUniteRecipes.Count == 0)
            return;
        
        foreach (var recipe in AllGameItems.RuneUniteRecipes
                     .OrderBy(r => r.Value)
                     .Skip(14*_recipeBookPage).Take(14))
        {
            if (item == 8)
            {
                x = 446;
                y = 192 + 20;
            }
            
            var tempX = x;
            var secRuneTexture = AllGameItems.FinishedRunes[recipe.Key.secondaryRuneId].Texture;
            var mainRuneTexture = AllGameItems.FinishedRunes[recipe.Key.mainRuneId].Texture;
            var resultRuneTexture = AllGameItems.FinishedRunes[recipe.Value].Texture;
            
            spriteBatch.Draw(
                secRuneTexture, new Vector2(tempX, y), 
                new Rectangle(0, 0, secRuneTexture.Width, secRuneTexture.Height), Color.White, 
                0f, Vector2.Zero, new Vector2(0.666f, 0.666f), SpriteEffects.None, 1f);
            tempX += secRuneTexture.Width / 2 + 20;
            
            spriteBatch.Draw(_smallArrowTexture, new Vector2(tempX, y + 14), Color.White);
            tempX += _smallArrowTexture.Width + 15;
            
            spriteBatch.Draw(_smallPurpleSlotTexture, new Vector2(tempX, y), Color.White);
            spriteBatch.Draw(
                mainRuneTexture, new Vector2(tempX, y), 
                new Rectangle(0, 0, mainRuneTexture.Width, mainRuneTexture.Height), Color.White, 
                0f, Vector2.Zero, new Vector2(0.666f, 0.666f), SpriteEffects.None, 1f);
            tempX += _smallPurpleSlotTexture.Width + 15;
            
            spriteBatch.Draw(_smallEqualitySignTexture, new Vector2(tempX, y + 16), Color.White);
            tempX += _smallEqualitySignTexture.Width + 15;
            
            spriteBatch.Draw(
                resultRuneTexture, new Vector2(tempX, y), 
                new Rectangle(0, 0, resultRuneTexture.Width, resultRuneTexture.Height), Color.White, 
                0f, Vector2.Zero, new Vector2(0.666f, 0.666f), SpriteEffects.None, 1f);
            
            y += _smallPurpleSlotTexture.Width + 30;
            item++;
        }
    }
}