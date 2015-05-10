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
        public static Vector2 Size;
        public static Vector2 CollisionSize = new Vector2(128, 128);
        private static Texture2D ExitTexture;

        private CollisionObjectRectangle ContainingBox;
        public CollisionObjectRectangle CollisionBox;

        public Vector2 Position;

        public LevelExit(Vector2 Position)
        {
            this.Position = Position;

            Vector2 CollisionBoxPosition = this.Position + Size / 2 - CollisionSize/2;

            ContainingBox = new CollisionObjectRectangle(Position, Size, CollisionObjectTypeEnum.NPCOnly);
            CollisionBox = new CollisionObjectRectangle(CollisionBoxPosition, CollisionSize, CollisionObjectTypeEnum.Exit);
        }

        public static void LoadContent(ContentManager Content)
        {
            ExitTexture = Content.Load<Texture2D>("Objects/Exit/Exit");

            Size.X = ExitTexture.Width;
            Size.Y = ExitTexture.Height;
            CollisionSize.Y = Size.Y;
        }

        public bool IsOnScreen()
        {
            return ContainingBox.IsOnScreen();
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            if (!IsOnScreen())
                return;

            //Draw open/closed
            SpriteBatch.Draw(ExitTexture, Position - Camera.CameraPosition, Color.White);

            ContainingBox.Draw(SpriteBatch, Camera.CameraPosition, false);
            CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
        }
    }
}
