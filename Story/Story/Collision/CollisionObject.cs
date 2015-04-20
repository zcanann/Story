using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class CollisionObject
    {
        private LinePrimitive Line;
        public Color Color;

        private static Texture2D VertexNode;
        public static Vector2 VertexNodeRadius = Vector2.Zero;
        public bool UpdateDrawLines = false;
        public int SelectedVertex = -1;

        public CollisionObjectTypeEnum CollisionObjectType;
        public Vector2[] Verticies;

        public Rectangle CollisionRect;

        public CollisionObject()
        {
            UpdateDrawLines = true;
        }

        public static void LoadContent(ContentManager Content)
        {
            VertexNode = Content.Load<Texture2D>("Debug/VertexNode");
            VertexNodeRadius.X = VertexNodeRadius.Y = VertexNode.Width / 2;
        }

        public bool IsOnScreen()
        {
            // Check if any verticies are on screen // Turns out this is not actually a valid way to do this
            for (int Index = 0; Index < Verticies.Length; Index++)
            {


                if (Camera.IsOnScreen(Verticies[Index]))
                {
                    return true;
                }
            }
            CollisionRect.Offset(-(int)Camera.CameraPosition.X, -(int)Camera.CameraPosition.Y);
            if (Camera.CameraRectangle.Intersects(CollisionRect))
            {
                CollisionRect.Offset((int)Camera.CameraPosition.X, (int)Camera.CameraPosition.Y);
                return true;
            }
            CollisionRect.Offset((int)Camera.CameraPosition.X, (int)Camera.CameraPosition.Y);

            return false;
        }

        public virtual void Resize(Vector2 NewPosition)
        {
            if (SelectedVertex < 0 || SelectedVertex > Verticies.Length)
            {
                return;
            }

            Verticies[SelectedVertex] = NewPosition;
            UpdateDrawLines = true;
        }

        private Vector2 MinVector = new Vector2(float.MinValue, float.MinValue);
        private Vector2 MaxVector = new Vector2(float.MaxValue, float.MaxValue);

        protected void SetRectangleBounds()
        {
            Vector2 MinCoords = MaxVector;
            Vector2 MaxCoords = MinVector;

            for (int Index = 0; Index < Verticies.Length; Index++)
            {
                if (Verticies[Index].X < MinCoords.X)
                    MinCoords.X = Verticies[Index].X;

                if (Verticies[Index].Y < MinCoords.Y)
                    MinCoords.Y = Verticies[Index].Y;

                if (Verticies[Index].X > MaxCoords.X)
                    MaxCoords.X = Verticies[Index].X;

                if (Verticies[Index].Y > MaxCoords.Y)
                    MaxCoords.Y = Verticies[Index].Y;
            }

            CollisionRect.X = (int)MinCoords.X;
            CollisionRect.Width = (int)(MaxCoords.X - MinCoords.X);

            CollisionRect.Y = (int)MinCoords.Y;
            CollisionRect.Height = (int)(MaxCoords.Y - MinCoords.Y);

        }

        public virtual float CalculateSlope()
        {
            return 0.0f;
        }

        public virtual float CalculateSlopeAt(float X, float Width)
        {
            return 0.0f;
        }

        public virtual float CalculateYIntercept()
        {
            return 0.0f;
        }

        public bool TrySelectVertex(Vector2 PointCordinates)
        {
            for (int index = 0; index < Verticies.Length; index++)
            {
                if (Vector2.Distance(Verticies[index], PointCordinates) < CollisionObject.VertexNodeRadius.X)
                {
                    SelectedVertex = index;
                    return true;
                }
            }

            return false;
        }

        public virtual bool TooSmall()
        {
            return true;
        }

        private void UpdateLines(GraphicsDevice Device)
        {
            // Create lines between each verticies
            Line = new LinePrimitive(Device);

            for (int Index = 0; Index < Verticies.Length; Index++)
                Line.AddVector(Verticies[Index]);

            if (Verticies.Length > 0)
                Line.AddVector(Verticies[0]);

            UpdateDrawLines = false;
        }

        public void Draw(SpriteBatch SpriteBatch, Vector2 Offset, bool DrawVertecies = true, bool AlwaysDraw = false)
        {
            if (!AlwaysDraw && (!Game.DebugMode || !IsOnScreen()))
                return;

            if (UpdateDrawLines == true)
                UpdateLines(SpriteBatch.GraphicsDevice);

            switch (CollisionObjectType)
            {
                case CollisionObjectTypeEnum.NPCOnly:
                    Color = Color.Pink;
                    break;
                case CollisionObjectTypeEnum.Liquid:
                    Color = Color.Blue;
                    break;
                case CollisionObjectTypeEnum.Passable:
                    Color = Color.LightGreen;
                    break;
                case CollisionObjectTypeEnum.Damaging:
                    Color = Color.Gray;
                    break;
                case CollisionObjectTypeEnum.Kill:
                    Color = Color.Red;
                    break;
                case CollisionObjectTypeEnum.Entity:
                    Color = Color.Cyan;
                    break;
                case CollisionObjectTypeEnum.Hover:
                    Color = Color.Purple;
                    break;
                case CollisionObjectTypeEnum.Collectable:
                    Color = Color.Orange;
                    break;
                case CollisionObjectTypeEnum.CheckPoint:
                    Color = Color.Green;
                    break;
                case CollisionObjectTypeEnum.Exit:
                    Color = Color.Black;
                    break;
                default:
                case CollisionObjectTypeEnum.Normal:
                    Color = Color.Yellow;
                    break;
            }

            Line.Draw(SpriteBatch, Color, Offset);


            if (DrawVertecies)
            {
                for (int Index = 0; Index < Verticies.Length; Index++)
                {
                    if (Index == SelectedVertex)
                        SpriteBatch.Draw(VertexNode, Verticies[Index] - VertexNodeRadius - Offset, Color.Red);
                    else
                        SpriteBatch.Draw(VertexNode, Verticies[Index] - VertexNodeRadius - Offset, Color.RoyalBlue);
                }
            }
        }
    }

    enum CollisionObjectTypeEnum
    {
        Normal,
        NPCOnly,
        Liquid,
        Passable,
        Damaging,
        Kill,

        Entity,
        Collectable,
        Hover,
        CheckPoint,
        Exit,
    }
}
