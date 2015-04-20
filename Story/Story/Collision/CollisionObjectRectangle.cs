using Microsoft.Xna.Framework;
using System;

namespace Story
{
    class CollisionObjectRectangle : CollisionObject
    {

        public CollisionObjectRectangle(Vector2 StartCoords)
            : base()
        {
            Verticies = new Vector2[4];
            for (int i = 0; i < Verticies.Length; i++)
                Verticies[i] = StartCoords;
            SelectedVertex = 0;
            SetRectangleBounds();
        }

        public CollisionObjectRectangle(Vector2 StartCoords, Vector2 Size, CollisionObjectTypeEnum CollisionObjectType)
        {
            this.CollisionObjectType = CollisionObjectType;
            Verticies = new Vector2[4];
            for (int i = 0; i < Verticies.Length; i++)
                Verticies[i] = StartCoords;
            SelectedVertex = 0;
            Resize(StartCoords + Size);
        }

        public CollisionObjectRectangle(Vector2[] Verticies, CollisionObjectTypeEnum CollisionObjectType)
        {
            this.CollisionObjectType = CollisionObjectType;
            this.Verticies = Verticies;
            this.CollisionObjectType = CollisionObjectType;
            SetRectangleBounds();
            SelectedVertex = -1;
        }

        public override void Resize(Vector2 NewPosition)
        {
            base.Resize(NewPosition);

            if (SelectedVertex < 0 || SelectedVertex > Verticies.Length)
            {
                return;
            }

            // Update horizontal
            Verticies[GetHorizontalNode(SelectedVertex)].Y = Verticies[SelectedVertex].Y;

            // Update vertical
            Verticies[GetVerticalNode(SelectedVertex)].X = Verticies[SelectedVertex].X;

            // Update diagonal
            Verticies[GetVerticalNode(GetHorizontalNode(SelectedVertex))].X = Verticies[GetHorizontalNode(SelectedVertex)].X;
            Verticies[GetVerticalNode(GetHorizontalNode(SelectedVertex))].Y = Verticies[GetVerticalNode(SelectedVertex)].Y;

            SetRectangleBounds();
        }

        private Vector2 EndVector = Vector2.Zero;
        public void UpdatePosition(Vector2 Position, int Width, int Height)
        {
            for (int i = 0; i < Verticies.Length; i++)
                Verticies[i] = Position;

            CollisionRect.X = (int)Position.X;
            CollisionRect.Y = (int)Position.Y;
            EndVector.X = CollisionRect.X + Width;
            EndVector.Y = CollisionRect.Y + Height;
            Resize(EndVector);
        }

        private int GetHorizontalNode(int AtIndex)
        {
            switch (AtIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 0;
                case 2:
                    return 3;
                case 3:
                    return 2;
                default:
                    throw new Exception("Invalid vertex index");
            }

        }

        private int GetVerticalNode(int AtIndex)
        {
            switch (AtIndex)
            {
                case 0:
                    return 3;
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 0;
                default:
                    throw new Exception("Invalid vertex index");
            }
        }

        public override bool TooSmall()
        {
            float v1 = Vector2.Distance(Verticies[0], Verticies[GetHorizontalNode(0)]);
            float v2 = Vector2.Distance(Verticies[0], Verticies[GetVerticalNode(0)]);
            if (v1 < 16.0f ||
                v2 < 16.0f)
                return true;
            else
                return false;
        }
    }
}
