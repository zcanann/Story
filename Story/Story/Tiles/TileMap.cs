using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Story
{
    class TileMap
    {
        public Tile[,] Tiles;

        public int Width;
        public int Height;

        public TileMap()
        {

        }

        public void Resize(int NewWidth, int NewHeight)
        {
            Tile[,] OldTiles = Tiles;
            Tile EmptyTile = Tile.LoadTile(EnvironmentEnum.Grass, (byte)TileTypesEnum.None);

            if (NewWidth < 0)
                NewWidth = 0;
            if (NewHeight < 0)
                NewHeight = 0;

            Tiles = new Tile[NewWidth, NewHeight];

            for (int Y = 0; Y < NewHeight; Y++)
            {
                for (int X = 0; X < NewWidth; X++)
                {
                    // Copy old tile if in bounds, otherwise place an empty tile
                    if (X < Width && Y < Height)
                        Tiles[X, Y] = OldTiles[X, Y];
                    else
                        Tiles[X, Y] = EmptyTile;
                }
            }

            this.Width = NewWidth;
            this.Height = NewHeight;
        }

        public void ReloadTiles(EnvironmentEnum TileEnvironment)
        {
            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    Tiles[X, Y] = Tile.LoadTile(TileEnvironment, (byte)Tiles[X, Y].TileType);
                }
            }
        }

        public void LoadTiles(EnvironmentEnum TileEnvironment, byte[,] TileIDs)
        {
            this.Width = TileIDs.GetLength(0);
            this.Height = TileIDs.GetLength(1);
            Tiles = new Tile[Width, Height];

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    Tiles[X, Y] = Tile.LoadTile(TileEnvironment, TileIDs[X, Y]);
                }
            }
        }

        /*public void LoadTiles(EnvironmentEnum TileEnvironment, string Path)
        {
            // Load the level and ensure all of the lines are the same length.
            int Width, Height;
            List<string> Lines = new List<string>();
            using (StreamReader StreamReader = new StreamReader(Path))
            {
                string Line = StreamReader.ReadLine();
                Width = Line.Length;
                while (Line != null)
                {
                    Lines.Add(Line);
                    if (Line.Length != Width)
                        throw new Exception(String.Format("Line {0} is a different length than the others.", Lines.Count));
                    Line = StreamReader.ReadLine();
                }
                Height = Lines.Count();
            }

            // Allocate the tile grid.
            Resize(Width, Height);
            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char TileType = Lines[y][x];
                    Tiles[x, y] = Tile.LoadTile(TileEnvironment, TileType, x, y);
                }
            }

        }*/

        private int GetCenterXTile()
        {
            int CenterX = (int)((Camera.CameraPosition.X + Game.HalfScreenSize.X) / Tile.Width);

            //Correct bounds
            if (CenterX < 0)
                CenterX = 0;

            if (CenterX > Width - 1)
                CenterX = Width - 1;

            return CenterX;
        }

        private int GetCenterYTile()
        {
            int CenterY = (int)((Camera.CameraPosition.Y + Game.HalfScreenSize.Y) / Tile.Height);

            if (CenterY < 0)
                CenterY = 0;
            if (CenterY > Height - 1)
                CenterY = Height - 1;

            return CenterY;
        }

        public void UpdateTileAtCameraCenter(EnvironmentEnum EnvironmentType, TileTypesEnum TileType)
        {
            int CenterX = GetCenterXTile();
            int CenterY = GetCenterYTile();

            if (CenterX < 0 || CenterX >= Width)
                return;
            if (CenterY < 0 || CenterY >= Height)
                return;

            Tiles[CenterX, CenterY] = Tile.LoadTile(EnvironmentType, (byte)TileType);
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch, bool DrawTransparent = false)
        {
            int StartX = (int)((Camera.CameraPosition.X) / Tile.Width);
            int StartY = (int)((Camera.CameraPosition.Y) / Tile.Height);
            int EndX = (int)((Camera.CameraPosition.X + Game.BackBufferWidth) / Tile.Width) + 1;
            int EndY = (int)((Camera.CameraPosition.Y + Game.BackBufferHeight) / Tile.Height) + 1;

            if (Game.VisualizeFrustumCulling)
            {
                StartX++;
                StartY++;
                EndX--;
                EndY--;
            }

            //Correct bounds
            if (StartX < 0)
                StartX = 0;
            if (StartY < 0)
                StartY = 0;
            if (StartX > Width)
                StartX = Width;
            if (StartY > Height)
                StartY = Height;
            if (EndX > Width)
                EndX = Width;
            if (EndY > Height)
                EndY = Height;

            // For each tile position
            for (int y = StartY; y < EndY; y++)
            {
                for (int x = StartX; x < EndX; x++)
                {
                    // If the tile exists, draw it
                    Texture2D Texture = Tiles[x, y].Texture;
                    if (Texture == null)
                        continue;

                    Vector2 Position = new Vector2(x * Tile.Size.X - Camera.CameraPosition.X, y * Tile.Size.Y - Camera.CameraPosition.Y);

                    int Opacity = 255;
                    if (DrawTransparent)
                        Opacity = 64;

                    SpriteBatch.Draw(Texture, Position, Color.FromNonPremultiplied(255, 255, 255, Opacity));
                } // End for x

            } // End for y
        }

        public void DrawSelectedCenteredTile(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            int CenterX = GetCenterXTile();
            int CenterY = GetCenterYTile();

            if (CenterX < 0 || CenterX >= Width)
                return;
            if (CenterY < 0 || CenterY >= Height)
                return;

            Texture2D Texture = Tiles[CenterX, CenterY].Texture;

            // If the tile exists, draw it
            if (Texture != null)
            {
                Vector2 Position = new Vector2(CenterX * Tile.Size.X - Camera.CameraPosition.X,
                    CenterY * Tile.Size.Y - Camera.CameraPosition.Y);

                SpriteBatch.Draw(Texture, Position, Color.FromNonPremultiplied(127, 127, 127, 127));
            }
        }
    }

    enum LayerDepthEnum
    {
        None,
        BackGround,
        Base,
        ForeGround
    }

    enum TileTypesEnum
    {
        None,
        Invisible,
        BridgeL,
        BridgeM,
        BridgeR,
        TLRuins,
        TMRuins,
        TRRuins,
        MLRuins,
        MMRuins,
        MRRuins,
        BLRuins,
        BMRuins,
        BRRuins,
        RR1Ruins,
        RR2Ruins,
        RR3Ruins,
        RL1Ruins,
        RL2Ruins,
        RL3Ruins,
        SEASONAL_TL,
        SEASONAL_TM,
        SEASONAL_TR,
        SEASONAL_ML,
        SEASONAL_MM,
        SEASONAL_MR,
        SEASONAL_BL,
        SEASONAL_BM,
        SEASONAL_BR,
        SEASONAL_RR1,
        SEASONAL_RR2,
        SEASONAL_RR3,
        SEASONAL_RL1,
        SEASONAL_RL2,
        SEASONAL_RL3
    }

}
