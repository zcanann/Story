using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    static class GUI
    {
        private static Texture2D[] HeartTexture = new Texture2D[Player.MaxPlayerHP + 1];
        private static Texture2D EggTexture;

        private static MindPowerEmitter MindPowerEmitter;
        private static Texture2D MindPowerCoverTexture;
        private static Texture2D MindPowerBackTexture;
        private static int StartMindPowerEffectY = 14;
        private static int EndMindPowerEffectY;
        private static float MindPowerEffectY = StartMindPowerEffectY;

        public static void LoadContent(ContentManager Content)
        {
            // Load textures
            MindPowerBackTexture = Content.Load<Texture2D>("GUI/MindPowerBack");
            MindPowerCoverTexture = Content.Load<Texture2D>("GUI/MindPowerCover");

            EggTexture = Content.Load<Texture2D>("GUI/Egg");

            for (int i = 0; i <= Player.MaxPlayerHP; i++)
                HeartTexture[i] = Content.Load<Texture2D>("GUI/Heart" + i);

            EndMindPowerEffectY = MindPowerCoverTexture.Height - 34;

            MindPowerEmitter = new MindPowerEmitter(new Vector2(
                Game.BackBufferWidth - MindPowerCoverTexture.Width / 2-8, StartMindPowerEffectY),
                new Color(32, 0, 255, 32),
                new Vector2(Game.BackBufferWidth - MindPowerCoverTexture.Width + 28, StartMindPowerEffectY),
                new Vector2(Game.BackBufferWidth, EndMindPowerEffectY),
                new Vector2(-0.2f, 0.4f),
                new Vector2(0.2f, 1.40f), new Vector2(-32, 0), new Vector2(32, 0));

            MindPowerEmitter.LoadContent(Content.Load<Texture2D>("Particles/MindPowerParticle"));
            MindPowerEmitter.StartEffect();
        }

        public static void Update(GameTime GameTime)
        {
            MindPowerEffectY = Player.MindPower / Player.MindPowerMax * (StartMindPowerEffectY - EndMindPowerEffectY) + EndMindPowerEffectY;
            MindPowerEmitter.StartPosition.Y = MindPowerEffectY;

            MindPowerEmitter.Update(GameTime);
        }

        public static void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            // Draw heart
            SpriteBatch.Draw(HeartTexture[Player.DisplayHealth], Vector2.Zero, Color.White);

            // Draw egg texture
            SpriteBatch.Draw(EggTexture, new Vector2(HeartTexture[0].Width, HeartTexture[0].Height - EggTexture.Height), Color.White);

            // Draw UI egg count
            Game.DrawShadowedString(SpriteBatch, Game.GameFont, "x" + Player.EggCount.ToString(),
                new Vector2(HeartTexture[0].Width + EggTexture.Width, HeartTexture[0].Height - EggTexture.Height + 8),
                Color.Yellow);

            // Draw Mind Power textures and effect
            SpriteBatch.Draw(MindPowerBackTexture, new Vector2(Game.BackBufferWidth - MindPowerBackTexture.Width - 8, 8), Color.White);
            MindPowerEmitter.Draw(GameTime, SpriteBatch, false);
            SpriteBatch.Draw(MindPowerCoverTexture, new Vector2(Game.BackBufferWidth - MindPowerBackTexture.Width - 8, 8), Color.White);
            
        }
    }
}
