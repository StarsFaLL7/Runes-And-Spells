using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.classes;
using Runes_and_Spells.Screens;

namespace Runes_and_Spells;

public class Game1 : Game
{

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Drawer _drawer;
    public SpriteFont LogText { get; private set; }
    public SpriteFont HeadingText { get; private set; }
    private GameScreen _currentScreen;
    public void SetScreen(GameScreen newScreen) => _currentScreen = newScreen;
    
    private MainMenuScreen _mainMenu;
    private BackStoryScreen _backStoryScreen;
    private MainHouseScreen _mainHouseScreen;
    private RuneCraftingTableScreen _runeCraftingTableScreen;
    private FurnaceScreen _furnaceScreen;
    private Dictionary<GameScreen, IScreen> _allScreens;
    public readonly Inventory Inventory;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        Inventory = new Inventory(this);
        Inventory.Initialize();
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.IsFullScreen = true; //FullScreen
        _graphics.ApplyChanges();
        _drawer = new Drawer();
        _mainMenu = new MainMenuScreen(this);
        _backStoryScreen = new BackStoryScreen(this);
        _mainHouseScreen = new MainHouseScreen(this);
        _runeCraftingTableScreen = new RuneCraftingTableScreen(this);
        _furnaceScreen = new FurnaceScreen(this);
        _allScreens = new Dictionary<GameScreen, IScreen>()
        {
            { GameScreen.Menu, _mainMenu },
            { GameScreen.Backstory, _backStoryScreen},
            { GameScreen.MainHouseScreen, _mainHouseScreen},
            { GameScreen.RuneCraftingTable, _runeCraftingTableScreen},
            { GameScreen.FurnaceScreen, _furnaceScreen}
        };
        
        foreach (var screen in _allScreens.Values)
        {
            screen.Initialize();
        }
        CountDrawer.Initialize(Content);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        AllGameItems.Initialize(Content);
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        LogText = Content.Load<SpriteFont>("logText");
        HeadingText = Content.Load<SpriteFont>("headingsText");
        Inventory.LoadContent(Content, _graphics);
        foreach (var screen in _allScreens.Values)
        {
            screen.LoadContent(Content, _graphics);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _allScreens[_currentScreen].Update(_graphics);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        _allScreens[_currentScreen].Draw(_graphics, _spriteBatch, _drawer);
        _spriteBatch.DrawString(LogText, $"Screen: {_currentScreen}\n", new Vector2(0,_graphics.PreferredBackBufferHeight-20), Color.Black);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}