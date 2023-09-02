using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses;

namespace Runes_and_Spells.UtilityClasses;

public record ItemInfo(string ID, Texture2D Texture, ItemType Type, string ToolTip);

public static class AllGameItems
{
    public const int HoverTime = 1000;
    
    public static SpriteFont ToolTipFont { get; private set; }
    public static SpriteFont Font18Px { get; private set; }
    public static SpriteFont Font20Px { get; private set; }
    public static SpriteFont Font24Px { get; private set; }
    public static SpriteFont Font30Px { get; private set; }
    public static SpriteFont Font36Px { get; private set; }
    public static SpriteFont Font40Px { get; private set; }

    private static ContentManager _content;
    
    public static ItemInfo Clay => ItemsDataHolder.OtherItems.Clay;
    public static ItemInfo ClaySmall => ItemsDataHolder.OtherItems.ClaySmall;
    public static ItemInfo Paper => ItemsDataHolder.OtherItems.Paper;
    
    public static Dictionary<string, ItemInfo> FinishedRunes => ItemsDataHolder.Runes.FinishedRunes;
    public static Dictionary<string, ItemInfo> UnknownRunes => ItemsDataHolder.Runes.UnknownRunes;
    private static Dictionary<List<bool>, string> RunesCreateRecipes => ItemsDataHolder.Runes.RunesCreateRecipes;
    
    public static Dictionary<ScrollType, (Texture2D texture2D, string rus)> ScrollsTypesInfo =>
        ItemsDataHolder.Scrolls.ScrollsTypesInfo;
    public static Dictionary<string, ItemInfo> Scrolls => ItemsDataHolder.Scrolls.AllScrolls;
    public static List<ScrollsRecipes.ScrollInfo> KnownScrollsCraftRecipes { get; set; } = new ();
    public static Dictionary<string, bool> KnownRunesCraftRecipes { get; set; } = new();
    public static Dictionary<(string mainRuneId, string secondaryRuneId), string> RuneUniteRecipes { get; set; } = new ();

    public static Dictionary<ScrollType, int> SellingScrollsPrices { get; set; } = new ();

    public static Texture2D ToolTipBackTexture;
    public static Texture2D ToolTipBorderTexture;
    public static SoundEffect ClickSound;
    public static SoundEffect ClickSound2;
    public static SoundEffect PageFlipSound;
    public static SoundEffect AlertSound;
    public static SoundEffect StepSound;
    public static Song MarketTheme;
    public static SoundEffect OutDoorTheme;
    public static SoundEffect ChestOpenDefault;
    public static SoundEffect ChestOpenMagic;

    public static void Initialize(ContentManager content)
    {
        _content = content;
        ToolTipFont = content.Load<SpriteFont>("ToolTipFont");
        Font18Px = content.Load<SpriteFont>("16PixelTimes18px");
        Font20Px = content.Load<SpriteFont>("16PixelTimes20px");
        Font24Px = content.Load<SpriteFont>("16PixelTimes24px");
        Font30Px = content.Load<SpriteFont>("16PixelTimes30px");
        Font36Px = content.Load<SpriteFont>("16PixelTimes36px");
        Font40Px = content.Load<SpriteFont>("16PixelTimes40px");
        
        ToolTipBackTexture = content.Load<Texture2D>("textures/Inventory/items/ToolTipBg");
        ToolTipBorderTexture = content.Load<Texture2D>("textures/Inventory/items/ToolTipBgOutline");
        ClickSound = content.Load<SoundEffect>("sounds/click_sound");
        ClickSound2 = content.Load<SoundEffect>("sounds/click_sound2");
        AlertSound = content.Load<SoundEffect>("sounds/alert_sound");
        PageFlipSound = content.Load<SoundEffect>("sounds/page_flip");
        StepSound = content.Load<SoundEffect>("sounds/step_sound");
        MarketTheme = content.Load<Song>("music/market_theme");
        OutDoorTheme = content.Load<SoundEffect>("music/outdoor_theme");
        ChestOpenDefault = content.Load<SoundEffect>("sounds/chest_open");
        ChestOpenMagic = content.Load<SoundEffect>("sounds/chest_open_magic");
        ItemsDataHolder.Initialize(content);
        ResetAllScrollsPrices();
    }

    public static void ResetAllScrollsPrices()
    {
        foreach (var type in ScrollsTypesInfo.Keys)
            SellingScrollsPrices[type] = Random.Shared.Next(30, 50)/5*5;
    }
    
    public static void MakeSmallChangesToScrollPrices()
    {
        foreach (var price in SellingScrollsPrices)
        {
            var modifier = Random.Shared.Next(-15, 15);
            SellingScrollsPrices[price.Key] = Math.Min(Math.Max(price.Value + modifier, 20), 60);
        }
    }

    public static string GetRuneIdByRecipe(List<bool> scheme)
    {
        var resultId = RunesCreateRecipes
            .Where(r => r.Key.Count == scheme.Count)
            .FirstOrDefault(r =>
            {
                for (var i = 0; i < r.Key.Count; i++)
                    if (r.Key[i] != scheme[i])
                        return false;
                return true;
            }).Value;
        if (resultId is null)
            return "clay_small";
        
        UnlockRuneCraftRecipe(resultId);
        
        return resultId;
    }

    public static void UnlockRuneCraftRecipe(string runeId)
    {
        if (!KnownRunesCraftRecipes.ContainsKey(runeId))
            KnownRunesCraftRecipes.Add(runeId, false);
    }

    public static void SetRuneRecipeFull(string id)
    {
        if (id.Contains("failed")) return;
        KnownRunesCraftRecipes[id] = true;
    }

    public static bool TryToGetScrollByRunes(out Item? scroll, out ScrollsRecipes.ScrollInfo scrollInfo, params Item[] runes)
    {
        return TryToGetScrollByRunes(out scroll, out scrollInfo, runes.Select(r => r.ID).ToArray());
    }
    
    public static bool TryToGetScrollByRunes(out Item? scroll, out ScrollsRecipes.ScrollInfo scrollInfo, params string[] ids)
    {
        if (ids.Length != 2)
        {
            scroll = null;
            scrollInfo = null;
            return false;
        }
        
        var rune1 = ids[0].Split('_');
        var rune2 = ids[1].Split('_');
        if (rune1[3] != rune2[3] ||
            ((rune1[2] != rune2[2] || rune1[4] == rune2[4]) &&
             (rune1[2] == rune2[2] || rune1[4] != rune2[4])))
        {
            scroll = null;
            scrollInfo = null;
            return false;
        }

        var runesIds = ids.Select(item => item.Split('_')[2] + "_" + item.Split('_')[4]).ToArray();

        foreach (var scrollRecipe in ScrollsRecipes.Recipes)
        {
            if ((scrollRecipe.Key.runeElementAndVariant1 == runesIds[0] && scrollRecipe.Key.runeElementAndVariant2 == runesIds[1]) || 
                (scrollRecipe.Key.runeElementAndVariant2 == runesIds[0] && scrollRecipe.Key.runeElementAndVariant1 == runesIds[1]))
            {
                var newScroll = new Item(
                    ItemType.Scroll, 
                    ItemsDataHolder.Scrolls.ScrollsTypesInfo[scrollRecipe.Value.Type].texture2D, 
                    scrollRecipe.Value.id + "_" + scrollRecipe.Value.Type.ToString().ToLower() + "_" +  rune1[3], 
                    $"Свиток {scrollRecipe.Value.rus}\n" +
                    $"Тип: {ItemsDataHolder.Scrolls.ScrollsTypesInfo[scrollRecipe.Value.Type].rus}\n" +
                    $"Сила: {rune1[3]}");
                scroll = newScroll;
                scrollInfo = scrollRecipe.Value;
                return true;
            }
        }
        scroll = null;
        scrollInfo = null;
        return false;
    }

    public static void TryToUnlockScrollRecipe(ScrollsRecipes.ScrollInfo scrollInfo)
    {
        if (!KnownScrollsCraftRecipes.Contains(scrollInfo))
        {
            KnownScrollsCraftRecipes.Add(scrollInfo);
        }
    }
    
    public static void DrawToolTip(Item toolTipItem, SpriteBatch spriteBatch)
    {
        var toolTipText = GetToolTipText(toolTipItem);
        if (toolTipItem.Type == ItemType.Scroll)
        {
            var id = toolTipItem.ID.Split('_');
            var element = (ScrollType)Enum.Parse(typeof(ScrollType), id[4], true);
            var power = int.Parse(id[5]);
            toolTipText += $"\n{Game1.GetText("Цена продажи")}: {SellingScrollsPrices[element]+20*(power-1)}";
        }
        
        var strSize = ToolTipFont.MeasureString(toolTipText);
        var rect = new Rectangle(
            (int)(((ToolTipBackTexture.Width - (int)strSize.X) / 2 - 16)*Game1.ResolutionScale.X), 
            (int)(((ToolTipBackTexture.Height - (int)strSize.Y) / 2 - 16)*Game1.ResolutionScale.Y),
            (int)((strSize.X + 16)*Game1.ResolutionScale.X), (int)((strSize.Y + 6)*Game1.ResolutionScale.Y));
        
        var posToDraw = new Vector2(Mouse.GetState().X + 12, Mouse.GetState().Y + 12);
        if (Mouse.GetState().X > Game1.ResolutionScale.X*1920 - rect.Width - 20)
            posToDraw = new Vector2(Mouse.GetState().X - rect.Width, Mouse.GetState().Y + 12);
        
        var border = (int)(6 * Game1.ResolutionScale.X);
        spriteBatch.Draw(ToolTipBorderTexture, new Vector2(posToDraw.X - border/2, posToDraw.Y - border/2), 
            new Rectangle(rect.X-border, rect.Y-border, rect.Width + border, rect.Height + border), 
            Color.White);
        
        spriteBatch.Draw(ToolTipBackTexture, posToDraw, rect, Color.White);
        
        var toolTipColor = Color.FromNonPremultiplied(48, 56, 67, 255);
        spriteBatch.DrawString(ToolTipFont, toolTipText,
            new Vector2(posToDraw.X+border*1.5f, posToDraw.Y+border/2), toolTipColor, 0f, Vector2.Zero, 
            Game1.ResolutionScale,SpriteEffects.None, 0f);
    }

    private static string GetToolTipText(Item item)
    {
        var toolTipText = item.ToolTipText;
        if (item.Type is ItemType.ClaySmall or ItemType.Clay or ItemType.Key or ItemType.Paper)
        {
            return Game1.GetText(item.ToolTipText);
        }
        if (item.Type == ItemType.Essence)
        {
            return Game1.GetText(toolTipText.Split('\n')[0]) +
                   $"\n{Game1.GetText("Power")}: {toolTipText[^1]}";
        }

        if (item.Type == ItemType.UnknownRune && (!KnownRunesCraftRecipes.ContainsKey(item.ID) || !KnownRunesCraftRecipes[item.ID]))
        {
            return Game1.GetText("Unknown rune cast");
        }

        if (item.Type == ItemType.Rune || (item.Type == ItemType.UnknownRune && KnownRunesCraftRecipes[item.ID]))
        {
            var info = item.ID.Split('_');
            var element = info[2];
            var prefix = item.Type == ItemType.Rune ? Game1.GetText("Rune") : Game1.GetText("Rune cast");
            return $"{prefix}\n" +
                   $"{Game1.GetText("Element")}: {Game1.GetText(element)}\n" +
                   $"{Game1.GetText("Power")}: {info[3]}\n" +
                   $"{Game1.GetText("Type")}: {info[4]}";
        }

        if (item.Type == ItemType.Scroll)
        {
            var info = item.ID.Split('_');
            var splittedToolTip = item.ToolTipText.Split('\n');
            var name = Game1.GetText(splittedToolTip[0].Replace("Свиток ", ""));
            var element = splittedToolTip[1].Replace("Тип: ", "");
            return $"{Game1.GetText("Scroll of")} {name}\n" +
                   $"{Game1.GetText("Type")}: {Game1.GetText(element)}\n" +
                   $"{Game1.GetText("Power")}: {info[5]}";
        }
        return "";
    }
}