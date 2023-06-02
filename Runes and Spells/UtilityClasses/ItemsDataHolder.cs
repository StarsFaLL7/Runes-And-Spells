using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runes_and_Spells.UtilityClasses;

public static class ItemsDataHolder
{

    public static class Scrolls
    {
        public static Dictionary<string, (Texture2D texture2D, string rus)> ScrollsTypes;
    
        public static readonly Dictionary<string, ItemInfo> AllScrolls = new ();

        public static Dictionary<string, (bool isVisible, Texture2D Texture)> ScrollsRecipesTextures { get; } = new();
        
        public static readonly (string id, string rus)[] ExistingScrollsRusNames = new (string id, string rus)[]
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
        
        public static readonly Dictionary<string[], string> ScrollCraftRecipes = new ()
        {
            {new[] {"rune_finished_water_1_1", "rune_finished_grass_1_1"}, "scroll_nature_heal_1"},
            {new[] {"rune_finished_water_1_2", "rune_finished_fire_1_2"}, "scroll_wind_fog_1"}, 
            {new[] {"rune_finished_water_1_1", "rune_finished_water_1_2"}, "scroll_ocean_flow_1"},
            {new[] {"rune_finished_water_1_1", "rune_finished_air_1_1"}, "scroll_ocean_tide_1"},
            {new[] {"rune_finished_air_1_1", "rune_finished_fire_1_1"}, "scroll_sun_tornado_1"},
            {new[] {"rune_finished_air_1_2", "rune_finished_grass_1_2"}, "scroll_nature_strength_1"},
            {new[] {"rune_finished_air_1_1", "rune_finished_air_1_2"}, "scroll_wind_levitation_1"},
            {new[] {"rune_finished_fire_1_1", "rune_finished_grass_1_1"}, "scroll_sun_lava_1"},
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

        public static void Initialize(ContentManager content)
        {
            ScrollsTypes = new ()
            {
                {"nature", (content.Load<Texture2D>("textures/scrolls/scroll_nature"), "Магия природы")},
                {"wind", (content.Load<Texture2D>("textures/scrolls/scroll_wind"), "Магия ветра")},
                {"ocean", (content.Load<Texture2D>("textures/scrolls/scroll_ocean"), "Магия океана")},
                {"sun", (content.Load<Texture2D>("textures/scrolls/scroll_sun"), "Магия солнца")},
                {"faerie", (content.Load<Texture2D>("textures/scrolls/scroll_faerie"), "Магия фей")},
                {"life", (content.Load<Texture2D>("textures/scrolls/scroll_life"), "Магия жизни")},
                {"ice", (content.Load<Texture2D>("textures/scrolls/scroll_ice"), "Магия льда")},
                {"toxic", (content.Load<Texture2D>("textures/scrolls/scroll_toxic"), "Магия яда")},
                {"corrupted", (content.Load<Texture2D>("textures/scrolls/scroll_corrupted"), "Тёмная магия")},
                {"ancient", (content.Load<Texture2D>("textures/scrolls/scroll_ancient"), "Магия древних")}
            };
        
            foreach (var scrollId in ExistingScrollsRusNames)
            {
                AllScrolls.Add($"scroll_{scrollId.id}", new ItemInfo(
                    $"scroll_{scrollId.id}", 
                    ScrollsTypes[scrollId.id.Split('_')[0]].texture2D, 
                    ItemType.Scroll, 
                    $"Свиток {scrollId.rus}\nСила: {scrollId.id.Split('_')[2]}"));
                ScrollsRecipesTextures.Add($"scroll_{scrollId.id}", 
                    (false, content.Load<Texture2D>($"textures/scrolls/recipes/recipe_{scrollId.id}")));
            }
        }
    }
    
    public static class Runes
    {
        public static Dictionary<List<bool>, string> RunesCreateRecipes = new Dictionary<List<bool>, string>()
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
        
        public static Dictionary<string, ItemInfo> UnknownRunes { get; private set; }
        
        public static readonly Dictionary<int, (string id, string rus)> Elements = new()
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
        
        public static Dictionary<string, ItemInfo> FinishedRunes { get; private set; }
        
        public static void Initialize(ContentManager content)
        {
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
        }
    }
    
    public static class OtherItems
    {
        public static ItemInfo Clay;
        public static ItemInfo ClaySmall;
        public static ItemInfo Paper;
        
        public static void Initialize(ContentManager content)
        {
            Clay = new ItemInfo("clay", 
                content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay"), 
                ItemType.Clay,
                "Глина");
            ClaySmall = new ItemInfo("clay_small", 
                content.Load<Texture2D>($"textures/Inventory/items/piece_of_clay_small"), 
                ItemType.ClaySmall,
                "Кусочки глины");
            Paper = new ItemInfo("paper",
                content.Load<Texture2D>("textures/other_items/paper"),
                ItemType.Paper,
                "Волшебный пергамент");
        }
    }
    
    public static class Catalysts
    {
        public static Dictionary<string, ItemInfo> AllCatalysts { get; } = new();
    }
    
    public static void Initialize(ContentManager content)
    {
        Scrolls.Initialize(content);
        Runes.Initialize(content);
        OtherItems.Initialize(content);
    }
}