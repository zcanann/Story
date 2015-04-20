using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Story
{
    class Player : Entity
    {
        private static Texture2D IdleTexture;
        private static Texture2D AttackTexture;
        private static Texture2D UnderwaterTexture;

        private const int HoverBuffer = 16;
        private float HoverHeightMax;
        private float HoverWidth;
        private float HoverHeight;

        private CollisionObjectRectangle HoverCollisionBox;
        private CollisionObjectRectangle HoverBufferCollisionBox;

        private BreathEmitter BreathEmitter;

        private const float AttackTimerMax = 200.0f;
        private float AttackTimer = AttackTimerMax;

        private const float MindPowerMax = 8000.0f;
        private float MindPower = MindPowerMax;

        private Vector2 LastCheckPointSpawn;

        private bool Glide = false;
        private bool GlideFatigued = false;
        private bool BreathingUW = false;

        private int EggCount = 0;

        private const float BounceHeight = 0.18f;
        private const float BounceRate = 3.0f;

        private float Bounce = 0.0f;

        public Player(Vector2 Position)
            : base(Position)
        {
            AnimationSet.IdleAnimation = new Animation(IdleTexture, 0.1f);
            AnimationSet.RunAnimation = new Animation(IdleTexture, 0.15f);
            AnimationSet.JumpAnimation = new Animation(IdleTexture, 0.1f);
            AnimationSet.AttackAnimation = new Animation(AttackTexture, 0.1f);
            AnimationSet.UnderwaterAnimation = new Animation(UnderwaterTexture, 0.1f);

            Sprite.PlayAnimation(AnimationSet.IdleAnimation);

            Width = IdleTexture.Width;
            Height = IdleTexture.Width;
            HoverWidth = Width;
            HoverHeight = HoverHeightMax = (float)Height * 2.5f;

            CollisionBox = new CollisionObjectRectangle(Position, new Vector2(Width, Height), CollisionObjectTypeEnum.Entity);
            HoverCollisionBox = new CollisionObjectRectangle(Position, new Vector2(HoverWidth, HoverHeight - HoverBuffer), CollisionObjectTypeEnum.Hover);
            HoverBufferCollisionBox = new CollisionObjectRectangle(Position + Vector2.UnitY * (HoverHeight - HoverBuffer), new Vector2(HoverWidth, HoverBuffer), CollisionObjectTypeEnum.Hover);

            BreathEmitter = new BreathEmitter(Vector2.Zero,
                new Color(255, 255, 255, 255), Vector2.Zero, Game.ScreenSize, new Vector2(-0.2f, -1.8f), new Vector2(0.2f, 0.2f),
                new Vector2(-4, -8), new Vector2(4, -8));

            LastCheckPointSpawn = Position;

            Health = MaxHealth = 10;
        }

        new public static void LoadContent(ContentManager Content)
        {
            IdleTexture = Content.Load<Texture2D>("Sprites/Player/Idle");
            AttackTexture = Content.Load<Texture2D>("Sprites/Player/Player_Attack");
            UnderwaterTexture = Content.Load<Texture2D>("Sprites/Player/Player_Underwater");
        }

        public override void Update(GameTime GameTime, List<CollisionObject> WorldCollisionObjects, bool IsNPC = false)
        {
            IsOnGround = false;
            if (!IsAlive)
            {
                base.Update(GameTime, WorldCollisionObjects);
                if (DeathTimer > DeathTimerMax)
                    Respawn();
                return;
            }

            ProcessInput(GameTime);
            Camera.UpdateCameraPan(Position, Width, Height);
            DoHoverCollision(GameTime, WorldCollisionObjects);
            base.Update(GameTime, WorldCollisionObjects);


            BreathEmitter.Update(GameTime);

            HoverCollisionBox.UpdatePosition(Position, (int)HoverWidth, (int)(HoverHeight - HoverBuffer));
            HoverBufferCollisionBox.UpdatePosition(Position + Vector2.UnitY * (HoverHeight - HoverBuffer), (int)HoverWidth, HoverBuffer);

            // Update breath effect
            if (Underwater)
            {
                MindPower -= GameTime.ElapsedGameTime.Milliseconds;

                BreathEmitter.StartPosition.X = Position.X + (int)Width / 2;
                BreathEmitter.StartPosition.Y = Position.Y + (int)Height / 2;

                if (Flip == SpriteEffects.FlipHorizontally)
                    BreathEmitter.StartPosition.X += 24.0f;
                else
                    BreathEmitter.StartPosition.X -= 24.0f;

                if (!BreathEmitter.CreateEffect)
                    BreathEmitter.StartEffect();
            }
            else
            {
                MindPower += GameTime.ElapsedGameTime.Milliseconds;
                BreathEmitter.EndEffect();
            }

            if (BlinkTimer <= 0.0f && MindPower < 0.0f)
                TakeDamage(1);

            MindPower = MathHelper.Clamp(MindPower, 0.0f, MindPowerMax);

            // Select appropriate animation
            if (AttackTimer < AttackTimerMax)
            {
                AttackTimer += GameTime.ElapsedGameTime.Milliseconds;
                Sprite.PlayAnimation(AnimationSet.AttackAnimation);
            }
            else
            {
                if (Underwater)
                {
                    Sprite.PlayAnimation(AnimationSet.UnderwaterAnimation);
                }
                else
                {
                    Sprite.PlayAnimation(AnimationSet.IdleAnimation);
                }
            }

            // Update projectiles
            for (int ProjectileIndex = 0; ProjectileIndex < Projectiles.Count; ProjectileIndex++)
            {
                Projectiles[ProjectileIndex].Update(GameTime);
                if (Projectiles[ProjectileIndex].Dead)
                {
                    Projectiles.RemoveAt(ProjectileIndex);
                }
            }

            Bounce = (float)Math.Sin(GameTime.TotalGameTime.TotalSeconds * BounceRate) * BounceHeight * Height;

        } //End update

        public void TestCollisionVsCheckPoints(List<CheckPoint> CheckPoints)
        {
            for (int Index = 0; Index < CheckPoints.Count; Index++)
            {
                CheckPoints[Index].IsOpen = false;
                if (CollisionBox.CollisionRect.Intersects(CheckPoints[Index].CollisionBox.CollisionRect))
                {
                    CheckPoints[Index].IsOpen = true;
                    LastCheckPointSpawn = CheckPoints[Index].Position;
                    return;
                }
            }
        }

        public void Respawn()
        {
            SetPosition(LastCheckPointSpawn);
            Velocity = Vector2.Zero;
            IsAlive = true;

            Health = MaxHealth;
            MindPower = MindPowerMax;

            DeathTimer = 0.0f;
        }

        public void TestCollisionsVsEnemies(List<Enemy> Enemies)
        {
            for (int Index = 0; Index < Enemies.Count; Index++)
            {
                if (!Enemies[Index].IsAlive)
                    continue;

                // Projectile collision
                for (int ProjectileIndex = 0; ProjectileIndex < Projectiles.Count; ProjectileIndex++)
                {
                    Projectiles[ProjectileIndex].TestCollisionVsEnemies(Enemies);
                }

                // Player collision
                if (BlinkTimer > 0.0f)
                    continue;

                if (CollisionBox.CollisionRect.Intersects(Enemies[Index].CollisionBox.CollisionRect))
                {
                    TakeDamage(1);
                }
            }


        }

        public void TestCollisionVsEggs(List<CollectableEgg> Eggs)
        {
            for (int Index = 0; Index < Eggs.Count; Index++)
            {
                if (CollisionBox.CollisionRect.Intersects(Eggs[Index].CollisionBox.CollisionRect))
                {
                    // Only allow collection of 1 per update cycle
                    EggCount++;
                    Eggs.RemoveAt(Index);
                    return;
                }
            }
        }

        private void DoHoverCollision(GameTime GameTime, List<CollisionObject> WorldCollisionObjects)
        {
            bool RegularCollision = false;

            IsOnGround = false;

            for (int Index = 0; Index < WorldCollisionObjects.Count; Index++)
            {
                // Check for hover buffer collision
                if (!IsColliding(HoverCollisionBox, WorldCollisionObjects[Index]))
                    continue;

                // Take care of special cases
                switch (WorldCollisionObjects[Index].CollisionObjectType)
                {
                    case CollisionObjectTypeEnum.Damaging:
                        continue;
                    case CollisionObjectTypeEnum.Kill:
                        continue;
                    case CollisionObjectTypeEnum.Liquid:
                        continue;
                    case CollisionObjectTypeEnum.NPCOnly:
                        continue;
                    case CollisionObjectTypeEnum.Passable:
                        if (Velocity.Y < 0)
                            continue;
                        break;
                }

                RegularCollision = true;
                IsOnGround = true;

                // Collison Detected, float upwards
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -ActualGravityAcceleration, 0);
                Velocity.Y -= ActualGravityAcceleration / 24.0f * (float)GameTime.ElapsedGameTime.TotalSeconds;
            }

            if (RegularCollision)
                return;

            // Hover buffer collision
            for (int Index = 0; Index < WorldCollisionObjects.Count; Index++)
            {
                if (!IsColliding(HoverBufferCollisionBox, WorldCollisionObjects[Index]))
                    continue;

                // Take care of special cases
                switch (WorldCollisionObjects[Index].CollisionObjectType)
                {
                    case CollisionObjectTypeEnum.Damaging:
                        continue;
                    case CollisionObjectTypeEnum.Kill:
                        continue;
                    case CollisionObjectTypeEnum.Liquid:
                        continue;
                    case CollisionObjectTypeEnum.NPCOnly:
                        continue;
                    case CollisionObjectTypeEnum.Passable:
                        if (Velocity.Y < 0)
                            continue;
                        break;
                }

                // Set velocity vector to ensure smooth ramp walking
                Velocity.Y = WorldCollisionObjects[Index].CalculateSlopeAt(Position.X, (float)Width) * Velocity.X;
                IsOnGround = true;
            }
        }

        private void ProcessInput(GameTime GameTime)
        {
            float ThumbStickMovement;
            Movement = Vector2.Zero;

            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Left, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                Movement.X += ThumbStickMovement;

            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Right, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                Movement.X += ThumbStickMovement;

            if (InputManager.CheckJustPressed(InputManager.JumpKeys, InputManager.JumpButtons, InputManager.NoInputDelay))
                JumpEngaged = true;

            if (InputManager.CheckJustReleased(InputManager.JumpKeys, InputManager.JumpButtons, InputManager.NoInputDelay))
                JumpEngaged = false;

            if (InputManager.CheckRightTriggerDown())
            {
                if (EggCount > 0)
                {
                    EggCount--;
                    AttackTimer = 0.0f;
                    Projectiles.Add(new Projectile(Position - Vector2.UnitY * Bounce, Flip));
                }
            }

            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement) ||
                InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons, InputManager.NoInputDelay))
            {
                // Sinking
                if (HoverHeight > Height + 1)
                {
                    HoverHeight -= GameTime.ElapsedGameTime.Milliseconds / 2.0f;
                    HoverHeight = MathHelper.Clamp(HoverHeight, Height + 1, HoverHeightMax);
                }
            }
            else
            {
                // Unsinking
                // HoverHeight = HoverHeightMax; // This is the same thing but doesnt look as clutch in debug mode
                if (HoverHeight < HoverHeightMax)
                {
                    HoverHeight += GameTime.ElapsedGameTime.Milliseconds / 5.0f;
                    HoverHeight = MathHelper.Clamp(HoverHeight, Height + 1, HoverHeightMax);
                }
            }
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch, float BounceOffset = 0.0f)
        {
            base.Draw(GameTime, SpriteBatch, Bounce);

            if (!IsAlive)
                return;

            HoverCollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
            HoverBufferCollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
            BreathEmitter.Draw(GameTime, SpriteBatch);
        }

    }
}
