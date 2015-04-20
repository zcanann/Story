using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    /// <summary>
    /// Manages every bloody menu on earth
    /// </summary>
    static class MenuManager
    {
        //Both sides
        public static TitleScreen TitleScreenMenu;

        //Right Side

        //Left Side


        private static MenuSideType CurrentFocus;

        private static Menu LeftMenu;   // Menu on the left or menus that take both sides
        private static Menu RightMenu;  // Menu on the right

        private static Rectangle BlackBGRectangle;
        public static Texture2D BlackBG;

        public static void Initialize()
        {
            TitleScreenMenu = new TitleScreen();
        }

        public static void LoadContent(ContentManager Content)
        {
            TitleScreenMenu.LoadContent(Content);

            BlackBG = Content.Load<Texture2D>(@"Menus\SolidBlackSquare");
            BlackBGRectangle = new Rectangle(0, 0, Game.BackBufferWidth, Game.BackBufferHeight);
        }

        public static bool HasOpenMenu()
        {
            if (LeftMenu != null || RightMenu != null)
                return true;

            return false;
        }

        public static void ChangeFocus()
        {
            if (CurrentFocus == MenuSideType.Left)
                CurrentFocus = MenuSideType.Right;
            else if (CurrentFocus == MenuSideType.Right)
                CurrentFocus = MenuSideType.Left;
            else
                throw new Exception("Hey jackass, you screwed up and somehow the current menu selection was both. (class MenuManager)");
        }

        public static bool HasFocus(Menu CallingMenu)
        {
            if (CallingMenu.GetMenuType() == CurrentFocus ||
                CallingMenu.GetMenuType() == MenuSideType.Both)
                return true;

            return false;
        }

        public static void CloseMenu(Menu CallingMenu)
        {
            if (LeftMenu == CallingMenu)
                LeftMenu = null;
            if (RightMenu == CallingMenu)
                RightMenu = null;
        }

        public static void CloseAll()
        {
            LeftMenu = null;
            RightMenu = null;
        }

        public static void ExitGame()
        {

        }

        private static void OpenMenu(Menu TargetMenu, bool GiveFocus)
        {
            if (TargetMenu.GetMenuType() == MenuSideType.Both)
            {
                LeftMenu = TargetMenu;
                RightMenu = null;
                if (GiveFocus)
                    CurrentFocus = MenuSideType.Left;
                LeftMenu.Open();
            }

            if (TargetMenu.GetMenuType() == MenuSideType.Left)
            {
                LeftMenu = TargetMenu;
                if (GiveFocus)
                    CurrentFocus = MenuSideType.Left;
                LeftMenu.Open();
            }

            if (TargetMenu.GetMenuType() == MenuSideType.Right)
            {
                RightMenu = TargetMenu;
                if (GiveFocus)
                    CurrentFocus = MenuSideType.Right;
                RightMenu.Open();
            }

        }

        public static void OpenTitleScreenMenu(bool GiveFocus)
        {
            OpenMenu(TitleScreenMenu, GiveFocus);
        }

        public static void Update(GameTime GameTime)
        {
            switch (CurrentFocus)
            {
                case MenuSideType.Right:
                    if (RightMenu != null)
                    {
                        if (RightMenu.ReadyToExit)
                        {
                            RightMenu = null;
                            break;
                        }
                        RightMenu.Update(GameTime);
                    }
                    break;

                default:
                case MenuSideType.Left:
                    if (LeftMenu != null)
                    {
                        if (LeftMenu.ReadyToExit)
                        {
                            LeftMenu = null;
                            break;
                        }
                        LeftMenu.Update(GameTime);
                    }
                    break;
            }

        }

        public static void Draw(SpriteBatch SpriteBatch)
        {
            if (RightMenu != null || LeftMenu != null)
                DrawTransparentBlackBG(SpriteBatch);

            if (RightMenu != null)
                RightMenu.Draw(SpriteBatch);

            if (LeftMenu != null)
                LeftMenu.Draw(SpriteBatch);

        }

        public static void DrawTransparentBlackBG(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(BlackBG, BlackBGRectangle, Color.White);
        }

    }

    public enum MenuType
    {
        TitleScreenMenu,
        LevelSelectMenu,
        OptionsMenu,
        IngameMenu,
    }

    public enum MenuSideType
    {
        Left,
        Right,
        Both,
    }
}
