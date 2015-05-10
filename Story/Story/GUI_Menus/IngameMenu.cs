using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Story
{
    class IngameMenu : Menu
    {
        // Selection Type
        private enum IngameMenuSelectionType
        {
            Return,
            ExitToMap,
            Options,
            QuitToTitle,
            ExitGame
        }

        private IngameMenuSelectionType MenuSelection;

        private Texture2D MenuBG;

        private Menu.MenuDelegate WorldMapCallBack;     // Call back function to exit to world map
        private Menu.MenuDelegate TitleScreenCallBack;  // Call back function to exit to title
        private Menu.MenuDelegate ExitGameCallBack;     // Call back function to exit game

        public IngameMenu(MenuSideType MenuType)
            : base(MenuType)
        {

        }

        public void InitializeCallBacks(MenuDelegate WorldMapCallBack, MenuDelegate TitleScreenCallBack, MenuDelegate ExitGameCallBack)
        {
            this.WorldMapCallBack = WorldMapCallBack;
            this.TitleScreenCallBack = TitleScreenCallBack;
            this.ExitGameCallBack = ExitGameCallBack;
        }

        public override void Open()
        {
            MenuSelection = IngameMenuSelectionType.Return;
        }

        public override void LoadContent(ContentManager Content)
        {
            MenuBG = Content.Load<Texture2D>(@"Menus\IngameMenu\IngameMenu");
        }

        public override void Update(GameTime GameTime)
        {
            // Check for closing menu with back buttons
            if (InputManager.CheckJustPressed(InputManager.DeclineKeys, InputManager.DeclineButtons))
            {
                MenuManager.CloseMenu(this);
                return;
            }

            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu))
                switch (MenuSelection)
                {
                    case IngameMenuSelectionType.Return:
                        MenuManager.CloseMenu(this);
                        break;
                    case IngameMenuSelectionType.ExitToMap:
                        WorldMapCallBack();
                        break;
                    case IngameMenuSelectionType.Options:
                        MenuManager.OpenOptionsMenu(true);
                        break;
                    case IngameMenuSelectionType.QuitToTitle:
                        TitleScreenCallBack();
                        break;
                    case IngameMenuSelectionType.ExitGame:
                        ExitGameCallBack();
                        break;
                }

            // Down
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down))
            {
                switch (MenuSelection)
                {
                    case IngameMenuSelectionType.Return:
                        MenuSelection = IngameMenuSelectionType.ExitToMap;
                        break;
                    case IngameMenuSelectionType.ExitToMap:
                        MenuSelection = IngameMenuSelectionType.Options;
                        break;
                    case IngameMenuSelectionType.Options:
                        MenuSelection = IngameMenuSelectionType.QuitToTitle;
                        break;
                    case IngameMenuSelectionType.QuitToTitle:
                        MenuSelection = IngameMenuSelectionType.ExitGame;
                        break;
                    case IngameMenuSelectionType.ExitGame:
                        MenuSelection = IngameMenuSelectionType.Return;
                        break;
                }
            }

            // Up
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Up))
            {
                switch (MenuSelection)
                {
                    case IngameMenuSelectionType.Return:
                        MenuSelection = IngameMenuSelectionType.ExitGame;
                        break;
                    case IngameMenuSelectionType.ExitToMap:
                        MenuSelection = IngameMenuSelectionType.Return;
                        break;
                    case IngameMenuSelectionType.Options:
                        MenuSelection = IngameMenuSelectionType.ExitToMap;
                        break;
                    case IngameMenuSelectionType.QuitToTitle:
                        MenuSelection = IngameMenuSelectionType.Options;
                        break;
                    case IngameMenuSelectionType.ExitGame:
                        MenuSelection = IngameMenuSelectionType.QuitToTitle;
                        break;
                }
            }

        }


        private Vector2 CursorPosition = Vector2.Zero;
        public override void Draw(SpriteBatch SpriteBatch)
        {
            // Draw background
            SpriteBatch.Draw(MenuBG, Game.HalfScreenSize - new Vector2(MenuBG.Width, MenuBG.Height / 2), Color.White);

            // Draw cursor at selection position
            switch (MenuSelection)
            {
                case IngameMenuSelectionType.Return:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X - MenuBG.Width / 2 - 112, Game.HalfScreenSize.Y - 212);
                    break;
                case IngameMenuSelectionType.ExitToMap:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X - MenuBG.Width / 2 - 128, Game.HalfScreenSize.Y - 212 + 90);
                    break;
                case IngameMenuSelectionType.Options:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X - MenuBG.Width / 2 - 112, Game.HalfScreenSize.Y - 212 + 90 * 2);
                    break;
                case IngameMenuSelectionType.QuitToTitle:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X - MenuBG.Width / 2 - 144, Game.HalfScreenSize.Y - 212 + 90 * 3);
                    break;
                case IngameMenuSelectionType.ExitGame:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X - MenuBG.Width / 2 - 128, Game.HalfScreenSize.Y - 212 + 90 * 4);
                    break;
            }
            if (MenuManager.HasFocus(this))
                SpriteBatch.Draw(MenuManager.VerticalSelect, CursorPosition, Color.White);
        }
    }
}