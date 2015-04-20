using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    static class Camera
    {
        public static Vector2 PanWindowOffset = Game.ScreenSize / 4;
        public static Vector2 PanWindowSize = Game.ScreenSize / 2;
        public static Vector2 CameraPosition = Vector2.Zero;
        public static Vector2 CameraSize = Game.ScreenSize;
        public static Rectangle CameraRectangle = new Rectangle((int)CameraPosition.X, (int)CameraPosition.Y, (int)CameraSize.X, (int)CameraSize.Y);
        private static Rectangle ObjectRectangle;

        private static LinePrimitive CameraPanLines;

        public static void UpdateCameraPan(Vector2 Position, float Width, float Height)
        {
            if (Position.X < CameraPosition.X - PanWindowOffset.X + PanWindowSize.X)
                CameraPosition.X = Position.X + PanWindowOffset.X - PanWindowSize.X;

            else if (Position.X + Width > CameraPosition.X - PanWindowOffset.X + PanWindowSize.X + PanWindowSize.X)
                CameraPosition.X = Position.X + Width + PanWindowOffset.X - PanWindowSize.X - PanWindowSize.X;

            if (Position.Y < CameraPosition.Y - PanWindowOffset.Y + PanWindowSize.Y)
                CameraPosition.Y = Position.Y + PanWindowOffset.Y - PanWindowSize.Y;

            else if (Position.Y + Height > CameraPosition.Y - PanWindowOffset.Y + PanWindowSize.Y + PanWindowSize.Y)
                CameraPosition.Y = Position.Y + Height + PanWindowOffset.Y - PanWindowSize.Y - PanWindowSize.Y;
        }

        public static bool IsOnScreen(Vector2 Position)
        {
            ObjectRectangle.X = (int)Position.X;
            ObjectRectangle.Y = (int)Position.Y;
            ObjectRectangle.Width = 0;
            ObjectRectangle.Height = 0;

            return IsOnScreen(ObjectRectangle);   
        }

        public static bool IsOnScreen(Vector2 Position, int Width, int Height)
        {
            ObjectRectangle.X = (int)Position.X;
            ObjectRectangle.Y = (int)Position.Y;
            ObjectRectangle.Width = Width;
            ObjectRectangle.Height = Height;

            return IsOnScreen(ObjectRectangle);  
        }

        public static bool IsOnScreen(Vector2 Position, Vector2 Size)
        {
            ObjectRectangle.X = (int)Position.X;
            ObjectRectangle.Y = (int)Position.Y;
            ObjectRectangle.Width = (int)Size.X;
            ObjectRectangle.Height = (int)Size.Y;

            return IsOnScreen(ObjectRectangle);  
        }

        public static bool IsOnScreen(Rectangle Rectangle)
        {
            CameraRectangle.X = (int)CameraPosition.X;
            CameraRectangle.Y = (int)CameraPosition.Y;
            //Rectangle.X -= (int)CameraPosition.X;
            //Rectangle.Y -= (int)CameraPosition.Y;

            if (Rectangle.Width < 0)
            {
                int temp = Rectangle.Width;
                Rectangle.X += Rectangle.Width;
                Rectangle.Width = -temp;
            }
            if (Rectangle.Height < 0)
            {
                int temp = Rectangle.Height;
                Rectangle.Y += Rectangle.Height;
                Rectangle.Height = -temp;
            }

            return CameraRectangle.Intersects(Rectangle);
        }

        private static void InitializePanLines(SpriteBatch SpriteBatch)
        {
            CameraPanLines = new LinePrimitive(SpriteBatch.GraphicsDevice);

            CameraPanLines.ClearVectors();
            CameraPanLines.AddVector(PanWindowOffset);
            CameraPanLines.AddVector(new Vector2(PanWindowOffset.X + PanWindowSize.X, PanWindowOffset.Y));
            CameraPanLines.AddVector(new Vector2(PanWindowOffset.X + PanWindowSize.X, PanWindowOffset.Y + PanWindowSize.Y));
            CameraPanLines.AddVector(new Vector2(PanWindowOffset.X, PanWindowOffset.Y + PanWindowSize.Y));
            CameraPanLines.AddVector(PanWindowOffset);
        }

        public static void Draw(SpriteBatch SpriteBatch)
        {
            if (!Game.DebugMode)
                return;

            if (CameraPanLines == null)
                InitializePanLines(SpriteBatch);

            CameraPanLines.Draw(SpriteBatch, Color.Green, Vector2.Zero);
        }
    }
}
