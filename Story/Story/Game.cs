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
        public const bool DebugMode = true;
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

        public void LoadGame()
        {
            Level = new Level();
            Level.LoadLevel(1);
        }

        public void NewGame()
        {
            Level = new Level();
            Level.LoadLevel(1);
        }

        public void OpenLevelEditor()
        {
            // Create level as a level editor
            Level = new LevelEditor();
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
            MenuManager.TitleScreenMenu.InitializeCallBacks(NewGame, LoadGame, OpenLevelEditor, QuitGame);

            MenuManager.OpenTitleScreenMenu(true);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);  // Create a new SpriteBatch to draw textures
            MenuManager.LoadContent(Content);

            Level.LoadContent(Content);
            LevelEditor.LoadContent(Content);
            Player.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime GameTime)
        {
            InputManager.BeginUpdate(GameTime);

            if (Level != null)
                Level.Update(GameTime);

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
            MenuManager.Draw(SpriteBatch);

            SpriteBatch.End();

            base.Draw(GameTime);
        }
    }
}
