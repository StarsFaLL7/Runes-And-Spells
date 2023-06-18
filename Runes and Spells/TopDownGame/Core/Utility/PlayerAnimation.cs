using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Runes_and_Spells.TopDownGame.Core.Enums;
using Runes_and_Spells.TopDownGame.Core.Records;

namespace Runes_and_Spells.TopDownGame.Core.Utility;

public static class PlayerAnimation
{
    public const int DefaultSpeed = 500;
    public enum AnimationType
    {
        StandUp,
        StandDown,
        StandRight,
        StandLeft,
        WalkRight,
        WalkUp,
        WalkDown,
        WalkLeft
    }

    public static Dictionary<Direction, AnimationInfo> _playerStandAnimations = new()
    {
        { Direction.Down, new AnimationInfo(new Vector2(18, 1), 6, DefaultSpeed)},
        { Direction.Up, new AnimationInfo(new Vector2(6, 1), 6, DefaultSpeed)},
        { Direction.Right, new AnimationInfo(new Vector2(0, 1), 6, DefaultSpeed)},
        { Direction.Left, new AnimationInfo(new Vector2(12, 1), 6, DefaultSpeed)}
    };
    
    public static Dictionary<Direction, AnimationInfo> _playerWalkAnimations = new()
    {
        { Direction.Down, new AnimationInfo(new Vector2(18, 2), 6, DefaultSpeed-400)},
        { Direction.Up, new AnimationInfo(new Vector2(6, 2), 6, DefaultSpeed-400)},
        { Direction.Right, new AnimationInfo(new Vector2(0, 2), 6, DefaultSpeed-400)},
        { Direction.Left, new AnimationInfo(new Vector2(12, 2), 6, DefaultSpeed-400)}
    };
}
