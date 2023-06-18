using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.classes;

namespace RunesAndSpells.Tests;

public class Tests
{
    private Game1 _game = new Game1();
    private const double TOLERANCE = 1e-5;
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ScreensAreChangingCorrectly()
    {
        _game.SetScreen(GameScreen.AltarScreen);
        Assert.That(_game.CurrentScreen, Is.EqualTo(GameScreen.AltarScreen));
        _game.SetScreen(GameScreen.Menu);
        Assert.That(_game.CurrentScreen, Is.EqualTo(GameScreen.Menu));
        _game.SetScreen(GameScreen.FurnaceScreen);
        Assert.That(_game.CurrentScreen, Is.EqualTo(GameScreen.FurnaceScreen));
    }

    [Test]
    public void TimerStopsProperly()
    {
        var timer = new Runes_and_Spells.UtilityClasses.Timer(1000, () => {});
        Assert.That(!timer.IsRunning);
        timer.Start();
        for (var i = 0; i < 1000; i++)
            timer.Tick();
        Assert.That(timer.Time, Is.EqualTo(0));
        Assert.That(!timer.IsRunning);
    }
    
    [Test]
    public void TimerResetsProperly()
    {
        var timer = new Runes_and_Spells.UtilityClasses.Timer(100, () => {});
        timer.Start();
        for (var i = 0; i < 100; i++)
            timer.Tick();
        Assert.That(timer.Time, Is.EqualTo(0));
        
        timer.StartAgain();
        for (var i = 0; i < 100; i++)
            timer.Tick();
        Assert.That(timer.Time, Is.EqualTo(0));
        Assert.That(!timer.IsRunning);
    }
    
    [Test]
    public void UiSliderClampsInvalidValues()
    {
        var slider = new UiSlider(
            new Texture2D(_game.Graphics.GraphicsDevice, 100, 100), 
            new Texture2D(_game.Graphics.GraphicsDevice, 10, 10),
            new Vector2(), 
            1, 
            0,  100, 50);
        Assert.That(Math.Abs(slider.Value - 50) < TOLERANCE);
        slider.SetValue(500);
        Assert.That(Math.Abs(slider.Value - 100) < TOLERANCE);
        slider.SetValue(-100);
        Assert.That(slider.Value == 0);
    }
    
    [Test]
    public void ProgressBarStaysInInterval()
    {
        var pg = new UiProgressBar(
            new Texture2D(_game.Graphics.GraphicsDevice, 100, 100), 
            new Texture2D(_game.Graphics.GraphicsDevice, 100, 100),
            Vector2.Zero, 
            10, 150, 20);
        Assert.That(Math.Abs(pg.Value - 20) < TOLERANCE);
        pg.Add(1000);
        Assert.That(Math.Abs(pg.Value - 150) < TOLERANCE);
        pg.Subtract(200);
        Assert.That(Math.Abs(pg.Value - 10) < TOLERANCE);
        pg.SetValue(999);
        Assert.That(Math.Abs(pg.Value - 150) < TOLERANCE);
    }

    [Test]
    public void ProgressBarThrowsExceptionWhenWrongArgument()
    {
        try
        {
            var pg = new UiProgressBar(
                new Texture2D(_game.Graphics.GraphicsDevice, 100, 100),
                new Texture2D(_game.Graphics.GraphicsDevice, 100, 100),
                Vector2.Zero,
                10, 150, 20);
            pg.Add(-10);
            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.Pass();
        }
        Assert.Fail();
    }

    //[Test]
    public void RightRuneReturnByScheme()
    {
        var id = AllGameItems.GetRuneIdByRecipe(
            new List<bool>(){
                false, false, false, 
                false, true, false, 
                false, false, false
            });
        Assert.That(id == "rune_unknown_grass_1_2");
        Assert.That(AllGameItems.KnownRunesCraftRecipes.ContainsKey("grass_1_2"));
        id = AllGameItems.GetRuneIdByRecipe(
            new List<bool>(){
                true, false, true,
                true, false, true,
                true, false, true
            });
        Assert.That(id == "rune_unknown_fire_1_1");
        Assert.That(AllGameItems.KnownRunesCraftRecipes.ContainsKey("fire_1_1"));
    }
    
    //[Test]
    public void RecipesBecomeFullProperly()
    {
        var id = Runes_and_Spells.UtilityClasses.AllGameItems.GetRuneIdByRecipe(
            new List<bool>(){
                false, false, false, 
                false, true, false, 
                false, false, false
            });
        AllGameItems.SetRuneRecipeFull(id);
        Assert.That(AllGameItems.KnownRunesCraftRecipes["grass_1_2"]);
    }

    [Test]
    public void ItemCreatesFromItemInfo()
    {
        var item = new Item(new ItemInfo("clay", 
            _game.Content.Load<Texture2D>("textures/Inventory/items/piece_of_clay"), 
            ItemType.Clay,
            "Глина"));
        Assert.That(item.ID == "clay" && item.Type == ItemType.Clay && item.Count == 1);
    }

    //[Test]
    public void FurnaceMiniGameTest()
    {
        var pg = new UiProgressBar(
            new Texture2D(_game.Graphics.GraphicsDevice, 100, 100),
            new Texture2D(_game.Graphics.GraphicsDevice, 100, 100),
            Vector2.Zero,
            10, 150, 20);
        var inpSlot1 = new UiSlot(
            Vector2.Zero, new Texture2D(_game.Graphics.GraphicsDevice, 10, 10), true);
        var minigame = new FurnaceMiniGame(pg, inpSlot1, Vector2.One, _game.Content, _game);
        inpSlot1.SetItem(new Item(ItemType.UnknownRune, new Texture2D(_game.Graphics.GraphicsDevice, 10, 10),
            "rune_unknown_grass_1_1", ""));
        minigame.Start(3);
        pg.SetValue(150);
        minigame.Update();
        Assert.That(inpSlot1.ContainsItem);
        Assert.That(inpSlot1.currentItem.ID == "rune_finished_grass_1_1");
    }

    //[Test]
    public void ScrollsCraft()
    {
        AllGameItems.TryToGetScrollByRunes(out var scroll, out var scrollInfo,
            new Item(AllGameItems.FinishedRunes["rune_finished_water_1_1"]),
            new Item(AllGameItems.FinishedRunes["rune_finished_grass_1_1"]));
        Assert.That(scroll.Count == 1);
        Assert.That(scroll.Type == ItemType.Scroll);
        Assert.That(scroll.ID == "scroll_nature_heal_1");
    }
}