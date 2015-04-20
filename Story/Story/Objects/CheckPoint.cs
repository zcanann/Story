using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class CheckPoint
    {
        private static Texture2D OuthouseOpen;
        private static Texture2D OuthouseClosed;
        public static Vector2 Size;

        private const int DeadSpace = 28;

        public CollisionObjectRectangle CollisionBox;

        public Vector2 Position;
        public bool IsOpen;

        public CheckPoint(Vector2 Position)
        {
            this.Position = Position;
            CollisionBox = new CollisionObjectRectangle(Position, Size, CollisionObjectTypeEnum.CheckPoint);
        }

        public static void LoadContent(ContentManager Content)
        {
            OuthouseClosed = Content.Load<Texture2D>("Objects/CheckPoint/OuthouseClosed");
            OuthouseOpen = Content.Load<Texture2D>("Objects/CheckPoint/OuthouseOpen");

            Size.X = OuthouseOpen.Width - DeadSpace;
            Size.Y = OuthouseOpen.Height;
        }

        public bool IsOnScreen()
        {
            return CollisionBox.IsOnScreen();
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            if (!IsOnScreen())
                return;

            //Draw open/closed
            if (IsOpen)
                SpriteBatch.Draw(OuthouseOpen, Position - Camera.CameraPosition, Color.White);
            else
                SpriteBatch.Draw(OuthouseClosed, Position - Camera.CameraPosition, Color.White);

            CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
        }
    }
}
