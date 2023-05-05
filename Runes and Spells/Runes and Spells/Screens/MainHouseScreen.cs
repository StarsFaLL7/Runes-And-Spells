using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.Screens;

public class MainHouseScreen : IScreen
{
    private readonly Game1 _game;
    private Texture2D _backgroundTexture;
    private UiButton _buttonBed;
    private UiButton _buttonTableScrolls;
    private UiButton _buttonTableRunes;
    private UiButton _buttonFurnace;
    private List<UiButton> _furnitureButtons;
    private bool _isButtonFocused;
    
    public MainHouseScreen(Game1 game) => _game = game;
    public void Initialize()
    {
    }

    public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
    {
        _backgroundTexture = content.Load<Texture2D>("textures/main_house_screen/background");
        _buttonBed = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/bed_default"),
            content.Load<Texture2D>("textures/main_house_screen/bed_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/bed_hovered"),
            new Vector2(0, 526),
            () => { });
        _buttonTableScrolls = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_default"),
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/table_scrolls_hovered"),
            new Vector2(978, 523),
            () => { });
        _buttonTableRunes = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/table_runes_default"),
            content.Load<Texture2D>("textures/main_house_screen/table_runes_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/table_runes_hovered"),
            new Vector2(565, 527),
            () => { _game.SetScreen(GameScreen.RuneCraftingTable);});
        _buttonFurnace = new UiButton(
            content.Load<Texture2D>("textures/main_house_screen/furnace_default"),
            content.Load<Texture2D>("textures/main_house_screen/furnace_hovered"),
            content.Load<Texture2D>("textures/main_house_screen/furnace_hovered"),
            new Vector2(1429, 0),
            () => { _game.SetScreen(GameScreen.FurnaceScreen);});
        _furnitureButtons = new List<UiButton> { _buttonBed, _buttonFurnace, _buttonTableRunes, _buttonTableScrolls };
    }

    public void Update(GraphicsDeviceManager graphics)
    {
        var mouseState = Mouse.GetState();
        foreach (var button in _furnitureButtons) button.Update(mouseState, ref _isButtonFocused);
    }

    public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Drawer drawer)
    {
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);
        foreach (var button in _furnitureButtons) button.Draw(spriteBatch);
    }
}