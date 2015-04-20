using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace Story
{
    class Projectile
    {
        private static Texture2D ProjectileTexture;
        private float MoveSpeed = 768.0f;
        private int Direction = 1;
        private SpriteEffects Flip;
        private HitEmitter ProjectileHitEmitter;
        public bool Dead = false;
        public bool HasCollided = false;

        private CollisionObjectRectangle CollisionBox;

        public Vector2 Position;

        public Projectile(Vector2 Position, SpriteEffects Flip)
        {
            this.Position = Position;
            this.Flip = Flip;
            if (Flip != SpriteEffects.FlipHorizontally)
                Direction = -1;

            CollisionBox = new CollisionObjectRectangle(Position, new Vector2((float)ProjectileTexture.Width, (float)ProjectileTexture.Height), CollisionObjectTypeEnum.Damaging);

            ProjectileHitEmitter = new HitEmitter(Vector2.Zero,
                new Color(255, 255, 255, 255), Vector2.Zero, Game.ScreenSize, new Vector2(-0.2f, -1.0f), new Vector2(2.4f, 0.4f),
                new Vector2(0, -8), Vector2.Zero);
        }

        public static void LoadContent(ContentManager Content)
        {
            ProjectileTexture = Content.Load<Texture2D>("Sprites/EggProjectile");
        }

        public void Update(GameTime GameTime)
        {
            ProjectileHitEmitter.Update(GameTime);
            if (ProjectileHitEmitter.CurrentTime > ProjectileHitEmitter.LiveTime)
                Dead = true;
            Position.X += MoveSpeed * Direction * (float)GameTime.ElapsedGameTime.TotalSeconds;

            CollisionBox.UpdatePosition(Position, (int)ProjectileTexture.Width, (int)ProjectileTexture.Height);
        }

        private Vector2 HitPosition;
        public void TestCollisionVsEnemies(List<Enemy> Enemies)
        {
            if (HasCollided)
                return;

            for (int Index = 0; Index < Enemies.Count; Index++)
            {
                if (!Enemies[Index].IsAlive || !Enemies[Index].CollisionBox.CollisionRect.Intersects(CollisionBox.CollisionRect))
                    continue;

                Enemies[Index].TakeDamage(1);

                HasCollided = true;
                this.HitPosition = Enemies[Index].Position;

                float StartXOffset = (float)ProjectileTexture.Width / 2f;

                // Reverse min/max velocities if facing other way
                if (Direction == -1)
                {
                    float Temp = ProjectileHitEmitter.MinVelocity.X;
                    ProjectileHitEmitter.MinVelocity.X = -ProjectileHitEmitter.MaxVelocity.X;
                    ProjectileHitEmitter.MaxVelocity.X = -Temp;
                    StartXOffset = -StartXOffset;
                }

                ProjectileHitEmitter.StartPosition.X = Position.X + StartXOffset;
                ProjectileHitEmitter.StartPosition.Y = Position.Y + (float)ProjectileTexture.Height;
                ProjectileHitEmitter.StartEffect();

                // Limit to one collision
                return;
            }
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            ProjectileHitEmitter.Draw(GameTime, SpriteBatch);

            if (!HasCollided)
            {
                SpriteBatch.Draw(ProjectileTexture, Position - Camera.CameraPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, Flip, 0);
                CollisionBox.Draw(SpriteBatch, Camera.CameraPosition, false);
            }
        }

    }
}
