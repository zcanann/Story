using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class WorldObject
    {
        private static Texture2D[] ObjectTextures;

        private Texture2D Texture;
        public LayerDepthEnum LayerDepth;

        // Saveable features that define this object
        public WorldObjectFeatures SaveableFeatures;
        public Color DrawColor = Color.White;

        private float Slope;

        public WorldObject(WorldObjectTypeEnum WorldObjectType, Vector2 Position)
        {
            this.SaveableFeatures.WorldObjectType = WorldObjectType;
            this.SaveableFeatures.Position = Position;
            this.SaveableFeatures.Start = Position;
        }

        public WorldObject(WorldObjectFeatures WorldObjectFeatures)
        {
            this.SaveableFeatures = WorldObjectFeatures;
        }

        public static void LoadContent(ContentManager Content)
        {

        }

        public void UpdatePathing(Vector2 Start, Vector2 Destination, Vector2 Velocity)
        {
            this.SaveableFeatures.Start = Start;
            this.SaveableFeatures.Destination = Destination;
            this.SaveableFeatures.Velocity = Velocity;

            // Solve for the slope of the moving object
            Slope = (Destination.Y - Start.Y) / (Destination.X - Start.X);
        }

        public void Update(GameTime GameTime)
        {
            // TODO: Figure out the math involved here
            Vector2 Movement = SaveableFeatures.Start * GameTime.TotalGameTime.Milliseconds * Slope;
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(Texture, SaveableFeatures.Position - Camera.CameraPosition, DrawColor);
        }

        
    }

    [Serializable]
    struct WorldObjectFeatures
    {
        public WorldObjectTypeEnum WorldObjectType;
        public Vector2 Position;
        public Vector2 Start;
        public Vector2 Destination;
        public Vector2 Velocity;

        public WorldObjectFeatures(WorldObjectTypeEnum WorldObjectType, Vector2 Position, Vector2 Start, Vector2 Destination, Vector2 Velocity)
        {
            this.WorldObjectType = WorldObjectType;
            this.Position = Position;
            this.Start = Start;
            this.Destination = Destination;
            this.Velocity = Velocity;
        }

    }

    enum WorldObjectTypeEnum
    {
        None,
        Some,
        Test,
        Tho,
    }
}

