using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class BreathEmitter : ParticleEmitter
    {
        private static Texture2D BreathTexture;

        public BreathEmitter(
            Vector2 StartPosition,
            Color ParticleColor,
            Vector2 MinPosition,
            Vector2 MaxPosition,
            Vector2 MinVelocity,
            Vector2 MaxVelocity,
            Vector2 MinOffset,
            Vector2 MaxOffset,
            int MaxParticles = 1500,
            int ParticleRate = 1,
            int ParticleSize = 32,
            float RotationSpeed = 0.0f,
            float OpacitySpeed = 0.55f,
            float ScaleSpeed = 0.0006f,
            float ScaleMax = 0.8f,
            bool KillOnMaxScale = true,
            float LiveTime = 0f)

            : base(StartPosition, ParticleColor, MinPosition, MaxPosition, MinVelocity, MaxVelocity, MinOffset, MaxOffset, MaxParticles, ParticleRate,
            ParticleSize, RotationSpeed, OpacitySpeed, ScaleSpeed, ScaleMax, KillOnMaxScale, LiveTime)
        {
            this.EffectTexture = BreathTexture;

            LoadParticles(EffectTexture);
        }

        public static void LoadContent(ContentManager Content)
        {
            BreathTexture = Content.Load<Texture2D>("Particles/BreathParticle");
        }
    }
}
