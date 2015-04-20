using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Story
{
    class Enemy : Entity
    {
        private static Random Random = new Random();

        private static AnimationSetStruct[] EnemyAnimationSets;

        public EnemyTypeEnum EnemyType;
        private float JumpTimer = 0.0f; // Used in faking input
        private double JumpVariation = 1d;

        public Enemy(Vector2 Position, EnemyTypeEnum EnemyType)
            : base(Position)
        {

            this.EnemyType = EnemyType;
            MaxHealth = 1;

            //Load stats based on type
            switch (EnemyType)
            {
                case EnemyTypeEnum.Velociraptor:
                    JumpVariation = RandomDouble(0.8d, 1.2d);
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 400f;
                    Width = 96;
                    Height = 96;

                    break;
                case EnemyTypeEnum.Dunkleosteus:
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 200f;

                    Width = 128;
                    Height = 64;
                    break;
                case EnemyTypeEnum.Pterodactyl:
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 200f;

                    Width = 96;
                    Height = 64;
                    break;
            }

            AnimationSet = EnemyAnimationSets[(int)EnemyType];

            Sprite.PlayAnimation(AnimationSet.IdleAnimation);

            CollisionBox = new CollisionObjectRectangle(Position, new Vector2(Width, Height), CollisionObjectTypeEnum.Entity);
        }

        new public static void LoadContent(ContentManager Content)
        {
            EnemyAnimationSets = new AnimationSetStruct[Enum.GetNames(typeof(EnemyTypeEnum)).Length];

            AnimationSetStruct RaptorAnimationSet;
            AnimationSetStruct PterodactylAnimationSet;
            AnimationSetStruct DunkleosteusAnimationSet;

            RaptorAnimationSet.AttackAnimation = new Animation(Content.Load<Texture2D>("Sprites/Raptor/Attack"), 0.1f, false);
            RaptorAnimationSet.IdleAnimation = new Animation(Content.Load<Texture2D>("Sprites/Raptor/Idle"), 0.1f, false);
            RaptorAnimationSet.RunAnimation = new Animation(Content.Load<Texture2D>("Sprites/Raptor/Run"), 0.8f / 10f, true);
            RaptorAnimationSet.JumpAnimation = new Animation(Content.Load<Texture2D>("Sprites/Raptor/Jump"), 0.1f, false);
            RaptorAnimationSet.UnderwaterAnimation = new Animation(Content.Load<Texture2D>("Sprites/Raptor/Idle"), 0.1f, true);
            EnemyAnimationSets[(int)EnemyTypeEnum.Velociraptor] = RaptorAnimationSet;

            PterodactylAnimationSet.AttackAnimation = new Animation(Content.Load<Texture2D>("Sprites/Pterodactyl/Pterodactyl"), 0.09f, true);
            PterodactylAnimationSet.IdleAnimation = new Animation(Content.Load<Texture2D>("Sprites/Pterodactyl/PterodactylIdle"), 0.09f, false);
            PterodactylAnimationSet.RunAnimation = new Animation(Content.Load<Texture2D>("Sprites/Pterodactyl/Pterodactyl"), 0.09f, true);
            PterodactylAnimationSet.JumpAnimation = new Animation(Content.Load<Texture2D>("Sprites/Pterodactyl/Pterodactyl"), 0.09f, true);
            PterodactylAnimationSet.UnderwaterAnimation = new Animation(Content.Load<Texture2D>("Sprites/Pterodactyl/Pterodactyl"), 0.09f, true);
            EnemyAnimationSets[(int)EnemyTypeEnum.Pterodactyl] = PterodactylAnimationSet;

            DunkleosteusAnimationSet.AttackAnimation = new Animation(Content.Load<Texture2D>("Sprites/Dunkleosteus/Dunkleosteus"), 0.1f, false);
            DunkleosteusAnimationSet.IdleAnimation = new Animation(Content.Load<Texture2D>("Sprites/Dunkleosteus/Dunkleosteus"), 0.1f, false);
            DunkleosteusAnimationSet.RunAnimation = new Animation(Content.Load<Texture2D>("Sprites/Dunkleosteus/Dunkleosteus"), 0.1f, false);
            DunkleosteusAnimationSet.JumpAnimation = new Animation(Content.Load<Texture2D>("Sprites/Dunkleosteus/Dunkleosteus"), 0.1f, false);
            DunkleosteusAnimationSet.UnderwaterAnimation = new Animation(Content.Load<Texture2D>("Sprites/Dunkleosteus/Dunkleosteus"), 0.1f, false);
            EnemyAnimationSets[(int)EnemyTypeEnum.Dunkleosteus] = DunkleosteusAnimationSet;

        }

        public override void Update(GameTime GameTime, List<CollisionObject> WorldCollisionObjects, bool IsNPC = true)
        {
            IsOnGround = false;
            Vector2 LastPosition = Position;
            SimulateInput(GameTime);

            base.Update(GameTime, WorldCollisionObjects, IsNPC);

            //ApplyPhysics(GameTime);

            // Reverse direction
            if (Math.Abs(Position.X - LastPosition.X) < 1.0f)
                Velocity.X *= -1;

            if (IsAlive && IsOnGround)
            {
                //if (Glide == true)
                //    sprite.PlayAnimation(glideAnimation);
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                    Sprite.PlayAnimation(AnimationSet.RunAnimation);
                else
                    Sprite.PlayAnimation(AnimationSet.IdleAnimation);
            }

            // Clear input.
            Movement.X = 0.0f;
            //JumpButtonHeld = false;
        }

        public void Attack()
        {
            switch (EnemyType)
            {
                case EnemyTypeEnum.Velociraptor:
                    IsAttacking = true;
                    break;
                default:
                    break;
            }
        }

        private void SimulateInput(GameTime gameTime)
        {
            switch (EnemyType)
            {
                case EnemyTypeEnum.Velociraptor:

                    JumpTimer += (float)(gameTime.ElapsedGameTime.Milliseconds * JumpVariation);

                    if (JumpTimer > 3500.0f && IsOnGround)
                    {
                        // Begin jump
                        JumpTimer = 0.0f;
                        JumpEngaged = true;
                        Velocity.Y = ActualJumpLaunchVelocity;
                        Sprite.PlayAnimation(AnimationSet.JumpAnimation);
                    }
                    else if (JumpTimer < 500.0f)
                    {
                        // Maintain jump
                        JumpEngaged = true;
                    }

                    // Move in the current direction (add noise for tie breaking -- ie when velocity = 0 we still want a +/-1 eventually)
                    Movement.X = Speed * (Math.Sign(Velocity.X + (float)Random.NextDouble() * 2 - (float)Random.NextDouble()));
                    break;
                default:
                    Movement.X = Speed * (Math.Sign(Velocity.X + (float)Random.NextDouble() * 2 - (float)Random.NextDouble()));
                    break;
            }
        }

        public enum EnemyTypeEnum
        {
            Velociraptor,
            Pterodactyl,
            Dunkleosteus,
        }
    }
}
