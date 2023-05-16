using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace FlatEngine.Util
{
    public static class FlatTools
    {
        private static readonly Exception MinMaxException = new ArgumentOutOfRangeException("Min must be less than Max.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetTime(GameTime gameTime)
        {
            return (double)gameTime.ElapsedGameTime.TotalSeconds;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetTime(GameTime gameTime, int interval)
        {
            return (double)gameTime.ElapsedGameTime.TotalSeconds / interval;
        }



        public static void ToggleFullscreen(GraphicsDeviceManager graphics, bool isExclusiveFullscreen = false)
        {
            graphics.HardwareModeSwitch = isExclusiveFullscreen;
            graphics.ToggleFullScreen();
        }



        public static int Clamp(int value, int min, int max)
        {
            if (min >= max) throw MinMaxException;
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        public static double Clamp(double value, double min, double max)
        {
            if (min >= max) throw MinMaxException;
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static int Loop(int value, int min, int max)
        {
            if (min >= max) throw MinMaxException;
            if (value < min) return max - min + value;
            else if (value >= max) return min + max - value;
            else return value;
        }
        public static double Loop(double value, double min, double max)
        {
            if (min >= max) throw MinMaxException;
            if (value < min) return max - min + value;
            else if (value >= max) return min + max - value;
            else return value;
        }
    }
}
