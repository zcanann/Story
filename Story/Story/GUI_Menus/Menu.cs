using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class Menu
    {
        protected MenuSideType MenuType;

        public delegate void MenuDelegate();

        public bool ReadyToExit = false;    // Flag indicating if menu is ready to be deleted

        public Menu(MenuSideType MenuSideType)
        {
            this.MenuType = MenuSideType;
        }

        public MenuSideType GetMenuType()
        {
            return MenuType;
        }

        // Calls the given callback and sets the flag indicating this menu is ready to be deleted
        protected void CallAndPrepareExit(MenuDelegate CallBackFunction)
        {
            CallBackFunction();
            ReadyToExit = true;
        }

        //Simply here to be overwritten
        public virtual void Open() { ReadyToExit = false; }
        //public virtual void InitializeCallBacks(params MenuDelegate args) { }
        public virtual void LoadContent(ContentManager Content) { }
        public virtual void Update(GameTime GameTime) { }
        public virtual void Draw(SpriteBatch SpriteBatch) { }

    }
}
