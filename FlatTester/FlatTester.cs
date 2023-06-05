using System;
using System.Collections.Generic;
using System.Diagnostics;
using FlatEngine;
using FlatEngine.Util;
using FlatPhysics;
using FlatPhysics.BodyType;
using FlatPhysics.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Rectangle = FlatPhysics.BodyType.Rectangle;

namespace FlatTester
{
    public class GameTester : FlatGame
    {
        private readonly Color? worldColor = new Color(29, 78, 137);
        private readonly Color  wallColor = new Color(247, 146, 86);
        private readonly Color[] shapeColorArray = { new Color(249, 178, 124), new Color(251, 209, 162), new Color(125, 207, 182), new Color(0, 178, 202) };
        private readonly Color  lineColor = Color.Black;
        private readonly double lineThickness = 0.01;

        private const int iterationCount = 8; // virtual fps, how many updates per frame
        private World world;
        private FlatHUD HUD;
        private Texture2D tennisBallTexture;
        private Texture2D basketBallTexture;

        private SpriteFont font;
        private Color fontColor = Color.White;
        private Stopwatch stopwatch = new Stopwatch();
        private Stopwatch sampleTimer = new Stopwatch();
        private double timerUpdateTime = 0.1;
        private double totalWorldStepTime = 0;
        private string worldUPSString = string.Empty;
        private string worldStepTimeString = string.Empty;
        private string bodyCountString = string.Empty;
        private int totalBodyCount = 0;
        private int totalSampleCount = 0;

        public GameTester() :
            base(1920, 1080, 0.005)
        { }

        protected override void InitializeGame()
        {
            this.camera.GetExtents(out double viewportWidth, out double viewportHeight);
            this.world = new World(viewportWidth, viewportHeight, this.worldColor);
            this.world.SetGround(Gravity.Earth, 0.1, 0.5, 0.6, 0.4, this.wallColor, this.lineColor, this.lineThickness);
            this.sampleTimer.Start();
        }
        protected override void LoadContent()
        {
            this.font = this.Content.Load<SpriteFont>("Consolas18");
            this.HUD = new FlatHUD(new FlatVector(-this.screen.HalfWidth, this.screen.HalfHeight), this.camera, this.font, this.fontColor);
            this.tennisBallTexture = this.Content.Load<Texture2D>("tennisBall");
            this.basketBallTexture = this.Content.Load<Texture2D>("basketBall");
        }



        protected override void UpdateMouse()
        {
            base.UpdateMouse();

            if (this.mouse.IsRightReleased())
            {
                double width = 0.5;
                double height = 1.8;
                Body player = new Rectangle(width, height, 0.5, 0.6, 0.4, 60);
                player.SetColor(FlatRandom.Element(this.shapeColorArray));
                player.SetOutline(this.lineColor, this.lineThickness);
                player.MoveTo(mouseWorldPosition);
                this.world.AddEntity(player);
            }
            if (this.mouse.IsLeftReleased())
            {
                double a = FlatRandom.Double(0.05, 0.5);
                double b = FlatRandom.Double(0.05, 0.5);
                double c = FlatRandom.Double(0.05, 0.5);
                Body tri = Triangle.ABC(a, b, c, 0.5, 0.6, 0.4, Triangle.GetMass(a, b, c, 1), true);
                tri.SetColor(FlatRandom.Element(this.shapeColorArray));
                tri.SetOutline(this.lineColor, this.lineThickness);
                tri.MoveTo(mouseWorldPosition);
                this.world.AddEntity(tri);
            }
            if (this.mouse.IsThumb1Released())
            {
                double width = FlatRandom.Double(0.1, 0.5);
                double height = FlatRandom.Double(0.1, 0.5);
                Body box = new Rectangle(width, height, 0.5, 0.6, 0.4, Rectangle.GetMass(width, height, 1), false);
                box.SetColor(FlatRandom.Element(this.shapeColorArray));
                box.SetOutline(this.lineColor, this.lineThickness);
                box.MoveTo(mouseWorldPosition);
                this.world.AddEntity(box);
            }
            if (this.mouse.IsThumb2Released())
            {
                for (int i = 0; i < 100; i++)
                {
                    double x = FlatRandom.Double(-this.world.HalfWidth, this.world.HalfWidth);
                    double y = FlatRandom.Double(-this.world.HalfHeight, this.world.HalfHeight);
                    double radius = FlatRandom.Double(0.05, 0.2);
                    Body ball = new Circle(radius, 0.5, 0.5, 0.5, 0.1, false);
                    ball.SetColor(FlatRandom.Element(this.shapeColorArray));
                    ball.SetOutline(this.lineColor, this.lineThickness);
                    ball.MoveTo(x, y);
                    this.world.AddEntity(ball);
                }
            }
            double camSpeed = this.camera.Zoom * 10;
            if (this.mouse.IsScrollingUp()) this.camera.MoveZ(-camSpeed);
            if (this.mouse.IsScrollingDown()) this.camera.MoveZ(camSpeed);
        }
        protected override void UpdateKeyboard()
        {
            base.UpdateKeyboard();

            double camSpeed = this.camera.Zoom * 1000 * this.time;
            if (this.keyboard.IsKeyDown(Keys.Up)) this.camera.Move(0, camSpeed);
            if (this.keyboard.IsKeyDown(Keys.Left)) this.camera.Move(-camSpeed, 0);
            if (this.keyboard.IsKeyDown(Keys.Down)) this.camera.Move(0, -camSpeed);
            if (this.keyboard.IsKeyDown(Keys.Right)) this.camera.Move(camSpeed, 0);

            if (this.keyboard.IsKeyClicked(Keys.Escape)) this.Exit();
            if (this.keyboard.IsKeyClicked(Keys.Q)) this.camera.Reset();
            if (this.keyboard.IsKeyClicked(Keys.R)) this.Initialize();
            if (this.keyboard.IsKeyClicked(Keys.F)) FlatTools.ToggleFullscreen(this.graphics);

            if (this.world.GetBody(5, out Body body))
            {
                double force = 200;
                if (keyboard.IsKeyDown(Keys.W)) body.ApplyLinearForce(new FlatVector(0, force));
                if (keyboard.IsKeyDown(Keys.A)) body.ApplyLinearForce(new FlatVector(-force, 0));
                if (keyboard.IsKeyDown(Keys.S)) body.ApplyLinearForce(new FlatVector(0, -force));
                if (keyboard.IsKeyDown(Keys.D)) body.ApplyLinearForce(new FlatVector(force, 0));
            }
        }
        protected override void UpdateGame()
        {
            if (this.sampleTimer.Elapsed.TotalSeconds > this.timerUpdateTime)
            {
                this.worldUPSString = "Updates/Sec: " + Math.Round(1000 * (this.totalSampleCount / this.totalWorldStepTime)).ToString();
                this.worldStepTimeString = "Update Duration: " + Math.Round(this.totalWorldStepTime / this.totalSampleCount, 4).ToString();
                this.bodyCountString = "Body Count: " + Math.Round((double)this.totalBodyCount / this.totalSampleCount).ToString();
                this.totalWorldStepTime = 0;
                this.totalBodyCount = 0;
                this.totalSampleCount = 0;
                this.sampleTimer.Restart();
            }

            this.stopwatch.Restart();
            this.world.Update(this.time, GameTester.iterationCount);
            this.HUD.Update(this.worldUPSString, this.worldStepTimeString, this.bodyCountString);
            this.stopwatch.Stop();
            this.totalWorldStepTime += this.stopwatch.Elapsed.TotalMilliseconds;
            this.totalBodyCount += this.world.BodyCount;
            this.totalSampleCount++;
        }
        protected override void DrawGame()
        {
            this.world.Draw(this.sprites, this.shapes, this.camera);
            this.HUD.Draw(this.sprites);
        }
    }
}