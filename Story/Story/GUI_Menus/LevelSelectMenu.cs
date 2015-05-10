using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class LevelSelectMenu : Menu
    {
        private List<Vector2> LevelNodes = new List<Vector2>();
        private List<Vector2> LevelLineWeights = new List<Vector2>();

        private PlayerSaveData PlayerSaveData;
        private bool[] CollectionData = new bool[3];
        private int HighestCompletedLevel = 0;

        public bool LevelEditorMode = false;
        public int LevelSelectionID;

        private Texture2D MenuBG;

        private Texture2D LevelNodeUnlockedTexture;
        private Texture2D LevelNodeLockedTexture;

        private Texture2D DashedLineLockedTexture;
        private Texture2D DashedLineUnlockedTexture;

        private Texture2D WorldMapSelectorTexture;

        private Texture2D ProgressPanelTexture;
        private Texture2D[] FruitTextures = new Texture2D[3];
        private Texture2D[] FruitGrayedTextures = new Texture2D[3];

        private Vector2 NodeSizeOver2 = Vector2.Zero;
        private Vector2 SelectorSizeOver2 = Vector2.Zero;

        private Menu.MenuDelegate BeginGameCallback;    // Call back function to begin game
        private Menu.MenuDelegate TitleScreenCallBack;  // Call back function to exit to title

        private float Bounce = 0.0f;
        private const float BounceHeight = 0.18f;
        private const float BounceRate = 3.0f;

        public LevelSelectMenu(MenuSideType MenuType)
            : base(MenuType)
        {
            LevelNodes.Add(new Vector2(110, 280));
            LevelNodes.Add(new Vector2(180, 190));
            LevelNodes.Add(new Vector2(340, 190));
            LevelNodes.Add(new Vector2(210, 360));
            LevelNodes.Add(new Vector2(300, 450));
            LevelNodes.Add(new Vector2(400, 540));
            LevelNodes.Add(new Vector2(580, 620));
            LevelNodes.Add(new Vector2(600, 390));
            LevelNodes.Add(new Vector2(720, 130));
            LevelNodes.Add(new Vector2(880, 140));
            LevelNodes.Add(new Vector2(1000, 210));
            LevelNodes.Add(new Vector2(1200, 180));
            LevelNodes.Add(new Vector2(1040, 330));
            LevelNodes.Add(new Vector2(720, 320));
            LevelNodes.Add(new Vector2(880, 490));
            LevelNodes.Add(new Vector2(1080, 430));
            LevelNodes.Add(new Vector2(1180, 380));
            LevelNodes.Add(new Vector2(1200, 490));
            LevelNodes.Add(new Vector2(990, 630));
        }

        public void InitializeCallBacks(MenuDelegate BeginGameCallback, MenuDelegate TitleScreenCallBack)
        {
            this.BeginGameCallback = BeginGameCallback;
            this.TitleScreenCallBack = TitleScreenCallBack;
        }

        public override void Open()
        {
            PlayerSaveData = SaveGame.LoadPlayerData();

            HighestCompletedLevel = 0;
            for (int i = 0; i < PlayerSaveData.LevelProgress.Count; i++)
            {
                if (PlayerSaveData.LevelProgress[i].Item1 > HighestCompletedLevel)
                    HighestCompletedLevel = PlayerSaveData.LevelProgress[i].Item1;
            }

            SetCompletionBooleans();
        }

        public override void LoadContent(ContentManager Content)
        {
            MenuBG = Content.Load<Texture2D>(@"Menus\WorldMap\WorldMap");
            LevelNodeUnlockedTexture = Content.Load<Texture2D>(@"Menus\WorldMap\LevelNodeUnlocked");
            LevelNodeLockedTexture = Content.Load<Texture2D>(@"Menus\WorldMap\LevelNodeLocked");

            DashedLineLockedTexture = Content.Load<Texture2D>(@"Menus\WorldMap\DashedLineLocked");
            DashedLineUnlockedTexture = Content.Load<Texture2D>(@"Menus\WorldMap\DashedLineUnlocked");

            WorldMapSelectorTexture = Content.Load<Texture2D>(@"Menus\WorldMap\WorldMapSelector");

            FruitTextures[0] = Content.Load<Texture2D>(@"Menus\WorldMap\Apple");
            FruitTextures[1] = Content.Load<Texture2D>(@"Menus\WorldMap\Banana");
            FruitTextures[2] = Content.Load<Texture2D>(@"Menus\WorldMap\Orange");
            FruitGrayedTextures[0] = Content.Load<Texture2D>(@"Menus\WorldMap\AppleGrayed");
            FruitGrayedTextures[1] = Content.Load<Texture2D>(@"Menus\WorldMap\BananaGrayed");
            FruitGrayedTextures[2] = Content.Load<Texture2D>(@"Menus\WorldMap\OrangeGrayed");

            ProgressPanelTexture = Content.Load<Texture2D>(@"Menus\WorldMap\ProgressPanel");

            NodeSizeOver2.X = DashedLineLockedTexture.Width / 2;
            NodeSizeOver2.Y = DashedLineLockedTexture.Height / 2;

            SelectorSizeOver2.X = WorldMapSelectorTexture.Width / 2;
            SelectorSizeOver2.Y = WorldMapSelectorTexture.Height / 2;
        }

        public override void Update(GameTime GameTime)
        {

            Bounce = (float)Math.Sin(GameTime.TotalGameTime.TotalSeconds * BounceRate) * BounceHeight * WorldMapSelectorTexture.Height;

            // Check for closing menu with back buttons
            if (InputManager.CheckJustPressed(InputManager.DeclineKeys, InputManager.DeclineButtons))
            {
                TitleScreenCallBack();
                return;
            }

            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu))
            {
                BeginGameCallback();
                MenuManager.CloseMenu(this);
                return;
            }

            // Down
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down))
            {
                if (LevelSelectionID > 0)
                {
                    LevelSelectionID--;
                    SetCompletionBooleans();
                }
            }

            // Up
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Up))
            {
                if (LevelSelectionID < Math.Min(HighestCompletedLevel, LevelNodes.Count - 1))
                {
                    LevelSelectionID++;
                    SetCompletionBooleans();
                }
            }

        }

        // Set the collection properties being displayed
        private void SetCompletionBooleans()
        {
            for (int i = 0; i < PlayerSaveData.LevelProgress.Count; i++)
            {
                if (PlayerSaveData.LevelProgress[i].Item1 == (LevelSelectionID + 1))
                {
                    CollectionData = PlayerSaveData.LevelProgress[i].Item2;
                    return;
                }
            }

            // No record found
            CollectionData = new bool[] { false, false, false };
        }

        private Vector2 CurrentLineStep = Vector2.Zero;
        private const float StepSize = 64.0f;

        public override void Draw(SpriteBatch SpriteBatch)
        {
            // Draw background
            SpriteBatch.Draw(MenuBG, Game.HalfScreenSize - new Vector2(MenuBG.Width / 2, MenuBG.Height / 2), Color.White);

            // Draw nodes
            for (int i = 0; i < LevelNodes.Count; i++)
            {
                // Draw dotted line between nodes
                if (i != 0)
                {
                    CurrentLineStep = LevelNodes[i - 1];
                    float Distance = (float)Vector2.Distance(CurrentLineStep, LevelNodes[i]);
                    float Angle = (float)Math.Atan2(CurrentLineStep.Y - LevelNodes[i].Y, CurrentLineStep.X - LevelNodes[i].X);

                    while (Distance > StepSize)
                    {
                        CurrentLineStep.X += (float)(StepSize * -Math.Cos(Angle));
                        CurrentLineStep.Y += (float)(StepSize * -Math.Sin(Angle));

                        if (i <= HighestCompletedLevel)
                            SpriteBatch.Draw(DashedLineUnlockedTexture, CurrentLineStep, null, Color.White, Angle, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                        else
                            SpriteBatch.Draw(DashedLineLockedTexture, CurrentLineStep, null, Color.White, Angle, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

                        Distance -= StepSize;
                    }
                }

                if (i > HighestCompletedLevel)
                    SpriteBatch.Draw(LevelNodeLockedTexture, LevelNodes[i] - NodeSizeOver2, Color.White);
                else
                    SpriteBatch.Draw(LevelNodeUnlockedTexture, LevelNodes[i] - NodeSizeOver2, Color.White);
            }

            // Draw selection
            SpriteBatch.Draw(WorldMapSelectorTexture, LevelNodes[LevelSelectionID] - SelectorSizeOver2 - Vector2.UnitY * (Bounce + 16.0f), Color.White);

            // Draw progress panel
            SpriteBatch.Draw(ProgressPanelTexture, Game.ScreenSize * Vector2.UnitY - Vector2.UnitY * ProgressPanelTexture.Height, Color.White);

            Game.DrawShadowedString(SpriteBatch, Game.GameFont, "Collected Fruits:",
                new Vector2(ProgressPanelTexture.Width / 2 - 86, Game.ScreenSize.Y - ProgressPanelTexture.Height + 48),
                Color.Black);

            // Draw progress for this level
            for (int i = 0; i < 3; i++)
            {
                if (CollectionData[i] == true)
                {
                    SpriteBatch.Draw(FruitTextures[i], new Vector2(FruitTextures[i].Width / 2 + FruitTextures[i].Width * 2 * i + ProgressPanelTexture.Width / 2 - FruitTextures[i].Width * 3, Game.ScreenSize.Y - ProgressPanelTexture.Height / 2), Color.White);
                }
                else
                {
                    SpriteBatch.Draw(FruitGrayedTextures[i], new Vector2(FruitTextures[i].Width / 2 + FruitTextures[i].Width * 2 * i + ProgressPanelTexture.Width / 2 - FruitTextures[i].Width * 3, Game.ScreenSize.Y - ProgressPanelTexture.Height / 2), Color.White);
                }
            }
        }
    }
}
