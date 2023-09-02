using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Objects;

public class MudPuddle : MapObject
{
    private readonly Texture2D _plusTexture;
    private Rectangle _useRect;

    private bool _isPlayingAnimation;
    private float _animAlpha = 1f;
    private Vector2 _animPos;
    private Timer _animTimer;
    private int _animClayCount;
    private float _animDeltaY;

    public MudPuddle(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo fromObjectInfo, Texture2D plusTexture, TopDownCore core, string name = "mud_puddle")
        : base(positionInPixelsLeftBottom, fromObjectInfo, name, core)
    {
        _useRect = new Rectangle((int)positionInPixelsLeftBottom.X,
            (int)positionInPixelsLeftBottom.Y - fromObjectInfo.Spritesheet.Height, fromObjectInfo.Spritesheet.Width, fromObjectInfo.Spritesheet.Height);
        _plusTexture = plusTexture;
    }

    public void Update()
    {
        if (_isPlayingAnimation)
            _animTimer.Tick();
        
        if (GameCore.Game.ClayClaimed) return;

        if (_useRect.Contains(GameCore.PlayerPosition) && Keyboard.GetState()[Keys.E] == KeyState.Down)
        {
            GameCore.Game.ClayClaimed = true;
            _animClayCount = Random.Shared.Next(5, 8);
            var random = Random.Shared.Next(0, 100);
            if (random < 10)
                _animClayCount += Random.Shared.Next(5, 8);
            else if (random < 30)
                _animClayCount += Random.Shared.Next(2, 4);
            
            _isPlayingAnimation = true;
            GameCore.Game.Inventory.AddItem(new Item(AllGameItems.Clay), _animClayCount);
            GameCore.Game.SubtractEnergy(1f);
            _animTimer = new Timer(50, () =>
            {
                if (_animAlpha > 0)
                {
                    _animAlpha -= 0.02f;
                    _animDeltaY += 0.5f;
                    _animTimer.StartAgain();
                    return;
                }

                _isPlayingAnimation = false;
                _animAlpha = 1f;
                _animDeltaY = 0;
            });
            _animTimer.StartAgain();
            if (GameCore.Game.Introduction.IsPlaying && GameCore.Game.Introduction.Step == 1)
            {
                GameCore.Game.Introduction.Step = 2;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var drawPos = new Vector2(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X, 
            PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - GameCore.CameraPosition.Y);
        _animPos = new Vector2(PositionInPixelsLeftBottom.X - GameCore.CameraPosition.X + SpriteSheetRectangle.Height/2, 
            PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height/2 - GameCore.CameraPosition.Y - _animDeltaY);
        if (!IsVisible) return;
        
        spriteBatch.Draw(SpriteSheet, drawPos, new Rectangle(
                (GameCore.Game.ClayClaimed ? 1 : 0) * GameMap.TileSize*2, 0, GameMap.TileSize*2, GameMap.TileSize*2), 
            Color.White);
        
        if (_isPlayingAnimation)
        {
            var numSize = CountDrawer.MeasureNumber(_animClayCount);
            
            var numberStr = _animClayCount.ToString();
            var numPos = new Vector2(_animPos.X + 5, _animPos.Y - _animDeltaY - numSize.Height);
            for (var i = 0; i < numberStr.Length; i++)
            {
                spriteBatch.Draw(CountDrawer.Textures[numberStr[i]], 
                    new Vector2(numPos.X + i*CountDrawer.TextureWidth, numPos.Y), Color.White*_animAlpha);
            }

            spriteBatch.Draw(_plusTexture, new Vector2(_animPos.X - _plusTexture.Width - 5, 
                _animPos.Y - _plusTexture.Height - (numSize.Height - _plusTexture.Height)/2 - _animDeltaY), Color.White*_animAlpha);
        }
    }
}