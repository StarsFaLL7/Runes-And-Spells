using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.classes;

public record ItemInfo(string ID, Texture2D Texture, ItemType Type, string ToolTip);

public static class AllGameItems
{
    public const int HoverTime = 1000;
    public static SpriteFont ToolTipFont;
    
    private static ContentManager _content;
    private static readonly Dictionary<int, (string id, string rus)> Elements = new()
    {
        {1, ("water", "Вода")},
        {2, ("ice", "Лед")},
        {3, ("grass", "Земля")},
        {4, ("fire", "Огонь")},
        {5, ("blood", "Кровь")},
        {6, ("distorted", "Искажение")},
        {7, ("black", "Тьма")},
        {8, ("air", "Воздух")},
        {9, ("moon", "Луна")}
    };

    private static Dictionary<string, (Texture2D texture2D, string rus)> ScrollsTypes;
    public static Dictionary<string, ItemInfo> FinishedRunes { get; private set; }
    public static ItemInfo Clay;
    public static ItemInfo ClaySmall;
    public static Dictionary<string, ItemInfo> UnknownRunes { get; private set; }
    
    public static Dictionary<string, ItemInfo> Scrolls { get; private set; }

    private static Dictionary<List<bool>, string> _runesCreateRecipes;
    public static Dictionary<string, (int Size, bool IsFull, Texture2D HalfTexture, Texture2D FullTexture)> KnownRunesCraftRecipes
    { get; private set; }

    private static Dictionary<string[], string> _scrollCraftRecipes;
    public static Dictionary<string, (bool isVisible, Texture2D Texture)> ScrollsRecipes { get; private set; }

    public static void Initialize(ContentManager content)
    {
        _content = content;
        ToolTipFont = ToolTipFont = content.Load<SpriteFont>("ToolTipFont");
        FinishedRunes = new Dictionary<string, ItemInfo>();
        for (var i = 1; i < 10; i++)
        for (var j = 1; j < 7; j++)
                FinishedRunes.Add($"rune_finished_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                    new ItemInfo($"rune_finished_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                        content.Load<Texture2D>($"textures/runes/spr_rune_x_{j + 6*(i-1)}"), 
                        ItemType.Rune,
                        $"Стихия: {Elements[i].rus}\n" +
                        $"Сила: {(j - 1) / 2 + 1}\n" +
                        $"Вид: {(j - 1) % 2 + 1}"));
        
        Clay = new ItemInfo("clay", 
            content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay"), 
            ItemType.Clay,
            "Глина");
        ClaySmall = new ItemInfo("clay_small", 
            content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay_small"), 
            ItemType.ClaySmall,
            "Кусочки глины");

        UnknownRunes = new Dictionary<string, ItemInfo>();
        for (var i = 1; i < 10; i++)
        for (var j = 1; j < 7; j++)
            UnknownRunes.Add($"rune_unknown_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                new ItemInfo($"rune_unknown_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}",
                    content.Load<Texture2D>($"textures/Inventory/items/empty_rune_{Elements[i].id}"), 
                    ItemType.UnknownRune,
                    "Неизвестная руна"));
        UnknownRunes.Add("rune_unknown_failed", 
            new ItemInfo("rune_unknown_failed", 
                content.Load<Texture2D>("textures/Inventory/items/empty_rune_failed"), 
                ItemType.UnknownRune,
                "Неизвестная руна"));
        ScrollsRecipes = new Dictionary<string, (bool isVisible, Texture2D Texture)>();
        AddScrolls();
        AddRunesCraftRecipes();
        KnownRunesCraftRecipes = new Dictionary<string, (int Size, bool IsFull, Texture2D HalfTexture, Texture2D FullTexture)>();
        
    }

    public static string GetIdByRecipe(List<bool> scheme)
    {
        var resultId = _runesCreateRecipes
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

    public static void SetRecipeFull(string id)
    {
        if (id.Contains("failed")) return;
        var recipeId = id;
        if (id.Split('_').Length > 3) recipeId = id[13..];

        if (KnownRunesCraftRecipes.ContainsKey(recipeId)) KnownRunesCraftRecipes.Remove(recipeId);
        
        KnownRunesCraftRecipes.Add(recipeId, (3, true,
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_half"),
            _content.Load<Texture2D>($"textures/rune_crafting_table/recipes/recipe_{recipeId}_full")));
    }

    private static void AddScrolls()
    {
        ScrollsTypes = new()
        {
            {"nature", (_content.Load<Texture2D>("textures/scrolls/scroll_nature"), "Магия природы")},
            {"ancient", (_content.Load<Texture2D>("textures/scrolls/scroll_ancient"), "Магия древних")},
            {"corrupted", (_content.Load<Texture2D>("textures/scrolls/scroll_corrupted"), "Тёмная магия")},
            {"faerie", (_content.Load<Texture2D>("textures/scrolls/scroll_faerie"), "Магия фей")},
            {"ice", (_content.Load<Texture2D>("textures/scrolls/scroll_ice"), "Магия льда")},
            {"life", (_content.Load<Texture2D>("textures/scrolls/scroll_life"), "Магия жизни")},
            {"sun", (_content.Load<Texture2D>("textures/scrolls/scroll_sun"), "Магия солнца")},
            {"toxic", (_content.Load<Texture2D>("textures/scrolls/scroll_toxic"), "Магия яда")},
            {"wind", (_content.Load<Texture2D>("textures/scrolls/scroll_wind"), "Магия ветра")},
            {"ocean", (_content.Load<Texture2D>("textures/scrolls/scroll_ocean"), "Магия океана")}
        };
        var ExistingScrolls = new (string id, string rus)[]
        {
            ("corrupted_curse_1", "проклятия"), ("corrupted_fear_1", "страха"), ("corrupted_hate_1", "ненависти"), ("corrupted_necromancy_1", "некромантии"), 
            ("faerie_apparitions_1", "видений"), ("faerie_charming_1", "очарования"), ("faerie_nightmare_1", "кошмаров"), 
            ("ice_cursedIce_1", "проклятого льда"), ("ice_magicIce_1", "магического льда"), ("ice_snowTornado_1", "снежного бурана"), 
            ("life_bloodControl_1", "управления кровью"), ("life_freezeBlood_1", "морозной крови"), 
            ("nature_growth_1", "роста"), ("nature_heal_1", "лечения"), ("nature_strength_1", "силы"), 
            ("ocean_flow_1", "водяного потока"), ("ocean_tide_1", "прилива"), 
            ("sun_explosion_1", "взрыва"), ("sun_lava_1", "кристал. лавы"), ("sun_tornado_1", "огненного смерча"), 
            ("toxic_blackHole_1", "червоточины"), ("toxic_poison_1", "отравления"), ("toxic_rage_1", "ярости"), 
            ("wind_fog_1", "тумана"), ("wind_levitation_1", "левитации") 
        };
        Scrolls = new Dictionary<string, ItemInfo>();
        foreach (var scrollId in ExistingScrolls)
        {
            Scrolls.Add($"scroll_{scrollId.id}", new ItemInfo(
                $"scroll_{scrollId.id}", 
                ScrollsTypes[scrollId.id.Split('_')[0]].texture2D, 
                ItemType.Scroll, 
                $"Свиток {scrollId.rus}\nСила: {scrollId.id.Split('_')[2]}"));
            ScrollsRecipes.Add($"scroll_{scrollId.id}", 
                (false, _content.Load<Texture2D>($"textures/scrolls/recipes/recipe_{scrollId.id}")));
        }

        _scrollCraftRecipes = new Dictionary<string[], string>()
        {
            {new[] {"rune_finished_water_1_1", "rune_finished_dirt_1_1"}, "scroll_nature_heal_1"},
            {new[] {"rune_finished_water_1_2", "rune_finished_fire_1_2"}, "scroll_wind_fog_1"}, 
            {new[] {"rune_finished_water_1_1", "rune_finished_water_1_2"}, "scroll_ocean_flow_1"},
            {new[] {"rune_finished_water_1_1", "rune_finished_air_1_1"}, "scroll_ocean_tide_1"},
            {new[] {"rune_finished_air_1_1", "rune_finished_fire_1_1"}, "scroll_sun_tornado_1"},
            {new[] {"rune_finished_air_1_2", "rune_finished_dirt_1_2"}, "scroll_nature_strength_1"},
            {new[] {"rune_finished_air_1_1", "rune_finished_air_1_2"}, "scroll_wind_levitation_1"},
            {new[] {"rune_finished_fire_1_1", "rune_finished_dirt_1_1"}, "scroll_sun_lava_1"},
            {new[] {"rune_finished_fire_1_1", "rune_finished_fire_1_2"}, "scroll_sun_explosion_1"},
            
            {new[] {"rune_finished_blood_1_1", "rune_finished_moon_1_1"}, "scroll_faerie_apparitions_1"},
            {new[] {"rune_finished_blood_1_1", "rune_finished_ice_1_1"}, "scroll_life_freezeBlood_1"},
            {new[] {"rune_finished_blood_1_2", "rune_finished_distorted_1_1"}, "scroll_toxic_poison_1"},
            {new[] {"rune_finished_blood_1_2", "rune_finished_black_1_2"}, "scroll_corrupted_necromancy_1"},
            {new[] {"rune_finished_blood_1_1", "rune_finished_blood_1_2"}, "scroll_life_bloodControl_1"},
            {new[] {"rune_finished_moon_1_1", "rune_finished_ice_1_1"}, "scroll_ice_magicIce_1"},
            {new[] {"rune_finished_moon_1_2", "rune_finished_distorted_1_2"}, "scroll_faerie_nightmare_1"},
            {new[] {"rune_finished_moon_1_2", "rune_finished_black_1_2"}, "scroll_corrupted_fear_1"},
            {new[] {"rune_finished_moon_1_1", "rune_finished_moon_1_2"}, "scroll_faerie_charming_1"},
            {new[] {"rune_finished_ice_1_2", "rune_finished_distorted_1_2"}, "scroll_ice_cursedIce_1"},
            {new[] {"rune_finished_ice_1_2", "rune_finished_black_1_2"}, "scroll_corrupted_hate_1"},
            {new[] {"rune_finished_ice_1_1", "rune_finished_ice_1_2"}, "scroll_ice_snowTornado_1"},
            {new[] {"rune_finished_distorted_1_1", "rune_finished_black_1_1"}, "toxic_rage_1"},
            {new[] {"rune_finished_distorted_1_1", "rune_finished_distorted_1_2"}, "scroll_toxic_blackHole_1"},
            {new[] {"rune_finished_black_1_1", "rune_finished_black_1_2"}, "scroll_corrupted_curse_1"}
        };
    }

    public static bool TryToGetScrollByRunes(out Item? scroll, params Item[] runes)
    {
        foreach (var scrollRecipe in _scrollCraftRecipes)
        {
            if (scrollRecipe.Key.Length != runes.Length)
                continue;
            if ((scrollRecipe.Key[0] == runes[0].ID && scrollRecipe.Key[1] == runes[1].ID) || 
                (scrollRecipe.Key[1] == runes[0].ID && scrollRecipe.Key[0] == runes[1].ID))
            {
                var newScroll = Scrolls[scrollRecipe.Value];
                scroll = new Item(newScroll);
                if (!ScrollsRecipes[scrollRecipe.Value].isVisible)
                    ScrollsRecipes[scrollRecipe.Value] = (true, ScrollsRecipes[scrollRecipe.Value].Texture);
                return true;
            }
        }
        scroll = null;
        return false;
    }
    
    private static void AddRunesCraftRecipes()
    {
        _runesCreateRecipes = new Dictionary<List<bool>, string>()
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
            }, "rune_unknown_air_1_2"},
            {new List<bool>() {
                false, true, false,
                false, true, false,
                true, false, true
            }, "rune_unknown_ice_1_1"},
            {new List<bool>() {
                false, true, true,
                true, true, false,
                true, false, false
            }, "rune_unknown_ice_1_2"},
            {new List<bool>() {
                false, true, false,
                true, false, true,
                true, true, false
            }, "rune_unknown_moon_1_1"},
            {new List<bool>() {
                false, false, false,
                true, true, true,
                true, true, true
            }, "rune_unknown_moon_1_2"},
            {new List<bool>() {
                false, true, false,
                true, false, true,
                true, true, true
            }, "rune_unknown_blood_1_1"},
            {new List<bool>() {
                false, true, true,
                true, false, false,
                false, true, true
            }, "rune_unknown_blood_1_2"},
            {new List<bool>() {
                true, false, true,
                false, false, false,
                false, true, false
            }, "rune_unknown_distorted_1_1"},
            {new List<bool>() {
                false, false, false,
                true, false, true,
                true, true, true
            }, "rune_unknown_distorted_1_2"},
            {new List<bool>() {
                true, true, true,
                true, false, true,
                false, false, false
            }, "rune_unknown_black_1_1"},
            {new List<bool>() {
                true, true, true,
                false, true, false,
                true, true, true
            }, "rune_unknown_black_1_2"},

        };
    }
}