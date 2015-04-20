using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    struct Tile
    {
        // Jagged array to store each tile, with the 2nd dimension storing season/environment tiles
        static Texture2D[][] TileTypes = new Texture2D[Enum.GetNames(typeof(TileTypesEnum)).Length][];

        public const int Width = 64;
        public const int Height = 48;
        public const float VerticalScale = 1.5f;
        public static readonly Vector2 Size = new Vector2(Width, Height);
        public static readonly Vector2 SizeOver2 = new Vector2(Width / 2, Height / 2);

        public Texture2D Texture;
        public bool Transparent;

        public TileTypesEnum TileType;

        public Tile(Texture2D Texture, bool Transparent, TileTypesEnum TileType)
        {
            this.Texture = Texture;
            this.Transparent = Transparent;
            this.TileType = TileType;
        }

        public static void LoadContent(ContentManager Content)
        {
            string TileName;
            string EnvironmentName;

            for (int TileIndex = 0; TileIndex < Enum.GetNames(typeof(TileTypesEnum)).Length; TileIndex++)
            {
                TileName = ((TileTypesEnum)TileIndex).ToString();

                // Check for seasonal flags
                if (TileName.StartsWith("SEASONAL_"))
                {
                    TileName = TileName.Replace("SEASONAL_", "");
                    // Allocate space for a tile of each season type
                    TileTypes[TileIndex] = new Texture2D[Enum.GetNames(typeof(EnvironmentEnum)).Length];

                    // Load in the tile for each season
                    for (int EnvironmentIndex = 0; EnvironmentIndex < Enum.GetNames(typeof(EnvironmentEnum)).Length; EnvironmentIndex++)
                    {
                        EnvironmentName = ((EnvironmentEnum)EnvironmentIndex).ToString();
                        TileTypes[TileIndex][EnvironmentIndex] = Content.Load<Texture2D>("Tiles/" + EnvironmentName + "/" + TileName + EnvironmentName);

                    }
                }
                // Not a seasonal tile, load it as normal
                else
                {
                    TileTypes[TileIndex] = new Texture2D[1];
                    if ((TileTypesEnum)(TileIndex) != TileTypesEnum.None)
                        TileTypes[TileIndex][0] = Content.Load<Texture2D>("Tiles/" + TileName);
                }
            }

        }

        public static Tile LoadTile(EnvironmentEnum TileEnvironment, byte TileChar)
        {
            TileTypesEnum TileType = (TileTypesEnum)TileChar;

            if (TileType.ToString().StartsWith("SEASONAL_"))
            {
                return LoadSeasonalTile(TileEnvironment, TileType);
            }
            else
            {
                return LoadTile(TileType);
            }
        }

        /*public static Tile LoadTile(EnvironmentEnum TileEnvironment, char TileType, int x, int y)
        {
            switch (TileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, false, (TileTypesEnum)TileTypesEnum.None);

                // Also a blank space when not in test mode
                case 'M':
                    if (!Game.DebugMode)
                        return new Tile(null, false, (TileTypesEnum)TileTypesEnum.None);
                    else
                        return LoadTile(TileTypesEnum.Invisible);

                //Standard tiles
                case '#':
                    return LoadTile(TileTypesEnum.BridgeL);
                case '~':
                    return LoadTile(TileTypesEnum.BridgeM);
                case '+':
                    return LoadTile(TileTypesEnum.BridgeR);

                case '9':
                    return LoadTile(TileTypesEnum.TLRuins);
                case '8':
                    return LoadTile(TileTypesEnum.TMRuins);
                case '7':
                    return LoadTile(TileTypesEnum.TRRuins);
                case '6':
                    return LoadTile(TileTypesEnum.MLRuins);
                case '5':
                    return LoadTile(TileTypesEnum.MMRuins);
                case '4':
                    return LoadTile(TileTypesEnum.MRRuins);
                case '3':
                    return LoadTile(TileTypesEnum.BLRuins);
                case '2':
                    return LoadTile(TileTypesEnum.BMRuins);
                case '1':
                    return LoadTile(TileTypesEnum.BRRuins);
                case 'q':
                    return LoadTile(TileTypesEnum.RR1Ruins);
                case 'w':
                    return LoadTile(TileTypesEnum.RR2Ruins);
                case 'y':
                    return LoadTile(TileTypesEnum.RR3Ruins);
                case 'e':
                    return LoadTile(TileTypesEnum.RL1Ruins);
                case 'r':
                    return LoadTile(TileTypesEnum.RL2Ruins);
                case 't':
                    return LoadTile(TileTypesEnum.RL3Ruins);

                //Terrain tiles
                case '(':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_TL);
                case '-':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_TM);
                case ')':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_TR);

                case '{':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_ML);
                case ',':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_MM);
                case '}':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_MR);

                case '[':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_BL);
                case '_':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_BM);
                case ']':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_BR);

                case '>':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RR1);
                case '/':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RR2);
                case ':':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RR3);

                case '<':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RL1);

                case '\\':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RL2);
                case ';':
                    return LoadSeasonalTile(TileEnvironment, TileTypesEnum.SEASONAL_RL3);

                // Unknown tile type character
                default:
                    return new Tile(null, false, (TileTypesEnum)TileTypesEnum.None);
                //throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", TileType, x, y));
            }
        }*/

        public static Tile LoadSeasonalTile(EnvironmentEnum TileEnvironment, TileTypesEnum TileType, bool Transparent = false)
        {
            if (TileEnvironment == EnvironmentEnum.Snow)
                Transparent = true;

            return new Tile(TileTypes[(int)TileType][(int)TileEnvironment], Transparent, (TileTypesEnum)TileType);
        }

        public static Tile LoadTile(TileTypesEnum TileType, bool Transparent = false)
        {
            return new Tile(TileTypes[(int)TileType][0], Transparent, (TileTypesEnum)TileType);
        }

    }
}
