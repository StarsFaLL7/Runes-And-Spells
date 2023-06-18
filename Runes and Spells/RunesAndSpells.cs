using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.classes;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.Screens;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells;

public class Game1 : Game
{
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;

    public readonly GraphicsDeviceManager Graphics;
    private SpriteBatch SpriteBatch { get; set; }
    //public Drawer _drawer;
    private OverlayMenu _overlayMenu;
    private MainMenuScreen _mainMenu;
    private BackStoryScreen _backStoryScreen;
    private MainHouseScreen _mainHouseScreen;
    private RuneCraftingTableScreen _runeCraftingTableScreen;
    private FurnaceScreen _furnaceScreen;
    private ScrollCraftingScreen _scrollCraftingScreen;
    private OutdoorScreen _outdoorScreen;
    private AltarScreen _altarScreen;
    private AltarRoomScreen _altarRoomScreen;
    private MarketScreen _marketScreen;
    private EndingScreen _endingScreen;
    private Dictionary<GameScreen, IScreen> _allScreens;
    private Texture2D _cursorTexture;

    public TopDownCore TopDownCore { get; private set; }
    
    public GameScreen CurrentScreen { get; private set; }
    public void SetScreen(GameScreen newScreen) => CurrentScreen = newScreen;

    public bool IsInTopDownView = false; // ИГРОК НА УЛИЦЕ
    
    public Introduction Introduction { get; private set; }
    public readonly Inventory Inventory;
    
    public Song MusicMenu { get; private set; }
    public Song FinalSong { get; set; }
    public List<Song> MusicsMainTheme { get; private set; }
    public Song BackstoryMusic { get; private set; }
    public int NextSongIndex { get; private set; } = 1;
    
    public int DayCount { get; private set; }
    public bool ClayClaimed;
    public int Balance { get; private set; }

    private bool _wasEscapePressed;
    private bool _isEscapePressed;
    
    public SpriteFont LogText { get; private set; }
    
    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Graphics.PreferredBackBufferWidth = 1920;
        Graphics.PreferredBackBufferHeight = 1080;
        Graphics.IsFullScreen = true; //FullScreen-----------
        Graphics.ApplyChanges();
        Inventory = new Inventory(this);
        Inventory.Initialize();
        Activated += ActivateGame;
        Deactivated += DeactivateGame;
    }

    private void DeactivateGame(object sender, EventArgs e)
    {
        MediaPlayer.Pause();
    }

    private void ActivateGame(object sender, EventArgs e)
    {
        MediaPlayer.Resume();
    }

    protected override void Initialize()
    {
        MediaPlayer.Volume = 0.1f;
        SoundEffect.MasterVolume = 0.5f;

        TopDownCore = new TopDownCore(Content, this);
        
        _overlayMenu = new OverlayMenu(this);
        _mainMenu = new MainMenuScreen(this);
        _backStoryScreen = new BackStoryScreen(this);
        _mainHouseScreen = new MainHouseScreen(this);
        _runeCraftingTableScreen = new RuneCraftingTableScreen(this);
        _furnaceScreen = new FurnaceScreen(this);
        _scrollCraftingScreen = new ScrollCraftingScreen(this);
        _outdoorScreen = new OutdoorScreen(this);
        _altarRoomScreen = new AltarRoomScreen(this);
        _altarScreen = new AltarScreen(this);
        _marketScreen = new MarketScreen(this);
        Introduction = new Introduction(this);
        _endingScreen = new EndingScreen(this);
        _allScreens = new Dictionary<GameScreen, IScreen>()
        {
            { GameScreen.Menu, _mainMenu },
            { GameScreen.Backstory, _backStoryScreen},
            { GameScreen.MainHouseScreen, _mainHouseScreen},
            { GameScreen.RuneCraftingTable, _runeCraftingTableScreen},
            { GameScreen.FurnaceScreen, _furnaceScreen},
            { GameScreen.ScrollsCraftingTable, _scrollCraftingScreen},
            { GameScreen.OutdoorScreen, _outdoorScreen},
            { GameScreen.AltarScreen, _altarScreen},
            { GameScreen.AltarRoomScreen, _altarRoomScreen},
            { GameScreen.TradingScreen, _marketScreen},
            { GameScreen.EndingScreen, _endingScreen}
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
        TopDownCore.Initialize(Content);
        _cursorTexture = Content.Load<Texture2D>("textures/cursor");
        Mouse.SetCursor(MouseCursor.FromTexture2D(_cursorTexture, 0, 0));
        Introduction.LoadContent(Content);
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        LogText = Content.Load<SpriteFont>("logText");
        Inventory.LoadContent(Content, Graphics);
        foreach (var screen in _allScreens.Values)
        {
            screen.LoadContent(Content, Graphics);
        }
        _overlayMenu.LoadContent(Content);
        MusicMenu = Content.Load<Song>("music/mainMenuMusic");
        FinalSong = Content.Load<Song>("music/mainMenuMusic");
        BackstoryMusic = Content.Load<Song>("music/backstoryTheme");
        MusicsMainTheme = new List<Song>
        {
            Content.Load<Song>("music/theme1"), Content.Load<Song>("music/theme2"),
            Content.Load<Song>("music/theme3"), Content.Load<Song>("music/theme4")
        };
        MediaPlayer.Play(MusicMenu);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_altarScreen.IsCraftingInProgress)
            _altarScreen.UpdateInBackground();
        
        if (MediaPlayer.State == MediaState.Stopped)
        {
            MediaPlayer.Play(MusicsMainTheme[NextSongIndex]);
            NextSongIndex++;
            if (NextSongIndex >= MusicsMainTheme.Count)
                NextSongIndex = 0;
        }
        
        if (!IsActive) 
        {
            return;
        }

        CheckForEscapePressed();
        
        if (!_overlayMenu.IsVisible)
        {
            if (IsInTopDownView)
                TopDownCore.Update();
            else
                _allScreens[CurrentScreen].Update(Graphics);

            if (Introduction.IsPlaying)
                Introduction.Update();
            
        }
        else
            _overlayMenu.Update();

        
        base.Update(gameTime);
    }

    private void CheckForEscapePressed()
    {
        _wasEscapePressed = _isEscapePressed;
        _isEscapePressed = Keyboard.GetState().IsKeyDown(Keys.Escape);
        if (!_wasEscapePressed && _isEscapePressed && CurrentScreen != GameScreen.Menu && !TopDownCore.IsDialogOpened)
        {
            _overlayMenu.Reset();
            _overlayMenu.IsVisible = !_overlayMenu.IsVisible;
            _overlayMenu.SliderEffectsVolume.SetValue(SoundEffect.MasterVolume);
            _overlayMenu.SliderMusicVolume.SetValue(MediaPlayer.Volume);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        SpriteBatch.Begin();
        
        if (IsInTopDownView)
            TopDownCore.Draw(SpriteBatch);
        else
            _allScreens[CurrentScreen].Draw(Graphics, SpriteBatch);
        
        if (Introduction.IsPlaying)
            Introduction.Draw(SpriteBatch);
        
        if (_overlayMenu.IsVisible) _overlayMenu.Draw(SpriteBatch);
        
        SpriteBatch.End();
        base.Draw(gameTime);
    }

    public void NextDay()
    {
        _altarScreen.SleepSkipCrafting();
        AllGameItems.MakeSmallChangesToScrollPrices();
        DayCount += 1;
        ClayClaimed = false;
        if (DayCount % 7 == 0)
        {
            AllGameItems.ResetAllScrollsPrices();
            _marketScreen.FillSellSlots();
        }
    }

    public void ResetForNewGame()
    {
        Balance = 100;
        Inventory.Clear();
        Inventory.AddItem(new Item(AllGameItems.Clay), 10);
        Inventory.AddItem(new Item(AllGameItems.Paper), 3);
        Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeySilver), 5);
        Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyGold), 1);
    }

    public void ResetBackStory() => _backStoryScreen.Reset();
    

    public void SetMusicVolume(float volume) => MediaPlayer.Volume = volume;
    
    public void SetSoundsVolume(float volume) => SoundEffect.MasterVolume = volume;
    
    public void AddToBalance(int sum) => Balance += sum;
    public void SubtractFromBalance(int sum) => Balance = Balance - sum >= 0 ? Balance - sum : 0;
}