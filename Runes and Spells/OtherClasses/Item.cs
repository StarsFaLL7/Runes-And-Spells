using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.OtherClasses;

public class Item
{
    public readonly ItemType Type;
    public readonly Texture2D Texture;
    public readonly string ID;
    public int Count { get; private set; }
    public bool IsBeingDragged;
    public Vector2 Position { get; private set;  }
    private bool _canBeDragged;

    public bool ShowToolTip { get; private set; }
    private bool _startHovered;
    private readonly Timer _hoverTimer;
    
    public string ToolTipText { get; init; }

    /*public Item(ItemType type, Texture2D texture, string id, string toolTipText)
    {
        Type = type;
        Texture = texture;
        ID = id;
        Count = 1;
        _canBeDragged = true;
        _hoverTimer = new Timer(AllGameItems.HoverTime, () => { ShowToolTip = true; });
        ToolTipText = toolTipText;
    }*/
    
    public Item(ItemType type, Texture2D texture, string id, string toolTipText, int count = 1)
    {
        Type = type;
        Texture = texture;
        ID = id;
        _canBeDragged = true;
        Count = count;
        _hoverTimer = new Timer(AllGameItems.HoverTime, () => { ShowToolTip = true; });
        ToolTipText = toolTipText;
    }

    public Item(ItemInfo fromInfo, int count = 1) : this(fromInfo.Type, fromInfo.Texture, fromInfo.ID, fromInfo.ToolTip, count)
    {
        
    }

    public void AddCount(int count = 1) => Count += count;
    public void SubtractCount(int count = 1) => Count = Count - count > 0 ? Count - count : 0;
    

    public void Update(Vector2 defaultPosition, List<UiSlot> dropableSlots)
    {
        var mouseState = Mouse.GetState();
        
        UpdateHover(mouseState);

        if (_canBeDragged)
        {
            if (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height).Contains(mouseState.X, mouseState.Y) && 
                mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!IsBeingDragged) Count--;
                IsBeingDragged = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                if (IsBeingDragged)
                {
                    if (!dropableSlots.Select(slot => slot.TryToAddItem(this)).Any(b => b))
                        Count++;
                }
                IsBeingDragged = false;
            }
        }
        Position = IsBeingDragged ? new Vector2(mouseState.X - Texture.Width / 2, mouseState.Y - Texture.Height / 2) : defaultPosition;
    }

    private void UpdateHover(MouseState mouseState)
    {
        if (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height).Contains(mouseState.X, mouseState.Y))
        {
            if (!_startHovered)
            {
                _hoverTimer.StartAgain();
                _startHovered = true;
            }
            else _hoverTimer.Tick();
        }
        else
        {
            _hoverTimer.Stop();
            ShowToolTip = false;
            _startHovered = false;
        }
    }
    

    public void Draw(SpriteBatch spriteBatch, Vector2 defaultPosition)
    {
        if (Count > 0)
            spriteBatch.Draw(Texture, defaultPosition, Color.White);
    }

    public void DrawAtMousePos(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.White);
    }

    public void Lock() => _canBeDragged = false;
    public void Unlock() => _canBeDragged = true;

    protected bool Equals(Item other) => ID == other.ID;

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
        return ID != null ? ID.GetHashCode() : 0;
    }

    public static bool operator ==(Item left, Item right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Item left, Item right)
    {
        return !Equals(left, right);
    }
}