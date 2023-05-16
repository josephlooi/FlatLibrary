using System;
using System.IO;
using FlatEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FlatEngine.Input
{
    public sealed class FlatMouse
    {
        private static readonly Lazy<FlatMouse> Lazy = new Lazy<FlatMouse>(() => new FlatMouse());
        public static FlatMouse Instance { get { return Lazy.Value; } }
        private MouseState prevMouseState;
        private MouseState currMouseState;
        private int prevScrollValue;
        private int currScrollValue;
        private Point position { get { return this.currMouseState.Position; } }

        public FlatMouse()
        {
            this.prevMouseState = Mouse.GetState();
            this.currMouseState = this.prevMouseState;
            this.prevScrollValue = this.ScrollWheelValue();
            this.currScrollValue = this.prevScrollValue;
        }
        public void Update()
        {
            this.prevMouseState = this.currMouseState;
            this.currMouseState = Mouse.GetState();
            this.prevScrollValue = this.currScrollValue;
            this.currScrollValue = this.ScrollWheelValue();
        }



        public bool IsLeftDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }
        public bool IsMiddleDown()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed;
        }
        public bool IsThumb1Down()
        {
            return this.currMouseState.XButton1 == ButtonState.Pressed;
        }
        public bool IsThumb2Down()
        {
            return this.currMouseState.XButton2 == ButtonState.Pressed;
        }
        public bool IsScrollingUp()
        {
            return this.currScrollValue - this.prevScrollValue > 0;
        }
        public bool IsScrollingDown()
        {
            return this.currScrollValue - this.prevScrollValue < 0;
        }
        public int ScrollWheelValue()
        {
            return this.currMouseState.ScrollWheelValue;
        }



        public bool IsLeftPressed()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }
        public bool IsRightPressed()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }
        public bool IsMiddlePressed()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed && this.prevMouseState.MiddleButton == ButtonState.Released;
        }
        public bool IsThumb1Pressed()
        {
            return this.currMouseState.XButton1 == ButtonState.Pressed && this.prevMouseState.XButton1 == ButtonState.Released;
        }
        public bool IsThumb2Pressed()
        {
            return this.currMouseState.XButton2 == ButtonState.Pressed && this.prevMouseState.XButton2 == ButtonState.Released;
        }



        public bool IsLeftReleased()
        {
            return this.currMouseState.LeftButton == ButtonState.Released && this.prevMouseState.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightReleased()
        {
            return this.currMouseState.RightButton == ButtonState.Released && this.prevMouseState.RightButton == ButtonState.Pressed;
        }
        public bool IsMiddleReleased()
        {
            return this.currMouseState.MiddleButton == ButtonState.Released && this.prevMouseState.MiddleButton == ButtonState.Pressed;
        }
        public bool IsThumb1Released()
        {
            return this.currMouseState.XButton1 == ButtonState.Released && this.prevMouseState.XButton1 == ButtonState.Pressed;
        }
        public bool IsThumb2Released()
        {
            return this.currMouseState.XButton2 == ButtonState.Released && this.prevMouseState.XButton2 == ButtonState.Pressed;
        }



        public FlatVector GetScreenPosition(Screen screen)
        {
            // Get the size and position of the screen when stretched to fit into the game window (keeping the correct aspect ratio).
            Rectangle screenRect = screen.GetDestinationRect();
            // Get the position of the mouse relative to the screen destination rectangle position.
            // Convert the position to a normalized ratio inside the screen destination rectangle.
            // Multiply the normalized coordinates by the actual size of the screen to get the location in screen coordinates.
            double x = (double)
                (this.position.X - screenRect.X) /
                screenRect.Width *
                screen.Width;
            double y = (double)
                (this.position.Y - screenRect.Y) /
                screenRect.Height *
                screen.Height;
            return new FlatVector(x, y);
        }

        public FlatVector GetWorldPosition(Screen screen, Camera camera)
        {
            // Create a viewport based on the game screen.
            Viewport viewport = new Viewport(0, 0, screen.Width, screen.Height);
            // Get the mouse pixel coordinates in that screen.
            Vector2 mouseScreenPosition = this.GetScreenPosition(screen).ToXna();
            // Create a ray that starts at the mouse screen position and points "into" the screen towards the game world plane.
            Ray mouseRay = this.CreateMouseRay(mouseScreenPosition, viewport, camera);
            // Plane where the flat 2D game world takes place.
            Plane worldPlane = new Plane(new Vector3(0f, 0f, 1f), 0f);
            // Determine the point where the ray intersects the game world plane.
            float? dist = mouseRay.Intersects(worldPlane);
            Vector3 ip = mouseRay.Position + mouseRay.Direction * dist.Value;
            // Send the result as a 2D world position vector.
            return new FlatVector(ip.X, ip.Y);
        }

        private Ray CreateMouseRay(Vector2 mouseScreenPosition, Viewport viewport, Camera camera)
        {
            // Near and far points that will indicate the line segment used to define the ray.
            Vector3 nearPoint = new Vector3(mouseScreenPosition, 0);
            Vector3 farPoint = new Vector3(mouseScreenPosition, 1);
            // Convert the near and far points to world coordinates.
            nearPoint = viewport.Unproject(nearPoint, camera.Projection, camera.View, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, camera.Projection, camera.View, Matrix.Identity);
            // Determine the direction.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            // Resulting ray starts at the near mouse position and points "into" the screen.
            return new Ray(nearPoint, direction);
        }
    }
}
