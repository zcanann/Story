using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Story
{
    class CreditsMenu : Menu
    {
        private float CreditsTimer = 0.0f;
        private Texture2D MenuBG;

        private List<string> CreditsStringList = new List<string>();

        public CreditsMenu(MenuSideType MenuSideType)
            : base(MenuSideType)
        {
            CreditsStringList.Add("Director - Zachary Canann");
            CreditsStringList.Add("Producer - Zachary Canann");
            CreditsStringList.Add("Senior Software Engineer - Zachary Canann");
            CreditsStringList.Add("Junior Software Engineer - Zachary Canann");
            CreditsStringList.Add("Programmer - Zachary Canann");
            CreditsStringList.Add("Effects Programmer - Zachary Canann");
            CreditsStringList.Add("Graphics Design Lead - Zachary Canann");
            CreditsStringList.Add("Assistant Graphics Designer - Zachary Canann");
            CreditsStringList.Add("Story Writer - Zachary Canann");
            CreditsStringList.Add("Character Animation - Zachary Canann");
            CreditsStringList.Add("Menu Design - Zachary Canann");
            CreditsStringList.Add("Concept Art - Zachary Canann");
            CreditsStringList.Add("Quality Assurance Manager - Zachary Canann");
            CreditsStringList.Add("Lead Product Analyst - Zachary Canann");
            CreditsStringList.Add("Assistant Lead Analyst - Zachary Canann");
            CreditsStringList.Add("Product Analysis - Zachary Canann");
            CreditsStringList.Add("Production Support - Zachary Canann");
            CreditsStringList.Add("Special Thanks - Zachary Canann");
            CreditsStringList.Add("Even More Special Thanks - Zachary Canann");
            CreditsStringList.Add("");
            CreditsStringList.Add("");
            CreditsStringList.Add("Even MORE Special Thanks - Matthew Gehring");
        }

        public override void Open()
        {
            CreditsTimer = 0.0f;
        }

        public override void LoadContent(ContentManager Content)
        {
            MenuBG = Content.Load<Texture2D>(@"Menus\IngameMenu\IngameMenu");
        }

        public override void Update(GameTime GameTime)
        {
            CreditsTimer += GameTime.ElapsedGameTime.Milliseconds;
        }

        public override void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(MenuBG, Vector2.Zero, Color.White);

            for (int Index = 0; Index < CreditsStringList.Count; Index++)
            {
                Game.DrawString(SpriteBatch, Game.GameFont, CreditsStringList[Index],
                    new Vector2(Game.HalfScreenSize.X - Game.GameFont.MeasureString(CreditsStringList[Index]).X / 2,
                        Game.ScreenSize.Y /*+ 128.0f * CreditsStringList.Count*/ + 196.0f * (Index) - CreditsTimer / 10.0f),
                    Color.White);
            }
        }
    }
}
