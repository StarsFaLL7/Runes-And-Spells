using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.classes;

public static class AllGameItems
{
    public static SpriteFont ToolTipFont;

    private static ContentManager _content;
    private static readonly Dictionary<int, string> Elements = new()
    {
        {1, "water"},
        {2, "ice"},
        {3, "grass"},
        {4, "fire"},
        {5, "blood"},
        {6, "distorted"},
        {7, "black"},
        {8, "air"},
        {9, "moon"}
    };
    public static Dictionary<string, (Texture2D Texture, ItemType Type, bool IsDraggable)> FinishedRunes { get; private set; }
    public static (string ID, Texture2D Texture, ItemType Type, bool isDraggable) Clay;
    public static (string ID, Texture2D Texture, ItemType Type, bool isDraggable) ClaySmall;
    public static Dictionary<string, (Texture2D Texture, ItemType Type, bool IsDraggable)> UnknownRunes { get; private set; }
    public static Dictionary<List<bool>, string> RunesCreateRecipes { get; private set;  }
    public static Dictionary<string, (int Size, bool IsFull, Texture2D HalfTexture, Texture2D FullTexture)> KnownCraftRecipes
    { get; private set; }

    public static void Initialize(ContentManager content)
    {
        _content = content;
        ToolTipFont = ToolTipFont = content.Load<SpriteFont>("ToolTipFont");
        FinishedRunes = new Dictionary<string, (Texture2D Texture, ItemType Type, bool IsDraggable)>();
        for (var i = 1; i < 10; i++)
        for (var j = 1; j < 7; j++)
                FinishedRunes.Add($"rune_finished_{Elements[i]}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                    (content.Load<Texture2D>($"textures/runes/spr_rune_x_{j + 6*(i-1)}"), ItemType.Rune, true));
        Clay = new ValueTuple<string, Texture2D, ItemType, bool>("clay", 
            content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay"), ItemType.Clay, true);
        ClaySmall = new ValueTuple<string, Texture2D, ItemType, bool>("clay_small", 
            content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay_small"), ItemType.ClaySmall, true);

        UnknownRunes = new Dictionary<string, (Texture2D Texture, ItemType Type, bool IsDraggable)>();
        for (var i = 1; i < 10; i++)
        for (var j = 1; j < 7; j++)
            UnknownRunes.Add($"rune_unknown_{Elements[i]}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                (content.Load<Texture2D>($"textures/Inventory/items/empty_rune_{Elements[i]}"), ItemType.UnknownRune, true));
        UnknownRunes.Add("rune_unknown_failed", 
            (content.Load<Texture2D>("textures/Inventory/items/empty_rune_failed"), ItemType.UnknownRune, true));
        AddRecipes();
        KnownCraftRecipes = new Dictionary<string, (int Size, bool IsFull, Texture2D HalfTexture, Texture2D FullTexture)>();
    }

    public static string GetIdByRecipe(List<bool> scheme)
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
        if (runeId is not null && !KnownCraftRecipes.ContainsKey(runeId))
        {
            KnownCraftRecipes.Add(runeId, (3, false,
                _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{runeId}_half"),
                _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{runeId}_full")));
        }
        return resultId ?? "rune_unknown_failed";
    }

    public static void SetRecipeFull(string id)
    {
        if (id.Contains("failed")) return;
        var recipeId = id;
        if (id.Split('_').Length > 3) recipeId = id[13..];

        if (KnownCraftRecipes.ContainsKey(recipeId)) KnownCraftRecipes.Remove(recipeId);
        
        KnownCraftRecipes.Add(recipeId, (3, true,
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_half"),
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_full")));
    }
    
    private static void AddRecipes()
    {
        RunesCreateRecipes = new Dictionary<List<bool>, string>()
        {
            {new List<bool>() {
                false, true, false,
                true, false, true,
                false, true, false
            }, "rune_unknown_water_1_1"},
            {new List<bool>() {
                false, true, false,
                false, false, false,
                true, false, true
            }, "rune_unknown_water_1_2"},
            {new List<bool>() {
                false, false, true,
                true, true, true,
                true, false, true
            }, "rune_unknown_grass_1_1"},
            {new List<bool>() {
                false, false, false,
                false, true, false,
                false, false, false
            }, "rune_unknown_grass_1_2"},
            {new List<bool>() {
                true, false, true,
                true, false, true,
                true, false, true
            }, "rune_unknown_fire_1_1"},
            {new List<bool>() {
                false, false, true,
                true, false, false,
                false, true, false
            }, "rune_unknown_fire_1_2"},
            {new List<bool>() {
                false, true, false,
                true, false, true,
                false, false, false
            }, "rune_unknown_air_1_1"},
            {new List<bool>() {
                false, true, false,
                true, true, true,
                true, false, false
            }, "rune_unknown_air_1_2"}
        };
    }
}