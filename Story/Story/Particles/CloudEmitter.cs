using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class CloudEmitter : ParticleEmitter
    {
        private static Texture2D CloudTexture;

        public CloudEmitter(
            Vector2 StartPosition,
            Color ParticleColor,
            Vector2 MinPosition,
            Vector2 MaxPosition,
            Vector2 MinVelocity,
            Vector2 MaxVelocity,
            Vector2 MinOffset,
            Vector2 MaxOffset,
            int MaxParticles = 1500,
            int ParticleRate = 5,
            int ParticleSize = 32,
            float RotationSpeed = 0.0025f,
            float OpacitySpeed = 0.75f,
            float ScaleSpeed = 0.002f,
            float ScaleMax = 0.6f,
            bool KillOnMaxScale = false,
            float LiveTime = 0.0f)

            : base(StartPosition, ParticleColor, MinPosition, MaxPosition, MinVelocity, MaxVelocity, MinOffset, MaxOffset, MaxParticles, ParticleRate,
            ParticleSize, RotationSpeed, OpacitySpeed, ScaleSpeed, ScaleMax, KillOnMaxScale, LiveTime)
        {
            this.EffectTexture = CloudTexture;

            LoadParticles(EffectTexture);
        }

        public static void LoadContent(ContentManager Content)
        {
            CloudTexture = Content.Load<Texture2D>("Particles/WindParticle");
        }
    }
}
