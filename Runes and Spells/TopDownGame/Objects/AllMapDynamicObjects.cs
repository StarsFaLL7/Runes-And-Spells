using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Objects;

public static class AllMapDynamicObjects
{
    public record DynamicObjectInfo(string Id, Texture2D Spritesheet, Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle);

    public static Dictionary<string, DynamicObjectInfo> AllObjects { get; private set; }

    public static Texture2D PlusTextureForChests { get; private set; }
    
    public static SpriteFont DialogSpriteFont { get; private set; }
    public static Texture2D DialogBgTexture;
    public static Texture2D DialogBorderTexture;
    public static Texture2D DialogTipTexture;
    public static Texture2D HintBgTexture;
    public static void Initialize(ContentManager content)
    {
        DialogSpriteFont = content.Load<SpriteFont>("SmallPixelFont20px");
        DialogBgTexture = content.Load<Texture2D>("top-down/dialog_bg");
        DialogBorderTexture = content.Load<Texture2D>("top-down/dialog_border");
        DialogTipTexture = content.Load<Texture2D>("top-down/dialog_tip");
        HintBgTexture = content.Load<Texture2D>("top-down/hint_bg");
        
        PlusTextureForChests = content.Load<Texture2D>("textures/scroll_crafting_table/recipes/plus");
        AllObjects = new Dictionary<string, DynamicObjectInfo>()
        {
            {"chest_silver", new DynamicObjectInfo("chest_silver", content.Load<Texture2D>("top-down/objects/chest_silver"), 
                new Rectangle(0,0,GameMap.TileSize,GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*4/16,0,GameMap.TileSize*24/16,GameMap.TileSize*12/16))},
            {"chest_gold", new DynamicObjectInfo("chest_golden", content.Load<Texture2D>("top-down/objects/chest_golden"), 
                new Rectangle(0,0,GameMap.TileSize,GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*4/16,0,GameMap.TileSize*24/16,GameMap.TileSize*12/16))},
            {"chest_emerald", new DynamicObjectInfo("chest_emerald", content.Load<Texture2D>("top-down/objects/chest_emerald"), 
                new Rectangle(0,0,GameMap.TileSize,GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*4/16,0,GameMap.TileSize*24/16,GameMap.TileSize*12/16))},
            {"mud_puddle", new DynamicObjectInfo("mud_puddle", content.Load<Texture2D>("top-down/objects/mud_puddle"),
                new Rectangle(0,0,GameMap.TileSize*2,GameMap.TileSize*2), Rectangle.Empty)},
            {"npc_woodman", new DynamicObjectInfo("npc_woodman", content.Load<Texture2D>("top-down/characters/woodman_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_fisherman", new DynamicObjectInfo("npc_fisherman", content.Load<Texture2D>("top-down/characters/fisherman_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_hunter", new DynamicObjectInfo("npc_hunter", content.Load<Texture2D>("top-down/characters/hunter_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_bard", new DynamicObjectInfo("npc_bard", content.Load<Texture2D>("top-down/characters/bard_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_witch", new DynamicObjectInfo("npc_witch", content.Load<Texture2D>("top-down/characters/witch_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_trader", new DynamicObjectInfo("npc_trader", content.Load<Texture2D>("top-down/characters/trader_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_thief", new DynamicObjectInfo("npc_thief", content.Load<Texture2D>("top-down/characters/thief_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
            {"npc_mage", new DynamicObjectInfo("npc_nage", content.Load<Texture2D>("top-down/characters/mage_tilesheet"),
                new Rectangle(GameMap.TileSize*18,GameMap.TileSize*2,GameMap.TileSize,GameMap.TileSize*2),
                new Rectangle(-GameMap.TileSize/2,-GameMap.TileSize/2,GameMap.TileSize*2,GameMap.TileSize))},
        };
    }
}