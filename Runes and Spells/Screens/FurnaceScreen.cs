using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.MiniGames;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.Screens;

public class FurnaceScreen : IScreen
{
    private readonly Game1 _game;
    public FurnaceScreen(Game1 game) => _game = game;
    
    private UiSlot _inputSlot;
    private Texture2D _backgroundTexture;
    private UiButton _buttonStartMiniGame;
    private UiButton _buttonGoBack;
    private UiProgressBar _progressBar;
    private bool _isButtonFocused;
    public FurnaceMiniGame MiniGame { get; private set; }
    
    public void Initialize()
    {
        
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/furnace_screen/background");
        _buttonGoBack = new UiButton(content.Load<Texture2D>("textures/buttons/button_back_wood_default"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_hovered"),
            content.Load<Texture2D>("textures/buttons/button_back_wood_pressed"),
            new Vector2(20, 20),
            "Back", AllGameItems.Font30Px, new Color(212, 165, 140),
            () =>
            {
                _game.SetScreen(GameScreen.MainHouseScreen);
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 14)
                {
                    _game.Introduction.Step = 15;
                    _inputSlot.Clear();
                }
            });
        _buttonStartMiniGame = new UiButton(content.Load<Texture2D>("textures/furnace_screen/button_wood_start_default"),
            content.Load<Texture2D>("textures/furnace_screen/button_wood_start_hovered"),
            content.Load<Texture2D>("textures/furnace_screen/button_wood_start_pressed"),
            new Vector2(794, 583),
            "Start", AllGameItems.Font30Px, new Color(212, 165, 140),
            () =>
            {
                if (_game.Introduction.IsPlaying && _game.Introduction.Step == 11) _game.Introduction.Step = 12;
                MiniGame.Start(_inputSlot.currentItem.ID.Contains("failed") ? 2 : int.Parse(_inputSlot.currentItem.ID.Split('_')[3]));
            });
        _inputSlot = new UiSlot(new Vector2(912, 444),
            content.Load<Texture2D>("textures/Inventory/slot_bg_stone"),
            _game, ItemType.UnknownRune);
        _progressBar = new UiProgressBar(content.Load<Texture2D>("textures/furnace_screen/mini_game/progress_bar_bg"),
            content.Load<Texture2D>("textures/furnace_screen/mini_game/progress_bar_progress_full"),
            UiProgressBar.ProgressDirection.ToRight,0, 0, 
            new Vector2(732, 555), 0, 1000, 400);
        MiniGame = new FurnaceMiniGame(_progressBar, _inputSlot, new Vector2(600, 691), content, _game);
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        if (MiniGame.IsActive) 
            MiniGame.Update();
        else
        {
            if (_inputSlot.currentItem is not null && _inputSlot.currentItem.Type == ItemType.UnknownRune) 
                _buttonStartMiniGame.Update(mouseState, ref _isButtonFocused);
        }
        _inputSlot.Update(_game.Inventory);
        _game.Inventory.Update(graphics, _inputSlot);
        if (_game.Introduction.IsPlaying)
        {
            if (_game.Introduction.Step == 10 && _inputSlot.ContainsItem())
            {
                _game.Introduction.Step = 11;
                _inputSlot.Lock();
            }
            if (_game.Introduction.Step == 14)
                _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        }
        else if (!MiniGame.IsActive)
        {
            _buttonGoBack.Update(mouseState, ref _isButtonFocused);
        }
        
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0,0), null, 
            Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        if (MiniGame.IsActive)
        {
            _progressBar.Draw(spriteBatch);
            MiniGame.Draw(graphics, spriteBatch);
        }
        else
        {
            _buttonGoBack.Draw(spriteBatch);
            if (_inputSlot.currentItem is not null && _inputSlot.currentItem.Type == ItemType.UnknownRune) _buttonStartMiniGame.Draw(spriteBatch);
        }
        _inputSlot.Draw(spriteBatch);
        _game.Inventory.Draw(graphics, spriteBatch);
    }
}