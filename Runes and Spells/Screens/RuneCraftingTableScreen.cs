using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private Texture2D _glowRuneTexture;
    private int _recipeBookPage;
    private UiSlot _dropSlot;
    private UiSlot _outputSlot;
    public RuneCraftingMiniGame MiniGame { get; private set; }
    private UiButton _craftButton;
    private UiButton _startCraftButton;
    private UiButton _goBackButton;
    private UiButton _recipePrevPage;
    private UiButton _recipeNextPage;
    private SpriteFont _recipeFont;
    private bool _buttonFocused;
    private KeyboardState _lastKBState;
    private MouseState _lastMouseState;
    private Rectangle _recipeUpdateRectangle;
    private string _recipeUpdateId;
    private SoundEffect[] _soundsFastCraft;
    
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _recipeFont = content.Load<SpriteFont>("16PixelTimes20px");
        _backgroundTexture = content.Load<Texture2D>("textures/backgrounds/background_table");
        _recipeBookTexture = content.Load<Texture2D>("textures/rune_crafting_table/recipes_book");
        _arrowTexture = content.Load<Texture2D>("textures/Inventory/arrow_down");
        _glowRuneTexture = content.Load<Texture2D>("textures/rune_crafting_table/glow_rune");
        _soundsFastCraft = new[]
        {
            content.Load<SoundEffect>("sounds/rune_craft_fast1"),
            content.Load<SoundEffect>("sounds/rune_craft_fast2"),
            content.Load<SoundEffect>("sounds/rune_craft_fast3"),
        };
        _dropSlot = new UiSlot(
            new Vector2(1128, 128), 
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"), 
            _game,
            ItemType.Clay);
        _outputSlot = new UiSlot(
            new Vector2(1128, 812), 
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"), 
            _game);
        MiniGame = new RuneCraftingMiniGame(new Vector2(1008, 350), RuneCraftingMiniGame.Mode.X3, content, _game);
        var buttonTextures = new[]
        {
            content.Load<Texture2D>("textures/buttons/small_stone_button_default"),
            content.Load<Texture2D>("textures/buttons/small_stone_button_hovered"),
            content.Load<Texture2D>("textures/buttons/small_stone_button_pressed")
        };
        _craftButton = new UiButton(
            buttonTextures[0],
            buttonTextures[1],
            buttonTextures[2],
            new Vector2(1224, 721),
            "Craft", AllGameItems.Font24Px, new Color(35, 35, 35),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 7) _game.Introduction.Step = 8;
                MiniGame.Stop(_dropSlot, _outputSlot);
            });
        _startCraftButton = new UiButton(
            buttonTextures[0],
            buttonTextures[1],
            buttonTextures[2],
            new Vector2(1224, 258),
            "Start", AllGameItems.Font24Px, new Color(35, 35, 35),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 5) _game.Introduction.Step = 6;
                MiniGame.Start(_dropSlot);
            });
        _goBackButton = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            "Back", AllGameItems.Font30Px, new Color(212, 165, 140),
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
            () =>
            {
                _recipeBookPage++;
                AllGameItems.PageFlipSound.Play();
            });
        _recipePrevPage = new UiButton(content.Load<Texture2D>("textures/buttons/button_previous_page_default"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            content.Load<Texture2D>("textures/buttons/button_previous_page_hovered"),
            new Vector2(93, 207),
            () =>
            {
                _recipeBookPage--;
                AllGameItems.PageFlipSound.Play();
            });
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (_dropSlot.currentItem is not null)
            _dropSlot.Update(_game.Inventory);
        MiniGame.Update();
        
        if (MiniGame.IsActive)
            _craftButton.Update(mouseState, ref _buttonFocused);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Update(mouseState, ref _buttonFocused);
        
        _outputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _dropSlot);
        
        if (_recipeUpdateRectangle.Contains(mouseState.Position) &&
            mouseState.LeftButton == ButtonState.Pressed &&
            _lastMouseState.LeftButton == ButtonState.Released)
        {
            FastCraftRune(_recipeUpdateId);
        }
        
        var kbState = Keyboard.GetState();
        if (_recipeBookPage > 0)
        {
            _recipePrevPage.Update(mouseState, ref _buttonFocused);
            if (!_lastKBState.IsKeyDown(Keys.A) && kbState.IsKeyDown(Keys.A) ||
                !_lastKBState.IsKeyDown(Keys.Left) && kbState.IsKeyDown(Keys.Left))
                _recipeBookPage--;
        }
        if (AllGameItems.KnownRunesCraftRecipes.Count > 10 * (1 + _recipeBookPage))
        {
            _recipeNextPage.Update(mouseState, ref _buttonFocused);
            if (!_lastKBState.IsKeyDown(Keys.D) && kbState.IsKeyDown(Keys.D) ||
                !_lastKBState.IsKeyDown(Keys.Right) && kbState.IsKeyDown(Keys.Right))
                _recipeBookPage++;
        }

        _lastMouseState = mouseState;
        _lastKBState = kbState;
        
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 4 && _dropSlot.ContainsItem())
        {
            _game.Introduction.Step = 5;
        }
        
        if (_game.Introduction.IsPlaying)
        {
            if (_game.Introduction.Step == 8)
                _goBackButton.Update(mouseState, ref _buttonFocused);
            if (_game.Introduction.Step == 6 && 
                !MiniGame.GetCurrentScheme()[0] && !MiniGame.GetCurrentScheme()[1] && !MiniGame.GetCurrentScheme()[2] &&
                !MiniGame.GetCurrentScheme()[3] && MiniGame.GetCurrentScheme()[4] && !MiniGame.GetCurrentScheme()[5] &&
                !MiniGame.GetCurrentScheme()[6] && !MiniGame.GetCurrentScheme()[7] && !MiniGame.GetCurrentScheme()[8])
            {
                _game.Introduction.Step = 7;
            }
        }
        else if (!MiniGame.IsActive)
            _goBackButton.Update(mouseState, ref _buttonFocused);
        
        if (_game.Introduction.IsPlaying && _game.Introduction.Step == 6 && 
            !MiniGame.GetCurrentScheme()[0] && !MiniGame.GetCurrentScheme()[1] && !MiniGame.GetCurrentScheme()[2] &&
            !MiniGame.GetCurrentScheme()[3] && MiniGame.GetCurrentScheme()[4] && !MiniGame.GetCurrentScheme()[5] &&
            !MiniGame.GetCurrentScheme()[6] && !MiniGame.GetCurrentScheme()[7] && !MiniGame.GetCurrentScheme()[8])
        {
            _game.Introduction.Step = 7;
        }
    }

    private void FastCraftRune(string id)
    {
        if (_game.Inventory.TryGetItem(i => i.Type == ItemType.Clay && i.Count > 0, out var clay))
        {
            var sound = _soundsFastCraft[Random.Shared.Next(_soundsFastCraft.Length)];
            sound.Play();
            clay.SubtractCount(1);
            var item = new Item(ItemsDataHolder.Runes.UnknownRunes[id]);
            item.ToolTipText = "Слепок руны\n" + ItemsDataHolder.Runes.FinishedRunes[id.Replace("unknown", "finished")].ToolTip;
            _game.Inventory.AddItem(item);
            _game.SubtractEnergy(2f);
        }
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        DrawRecipes(spriteBatch, new Vector2(90, 204)*Game1.ResolutionScale);
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,239)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        spriteBatch.Draw(_arrowTexture, new Vector2(1149 ,701)*Game1.ResolutionScale, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (MiniGame.IsActive)
            _craftButton.Draw(spriteBatch);
        else if (_dropSlot.currentItem is not null && _outputSlot.currentItem is null)
            _startCraftButton.Draw(spriteBatch);
        if (!MiniGame.IsActive)
            _goBackButton.Draw(spriteBatch);
        MiniGame.Draw(spriteBatch);
        _dropSlot.Draw(spriteBatch);
        _outputSlot.Draw(spriteBatch);
        _game.Inventory.Draw(graphics, spriteBatch);
    }

    private void DrawRecipes(SpriteBatch spriteBatch, Vector2 startPosition)
    {
        spriteBatch.Draw(_recipeBookTexture, startPosition, null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (_recipeBookPage > 0)
            _recipePrevPage.Draw(spriteBatch);
        if (AllGameItems.KnownRunesCraftRecipes.Count > 10 * (1 + _recipeBookPage))
            _recipeNextPage.Draw(spriteBatch);
        var index = 0;
        foreach (var recipe in AllGameItems.KnownRunesCraftRecipes
                     .OrderBy(p => p.Key)
                     .Skip(10*_recipeBookPage)
                     .Take(10))
        {
            var position = new Vector2(
                startPosition.X + (2 + 414 * (index / 5))*Game1.ResolutionScale.X,
                startPosition.Y + (33 + (112 + 16) * (index % 5))*Game1.ResolutionScale.Y
                );
            index++;
            spriteBatch.Draw(ItemsDataHolder.Runes.RecipesBack, new Vector2(position.X + 12*Game1.ResolutionScale.X, position.Y), null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            var cellIndex = 0;
            foreach (var cell in ItemsDataHolder.Runes.RunesCreateRecipes.First(p => p.Value == recipe.Key).Key)
            {
                spriteBatch.Draw(cell ? ItemsDataHolder.Runes.RecipeCellFull : ItemsDataHolder.Runes.RecipeCellEmpty, 
                    new Vector2(
                        position.X + (18 + cellIndex % 3 * 34)*Game1.ResolutionScale.X, 
                        position.Y + (6 + cellIndex / 3 * 34)*Game1.ResolutionScale.Y
                        ), 
                    null, Color.White, 0f, Vector2.Zero, 
                    Game1.ResolutionScale, SpriteEffects.None, 1f);
                cellIndex++;
            }

            var recipeInfo = recipe.Key.Split('_');
            if (recipe.Value)
            {
                spriteBatch.DrawString(_recipeFont, $"{Game1.GetText("Power")}: {recipeInfo[3]}, {Game1.GetText("Type")}: {recipeInfo[4]}.", 
                    new Vector2(position.X + (12 + ItemsDataHolder.Runes.RecipesBack.Width + 12)*Game1.ResolutionScale.X, position.Y), new Color(82, 39, 15),
                    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
            
            spriteBatch.Draw(ItemsDataHolder.Runes.RecipeArrow,
                new Vector2(position.X + 130*Game1.ResolutionScale.X, position.Y + 38*Game1.ResolutionScale.Y), 
                null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            position.X += (130 + ItemsDataHolder.Runes.RecipeArrow.Width)*Game1.ResolutionScale.X;

            var unknownRuneTexture = ItemsDataHolder.Runes.RecipeX64UnknownRunesTextures[recipe.Key.Split('_')[2]];
            var rDeltaPosY = (ItemsDataHolder.Runes.RecipesBack.Height - unknownRuneTexture.Height) * Game1.ResolutionScale.Y / 2;

            spriteBatch.Draw(unknownRuneTexture,new Vector2(position.X, position.Y + rDeltaPosY), null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            if (!recipe.Value) continue;
            position.X += 64*Game1.ResolutionScale.X;
            spriteBatch.Draw(ItemsDataHolder.Runes.RecipeArrow,new Vector2(position.X, position.Y + 38*Game1.ResolutionScale.Y), null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            spriteBatch.Draw(ItemsDataHolder.Runes.RecipeFire, 
                new Vector2(position.X + 17*Game1.ResolutionScale.X, position.Y + 70*Game1.ResolutionScale.Y), 
                null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            
            position.X += ItemsDataHolder.Runes.RecipeArrow.Width*Game1.ResolutionScale.X;
            var finishRuneTexture = ItemsDataHolder.Runes.RecipeX64RunesTextures[recipe.Key.Replace("unknown_", "")];
            
            if (new Rectangle((int)position.X, (int)(position.Y + rDeltaPosY), 
                    (int)(finishRuneTexture.Width*Game1.ResolutionScale.X), (int)(finishRuneTexture.Height*Game1.ResolutionScale.Y))
                .Contains(Mouse.GetState().Position) && _game.Inventory.GetCountOfItems(i => i.Type == ItemType.Clay) > 0)
            {
                spriteBatch.Draw(_glowRuneTexture, new Vector2(
                    position.X - (_glowRuneTexture.Width - finishRuneTexture.Width)*Game1.ResolutionScale.X/2, 
                    position.Y + rDeltaPosY - (_glowRuneTexture.Width - finishRuneTexture.Width)*Game1.ResolutionScale.Y/2), 
                    null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                _recipeUpdateRectangle = new Rectangle(
                    (int)position.X, 
                    (int)(position.Y + rDeltaPosY),
                    (int)(finishRuneTexture.Width*Game1.ResolutionScale.X), 
                    (int)(finishRuneTexture.Height*Game1.ResolutionScale.Y));
                _recipeUpdateId = recipe.Key;
            }
            
            
            spriteBatch.Draw(finishRuneTexture,new Vector2(position.X, position.Y + rDeltaPosY), 
                null, Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        }
    }
}