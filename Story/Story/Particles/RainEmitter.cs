using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class RainEmitter : ParticleEmitter
    {
        private static Texture2D RainTexture;

        public RainEmitter(
            Vector2 StartPosition,
            Color ParticleColor,
            Vector2 MinPosition,
            Vector2 MaxPosition,
            Vector2 MinVelocity,
            Vector2 MaxVelocity,
            Vector2 MinOffset,
            Vector2 MaxOffset,
            int MaxParticles = 2500,
            int ParticleRate = 8,
            int ParticleSize = 32,
            float RotationSpeed = 0.0f,
            float OpacitySpeed = 0.0f,
            float ScaleSpeed = 0.006f,
            float ScaleMax = 0.1f,
            bool KillOnMaxScale = false,
            float LiveTime = 0.0f)

            : base(StartPosition, ParticleColor, MinPosition, MaxPosition, MinVelocity, MaxVelocity, MinOffset, MaxOffset, MaxParticles, ParticleRate,
            ParticleSize, RotationSpeed, OpacitySpeed, ScaleSpeed, ScaleMax, KillOnMaxScale, LiveTime)
        {
            this.EffectTexture = RainTexture;

            LoadParticles(EffectTexture);
        }

        public static void LoadContent(ContentManager Content)
        {
            RainTexture = Content.Load<Texture2D>("Particles/RainParticle");
        }
    }
}
