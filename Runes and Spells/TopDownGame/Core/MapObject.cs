using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Runes_and_Spells.TopDownGame.Core.Utility;
using Runes_and_Spells.TopDownGame.Objects;

namespace Runes_and_Spells.TopDownGame.Core;

public class MapObject
{
    public Vector2 PositionInPixelsLeftBottom;
    public string Name { get; init; }
    
    [JsonIgnore]
    public bool IsVisible { get; set; }
    [JsonIgnore]
    public Rectangle SpriteSheetRectangle { get; init; }
    [JsonIgnore]
    public Rectangle CollisionRectangle { get; init; }
    [JsonIgnore]
    public Texture2D SpriteSheet { get; init; }
    [JsonIgnore]
    public readonly TopDownCore GameCore;

    public MapObject(Vector2 positionInPixelsLeftBottom, Texture2D spriteSheet, Rectangle spriteSheetRectangle, Rectangle collisionRectangle, string name, TopDownCore core)
    {
        PositionInPixelsLeftBottom = positionInPixelsLeftBottom;
        SpriteSheetRectangle = spriteSheetRectangle;
        CollisionRectangle = collisionRectangle;
        Name = name;
        GameCore = core;
        SpriteSheet = spriteSheet;
    }

    public MapObject(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo fromObjectInfo, string name, TopDownCore core)
    {
        PositionInPixelsLeftBottom = positionInPixelsLeftBottom;
        SpriteSheetRectangle = fromObjectInfo.SpriteSheetRectangle;
        CollisionRectangle = fromObjectInfo.CollisionRectangle;
        Name = name;
        GameCore = core;
        SpriteSheet = fromObjectInfo.Spritesheet;
    }
    [JsonConstructor]
    public MapObject(Vector2 positionInPixelsLeftBottom, string name)
    {
        //var fromObjectInfo = AllMapStaticObjectsInfo.AllGeneratedSolidObjects.First(p => p.Name == name);
        PositionInPixelsLeftBottom = positionInPixelsLeftBottom;
        //SpriteSheetRectangle = fromObjectInfo.SpriteSheetRectangle;
        //CollisionRectangle = fromObjectInfo.CollisionRectangle;
        Name = name;
        //GameCore = core;
        //SpriteSheet = fromObjectInfo.Spritesheet;
    }
}