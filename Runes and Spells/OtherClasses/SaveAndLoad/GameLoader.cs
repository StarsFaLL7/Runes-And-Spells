using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses.SaveAndLoad.Records;
using Runes_and_Spells.TopDownGame;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.OtherClasses.SaveAndLoad
{
    public static class GameLoader
    {
        private static Dictionary<string, ItemInfo> _deserializeInfo;

        public enum NPCState
        {
            BeforeQuest,
            QuestInProgress,
            QuestFinishedGood,
            QuestFinishedBad,
        }
    
        public static void SaveGame(Game1 game, int toSlot)
        {
            if (!Directory.Exists("saves"))
            {
                Directory.CreateDirectory("saves");
            }
            using (StreamWriter file = File.CreateText($@"saves\save{toSlot}.sav"))
            {
                file.Write($"Inventory:[{string.Join(", ",game.Inventory.Items.Where(i => i.Count > 0).Select(i => $"{{{i.ID},{i.Count}}}"))}]\n");
                var mapStr = JsonConvert.SerializeObject(game.TopDownCore.Map.Tiles);
                file.Write(mapStr+"\n");
                var frontObjectsStr = JsonConvert.SerializeObject(game.TopDownCore.Map.FrontObjects.Where(obj => obj.GetType() != typeof(Chest)));
                file.Write(frontObjectsStr+"\n");
                var chestsStr = JsonConvert.SerializeObject(game.TopDownCore.Map.FrontObjects.Where(obj => obj.GetType() == typeof(Chest)));
                file.Write(chestsStr+"\n");
                file.Write(JsonConvert.SerializeObject(game.TopDownCore.Map.NPCList)+"\n");
                var knownRunesRecipes = JsonConvert.SerializeObject(AllGameItems.KnownRunesCraftRecipes);
                file.Write(knownRunesRecipes + "\n");
                var knownScrollsRecipes = JsonConvert.SerializeObject(AllGameItems.KnownScrollsCraftRecipes);
                file.Write(knownScrollsRecipes + "\n");
                
                var scrollsPrices = JsonConvert.SerializeObject(AllGameItems.SellingScrollsPrices);
                file.Write(scrollsPrices + "\n");
                file.Write(
                    $"[{string.Join(", ", game.MarketScreen.SellingSlots.Where(i => i.CurrentItem is not null && i.Count > 0).Select(i => $"{{\"Index\":{game.MarketScreen.SellingSlots.IndexOf(i)},\"ID\":\"{i.CurrentItem.ID}\",\"Price\":{i.Price},\"Count\":{i.Count}}}"))}]\n");
                var gameState = new GameStateLoad(game.Balance, game.DayCount, game.Energy, game.TopDownCore.IsFinalQuestPlaying,
                    game.TopDownCore.PlayerHasWings, game.TopDownCore.PlayerHasCompass, game.TopDownCore.FinalScroll, game.CurrentScreen, 
                    AllGameItems.KnownRunesCraftRecipes.Count, AllGameItems.KnownScrollsCraftRecipes.Count);
                file.Write(JsonConvert.SerializeObject(gameState));
            }
        }
    
        public static void LoadGame(Game1 game, int fromSlot)
        {
            var str = File.ReadAllLines($@"saves\save{fromSlot}.sav");
            _deserializeInfo = new Dictionary<string, ItemInfo>()
            {
                {"clay", ItemsDataHolder.OtherItems.Clay},
                {"clay_small", ItemsDataHolder.OtherItems.ClaySmall},
                {"paper", ItemsDataHolder.OtherItems.Paper},
                {"key_emerald", ItemsDataHolder.OtherItems.KeyEmerald},
                {"key_gold", ItemsDataHolder.OtherItems.KeyGold},
                {"key_silver", ItemsDataHolder.OtherItems.KeySilver}
            };
            game.Inventory.Clear();
            var invStr = str[0][(str[0].IndexOf('[') + 1)..str[0].IndexOf(']')];
            foreach (var inf in invStr.Split(", "))
            {
                if (inf == "") break;
                var parse = inf.Substring(1, inf.Length-2).Split(',');
                GiveItem(game, parse[0], int.Parse(parse[1]));
            }
        
            var mapTilesStr = str[1];
            game.TopDownCore.Map.Tiles = JsonConvert.DeserializeObject<Tile[,]>(mapTilesStr);
        
            var frontObjectsStr = str[2];
            game.TopDownCore.Map.FrontObjects = new List<MapObject>();
            var list = JsonConvert.DeserializeObject<List<MapObject>>(frontObjectsStr);
            foreach (var obj in list)
            {
                game.TopDownCore.Map.LoadFrontObjectFromString(obj.Name, obj.PositionInPixelsLeftBottom);
            }
        
            var chestsStr = str[3];
            var chestsInfo = JsonConvert.DeserializeObject<List<ChestLoad>>(chestsStr);
            foreach (var obj in chestsInfo)
            {
                game.TopDownCore.Map.LoadChest((Chest.ChestType)Enum.Parse(typeof(Chest.ChestType), obj.Name.Split('_')[1], true), obj.PositionInPixelsLeftBottom, obj.IsOpened);
            }
        
            var npcStr = str[4];
            var npcsInfo = JsonConvert.DeserializeObject<List<NPCLoad>>(npcStr);
            game.TopDownCore.Map.NPCList = new List<NPC>();
            foreach (var obj in npcsInfo)
            {
                game.TopDownCore.Map.LoadNpc(obj);
            }

            var runesRecipes = str[5];
            AllGameItems.KnownRunesCraftRecipes = JsonConvert.DeserializeObject<Dictionary<string, bool>>(runesRecipes);
            
            var scrollsRecipes = str[6];
            AllGameItems.KnownScrollsCraftRecipes = JsonConvert.DeserializeObject<List<ScrollsRecipes.ScrollInfo>>(scrollsRecipes);

            var scrollPrices = str[7];
            AllGameItems.SellingScrollsPrices = JsonConvert.DeserializeObject<Dictionary<ScrollType, int>>(scrollPrices);

            var sellSlotsStr = str[8];
            var sellSlots = JsonConvert.DeserializeObject<List<MarketSlotLoad>>(sellSlotsStr);
            foreach (var slotInfo in sellSlots)
                game.MarketScreen.LoadInfoToSlot(slotInfo.Index, GetItemInfoById(slotInfo.ID), slotInfo.Price, slotInfo.Count);
            
            var gameStateStr = str[9];
            var gameState = JsonConvert.DeserializeObject<GameStateLoad>(gameStateStr);
            game.LoadFromState(gameState);
        }

        private static ItemInfo GetItemInfoById(string id)
        {
            if (_deserializeInfo.ContainsKey(id))
                return _deserializeInfo[id];
            
            if (id.Contains("scroll"))
                return ItemsDataHolder.Scrolls.AllScrolls[id];
            
            if (id.Contains("rune"))
                return id.Contains("finished") ? ItemsDataHolder.Runes.FinishedRunes[id] : ItemsDataHolder.Runes.UnknownRunes[id];

            if (id.Contains("essence"))
                return ItemsDataHolder.PowerEssences.GetEssenceInfo(id);
            
            return null;
        }
        
        private static void GiveItem(Game1 game, string id, int count)
        {
            game.Inventory.AddItem(new Item(GetItemInfoById(id)), count);
        }
    }
}