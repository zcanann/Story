using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class LevelExit
    {
        private static Texture2D ExitTexture;
        public static Vector2 Size;

        private CollisionObjectRectangle CollisionBox;

        public Vector2 Position;

        public LevelExit(Vector2 Position)
        {
            this.Position = Position;
            CollisionBox = new CollisionObjectRectangle(Position, Size, CollisionObjectTypeEnum.Exit);
        }

        public static void LoadContent(ContentManager Content)
        {
            ExitTexture = Content.Load<Texture2D>("Objects/Exit/Exit");

            Size.X = ExitTexture.Width;
            Size.Y = ExitTexture.Height;
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
            SpriteBatch.Draw(ExitTexture, Position - Camera.CameraPosition, Color.White);

            CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
        }
    }
}
