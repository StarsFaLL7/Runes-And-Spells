using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses;

namespace Runes_and_Spells.UtilityClasses;

public record ItemInfo(string ID, Texture2D Texture, ItemType Type, string ToolTip);

public static class AllGameItems
{
    public const int HoverTime = 1000;
    public static SpriteFont ToolTipFont;
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
    public static List<ScrollsRecipes.ScrollInfo> KnownScrollsCraftRecipes { get; } = new ();
    public static Dictionary<string, bool> KnownRunesCraftRecipes { get; } = new();
    public static Dictionary<(string mainRuneId, string secondaryRuneId), string> RuneUniteRecipes { get; } = new ();

    public static Dictionary<ScrollType, int> SellingScrollsPrices { get; } = new ();

    public static void Initialize(ContentManager content)
    {
        _content = content;
        ToolTipFont = content.Load<SpriteFont>("ToolTipFont");
        ItemsDataHolder.Initialize(content);
        ResetAllScrollsPrices();
    }

    public static void ResetAllScrollsPrices()
    {
        foreach (var type in ScrollsTypesInfo.Keys)
            SellingScrollsPrices[type] = Random.Shared.Next(30, 60)/5*5;
    }
    
    public static void MakeSmallChangesToScrollPrices()
    {
        foreach (var price in SellingScrollsPrices)
        {
            var modifier = Random.Shared.Next(-15, 10);
            SellingScrollsPrices[price.Key] = Math.Min(Math.Max(price.Value + modifier, 20), 100);
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
            return "rune_unknown_failed";
        
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

    public static bool TryToUniteRunes(string mainRuneId, string secondaryRuneId, out string resultRuneId)
    {
        var mainId = mainRuneId.Split('_');
        var secondId = secondaryRuneId.Split('_');
        if (mainId[2] == secondId[2] && mainId[3] == secondId[3] && mainId[4] != secondId[4] && mainId[3] != "3")
        {
            resultRuneId = string.Join('_', "rune", "finished", mainId[2], int.Parse(mainId[3]) + 1, mainId[4]);
            RuneUniteRecipes[(mainRuneId, secondaryRuneId)] = resultRuneId;
            return true;
        }
        resultRuneId = null;
        return false;
    }

    public static void TryToUnlockScrollRecipe(ScrollsRecipes.ScrollInfo scrollInfo)
    {
        if (!KnownScrollsCraftRecipes.Contains(scrollInfo))
        {
            KnownScrollsCraftRecipes.Add(scrollInfo);
        }
    }
}