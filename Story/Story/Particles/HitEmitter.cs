using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class HitEmitter : ParticleEmitter
    {
        private static Texture2D HitTexture;

        public HitEmitter(
            Vector2 StartPosition,
            Color ParticleColor,
            Vector2 MinPosition,
            Vector2 MaxPosition,
            Vector2 MinVelocity,
            Vector2 MaxVelocity,
            Vector2 MinOffset,
            Vector2 MaxOffset,
            int MaxParticles = 8500,
            int ParticleRate = 2,
            int ParticleSize = 32,
            float RotationSpeed = 0.0025f,
            float OpacitySpeed = 0.75f,
            float ScaleSpeed = 0.006f,
            float ScaleMax = 0.2f,
            bool KillOnMaxScale = false,
            float LiveTime = 300f)

            : base(StartPosition, ParticleColor, MinPosition, MaxPosition, MinVelocity, MaxVelocity, MinOffset, MaxOffset, MaxParticles, ParticleRate,
            ParticleSize, RotationSpeed, OpacitySpeed, ScaleSpeed, ScaleMax, KillOnMaxScale, LiveTime)
        {
            this.EffectTexture = HitTexture;

            LoadParticles(EffectTexture);
        }

        public static void LoadContent(ContentManager Content)
        {
            HitTexture = Content.Load<Texture2D>("Particles/ProjectileParticle");
        }
    }
}
