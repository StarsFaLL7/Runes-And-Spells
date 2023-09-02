using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Runes_and_Spells.classes;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.Interfaces;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.OtherClasses.SaveAndLoad.Records;
using Runes_and_Spells.Screens;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells;

public class Game1 : Game
{
    public int ScreenWidth { get; private set; } = 1920;
    public int ScreenHeight { get; private set; } = 1080;
    public static Language CurrentLanguage { get; private set; } = Language.English;
    public static Vector2 ResolutionScale { get; private set; }
    
    public static ResourceManager ResManager = new ("Runes_and_Spells.Resources.Resources", Assembly.GetExecutingAssembly());
    public static DefaultResolutions DefaultResolutions;

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
    private AltarScreen _altarScreen;
    public MarketScreen MarketScreen { get; private set; }
    private EndingScreen _endingScreen;
    private Dictionary<GameScreen, IScreen> _allScreens;
    private Texture2D _cursorTexture;
    private UiProgressBar _energyProgressBar;
    private float _lastMusicVolume;
    private float _lastEffectsVolume;

    public string[] SavesFilesPaths { get; private set; }
    public TopDownCore TopDownCore { get; private set; }
    public GameScreen CurrentScreen { get; private set; }
    public void SetScreen(GameScreen newScreen) => CurrentScreen = newScreen;
    
    public Introduction Introduction { get; private set; }
    public  Inventory Inventory { get; private set; }
    public Item ToolTipItem { get; set; }
    
    public Song MusicMenu { get; private set; }
    public Song FinalSong { get; set; }
    public List<Song> MusicsMainTheme { get; private set; }
    public Song BackstoryMusic { get; private set; }
    public int NextSongIndex { get; private set; } = 1;
    
    public bool IsInTopDownView; // ИГРОК НА УЛИЦЕ
    public int DayCount { get; private set; }
    public bool ClayClaimed;
    public int Balance { get; private set; }
    public float Energy { get; private set; } = 100;
    private KeyboardState _lastKbState;
    private KeyboardState _kbState;
    public DevConsole DevConsole { get; private set; }
    
    public SpriteFont LogText { get; private set; }
    
    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        var loadSettings = LoadSettings();
        
        var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        var customResolution = new UiDropdown.DdVariant(
            $"{width}x{height}", () => SetResolution(width, height)
        );
        DefaultResolutions = new DefaultResolutions(this, customResolution);
        
        if (loadSettings.FirstLaunch)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            Graphics.IsFullScreen = true;
            MediaPlayer.Volume = 0.3f;
            SoundEffect.MasterVolume = 0.5f;
            CurrentLanguage = Language.English;
            SaveSettings();
        }
        else
        {
            ScreenWidth = int.Parse(loadSettings.Resolution.Split('x')[0]);
            ScreenHeight = int.Parse(loadSettings.Resolution.Split('x')[1]);
            Graphics.IsFullScreen = loadSettings.IsFullscreen;
            MediaPlayer.Volume = _lastMusicVolume = loadSettings.MusicVolume;
            SoundEffect.MasterVolume = _lastEffectsVolume = loadSettings.EffectsVolume;
            CurrentLanguage = loadSettings.Language;
        }
        SetLanguage(CurrentLanguage);
        SetResolution(ScreenWidth, ScreenHeight);
        
        Inventory = new Inventory(this);
        Inventory.Initialize();
        Activated += ActivateGame;
        Deactivated += DeactivateGame;
    }
    
    private Settings LoadSettings()
    {
        var str = File.ReadAllLines($@"settings.json");
        return JsonConvert.DeserializeObject<Settings>(str[0]);
    }

    public void SaveSettings()
    {
        var set = new Settings()
        {
            IsFullscreen = Graphics.IsFullScreen,
            Resolution = $"{ScreenWidth}x{ScreenHeight}",
            MusicVolume = MediaPlayer.Volume,
            EffectsVolume = SoundEffect.MasterVolume,
            Language = CurrentLanguage,
            FirstLaunch = false
        };
        using (StreamWriter file = File.CreateText(@"settings.json"))
        {
            var str = JsonConvert.SerializeObject(set);
            file.Write(str);
        }
    }
    
    public void SetResolution(int width, int height)
    {
        if (DefaultResolutions.Variants.All(v => v.VisibleText != $"{width}x{height}"))
        {
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        ScreenWidth = width;
        ScreenHeight = height;
        ResolutionScale = new Vector2(ScreenWidth / 1920f, ScreenHeight / 1080f);
        Graphics.PreferredBackBufferWidth = ScreenWidth;
        Graphics.PreferredBackBufferHeight = ScreenHeight;
        Graphics.ApplyChanges();
        
        SaveSettings();
    }

    private void DeactivateGame(object sender, EventArgs e)
    {
        MediaPlayer.Pause();
        if (IsInTopDownView)
        {
            TopDownCore.SoundTheme.Pause();
        }
        if (CurrentScreen == GameScreen.FurnaceScreen && _furnaceScreen.MiniGame.BurningSoundInstance != null && 
            _furnaceScreen.MiniGame.BurningSoundInstance.State == SoundState.Playing)
        {
            _furnaceScreen.MiniGame.BurningSoundInstance.Pause();
        }
    }

    private void ActivateGame(object sender, EventArgs e)
    {
        MediaPlayer.Resume();
        if (IsInTopDownView)
        {
            TopDownCore.SoundTheme.Resume();
        }
        if (CurrentScreen == GameScreen.FurnaceScreen && _furnaceScreen.MiniGame.IsActive)
        {
            _furnaceScreen.MiniGame.BurningSoundInstance.Resume();
        }
    }

    protected override void Initialize()
    {
        
        SavesFilesPaths = Directory.GetFiles("saves", "save?.sav");
        TopDownCore = new TopDownCore(Content, this);
        _overlayMenu = new OverlayMenu(this);
        _mainMenu = new MainMenuScreen(this);
        _backStoryScreen = new BackStoryScreen(this);
        _mainHouseScreen = new MainHouseScreen(this);
        _runeCraftingTableScreen = new RuneCraftingTableScreen(this);
        _furnaceScreen = new FurnaceScreen(this);
        _scrollCraftingScreen = new ScrollCraftingScreen(this);
        _altarScreen = new AltarScreen(this);
        MarketScreen = new MarketScreen(this);
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
            { GameScreen.AltarScreen, _altarScreen},
            { GameScreen.TradingScreen, MarketScreen},
            { GameScreen.EndingScreen, _endingScreen}
        };
        
        foreach (var screen in _allScreens.Values)
        {
            screen.Initialize();
        }
        CountDrawer.Initialize(Content);
        base.Initialize();
    }

    [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.Byte[]; size: 57MB")]
    protected override void LoadContent()
    {
        DevConsole = new DevConsole(this, Content.Load<Texture2D>("textures/backgrounds/dev"));
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
        _energyProgressBar = new UiProgressBar(
            Content.Load<Texture2D>("textures/energy_pg/energy_back"),
            Content.Load<Texture2D>("textures/energy_pg/energy_full"),
            UiProgressBar.ProgressDirection.ToRight, 30, 30, new Vector2(1578, 9),
            0, 273, 273);
        
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
        if (Math.Abs(_lastEffectsVolume - SoundEffect.MasterVolume) > 1e-5 || 
            Math.Abs(_lastMusicVolume - MediaPlayer.Volume) > 1e-5)
        {
            SaveSettings();
            _lastEffectsVolume = SoundEffect.MasterVolume;
            _lastMusicVolume = MediaPlayer.Volume;
        }
        
        if (CurrentScreen == GameScreen.Menu)
            SavesFilesPaths = Directory.GetFiles("saves", "save?.sav");
        
        if (_altarScreen.IsUniteInProgress || _altarScreen.IsExtractInProgress)
            _altarScreen.UpdateInBackground();
        
        if (MediaPlayer.State == MediaState.Stopped)
        {
            NextSongIndex++;
            if (NextSongIndex >= MusicsMainTheme.Count)
                NextSongIndex = 0;
            MediaPlayer.Play(MusicsMainTheme[NextSongIndex]);
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

            if (DevConsole.IsOpen)
                DevConsole.Update();
            if (_lastKbState.IsKeyUp(Keys.OemTilde) && _kbState.IsKeyDown(Keys.OemTilde))
            {
                DevConsole.IsOpen = !DevConsole.IsOpen;
            }
        }
        else
            _overlayMenu.Update();

        
        base.Update(gameTime);
    }

    private void CheckForEscapePressed()
    {
        _lastKbState = _kbState;
        _kbState = Keyboard.GetState();
        if (CurrentScreen == GameScreen.ScrollsCraftingTable && _scrollCraftingScreen.IsNotePadVisible)
        {
            return;
        }

        if (_lastKbState.IsKeyUp(Keys.Escape) && _kbState.IsKeyDown(Keys.Escape) && CurrentScreen != GameScreen.Menu && !TopDownCore.IsDialogOpened)
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

        if (IsInTopDownView)
        {
            if (TopDownCore.PlayerHasCompass)
            {
                DrawEnergyBar(new Vector2(334, 9));
            }
            else
            {
                DrawEnergyBar(new Vector2(9, 9));
            }
        }
        else if (!_mainHouseScreen.SleepingBg.IsFading && !_mainHouseScreen.IsSleeping &&
                 CurrentScreen != GameScreen.Menu && CurrentScreen != GameScreen.Backstory && CurrentScreen != GameScreen.EndingScreen 
                 && CurrentScreen != GameScreen.TradingScreen)
        {
            DrawEnergyBar(new Vector2(309, 3));
        }
        else if (!_mainHouseScreen.SleepingBg.IsFading && !_mainHouseScreen.IsSleeping &&
                 CurrentScreen == GameScreen.TradingScreen && MarketScreen.CurrentTab == MarketScreen.Tab.Sell)
        {
            DrawEnergyBar(new Vector2(1041, 215));
        }
        
        if (Introduction.IsPlaying)
            Introduction.Draw(SpriteBatch);
        
        if (ToolTipItem is not null && !_overlayMenu.IsVisible)
        {
            AllGameItems.DrawToolTip(ToolTipItem, SpriteBatch);
            ToolTipItem = null;
        }
        
        if (_overlayMenu.IsVisible) _overlayMenu.Draw(SpriteBatch);
        
        if (DevConsole.IsOpen) DevConsole.Draw(SpriteBatch);
        SpriteBatch.End();
        base.Draw(gameTime);
    }

    public void DrawEnergyBar(Vector2 position)
    {
        _energyProgressBar.SetValue(Energy/100*273);
        _energyProgressBar.SetPosition(position);
        _energyProgressBar.Draw(SpriteBatch);
    }

    public void SubtractEnergy(float amount)
    {
        Energy = Energy - amount >= 0 ? Energy - amount : 0;
        if (Energy <= 0)
        {
            if (IsInTopDownView)
            {
                IsInTopDownView = false;
                TopDownCore.PlayerPosition = new Vector2((GameMap.MapWidth-3)*GameMap.TileSize, 640);
            }
            _mainHouseScreen.SleepingBg.StartFade();
            NextDay();
            SetScreen(GameScreen.MainHouseScreen);
        }
    }

    public void SetLanguage(Language newLanguage)
    {
        CurrentLanguage = newLanguage;
        if (CurrentLanguage == Language.English)
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        }
        else if (CurrentLanguage == Language.Russian)
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
        }
        SaveSettings();
    }

    public void NextDay()
    {
        Energy = 100;
        _altarScreen.SleepSkipCrafting();
        AllGameItems.MakeSmallChangesToScrollPrices();
        DayCount += 1;
        ClayClaimed = false;
        if (DayCount % 7 == 0)
        {
            AllGameItems.ResetAllScrollsPrices();
            MarketScreen.FillSellSlots();
        }
    }

    public void LoadFromState(GameStateLoad gameState)
    {
        Balance = gameState.Balance;
        DayCount = gameState.DayCount;
        Energy = gameState.Energy;
        TopDownCore.IsFinalQuestPlaying = gameState.IsFinalQuestIsPlaying;
        TopDownCore.PlayerHasCompass = gameState.PlayerHasCompass;
        TopDownCore.PlayerHasWings = gameState.PlayerHasWings;
        TopDownCore.FinalScroll = gameState.FinalScroll;
        CurrentScreen = gameState.LastScreen;
        IsInTopDownView = false;
        MediaPlayer.Stop();
        MediaPlayer.Play(MusicsMainTheme[0]);
    }
    
    public void ResetAfterIntroduction()
    {
        Balance = 100;
        Inventory.Clear();
        Inventory.AddItem(new Item(AllGameItems.Clay), 10);
        Inventory.AddItem(new Item(AllGameItems.Paper), 3);
        Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeySilver), 5);
        Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyGold), 1);
    }
    
    public void StartNewGame()
    {
        _backStoryScreen = new BackStoryScreen(this);
        _mainHouseScreen = new MainHouseScreen(this);
        _runeCraftingTableScreen = new RuneCraftingTableScreen(this);
        _furnaceScreen = new FurnaceScreen(this);
        _scrollCraftingScreen = new ScrollCraftingScreen(this);
        _altarScreen = new AltarScreen(this);
        MarketScreen = new MarketScreen(this);
        _endingScreen = new EndingScreen(this);
        
        _overlayMenu = new OverlayMenu(this);
        _overlayMenu.LoadContent(Content);
        Introduction = new Introduction(this);
        Introduction.LoadContent(Content);
        Inventory = new Inventory(this);
        Inventory.LoadContent(Content, Graphics);
        _allScreens = new Dictionary<GameScreen, IScreen>()
        {
            { GameScreen.Menu, _mainMenu },
            { GameScreen.Backstory, _backStoryScreen},
            { GameScreen.MainHouseScreen, _mainHouseScreen},
            { GameScreen.RuneCraftingTable, _runeCraftingTableScreen},
            { GameScreen.FurnaceScreen, _furnaceScreen},
            { GameScreen.ScrollsCraftingTable, _scrollCraftingScreen},
            { GameScreen.AltarScreen, _altarScreen},
            { GameScreen.TradingScreen, MarketScreen},
            { GameScreen.EndingScreen, _endingScreen}
        };
        foreach (var screen in _allScreens.Values)
        {
            if (screen.GetType() == typeof(MainMenuScreen)) continue;
            
            screen.Initialize();
            screen.LoadContent(Content, Graphics);
        }

        AllGameItems.KnownRunesCraftRecipes = new Dictionary<string, bool>();
        AllGameItems.KnownScrollsCraftRecipes = new List<ScrollsRecipes.ScrollInfo>();
        AllGameItems.RuneUniteRecipes = new Dictionary<(string mainRuneId, string secondaryRuneId), string>();
        AllGameItems.ResetAllScrollsPrices();
        TopDownCore = new TopDownCore(Content, this);
        TopDownCore.Initialize(Content);
        _backStoryScreen.Reset();
        SetScreen(GameScreen.Backstory);
        Balance = 0;
        DayCount = 0;
        Energy = 100;
        IsInTopDownView = false;
    }

    public static string GetText(string text) => ResManager.GetString(text);

    public void ResetBackStory() => _backStoryScreen.Reset();
    
    public void SetMusicVolume(float volume) => MediaPlayer.Volume = volume;
    
    public void SetSoundsVolume(float volume) => SoundEffect.MasterVolume = volume;
    
    public void AddToBalance(int sum) => Balance += sum;
    public void SubtractFromBalance(int sum) => Balance = Balance - sum >= 0 ? Balance - sum : 0;
}