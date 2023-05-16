using System;
using FlatEngine;
using FlatEngine.Graphics;
using FlatEngine.Input;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatTester
{
    public abstract class FlatGame : Game
    {
        protected readonly GraphicsDeviceManager graphics;
        private readonly Color backgroundColor = Color.Black;

        protected readonly int ResolutionX;
        protected readonly int ResolutionY;
        protected readonly double baseZoom; // 1 pixel = ? metres

        protected double time;
        protected FlatKeyboard keyboard;
        protected FlatMouse mouse;
        protected FlatVector mouseWorldPosition;

        protected Sprites sprites;
        protected Screen screen;
        protected Shapes shapes;
        protected Camera camera;
        

        public FlatGame(
            int resolutionX, int resolutionY, double baseZoom, 
            int targetFPS = 0, bool vSync = false, bool isMouseVisible = true) :
            base()
        {
            this.Content.RootDirectory = "Content";

            this.ResolutionX = resolutionX;
            this.ResolutionY = resolutionY;
            this.baseZoom = baseZoom;
            this.IsMouseVisible = isMouseVisible;

            this.graphics = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = vSync
            };

            if (targetFPS > 0)
            {
                this.TargetElapsedTime = TimeSpan.FromTicks((long)Math.Round(
                    TimeSpan.TicksPerSecond / (double)targetFPS));
                this.IsFixedTimeStep = true;
            }
            else this.IsFixedTimeStep = false;
        }
        protected override void UnloadContent()
        {
            this.Content.Unload();
            base.UnloadContent();
        }



        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = this.ResolutionX;
            this.graphics.PreferredBackBufferHeight = this.ResolutionY;
            this.graphics.ApplyChanges();

            this.screen = new Screen(this, this.ResolutionX, this.ResolutionY);
            this.camera = new Camera(this.screen, this.baseZoom);
            this.sprites = new Sprites(this, this.camera);
            this.shapes = new Shapes(this, this.camera);

            this.InitializeGame();
            base.Initialize();
        }
        protected abstract void InitializeGame();



        protected override void Update(GameTime gameTime)
        {
            this.time = FlatTools.GetTime(gameTime);
            this.UpdateMouse();
            this.UpdateKeyboard();
            this.UpdateGame();
            base.Update(gameTime);
        }
        protected virtual void UpdateMouse()
        {
            this.mouse = FlatMouse.Instance;
            this.mouse.Update();
            this.mouseWorldPosition = this.mouse.GetWorldPosition(this.screen, this.camera);
        }
        protected virtual void UpdateKeyboard()
        {
            this.keyboard = FlatKeyboard.Instance;
            this.keyboard.Update();
        }
        protected abstract void UpdateGame();



        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            this.GraphicsDevice.Clear(this.backgroundColor);
            this.shapes.Begin(this.camera);
            this.sprites.Begin(this.camera);

            this.DrawGame();

            this.shapes.End();
            this.sprites.End();
            this.screen.UnSet();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }
        protected abstract void DrawGame();
    }
}