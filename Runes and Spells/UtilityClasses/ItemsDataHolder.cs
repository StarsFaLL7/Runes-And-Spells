using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.Content.data;

namespace Runes_and_Spells.UtilityClasses;

public static class ItemsDataHolder
{

    public static class Scrolls
    {
        public static Dictionary<ScrollType, (Texture2D texture2D, string rus)> ScrollsTypesInfo;

        public static Dictionary<string, Texture2D> RunesWithNumbersTextures = new ();

        public static Dictionary<ScrollType, Texture2D> ScrollTexturesX64 = new ();

        public static readonly Dictionary<string, ItemInfo> AllScrolls = new ();

        public static void Initialize(ContentManager content)
        {
            ScrollsTypesInfo = new ()
            {
                {ScrollType.Nature, (content.Load<Texture2D>("textures/scrolls/scroll_nature"), "Магия природы")},
                {ScrollType.Wind, (content.Load<Texture2D>("textures/scrolls/scroll_wind"), "Магия ветра")},
                {ScrollType.Ocean, (content.Load<Texture2D>("textures/scrolls/scroll_ocean"), "Магия океана")},
                {ScrollType.Sun, (content.Load<Texture2D>("textures/scrolls/scroll_sun"), "Магия солнца")},
                {ScrollType.Faerie, (content.Load<Texture2D>("textures/scrolls/scroll_faerie"), "Магия фей")},
                {ScrollType.Life, (content.Load<Texture2D>("textures/scrolls/scroll_life"), "Магия жизни")},
                {ScrollType.Ice, (content.Load<Texture2D>("textures/scrolls/scroll_ice"), "Магия льда")},
                {ScrollType.Toxic, (content.Load<Texture2D>("textures/scrolls/scroll_toxic"), "Магия яда")},
                {ScrollType.Corrupted, (content.Load<Texture2D>("textures/scrolls/scroll_corrupted"), "Темная магия")}
            };
            foreach (var element in Runes.Elements.Values)
            {
                RunesWithNumbersTextures.Add($"{element.id}_1", content.Load<Texture2D>($"textures/runes/x64WithNumbers/{element.id}_1"));
                RunesWithNumbersTextures.Add($"{element.id}_2", content.Load<Texture2D>($"textures/runes/x64WithNumbers/{element.id}_2"));
            }

            foreach (var type in ScrollsTypesInfo.Keys)
            {
                ScrollTexturesX64.Add(type, content.Load<Texture2D>($"textures/scrolls/x64/scroll_{type.ToString().ToLower()}"));
            }
        }
    }
    
    public static class Runes
    {
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

        public static Dictionary<List<bool>, string> RunesCreateRecipes { get; } = new();
        public static Texture2D RecipeCellFull;
        public static Texture2D RecipeCellEmpty;
        public static Texture2D RecipesBack;
        public static Texture2D RecipeFire;
        public static Texture2D RecipeArrow;
        public static Dictionary<string, Texture2D> RecipeX64RunesTextures { get; } = new();
        public static Dictionary<string, Texture2D> RecipeX64UnknownRunesTextures { get; } = new();



        public static void Initialize(ContentManager content)
        {
            FillRunesRecipes();
            RecipeCellFull = content.Load<Texture2D>("textures/runes/recipe/cell_filled");
            RecipeCellEmpty = content.Load<Texture2D>("textures/runes/recipe/cell_empty");
            RecipesBack = content.Load<Texture2D>("textures/runes/recipe/cells_background");
            RecipeFire = content.Load<Texture2D>("textures/runes/recipe/fire_small");
            RecipeArrow = content.Load<Texture2D>("textures/runes/recipe/arrow_small");
            
            UnknownRunes = new Dictionary<string, ItemInfo>();
            
            UnknownRunes.Add("rune_unknown_failed", 
                new ItemInfo("rune_unknown_failed", 
                    content.Load<Texture2D>("textures/Inventory/items/empty_rune_failed"), 
                    ItemType.UnknownRune,
                    "Неизвестная руна"));
            
            FinishedRunes = new Dictionary<string, ItemInfo>();
            for (var i = 1; i < 10; i++)
            for (var j = 1; j < 7; j++)
            {
                FinishedRunes.Add($"rune_finished_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}",
                    new ItemInfo($"rune_finished_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}",
                        content.Load<Texture2D>($"textures/runes/spr_rune_x_{j + 6 * (i - 1)}"),
                        ItemType.Rune,
                        $"Стихия: {Elements[i].rus}\n" +
                        $"Сила: {(j - 1) / 2 + 1}\n" +
                        $"Вид: {(j - 1) % 2 + 1}"));
                UnknownRunes.Add($"rune_unknown_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}", 
                    new ItemInfo($"rune_unknown_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}",
                        content.Load<Texture2D>($"textures/Inventory/items/empty_rune_{Elements[i].id}"), 
                        ItemType.UnknownRune,
                        "Неизвестная руна"));
                
                RecipeX64RunesTextures[$"rune_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}"] = 
                    content.Load<Texture2D>($"textures/runes/x64/rune_{Elements[i].id}_{(j - 1) / 2 + 1}_{(j - 1) % 2 + 1}");
                RecipeX64UnknownRunesTextures[Elements[i].id] = 
                    content.Load<Texture2D>($"textures/runes/x64/empty_{Elements[i].id}");
            }
        }

        private static void FillRunesRecipes()
        {
            var lines = RunesRecipes.Recipes.Split("\r\n");
            var recipe = new List<bool>();
            foreach (var line in lines)
            {
                if (line == "-----")
                {
                    recipe = new List<bool>(9);
                    continue;
                }
                if (line[0] == 'r')
                {
                    RunesCreateRecipes[recipe] = line;
                    continue;
                }
                recipe.AddRange(line.Select(symbol => symbol == '*'));
            }
        }
    }
    
    public static class OtherItems
    {
        public static ItemInfo Clay;
        public static ItemInfo ClaySmall;
        public static ItemInfo Paper;
        public static ItemInfo KeySilver;
        public static ItemInfo KeyGold;
        public static ItemInfo KeyEmerald;
        public static Texture2D RuneCraftRecipePaperTexture;
        public static Texture2D ScrollCraftRecipePaperTexture;
        public static Texture2D PaperTextureX64;
        
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
            KeySilver = new ItemInfo("key_silver",
                content.Load<Texture2D>("textures/other_items/key_silver"),
                ItemType.Key,
                "Старый серебряный ключ");
            KeyGold = new ItemInfo("key_gold",
                content.Load<Texture2D>("textures/other_items/key_gold"),
                ItemType.Key,
                "Старый золотой ключ");
            KeyEmerald = new ItemInfo("key_emerald",
                content.Load<Texture2D>("textures/other_items/key_emerald"),
                ItemType.Key,
                "Древний изумрудный ключ");
            RuneCraftRecipePaperTexture = content.Load<Texture2D>("textures/other_items/rune_craft_recipe_x64");
            ScrollCraftRecipePaperTexture = content.Load<Texture2D>("textures/other_items/scroll_craft_recipe_x64");
            PaperTextureX64 = content.Load<Texture2D>("textures/other_items/paper_x64");
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