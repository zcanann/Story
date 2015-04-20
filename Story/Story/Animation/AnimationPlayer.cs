using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    struct AnimationPlayer
    {
        public Animation Animation;
        public int FrameIndex;
        private float Time;

        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        public void PlayAnimation(Animation Animation)
        {
            // If this animation is already running, do not restart it.
            if (this.Animation == Animation)
                return;

            // Start the new animation.
            this.Animation = Animation;
            this.FrameIndex = 0;
            this.Time = 0.0f;
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 Position, float Rotation, SpriteEffects AnimationSpriteEffects, bool Underwater)
        {
            if (Animation == null)
                return;
                //throw new NotSupportedException("No animation is currently playing.");

            //Process passing time.
            Time += (float)GameTime.ElapsedGameTime.TotalSeconds;
            while (Time > Animation.FrameTime)
            {
                Time -= Animation.FrameTime;

                //Advance the frame index; looping or clamping as appropriate.
                if (Animation.IsLooping)
                    FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                else
                    FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);
            }

            //Calculate the source rectangle of the current frame.
            Rectangle Source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            Color DrawColor = new Color(255, 255, 255, 255);
            if (Underwater)
            {
                DrawColor.R = 255;
                DrawColor.G = 255;
                DrawColor.B = 255;
                DrawColor.A = 255;
            }

            //Draw the current frame.
            SpriteBatch.Draw(Animation.Texture, Position, Source, DrawColor, Rotation, Vector2.Zero, 1.0f, AnimationSpriteEffects, 1f);
        }
    }
}
