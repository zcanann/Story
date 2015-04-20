using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class CollisionObjectTriangle : CollisionObject
    {
        private const int Right = 1;

        public CollisionObjectTriangle(Vector2 StartCoords)
            : base()
        {
            Verticies = new Vector2[3];
            for (int i = 0; i < Verticies.Length; i++)
                Verticies[i] = StartCoords;
            SelectedVertex = 2;
            SetRectangleBounds();
        }

        public CollisionObjectTriangle(Vector2[] Verticies, CollisionObjectTypeEnum CollisionObjectType)
        {
            this.Verticies = Verticies;
            this.CollisionObjectType = CollisionObjectType;
            SetRectangleBounds();
        }

        public override float CalculateSlope()
        {
            // Our focus will be on verticies 0 and 2 since 1 is the right angle and thus irrelevant
            float Slope = (Verticies[2].Y - Verticies[0].Y) / (Verticies[2].X - Verticies[0].X);
            return Slope;
        }

        public override float CalculateSlopeAt(float X, float Width)
        {
            if (CalculateSlope() < 0.0f)
                X += Width;

            if (X < CollisionRect.Left || X > CollisionRect.Right)
                return 0;
            else
                return CalculateSlope();
        }

        public override float CalculateYIntercept()
        {
            float YIntercept = Verticies[0].Y - (CalculateSlope() * Verticies[0].X);
            return YIntercept;
        }

        public override void Resize(Vector2 NewPosition)
        {
            base.Resize(NewPosition);

            if (SelectedVertex < 0 || SelectedVertex > Verticies.Length)
            {
                return;
            }

            // Easiest case, adjusting corner (where right angle is formed)
            if (SelectedVertex == 1)
            {
                // One will always adjust horizontally, the other will adjust vertically
                if (Verticies[0].Y < Verticies[2].Y)
                {
                    Verticies[0].X = Verticies[Right].X;
                    Verticies[2].Y = Verticies[Right].Y;
                }
                else
                {
                    Verticies[2].X = Verticies[Right].X;
                    Verticies[0].Y = Verticies[Right].Y;
                }
                return;
            }

            int Pivot = 0;

            if (SelectedVertex == 0)
                Pivot = 2;
            else if (SelectedVertex == 2)
                Pivot = 0;
            else
                return;

            // Adjust triangles such that the slope is always exposed upwards
            Verticies[Right].X = Verticies[SelectedVertex].X;
            Verticies[Right].Y = Verticies[Pivot].Y;

            if (Verticies[SelectedVertex].Y > Verticies[Pivot].Y)
            {
                Verticies[Right].X = Verticies[Pivot].X;
                Verticies[Right].Y = Verticies[SelectedVertex].Y;
            }
        }

        public override bool TooSmall()
        {
            // Filter out upside down triangles too
            if (Verticies[1].Y < Verticies[0].Y)
                return true;

            float v1 = Vector2.Distance(Verticies[1], Verticies[2]);
            float v2 = Vector2.Distance(Verticies[0], Verticies[1]);
            if (v1 < 16.0f ||
                v2 < 16.0f)
                return true;
            else
                return false;
        }
    }
}
