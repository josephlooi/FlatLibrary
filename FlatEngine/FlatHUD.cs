using FlatEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FlatEngine
{
    public class FlatHUD
    {
        private readonly Camera camera;
        private readonly SpriteFont font;
        private FlatVector camPosition;
        private FlatVector position;
        private List<string> textList;
        private Color color;

        public FlatHUD(FlatVector camPosition, Camera camera, SpriteFont font, Color color)
        {
            this.textList = new List<string>();
            this.camPosition = camPosition;
            this.camera = camera;
            this.font = font;
            this.color = color;
        }



        public void Update(List<string> text)
        {
            this.textList = text;
            this.UpdatePosition();
        }
        public void Update(params string[] text)
        {
            this.textList = text.ToList();
            this.UpdatePosition();
        }
        private void UpdatePosition()
        {
            this.position = this.camPosition * this.camera.Zoom + this.camera.Position;
        }



        public void Draw(Sprites sprites)
        {
            double stringHeight = this.font.MeasureString("Ay").Y * this.camera.Zoom;
            FlatVector lineHeight = new FlatVector(0, stringHeight);

            for (int i = 0; i < this.textList.Count; i++)
            {
                FlatVector position = this.position - lineHeight * (i + 1);
                sprites.DrawString(this.font, this.textList[i], FlatVector.Zero, position, 0, this.camera.Zoom, this.color);
            }
        }



        public void AddText(string text)
        {
            this.textList.Add(text);
        }
        public bool RemoveText(string text)
        {
            return this.textList.Remove(text);
        }
        public bool ReplaceText(int lineNum, string text)
        {
            if (Math.Abs(lineNum) >= this.textList.Count) return false;
            if (lineNum < 0) lineNum += this.textList.Count;
            this.textList[lineNum] = text;
            return true;
        }
    }
}
