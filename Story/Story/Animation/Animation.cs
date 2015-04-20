using System;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    class Animation
    {
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        public float FrameTime
        {
            get { return frameTime; }
        } private float frameTime;

        public bool IsLooping
        {
            get { return isLooping; }
        } private bool isLooping;

        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        public Animation(Texture2D Texture, float FrameTime, bool IsLooping = false)
        {
            this.texture = Texture;
            this.frameTime = FrameTime;
            this.isLooping = IsLooping;
        }
    }
}
