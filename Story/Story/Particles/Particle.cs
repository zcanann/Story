using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    public class Particle
    {

        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 velocity;

        public float Rotation;
        public float Scale;
        public Color Color;
        public int Width;
        public int Height;
        public Vector2 Origin;
        public bool Alive;
        public float Opacity;

        public Particle()
        {
            //Arbitrary defaults
            Position = Vector2.Zero;
            velocity = Vector2.Zero;
            Origin = Vector2.Zero;
            Color = Color.White;
            Rotation = 0f;
            Scale = 0.5f;
            Opacity = 0f;
            Alive = false;
        }

        public void Draw(SpriteBatch SpriteBatch, Rectangle DrawRectangle)
        {
            Color.A = (byte)Opacity;
            SpriteBatch.Draw(Texture, DrawRectangle, new Rectangle(0, 0, Texture.Width, Texture.Height),
                Color, Rotation, Origin, SpriteEffects.None, 0);
        }

    }
}
