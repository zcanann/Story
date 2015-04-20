using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class ParticleEmitter
    {
        private static Random Random = new Random();

        public Particle[] Particles;
        public bool CreateEffect = false;
        public int Next = 0;

        public Vector2 StartPosition;   // Start position of particle
        public Color ParticleColor;     // Color of particle
        public Vector2 MinPosition;     // Position lower limit
        public Vector2 MaxPosition;     // Position upper limit
        public Vector2 MinVelocity;     // Min velocity
        public Vector2 MaxVelocity;     // Max velocity
        public Vector2 MinOffset;       // Min position offset
        public Vector2 MaxOffset;       // Max position offset
        public int MaxParticles;        // Max number on screen at once
        public float ParticleRate;      // Rate of creation
        public int ParticleSize;        // Size of particle (x & y)
        public float RotationSpeed;     // Speed of rotation
        public float OpacitySpeed;      // Fade speed
        public float ScaleSpeed;        // Speed of rescale
        public float ScaleMax;          // Max scale
        public bool KillOnMaxScale;     // Kill particle after too large?
        public float LiveTime;

        public float CurrentTime = 0.0f;

        protected Texture2D EffectTexture;

        // Giant constructor with all of the needed particle emitter information
        public ParticleEmitter(Vector2 StartPosition, Color ParticleColor, Vector2 MinPosition, Vector2 MaxPosition,
             Vector2 MinVelocity, Vector2 MaxVelocity, Vector2 MinOffset, Vector2 MaxOffset, int MaxParticles, int ParticleRate,
            int ParticleSize, float RotationSpeed, float OpacitySpeed, float ScaleSpeed, float ScaleMax, bool KillOnMaxScale, float LiveTime)
        {
            this.StartPosition = StartPosition;
            this.ParticleColor = ParticleColor;
            this.MinPosition = MinPosition;
            this.MaxPosition = MaxPosition;
            this.MinVelocity = MinVelocity;
            this.MaxVelocity = MaxVelocity;
            this.MinOffset = MinOffset;
            this.MaxOffset = MaxOffset;
            this.MaxParticles = MaxParticles;
            this.ParticleRate = ParticleRate;
            this.ParticleSize = ParticleSize;
            this.RotationSpeed = RotationSpeed;
            this.OpacitySpeed = OpacitySpeed;
            this.ScaleSpeed = ScaleSpeed;
            this.ScaleMax = ScaleMax;
            this.KillOnMaxScale = KillOnMaxScale;
            this.LiveTime = LiveTime;

            Particles = new Particle[MaxParticles];
        }

        public static void LoadContent(Texture2D EffectTexture)
        {
            
        }

        public void LoadParticles(Texture2D Texture)
        {
            Particles = new Particle[MaxParticles];

            for (int Index = 0; Index < Particles.Length; Index++)
            {
                Particles[Index] = new Particle();
                Particles[Index].Position = StartPosition;
                Particles[Index].Texture = Texture;
                Particles[Index].Color = ParticleColor;
                Particles[Index].Width = ParticleSize;
                Particles[Index].Height = ParticleSize;
            }

        }

        public void Update(GameTime gameTime)
        {
            // CREATE NEW PARTICLES
            if (CreateEffect == true)
            {
                CurrentTime += gameTime.ElapsedGameTime.Milliseconds;

                if (CurrentTime > LiveTime && LiveTime != 0.0f)
                    EndEffect();

                for (int Index = 0; Index < ParticleRate; Index++)
                {
                    // Set initial particle values
                    Particles[Next].Opacity = ParticleColor.A;
                    Particles[Next].Color = ParticleColor;
                    Particles[Next].Scale = 0.0f;
                    Particles[Next].Alive = true;
                    // Set position/velocity
                    Particles[Next].Position = StartPosition;
                    Particles[Next].Position.X += RandomInt((int)MinOffset.X, (int)MaxOffset.X);
                    Particles[Next].Position.Y += RandomInt((int)MinOffset.Y, (int)MaxOffset.Y);
                    Particles[Next].velocity.X = (float)RandomDouble(MinVelocity.X, MaxVelocity.X);
                    Particles[Next].velocity.Y = (float)RandomDouble(MinVelocity.Y, MaxVelocity.Y);
                    Particles[Next].Opacity = Particles[Next].Color.A;

                    if (++Next == MaxParticles - 1)
                        Next = 0;
                }
            }

            // UPDATE PARTICLE POSITION
            float EndMultiplier = 1.0f; // Used to speed up disappearance of particles when effect ends
            if (!CreateEffect)
                EndMultiplier = 2.5f;

            // Update all active particles
            for (int Index = 0; Index < MaxParticles; Index++)
            {
                // Update alive only
                if (Particles[Index].Alive)
                {
                    // Fade
                    Particles[Index].Opacity -= OpacitySpeed * gameTime.ElapsedGameTime.Milliseconds * EndMultiplier;
                    if (Particles[Index].Opacity < 0f)
                    {
                        // Kill particle
                        Particles[Index].Alive = false;
                        Particles[Index].Opacity = 0f;
                    }

                    // Rotate
                    Particles[Index].Rotation += RotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

                    if (Particles[Index].Scale < ScaleMax)
                        Particles[Index].Scale += ScaleSpeed * gameTime.ElapsedGameTime.Milliseconds;
                    else if (KillOnMaxScale)
                        Particles[Index].Alive = false;

                    if (Particles[Index].Alive)
                    {
                        // Update position & travel distance
                        Particles[Index].Position = Particles[Index].Position + Particles[Index].velocity;
                    }
                }
            }
        }

        private void ResetPositions()
        {
            for (int Index = 0; Index < Particles.Length; Index++)
            {
                Particles[Index].Position = StartPosition;
                Particles[Index].Scale = 0.0f;
                Particles[Index].Opacity = Particles[Index].Color.A;
            }
        }

        public void StartEffect()
        {
            CreateEffect = true;
            Next = 0;
        }

        public void EndEffect()
        {
            CreateEffect = false;
        }

        public int RandomInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        public double RandomDouble(double start, double end)
        {
            return (Random.NextDouble() * Math.Abs(end - start)) + start;
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            Rectangle DrawRectangle = new Rectangle();

            // Restart SpriteBatch with desired parameters
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            for (int P = 0; P < Particles.Length; P++)
            {
                if (Particles[P].Alive == true)
                {
                    DrawRectangle.X = (int)Particles[P].Position.X - (int)Camera.CameraPosition.X;
                    DrawRectangle.Y = (int)Particles[P].Position.Y - (int)Camera.CameraPosition.Y;
                    DrawRectangle.Width = (int)(Particles[P].Width * Particles[P].Scale);
                    DrawRectangle.Height = (int)(Particles[P].Height * Particles[P].Scale);

                    if (DrawRectangle.X >= MinPosition.X && DrawRectangle.Y >= MinPosition.Y
                        && DrawRectangle.X + DrawRectangle.Width <= MaxPosition.X && DrawRectangle.Y + DrawRectangle.Height <= MaxPosition.Y)
                        Particles[P].Draw(SpriteBatch, DrawRectangle);
                    else
                        Particles[P].Alive = false;
                }
            }

            // End and begin to clear effects
            SpriteBatch.End();
            SpriteBatch.Begin();
        }


    }
}
