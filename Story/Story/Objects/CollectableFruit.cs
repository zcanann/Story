using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class CollectableFruit : Collectable
    {
        private static Texture2D[] FruitTextures = new Texture2D[3];

        public enum FruitTypeEnum
        {
            Apple,
            Banana,
            Orange,
        }

        public CollectableFruit(Vector2 Position, FruitTypeEnum FruitType)
            : base(Position)
        {
            this.Texture = FruitTextures[(int)FruitType];
            CollisionBox = new CollisionObjectRectangle(Position, new Vector2(Texture.Width, Texture.Height), CollisionObjectTypeEnum.Collectable);
        }

        public static void LoadContent(ContentManager Content)
        {
            FruitTextures[0] = Content.Load<Texture2D>("Objects/Collectables/Apple");
            FruitTextures[1] = Content.Load<Texture2D>("Objects/Collectables/Banana");
            FruitTextures[2] = Content.Load<Texture2D>("Objects/Collectables/Orange");
        }


    }
}
