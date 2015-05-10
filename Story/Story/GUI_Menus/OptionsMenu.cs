using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class OptionsMenu : Menu
    {

        // Selection Type
        private enum IngameMenuSelectionType
        {
            Return,
        }

        private IngameMenuSelectionType MenuSelection;

        private Texture2D MenuBG;

        public OptionsMenu(MenuSideType MenuType)
            : base(MenuType)
        {

        }

        public override void Open()
        {
            MenuSelection = IngameMenuSelectionType.Return;
        }

        public override void LoadContent(ContentManager Content)
        {
            MenuBG = Content.Load<Texture2D>(@"Menus\OptionsMenu\OptionsMenu");
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
                }

            // Down
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down))
            {
                switch (MenuSelection)
                {
                    case IngameMenuSelectionType.Return:
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
                        MenuSelection = IngameMenuSelectionType.Return;
                        break;
                }
            }

        }


        private Vector2 CursorPosition = Vector2.Zero;
        public override void Draw(SpriteBatch SpriteBatch)
        {
            // Draw background
            SpriteBatch.Draw(MenuBG, Game.HalfScreenSize - new Vector2(0, MenuBG.Height / 2), Color.White);

            // Draw cursor at selection position
            switch (MenuSelection)
            {
                case IngameMenuSelectionType.Return:
                    CursorPosition = new Vector2(Game.HalfScreenSize.X + MenuBG.Width / 2 - 112, Game.HalfScreenSize.Y - 212 + 90 * 3);
                    break;
            }

            if (MenuManager.HasFocus(this))
                SpriteBatch.Draw(MenuManager.VerticalSelect, CursorPosition, Color.White);
        }

    }
}
