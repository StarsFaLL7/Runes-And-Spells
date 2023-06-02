using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
    
    public static Dictionary<string, (Texture2D texture2D, string rus)> ScrollsTypes =>
        ItemsDataHolder.Scrolls.ScrollsTypes;
    public static Dictionary<string, ItemInfo> Scrolls => ItemsDataHolder.Scrolls.AllScrolls;
    private static Dictionary<string[], string> ScrollCraftRecipes => ItemsDataHolder.Scrolls.ScrollCraftRecipes;
    public static Dictionary<string, (bool isVisible, Texture2D Texture)> ScrollsRecipesTextures => 
        ItemsDataHolder.Scrolls.ScrollsRecipesTextures;
    public static Dictionary<string, ItemInfo> Catalysts => ItemsDataHolder.Catalysts.AllCatalysts;

    public static Dictionary<string, (int Size, bool IsFull, Texture2D HalfTexture, Texture2D FullTexture)>
        KnownRunesCraftRecipes { get; private set; } = new();
    public static Dictionary<(string mainRuneId, string secondaryRuneId), string> RuneUniteRecipes { get; } = new ();

    public static Dictionary<string, int> SellingScrollsPrices { get; } = new ();

    public static void Initialize(ContentManager content)
    {
        _content = content;
        ToolTipFont = content.Load<SpriteFont>("ToolTipFont");
        ItemsDataHolder.Initialize(content);
        ResetAllScrollsPrices();
    }

    public static void ResetAllScrollsPrices()
    {
        foreach (var type in ScrollsTypes.Keys.Where(k => k != "ancient"))
            SellingScrollsPrices[type] = Random.Shared.Next(30, 70)/5*5;
    }
    
    public static void MakeSmallChangesToPrices()
    {
        foreach (var price in SellingScrollsPrices)
        {
            var modifier = Random.Shared.Next(-15, 15);
            SellingScrollsPrices[price.Key] = price.Value + modifier >= 20 ?
                price.Value + modifier <= 100 ? 
                    price.Value + modifier 
                    : 100 
                : 20;
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
        var runeId = resultId?[13..];
        if (runeId is not null && !KnownRunesCraftRecipes.ContainsKey(runeId))
        {
            KnownRunesCraftRecipes.Add(runeId, (3, false,
                _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{runeId}_half"),
                _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{runeId}_full")));
        }
        return resultId ?? "rune_unknown_failed";
    }

    public static void SetRuneRecipeFull(string id)
    {
        if (id.Contains("failed")) return;
        var recipeId = id;
        if (id.Split('_').Length > 3) recipeId = id[13..];

        KnownRunesCraftRecipes[recipeId] = (3, true,
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_half"),
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_full")
            );
    }

    public static bool TryToGetScrollByRunes(out Item? scroll, params Item[] runes)
    {
        foreach (var scrollRecipe in ScrollCraftRecipes)
        {
            if (scrollRecipe.Key.Length != runes.Length)
                continue;
            if ((scrollRecipe.Key[0] == runes[0].ID && scrollRecipe.Key[1] == runes[1].ID) || 
                (scrollRecipe.Key[1] == runes[0].ID && scrollRecipe.Key[0] == runes[1].ID))
            {
                var newScroll = Scrolls[scrollRecipe.Value];
                scroll = new Item(newScroll);
                if (!ScrollsRecipesTextures[scrollRecipe.Value].isVisible)
                    ScrollsRecipesTextures[scrollRecipe.Value] = (true, ScrollsRecipesTextures[scrollRecipe.Value].Texture);
                return true;
            }
        }
        scroll = null;
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
}