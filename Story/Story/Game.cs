using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Story
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Debug constants
        public static bool DebugMode = true;
        public const bool GodMode = false;
        public const bool Muted = true;
        public const bool VisualizeFrustumCulling = false;

        // Rendering
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;

        // Screen Constants
        public const int BackBufferWidth = 1280;
        public const int BackBufferHeight = 720;
        public static Vector2 ScreenSize = new Vector2(BackBufferWidth, BackBufferHeight);
        public static Vector2 HalfScreenSize = new Vector2(BackBufferWidth / 2, BackBufferHeight / 2);

        public static SpriteFont GameFont;

        public static float PauseTime;

        // Game related
        private Level Level;

        public Game()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferWidth = BackBufferWidth;
            Graphics.PreferredBackBufferHeight = BackBufferHeight;
            IsMouseVisible = true;
        }

        

        public void NewGame()
        {
            // TODO PLAY OPENING SEQUENCE (AS MENU). KILL SAVE FILE.
            Level = new Level();
            Level.LoadLevel(1);
        }

        public void LoadGame()
        {
            // ENSURE SAVE DATA IS LOADED
            Level = new Level();
            Level.LoadLevel(MenuManager.LevelSelectMenu.LevelSelectionID + 1);
        }

        public void OpenLevelEditor()
        {
            // Create level as a level editor
            Level = new LevelEditor(MenuManager.LevelSelectMenu.LevelSelectionID + 1);
        }

        public void OpenTitleScreen()
        {
            MenuManager.OpenTitleScreenMenu(true);
        }

        public void OpenWorldMap()
        {
            // Set callback to load the game normally
            MenuManager.LevelSelectMenu.InitializeCallBacks(LoadGame, OpenTitleScreen);
            MenuManager.OpenLevelSelectMenu(true);
        }

        public void OpenWorldMapEditor()
        {
            // Set callback to load the game in editor mode
            MenuManager.LevelSelectMenu.InitializeCallBacks(OpenLevelEditor, OpenTitleScreen);
            MenuManager.OpenLevelSelectMenu(true);
        }

        private void QuitGame()
        {
            this.Exit();
        }

        protected override void Initialize()
        {
            InitializeMenus();

            base.Initialize();
        }

        private void InitializeMenus()
        {
            MenuManager.Initialize();

            // Initialize menu callback functions
            MenuManager.TitleScreenMenu.InitializeCallBacks(NewGame, OpenWorldMap, OpenWorldMapEditor, QuitGame);
            MenuManager.IngameMenu.InitializeCallBacks(OpenWorldMap, OpenTitleScreen, QuitGame);
            MenuManager.LevelSelectMenu.InitializeCallBacks(LoadGame, OpenTitleScreen);

            MenuManager.OpenTitleScreenMenu(true);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);  // Create a new SpriteBatch to draw textures

            // Load font
            GameFont = Content.Load<SpriteFont>("Font/GameFont");

            Level.LoadContent(Content);
            LevelEditor.LoadContent(Content);
            Player.LoadContent(Content);

            GUI.LoadContent(Content);
            MenuManager.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime GameTime)
        {
            InputManager.BeginUpdate(GameTime);

            if (InputManager.CheckJustPressed(InputManager.DebugKeys, InputManager.DebugButtons))
                DebugMode = !DebugMode;

            if (!MenuManager.HasOpenMenu() && Level != null)
                Level.Update(GameTime);
            else
                PauseTime += GameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            GUI.Update(GameTime);
            MenuManager.Update(GameTime);

            InputManager.EndUpdate();
            base.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();

            if (Level != null)
                Level.Draw(GameTime, SpriteBatch);

            //Camera.Draw(SpriteBatch);
            GUI.Draw(GameTime, SpriteBatch);
            MenuManager.Draw(SpriteBatch);

            SpriteBatch.End();

            base.Draw(GameTime);
        }

        public static void DrawShadowedString(SpriteBatch SpriteBatch, SpriteFont Font, string Value, Vector2 Position, Color Color)
        {
            SpriteBatch.DrawString(Font, Value, Position + Vector2.One, Color.Black);
            SpriteBatch.DrawString(Font, Value, Position, Color);
        }

        public static void DrawString(SpriteBatch SpriteBatch, SpriteFont Font, string Value, Vector2 Position, Color Color)
        {
            SpriteBatch.DrawString(Font, Value, Position, Color);
        }
    }
}
