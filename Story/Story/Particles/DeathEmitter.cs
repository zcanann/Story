using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class DeathEmitter : ParticleEmitter
    {
        private static Texture2D BloodTexture;

        public DeathEmitter
            (Vector2 StartPosition,
            Color ParticleColor,
            Vector2 MinPosition,
            Vector2 MaxPosition,
            Vector2 MinVelocity,
            Vector2 MaxVelocity,
            Vector2 MinOffset,
            Vector2 MaxOffset,
            int MaxParticles = 1500, 
            int ParticleRate = 20,
            int ParticleSize = 32,
            float RotationSpeed = 0f,
            float OpacitySpeed = 0.75f, 
            float ScaleSpeed = 0.0009f,
            float ScaleMax = 0.8f,
            bool KillOnMaxScale = false,
            float LiveTime = 500f)
            : base(StartPosition, ParticleColor, MinPosition, MaxPosition, MinVelocity, MaxVelocity, MinOffset, MaxOffset, MaxParticles, ParticleRate,
            ParticleSize, RotationSpeed, OpacitySpeed, ScaleSpeed, ScaleMax, KillOnMaxScale, LiveTime)
        {
            this.EffectTexture = BloodTexture;
            
            LoadParticles(EffectTexture);
        }

        public static void LoadContent(ContentManager Content)
        {
            BloodTexture = Content.Load<Texture2D>("Particles/DeathParticle");
        }
    }
}
