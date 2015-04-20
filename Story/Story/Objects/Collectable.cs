using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class Collectable
    {
        public bool Collected = false;

        protected Texture2D Texture;
        protected SoundEffect CollectedSound;
        public Vector2 Position;
        private Vector2 DrawPosition;

        public CollisionObjectRectangle CollisionBox;

        // Bounce control constants
        private const float BounceHeight = 0.18f;
        private const float BounceRate = 3.0f;
        private const float BounceSync = -0.75f;

        private float Bounce;

        private bool UpdateDrawLines = true;

        public Collectable(Vector2 Position)
        {
            this.Position = Position;
        }

        public void Update(GameTime GameTime)
        {
            // Bounce offset for drawing    
            Bounce = (float)Math.Sin(GameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync) * BounceHeight * Texture.Height;
        }

        public bool IsOnScreen()
        {
            return CollisionBox.IsOnScreen();
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            if (!IsOnScreen())
                return;

            // Draw collision box
            DrawPosition = Position - Camera.CameraPosition;
            DrawPosition.Y += Bounce;
            SpriteBatch.Draw(Texture, DrawPosition, Color.White);

            CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
        }

    }
}
