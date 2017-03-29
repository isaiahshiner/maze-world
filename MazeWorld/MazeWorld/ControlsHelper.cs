using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /* Defines helper methods for making controls simplier to work with. 
     * Stores the previous frame states of several control related items.
     * 
     * TODO: design a system to make controls based on events and delegates.
     */
    public class ControlsHelper
    {
        public DrawHelper draw { get; set; }
        public KeyboardState KPrevious { get; set; }
        public MouseState MPrevious { get; set; }
        public Location LPrevious { get; set; }

        public ControlsHelper(DrawHelper d)
        {
            draw = d;
        }

        public void Update()
        {
            KPrevious = Keyboard.GetState();
            MPrevious = Mouse.GetState();
            if (MouseIsOnGrid())
                LPrevious = LocationOfMouse();
        }

        /* Four possible states for the position of a key/mouse:
         * UP     : NOT pressed last frame OR this frame
         * DOWN   : pressed last frame AND this frame
         * FALLING: just pressed, not yet being held down
         * RISING : just released, no longer being held down
         */
        public bool KeyUp(Keys k)       { return KPrevious.IsKeyUp(k)   && Keyboard.GetState().IsKeyUp(k); }
        public bool KeyDown(Keys k)     { return KPrevious.IsKeyDown(k) && Keyboard.GetState().IsKeyDown(k); }
        public bool KeyFalling(Keys k)  { return KPrevious.IsKeyUp(k)   && Keyboard.GetState().IsKeyDown(k); }       
        public bool KeyRising(Keys k)   { return KPrevious.IsKeyDown(k) && Keyboard.GetState().IsKeyUp(k); }

        public bool MouseUp()           { return MPrevious.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Released; }
        public bool MouseDown()         { return MPrevious.LeftButton == ButtonState.Pressed  && Mouse.GetState().LeftButton == ButtonState.Pressed; }
        public bool MouseFalling()      { return MPrevious.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed; }
        public bool MouseRising()       { return MPrevious.LeftButton == ButtonState.Pressed  && Mouse.GetState().LeftButton == ButtonState.Released; }

        public bool MouseIsOnGrid()
        {
            return draw.PlayArea.Contains(Mouse.GetState().Position);
        }

        public Location LocationOfMouse()
        {
            int x = Mouse.GetState().Position.X - draw.PlayArea.Left;
            int y = Mouse.GetState().Position.Y - draw.PlayArea.Top;
            return new Location((int)Math.Floor((double)(x) / draw.TexSize),
                                (int)Math.Floor((double)(y) / draw.TexSize));
        }

        public bool IsLocationChanging()
        {
            return !(LPrevious.Equals(LocationOfMouse()));
        }
    }
}
