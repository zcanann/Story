using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class CollectableEgg : Collectable
    {
        static Texture2D EggTexture;

        public CollectableEgg(Vector2 Position) : base(Position)
        {
            this.Texture = EggTexture;
            CollisionBox = new CollisionObjectRectangle(Position, new Vector2(Texture.Width, Texture.Height), CollisionObjectTypeEnum.Collectable);
        }

        public static void LoadContent(ContentManager Content)
        {
            EggTexture = Content.Load<Texture2D>("Sprites/Collectables/Egg");
        }

    }
}
