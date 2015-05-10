using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Story
{
    class Entity
    {
        #region Variables

        // Flip effect
        protected SpriteEffects Flip = SpriteEffects.None;
        public CollisionObjectRectangle CollisionBox;

        protected AnimationPlayer Sprite;
        protected AnimationSetStruct AnimationSet;
        protected int Width;
        protected int Height;

        protected SoundEffect DeathSound;
        protected SoundEffect JumpSound;
        protected SoundEffect FallSound;

        private DeathEmitter DeathEmitter;   // Emits red particles upon death

        // Projectiles the entity owns
        protected List<Projectile> Projectiles = new List<Projectile>();

        public bool IsAlive = true;
        protected int MaxHealth = 10;
        protected int Health = 10;

        private const float BlinkTimeMax = 1000.0f;
        protected float BlinkTimer = 0.0f;

        private const float RotationSpeed = 0.045f;
        protected float Rotation = 0.0f;

        // Used to continue to draw an entity's effects after it is dead before removing it from the game
        public const float DeathTimerMax = 2000.0f;
        public float DeathTimer = 0.0f;

        // STATE VARIABLES
        protected bool IsAttacking = false;
        protected bool Underwater = false;
        protected bool IsJumping = false;
        protected bool JumpEngaged = false;
        protected bool IsOnGround = false;

        protected bool IsGliding = false;
        protected bool IsFatigued = false;

        // PHYSICS
        protected float Speed;
        protected Vector2 Movement; // Current analog movement
        public Vector2 Position;
        protected Vector2 Velocity = Vector2.Zero;

        // STANDARD
        protected float GroundDragFactor = .58f; //.58 normal
        protected float MaxMoveSpeed = 360.0f;

        private float MoveAcceleration = 14000.0f;
        private float AirDragFactor = 0.65f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 4000.0f;
        private const float MaxFallSpeed = 600.0f;

        // UNDERWATER
        private const float UnderwaterJumpLaunchVelocity = -3000.0f;
        private const float UnderwaterGravityAcceleration = 2000.0f;
        private const float UnderwaterMaxFallSpeed = 200.0f;

        // GLIDE
        private const float GlideMaxFallSpeed = 150.0f;

        // CURRENT STATE (Selected from underwater or standard)
        protected float ActualJumpLaunchVelocity;
        protected float ActualGravityAcceleration;
        protected float ActualMaxFallSpeed;

        #endregion

        #region Initialization

        public Entity(Vector2 Position)
        {

            DeathEmitter = new DeathEmitter(Vector2.Zero,
                new Color(255, 0, 255, 255),
                Vector2.Zero,
                Game.ScreenSize,
                new Vector2(-0.8f, -0.8f),
                new Vector2(0.8f, 0.8f),
                new Vector2(-16, -16),
                new Vector2(16, 16));

            SetPosition(Position);
        }

        public static void LoadContent(ContentManager Content)
        {
            //Death emitter particle
            ParticleEmitter.LoadContent(Content.Load<Texture2D>("Particles/DeathParticle"));
        }

        #endregion

        #region Update

        public virtual void Update(GameTime GameTime, List<CollisionObject> WorldCollisionObjects, bool IsNPC = false)
        {
            //Update blink timer
            if (BlinkTimer > 0.0f)
                BlinkTimer -= GameTime.ElapsedGameTime.Milliseconds;
            if (BlinkTimer < 0.0f)
                BlinkTimer = 0.0f;

            if (!IsAlive)
            {
                DeathEmitter.Update(GameTime);
                DeathTimer += GameTime.ElapsedGameTime.Milliseconds;
            }
            if (Underwater)
            {
                // Underwater physics
                ActualGravityAcceleration = UnderwaterGravityAcceleration;
                ActualMaxFallSpeed = UnderwaterMaxFallSpeed;
                ActualJumpLaunchVelocity = UnderwaterJumpLaunchVelocity;
            }
            else if (IsGliding)
            {
                // Gliding physics
                ActualGravityAcceleration = GravityAcceleration;
                ActualJumpLaunchVelocity = JumpLaunchVelocity;
                ActualMaxFallSpeed = GlideMaxFallSpeed;
            }
            else
            {
                // Normal physics
                ActualGravityAcceleration = GravityAcceleration;
                ActualJumpLaunchVelocity = JumpLaunchVelocity;
                ActualMaxFallSpeed = MaxFallSpeed;
            }

            ApplyPhysics(GameTime);

            HandleCollisions(GameTime, WorldCollisionObjects, IsNPC);

            CollisionBox.UpdatePosition(Position, Width, Height);
        }

        #endregion

        #region Physics & Collisions

        public void ApplyPhysics(GameTime GameTime)
        {
            float Elapsed = (float)GameTime.ElapsedGameTime.TotalSeconds;

            Velocity.X += Movement.X * MoveAcceleration * Elapsed;

            //Slowly restore entity rotation
            //Adjust depending on what direction entity is rotated
            if (Rotation < 0)
            {
                Rotation += RotationSpeed;
                if (Rotation > 0)
                    Rotation = 0;
            }
            if (Rotation > 0)
            {
                Rotation -= RotationSpeed;
                if (Rotation < 0)
                    Rotation = 0;
            }

            //Apply drag in X direction
            if (IsOnGround)
                Velocity.X *= GroundDragFactor;
            else
                Velocity.X *= AirDragFactor;

            UpdateGravityAndJump(GameTime);

            // Prevent fast speeds
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -ActualMaxFallSpeed, ActualMaxFallSpeed);

            // Apply velocity
            Position += Velocity * Elapsed;

            if (Velocity.X > 0)
                Flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                Flip = SpriteEffects.None;

        }

        private void UpdateGravityAndJump(GameTime GameTime)
        {
            float Elapsed = (float)GameTime.ElapsedGameTime.TotalSeconds;
            float LastVelocityY = Velocity.Y;

            // Apply gravity
            if (!IsOnGround)
                Velocity.Y += ActualGravityAcceleration * Elapsed;

            // Disengage jump at peak of jump
            if (LastVelocityY < 0.0f && Velocity.Y >= 0.0f)
                JumpEngaged = false;

            // Do not add to power of jump if jump button is not engaged
            if (!JumpEngaged)
                return;

            // Powerful initial jump
            if (IsOnGround)
            {
                IsOnGround = false;
                Velocity.Y = ActualJumpLaunchVelocity;
            }
            // Less powerful sustained jump while holding button
            else
            {
                // Just "slow" gravity by a factor of 70% when jumping upwards.
                Velocity.Y -= ActualGravityAcceleration * Elapsed * 0.70f;
                //Velocity.Y -= GameTime.ElapsedGameTime.Milliseconds / 4.0f / ActualJumpControlPower;
                //Velocity.Y = -((float)Math.Pow(MathHelper.Clamp(-Velocity.Y, 0.0f, ActualMaxFallSpeed), 0.99f));
            }

        }

        protected virtual void HandleCollisions(GameTime GameTime, List<CollisionObject> WorldCollisionObjects, bool IsNPC = false)
        {
            CollisionBox.UpdatePosition(Position, Width, Height);
            Underwater = false;
            // IsOnGround = false; // This is set by overriding classes ASAP (most organized this way due to collision + hover collision conflicts)

            for (int Index = 0; Index < WorldCollisionObjects.Count; Index++)
            {
                if (!IsColliding(CollisionBox, WorldCollisionObjects[Index]))
                    continue;

                // Take care of special cases
                switch (WorldCollisionObjects[Index].CollisionObjectType)
                {
                    case CollisionObjectTypeEnum.Damaging:
                        continue;
                    case CollisionObjectTypeEnum.Kill:

                        continue;
                    case CollisionObjectTypeEnum.Liquid:
                        Underwater = true;
                        continue;
                    case CollisionObjectTypeEnum.NPCOnly:
                        if (!IsNPC)
                            continue;
                        break;
                    case CollisionObjectTypeEnum.Passable:
                        if (Velocity.Y < 0)
                            continue;
                        break;
                }

                //IsOnGround = true;

                Vector2 ShiftAmount = DetermineNewOffset(CollisionBox, WorldCollisionObjects[Index]);

                Position -= ShiftAmount;
                CollisionBox.UpdatePosition(Position, Width, Height);
            }

        } // End HandleCollisions()

        protected Vector2 DetermineNewOffset(CollisionObject TargetObject, CollisionObject WorldObject)
        {
            Vector2 ShiftAmount = Vector2.Zero;

            const int TopBufferY = 16; // Allow for walking up small jagged stair configurations

            bool LandedOnGround = false;

            int Left = TargetObject.CollisionRect.X + TargetObject.CollisionRect.Width - WorldObject.CollisionRect.X;
            int Right = TargetObject.CollisionRect.X - (WorldObject.CollisionRect.X + WorldObject.CollisionRect.Width);
            int Top = TargetObject.CollisionRect.Y + TargetObject.CollisionRect.Height - WorldObject.CollisionRect.Y;
            int Bottom = TargetObject.CollisionRect.Y - (WorldObject.CollisionRect.Y + WorldObject.CollisionRect.Height);

            // Special case for triangles
            if (WorldObject.Verticies.Length == 3)
            {
                // Calculate the top as a linear function of the entity's position
                int IntendedYPosition = CalculateIntendedYOnRamp(TargetObject, WorldObject);

                Top = TargetObject.CollisionRect.Y + TargetObject.CollisionRect.Height - IntendedYPosition;

                // If we are above this line, we do not want to deal with left/right collision
                if (TargetObject.CollisionRect.Y + TargetObject.CollisionRect.Height < IntendedYPosition)
                {
                    Left = Right = 0;
                }
            }

            if (Math.Abs(Left) < Math.Abs(Right))
            {
                ShiftAmount.X = Left;
            }
            else
            {
                ShiftAmount.X = Right;
            }

            if (Math.Abs(Top) < Math.Abs(Bottom))
            {
                LandedOnGround = true;
                ShiftAmount.Y = Top;
            }
            else
            {
                ShiftAmount.Y = Bottom;
            }

            // Choose a direction to shift our collision box. Only do one direction at a time to prevent getting stuck.
            if (Math.Abs(ShiftAmount.X) < Math.Abs(ShiftAmount.Y) - TopBufferY)
            {
                ShiftAmount.Y = 0f;
            }
            else
            {
                ShiftAmount.X = 0f;
            }

            // Threshold the amount that can be shifted. If its too far we probably aren't colliding on that axis
            if (ShiftAmount.X < -32.0f || ShiftAmount.X > 32.0f)
                ShiftAmount.X = 0f;
            if (ShiftAmount.Y < -32.0f || ShiftAmount.Y > 32.0f)
                ShiftAmount.Y = 0f;

            if (LandedOnGround && ShiftAmount.Y != 0)
            {
                IsOnGround = true;
            }
            return ShiftAmount;
        }

        protected bool IsColliding(CollisionObject TargetObject, CollisionObject WorldObject)
        {
            if (!TargetObject.CollisionRect.Intersects(WorldObject.CollisionRect))
                return false;

            // Special tests for triangles
            if (WorldObject.Verticies.Length == 3)
            {
                // Calculate the top as a linear function of the entity's position
                int IntendedYPosition = CalculateIntendedYOnRamp(TargetObject, WorldObject);

                // If we are above this line, we do not want to deal with left/right collision
                if (TargetObject.CollisionRect.Y + TargetObject.CollisionRect.Height < IntendedYPosition)
                {
                    return false;
                }
            }

            return true;
        }

        private int CalculateIntendedYOnRamp(CollisionObject TargetObject, CollisionObject WorldObject)
        {
            // Y = mx + b, where m is the slope of the triangle, b is the y intercept of the triangle, and x is the horizontal
            // origin of the collision object.
            // However this can produce effects where a triangle collision object meets a rectangle collison object, and when stepping
            // From the rectangle to the triangle there are noticeable effects because the function is extended into space passed
            // it's actual horizontal size, thus we want to clamp it to its X bounds

            // Determine which corner of our rectangle should be colliding with the triangle
            int X = CollisionBox.CollisionRect.X;
            if (WorldObject.CalculateSlope() < 0.0f)
                X += CollisionBox.CollisionRect.Width;

            int IntendedYOnRamp = (int)(WorldObject.CalculateSlope() *
                MathHelper.Clamp(X, WorldObject.CollisionRect.X, WorldObject.CollisionRect.X + WorldObject.CollisionRect.Width) +
                WorldObject.CalculateYIntercept());

            return IntendedYOnRamp;
        }

        #endregion

        #region Methods
        private static Random Random = new Random();
        public double RandomDouble(double Start, double End)
        {
            return (Random.NextDouble() * Math.Abs(End - Start)) + Start;
        }

        public bool IsOnScreen()
        {
            return CollisionBox.IsOnScreen();
        }

        public void SetPosition(Vector2 Position)
        {
            this.Position = Position;
        }

        public void TakeDamage(int Damage, bool InstaKill = false)
        {
            Health -= Damage;
            BlinkTimer = BlinkTimeMax;

            if (Health <= 0 || InstaKill)
            {
                IsAlive = false;
                DeathEmitter.StartPosition.X = Position.X + (float)Width / 2;
                DeathEmitter.StartPosition.Y = Position.Y + (float)Height / 2;
                DeathEmitter.StartEffect();
            }
        }

        #endregion

        #region Draw

        public virtual void Draw(GameTime GameTime, SpriteBatch SpriteBatch, float BounceOffset = 0.0f)
        {

            for (int Index = 0; Index < Projectiles.Count; Index++)
            {
                Projectiles[Index].Draw(GameTime, SpriteBatch);
            }

            DeathEmitter.Draw(GameTime, SpriteBatch);

            if (!IsAlive || !IsOnScreen())
                return;

            // Draw entity
            Vector2 DrawPosition = Position - Camera.CameraPosition - Vector2.UnitY * BounceOffset;

            float Fraction = BlinkTimer / 100 - (float)Math.Truncate(BlinkTimer / 100);
            if (Fraction < 0.40f) // Blink 40% of the time if blinking
                Sprite.Draw(GameTime, SpriteBatch, DrawPosition, Rotation, Flip, Underwater);


            CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);

        }

        #endregion

        protected struct AnimationSetStruct
        {
            public Animation IdleAnimation;
            public Animation RunAnimation;
            public Animation JumpAnimation;
            public Animation AttackAnimation;
            public Animation UnderwaterAnimation;

            public AnimationSetStruct(Animation IdleAnimation, Animation RunAnimation, Animation JumpAnimation,
                Animation AttackAnimation, Animation UnderwaterAnimation)
            {
                this.IdleAnimation = IdleAnimation;
                this.RunAnimation = RunAnimation;
                this.JumpAnimation = JumpAnimation;
                this.AttackAnimation = AttackAnimation;
                this.UnderwaterAnimation = UnderwaterAnimation;
            }
        }
    }
}