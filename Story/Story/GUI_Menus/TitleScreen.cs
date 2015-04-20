using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class TitleScreen : Menu
    {
        public enum ScreenType
        {
            Title,
            Closed,
            GameExited,
        }

        private enum TitleSelectionType
        {
            NewGame,
            LoadGame,
            LevelEditor,
            Quit
        }

        private ScreenType CurrentScreen;
        private TitleSelectionType TitleSelection;

        private Texture2D Background;
        private Texture2D HorizontalSelect;
        private Texture2D VerticalSelect;

        private Menu.MenuDelegate NewGameCallBack;      // Call back function when new game is selected
        private Menu.MenuDelegate LoadGameCallBack;     // Call back function when load game is selected
        private Menu.MenuDelegate LevelEditorCallBack;  // Call back function when load game is selected
        private Menu.MenuDelegate ExitGameCallBack;     // Call back function when exit game is selected

        public TitleScreen(bool DefaultSelectNewGame = false)
            : base(MenuSideType.Both)
        {

            CurrentScreen = TitleScreen.ScreenType.Title;

            if (DefaultSelectNewGame)
                TitleSelection = TitleSelectionType.NewGame;
            else
                TitleSelection = TitleSelectionType.LoadGame;
        }

        public void InitializeCallBacks(MenuDelegate NewGameCallBack, MenuDelegate LoadGameCallBack,
            MenuDelegate LevelEditorCallBack, MenuDelegate ExitGameCallBack)
        {
            this.NewGameCallBack = NewGameCallBack;
            this.LoadGameCallBack = LoadGameCallBack;
            this.LevelEditorCallBack = LevelEditorCallBack;
            this.ExitGameCallBack = ExitGameCallBack;
        }

        public override void Update(GameTime GameTime)
        {
            switch (CurrentScreen)
            {
                // UPDATE TITLE SCREEN
                case ScreenType.Title:

                    if (InputManager.CheckPrimaryDirectionJustReleased(InputDirectionEnum.Up))
                        InputManager.ClearPrimaryDirectionalDelay(InputDirectionEnum.Up);
                    if (InputManager.CheckPrimaryDirectionJustReleased(InputDirectionEnum.Down))
                        InputManager.ClearPrimaryDirectionalDelay(InputDirectionEnum.Down);

                    // Up
                    if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Up))
                    {
                        switch (TitleSelection)
                        {
                            case TitleSelectionType.NewGame:
                                TitleSelection = TitleSelectionType.LevelEditor;
                                break;
                            case TitleSelectionType.LoadGame:
                                TitleSelection = TitleSelectionType.NewGame;
                                break;
                            case TitleSelectionType.Quit:
                                TitleSelection = TitleSelectionType.LoadGame;
                                break;
                            case TitleSelectionType.LevelEditor:
                                TitleSelection = TitleSelectionType.Quit;
                                break;
                        }
                    }

                    // Down
                    if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down) ||
                        InputManager.CheckPrimaryDirectionJustPressed(InputDirectionEnum.Down, InputManager.NoInputDelay, InputManager.ThumbStickPressThreshold))
                    {
                        switch (TitleSelection)
                        {
                            case TitleSelectionType.NewGame:
                                TitleSelection = TitleSelectionType.LoadGame;
                                break;
                            case TitleSelectionType.LoadGame:
                                TitleSelection = TitleSelectionType.Quit;
                                break;
                            case TitleSelectionType.Quit:
                                TitleSelection = TitleSelectionType.LevelEditor;
                                break;
                            case TitleSelectionType.LevelEditor:
                                TitleSelection = TitleSelectionType.NewGame;
                                break;
                        }
                    }

                    if (InputManager.CheckJustPressed(InputManager.AcceptKeys, InputManager.AcceptButtons))
                    {

                        switch (TitleSelection)
                        {
                            case TitleSelectionType.NewGame:
                                CallAndPrepareExit(NewGameCallBack);
                                break;
                            case TitleSelectionType.LoadGame:
                                CallAndPrepareExit(LoadGameCallBack);
                                break;
                            case TitleSelectionType.LevelEditor:
                                CallAndPrepareExit(LevelEditorCallBack);
                                break;
                            case TitleSelectionType.Quit:
                                CallAndPrepareExit(ExitGameCallBack);
                                break;
                        }

                    }
                    else if (InputManager.CheckJustPressed(InputManager.DeclineKeys, InputManager.DeclineButtons))
                    {
                        CurrentScreen = ScreenType.Closed;
                        //MainPTR.GameEditorStart();
                    }
                    break;


            }
        }

        public override void LoadContent(ContentManager Content)
        {
            Background = Content.Load<Texture2D>("Menus/TitleScreen/TitleScreen");
            HorizontalSelect = Content.Load<Texture2D>("Menus/TitleScreen/HorizontalSelect");
            VerticalSelect = Content.Load<Texture2D>("Menus/TitleScreen/VerticalSelect");
        }

        public override void Draw(SpriteBatch SpriteBatch)
        {
            switch (CurrentScreen)
            {
                case TitleScreen.ScreenType.Title:
                    SpriteBatch.Draw(Background, Vector2.Zero, Color.White);

                    Vector2 TitleCursorPosition = Vector2.Zero;
                    switch (TitleSelection)
                    {
                        case TitleSelectionType.NewGame:
                            TitleCursorPosition = new Vector2(Game.BackBufferWidth / 2 - 180, 140);
                            break;
                        case TitleSelectionType.LoadGame:
                            TitleCursorPosition = new Vector2(Game.BackBufferWidth / 2 - 180, 260);
                            break;
                        case TitleSelectionType.Quit:
                            TitleCursorPosition = new Vector2(Game.BackBufferWidth / 2 - 200, 380);
                            break;
                        case TitleSelectionType.LevelEditor:
                            TitleCursorPosition = new Vector2(Game.BackBufferWidth / 2 - 180, 500);
                            break;
                    }
                    SpriteBatch.Draw(VerticalSelect, TitleCursorPosition, Color.White);
                    break;

            }
        }

    }
}
