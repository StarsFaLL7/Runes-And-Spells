using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.TopDownGame.Objects;

namespace Runes_and_Spells.TopDownGame.Core;

public class MapObject
{
    public Vector2 PositionInPixelsLeftBottom;
    public bool IsVisible { get; set; }
    public string Name { get; init; }
    public Rectangle SpriteSheetRectangle { get; init; }
    public Rectangle CollisionRectangle { get; init; }
    
    public Texture2D SpriteSheet { get; init; }

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
}