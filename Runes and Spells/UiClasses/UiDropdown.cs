using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runes_and_Spells.UiClasses;

public class UiDropdown
{
    public record DdVariant(string VisibleText, Action ActionOnChoose, params object[] otherParams);

    private Game1 _game;
    private SpriteFont _font;
    private Texture2D _borderTexture;
    private Texture2D _backTexture;
    private MouseState _lastMouseState;
    private Rectangle _defaultRectangle;
    private Color _fontColor;
    private float _borderWidth = 4;
    private int _hoveredVariant = -1;
    
    public List<DdVariant> Variants { get; init; }
    public bool isOpened { get; private set; }
    
    public DdVariant CurrentVariant { get; private set; }
    
    public UiDropdown(SpriteFont font, Color fontColor,Texture2D borderTexture, Texture2D backTexture, Game1 game, Rectangle defaultRectangle, params DdVariant[] variants)
    {
        _game = game;
        _font = font;
        _borderTexture = borderTexture;
        _backTexture = backTexture;
        Variants = variants.ToList();
        CurrentVariant = Variants[0];
        _defaultRectangle = defaultRectangle;
        _fontColor = fontColor;
    }

    public void AddVariant(DdVariant ddVariant)
    {
        Variants.Add(ddVariant);
    }

    public void SelectVariant(DdVariant variant)
    {
        if (!Variants.Contains(variant))
            throw new ArgumentException($"No such variant in DD: {variant}");
        CurrentVariant = variant;
        variant.ActionOnChoose();
    }

    public void Update(MouseState mouseState)
    {
        var rect = new Rectangle((int)(_defaultRectangle.X*Game1.ResolutionScale.X), (int)(_defaultRectangle.Y*Game1.ResolutionScale.Y),
            (int)(_defaultRectangle.Width*Game1.ResolutionScale.X), (int)(_defaultRectangle.Height*Game1.ResolutionScale.Y));
        if (isOpened)
        {
            var hoveredIndex = -1;
            for (var i = 0; i < Variants.Count; i++)
            {
                var varRect = new Rectangle(
                    (int)(rect.X + _borderWidth*Game1.ResolutionScale.X), 
                    (int)(rect.Y+rect.Height+2*_borderWidth*Game1.ResolutionScale.Y+i*(rect.Height+_borderWidth*Game1.ResolutionScale.Y)), 
                    rect.Width, 
                    rect.Height);
                
                if (varRect.Contains(mouseState.Position) && varRect.Contains(_lastMouseState.Position))
                {
                    hoveredIndex = i;
                    if (_lastMouseState.LeftButton == ButtonState.Released &&
                        mouseState.LeftButton == ButtonState.Pressed)
                    {
                        SelectVariant(Variants[i]);
                        isOpened = false;
                        break;
                    }
                }
            }
            _hoveredVariant = hoveredIndex;
        }

        if (!isOpened)
        {
            _hoveredVariant = -1;
        }

        var openRect = new Rectangle(rect.X, rect.Y, 
            (int)(rect.Width + _borderWidth * 2*Game1.ResolutionScale.X), 
            (int)(rect.Height + _borderWidth * 2*Game1.ResolutionScale.Y));
        if (openRect.Contains(mouseState.Position) && openRect.Contains(_lastMouseState.Position) &&
            _lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
        {
            isOpened = !isOpened;
        }

        _lastMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var rect = new Rectangle((int)(_defaultRectangle.X*Game1.ResolutionScale.X), (int)(_defaultRectangle.Y*Game1.ResolutionScale.Y),
            (int)(_defaultRectangle.Width*Game1.ResolutionScale.X), (int)(_defaultRectangle.Height*Game1.ResolutionScale.Y));

        if (isOpened)
        {
            spriteBatch.Draw(_borderTexture, 
                new Vector2(rect.X, rect.Y), 
                new Rectangle(0 ,0, 
                    (int)(rect.Width + _borderWidth * 2 * Game1.ResolutionScale.X), 
                    (int)(rect.Height + _borderWidth * 2 * Game1.ResolutionScale.X + Variants.Count * (rect.Height+_borderWidth*Game1.ResolutionScale.X))), Color.White);
            spriteBatch.Draw(_backTexture, 
                new Vector2(rect.X+_borderWidth * Game1.ResolutionScale.X, rect.Y + _borderWidth * Game1.ResolutionScale.Y), 
                new Rectangle(0 ,0, rect.Width, (int)(rect.Height +  Variants.Count * (rect.Height+_borderWidth*Game1.ResolutionScale.Y))), Color.White);
        }
        
        spriteBatch.Draw(_borderTexture, 
            new Vector2(rect.X, rect.Y), 
            new Rectangle(0 ,0, 
                (int)(rect.Width + _borderWidth * 2 * Game1.ResolutionScale.X), 
                (int)(rect.Height + _borderWidth * 2 * Game1.ResolutionScale.X)), Color.White);
        spriteBatch.Draw(_backTexture, 
            new Vector2(rect.X+_borderWidth * Game1.ResolutionScale.X, rect.Y + _borderWidth * Game1.ResolutionScale.Y), 
            new Rectangle(0 ,0, rect.Width, rect.Height), Color.White);
        var text = CurrentVariant.VisibleText ?? "No variants";
        var textSize = _font.MeasureString(text);
        spriteBatch.DrawString(_font, text, 
            new Vector2(rect.X+(rect.Width - textSize.X*Game1.ResolutionScale.X)/2, 
                rect.Y+(rect.Height - textSize.Y*0.5f*Game1.ResolutionScale.Y)/2), 
            _fontColor, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
        
        if (isOpened)
        {
            var index = 0;
            foreach (var variant in Variants)
            {
                var varRect = new Rectangle(rect.X + (int)(_borderWidth*Game1.ResolutionScale.X), 
                    (int)(rect.Y+rect.Height+2*_borderWidth*Game1.ResolutionScale.Y+index*(rect.Height+_borderWidth*Game1.ResolutionScale.Y)), 
                    rect.Width, 
                    rect.Height);
                
                var color = _fontColor;
                if (index == _hoveredVariant)
                    color = _fontColor * 2f;

                var varText = variant.VisibleText;
                var varTextSize = _font.MeasureString(varText);
                spriteBatch.DrawString(_font, variant.VisibleText, 
                    new Vector2(varRect.X + (varRect.Width - varTextSize.X*Game1.ResolutionScale.X)/2, 
                        varRect.Y + (varRect.Height - varTextSize.Y*0.7f*Game1.ResolutionScale.Y)/2), color,
                    0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
                spriteBatch.Draw(_borderTexture, new Vector2(varRect.X, varRect.Y + varRect.Height), 
                    new Rectangle(0, 0, varRect.Width, (int)(_borderWidth*Game1.ResolutionScale.Y)), Color.White*0.5f);
                index++;
            }
        }
    }
}