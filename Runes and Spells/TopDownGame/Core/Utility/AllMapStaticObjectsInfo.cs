using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Runes_and_Spells.TopDownGame.Core.Utility;

public static class AllMapStaticObjectsInfo
{
   

    public static List<MapObjectInfo> AllFloorObjects = new List<MapObjectInfo>()
    {
        new MapObjectInfo(new Rectangle(2*GameMap.TileSize,2*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_white_1"),
        new MapObjectInfo(new Rectangle(2*GameMap.TileSize,1*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_white_2"),
        new MapObjectInfo(new Rectangle(3*GameMap.TileSize,1*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_white_3"),
        new MapObjectInfo(new Rectangle(4*GameMap.TileSize,1*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_white_4"),
        new MapObjectInfo(new Rectangle(4*GameMap.TileSize,2*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_white_5"),
        
        new MapObjectInfo(new Rectangle(0*GameMap.TileSize,8*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "small_stone_1"),
        new MapObjectInfo(new Rectangle(1*GameMap.TileSize,8*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "small_stone_2"),
        new MapObjectInfo(new Rectangle(10*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "small_stone_3"),
        new MapObjectInfo(new Rectangle(10*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "small_stone_4"),
        new MapObjectInfo(new Rectangle(11*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "small_stone_5"),
        new MapObjectInfo(new Rectangle(11*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "big_flower_blue"),
        new MapObjectInfo(new Rectangle(12*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "big_flower_yellow"),
        
        new MapObjectInfo(new Rectangle(6*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_1"),
        new MapObjectInfo(new Rectangle(6*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_2"),
        new MapObjectInfo(new Rectangle(7*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_3"),
        new MapObjectInfo(new Rectangle(7*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_4"),
        new MapObjectInfo(new Rectangle(8*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_5"),
        new MapObjectInfo(new Rectangle(8*GameMap.TileSize,11*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 5, "flowers_blue_6"),

        new MapObjectInfo(new Rectangle(2*GameMap.TileSize,8*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_1"),
        new MapObjectInfo(new Rectangle(0*GameMap.TileSize,9*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_2"),
        new MapObjectInfo(new Rectangle(1*GameMap.TileSize,9*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_3"),
        new MapObjectInfo(new Rectangle(2*GameMap.TileSize,9*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_4"),
        new MapObjectInfo(new Rectangle(3*GameMap.TileSize,9*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_5"),
        new MapObjectInfo(new Rectangle(0*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_6"),
        new MapObjectInfo(new Rectangle(1*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_7"),
        new MapObjectInfo(new Rectangle(2*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 2, "mushrooms_8"),
        new MapObjectInfo(new Rectangle(3*GameMap.TileSize,10*GameMap.TileSize, GameMap.TileSize, GameMap.TileSize), true,
            new Vector2(1, 1), 1, "mushrooms_9"),
        
    };
    
    public static List<(Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle, bool GroupGeneration, int GenerationWeight, string Name)> AllGeneratedSolidObjects = new ()
    {
        (new Rectangle(9*GameMap.TileSize,0*GameMap.TileSize, 2*GameMap.TileSize, 3*GameMap.TileSize), 
            new Rectangle(GameMap.TileSize/2, 0, GameMap.TileSize, GameMap.TileSize/2),
            true, 15, "tree_1"),
        (new Rectangle(11*GameMap.TileSize,0*GameMap.TileSize, 2*GameMap.TileSize, 3*GameMap.TileSize), 
            new Rectangle(GameMap.TileSize/2, 0, GameMap.TileSize, GameMap.TileSize/2),
            true, 5, "tree_2"),
        (new Rectangle(0*GameMap.TileSize,4*GameMap.TileSize, 2*GameMap.TileSize, 2*GameMap.TileSize), 
            new Rectangle(GameMap.TileSize/8, -GameMap.TileSize/4, GameMap.TileSize*27/16, GameMap.TileSize),
            true, 1, "big_stone_1"),
        (new Rectangle(0*GameMap.TileSize,6*GameMap.TileSize, 2*GameMap.TileSize, 2*GameMap.TileSize), 
            new Rectangle(0, -GameMap.TileSize/4, GameMap.TileSize*2, GameMap.TileSize*7/8),
            true, 1, "big_stone_2"),
        (new Rectangle(1*GameMap.TileSize,3*GameMap.TileSize, 1*GameMap.TileSize, 1*GameMap.TileSize), 
            new Rectangle(-GameMap.TileSize*4/16, 0, GameMap.TileSize*24/16, GameMap.TileSize*9/16),
            true, 1, "medium_stone_1"),
        (new Rectangle(3*GameMap.TileSize,2*GameMap.TileSize, 1*GameMap.TileSize, 2*GameMap.TileSize), 
            new Rectangle(-GameMap.TileSize*4/16, 0, GameMap.TileSize*24/16, GameMap.TileSize*10/16),
            true, 1, "medium_stone_2"),
        (new Rectangle(14*GameMap.TileSize,6*GameMap.TileSize, 5*GameMap.TileSize, 3*GameMap.TileSize), 
            new Rectangle(GameMap.TileSize/2, 0, GameMap.TileSize*4, GameMap.TileSize),
            true, 2, "small_tree_forest"),
        (new Rectangle(7*GameMap.TileSize,0*GameMap.TileSize, 1*GameMap.TileSize, 1*GameMap.TileSize), 
            new Rectangle(-GameMap.TileSize*4/16, -GameMap.TileSize*4/16, GameMap.TileSize*24/16, GameMap.TileSize*14/16),
            true, 1, "bush"),
    };
    
    public static Dictionary<string, (Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle, string Name)> AllOtherSolidObjects = new ()
    {
        { "fence_horizontal_1", (new Rectangle(5 * GameMap.TileSize, 3 * GameMap.TileSize, 1 * GameMap.TileSize, 1 * GameMap.TileSize),
                new Rectangle(0, 0, GameMap.TileSize, GameMap.TileSize*4/16),
                "fence_horizontal_1") },
        { "fence_horizontal_end_left_1", (new Rectangle(6 * GameMap.TileSize, 2 * GameMap.TileSize, 1 * GameMap.TileSize, 1 * GameMap.TileSize),
            new Rectangle(GameMap.TileSize*6/16, 0, GameMap.TileSize*10/16, GameMap.TileSize*4/16),
            "fence_horizontal_end_left_1") },
        { "flower_bed_yellow",
            (new Rectangle(5 * GameMap.TileSize, 6 * GameMap.TileSize, 1 * GameMap.TileSize, 1 * GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*5/16, -GameMap.TileSize*2/16, GameMap.TileSize*26/16, GameMap.TileSize*17/16),
                "flower_bed_yellow") },
        { "flower_bed_white",
            (new Rectangle(6 * GameMap.TileSize, 6 * GameMap.TileSize, 1 * GameMap.TileSize, 1 * GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*5/16, -GameMap.TileSize*2/16, GameMap.TileSize*26/16, GameMap.TileSize*17/16),
                "flower_bed_white") },
        { "flower_bed_blue",
            (new Rectangle(4 * GameMap.TileSize, 6 * GameMap.TileSize, 1 * GameMap.TileSize, 1 * GameMap.TileSize),
                new Rectangle(-GameMap.TileSize*5/16, -GameMap.TileSize*2/16, GameMap.TileSize*26/16, GameMap.TileSize*17/16),
                "flower_bed_blue") },
    };
}