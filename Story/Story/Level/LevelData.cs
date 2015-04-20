using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    [Serializable]
    class LevelData
    {
        public int LevelID = 0;

        public byte[,] TilesBackGround;
        public byte[,] GroundTiles;
        public byte[,] TilesForeGround;

        public Vector2 PlayerSpawn = Vector2.Zero;
        public EnvironmentEnum Environment = EnvironmentEnum.Grass;
        public List<Vector2> EggSpawns = new List<Vector2>();
        public List<Tuple<Vector2, Enemy.EnemyTypeEnum>> EnemySpawns = new List<Tuple<Vector2, Enemy.EnemyTypeEnum>>();
        public List<Vector2> CheckPointSpawns = new List<Vector2>();
        public List<Tuple<Vector2[], CollisionObjectTypeEnum>> CollisionObjects = new List<Tuple<Vector2[], CollisionObjectTypeEnum>>();
        public List<WorldObjectFeatures> WorldObjects = new List<WorldObjectFeatures>();

        public Vector2 AppleSpawn = Vector2.Zero;
        public Vector2 BananaSpawn = Vector2.Zero;
        public Vector2 OrangeSpawn = Vector2.Zero;

        public Vector2 ExitSpawn = Vector2.Zero;

        public LevelData()
        {

        }
    }
}
