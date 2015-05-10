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
    class Level
    {
        protected TileMap BackGroundTiles = new TileMap();
        protected TileMap GroundTiles = new TileMap();
        protected TileMap ForeGroundTiles = new TileMap();

        protected Player Player = new Player(Vector2.Zero);
        protected List<CheckPoint> CheckPoints = new List<CheckPoint>();
        protected List<CollisionObject> CollisionObjects = new List<CollisionObject>();
        protected List<WorldObject> WorldObjects = new List<WorldObject>();
        protected List<CollectableEgg> CollectableEggs = new List<CollectableEgg>();
        protected List<Enemy> Enemies = new List<Enemy>();

        private ParticleEmitter EnvironmentEmitterBG;
        private ParticleEmitter EnvironmentEmitterFG;

        protected CollectableFruit[] CollectableFruits = new CollectableFruit[3];
        protected bool AppleCollected = false;
        protected bool BananaCollected = false;
        protected bool OrangeCollected = false;
        protected bool LevelCompleted = false;

        protected LevelExit LevelExit = new LevelExit(Vector2.Zero);

        protected int WaterLine = 0;
        protected EnvironmentEnum CurrentEnvironment = EnvironmentEnum.Grass;
        protected LiquidType CurrentLiquidType = LiquidType.Water;

        protected int LevelID;

        public Level()
        {
            for (int Index = 0; Index < CollectableFruits.Length; Index++)
                CollectableFruits[Index] = new CollectableFruit(Vector2.Zero, (Story.CollectableFruit.FruitTypeEnum)Index);
        }

        public void LoadLevel(int LevelID)
        {
            this.LevelID = LevelID;

            bool FoundLevelData = false;
            int LevelDataIndex = -1;

            for (int Index = 0; Index < SaveGame.LevelSaveData.Count; Index++)
            {
                if (SaveGame.LevelSaveData[Index].LevelID != LevelID)
                    continue;

                LevelDataIndex = Index;
                FoundLevelData = true;
            }

            if (!FoundLevelData)
            {
                return;
            }

            LevelData LevelData = SaveGame.LevelSaveData[LevelDataIndex];

            // Reconstruct level from the save data
            Player = new Player(LevelData.PlayerSpawn);
            CurrentEnvironment = LevelData.Environment;
            CollectableFruits[0] = new CollectableFruit(LevelData.AppleSpawn, CollectableFruit.FruitTypeEnum.Apple);
            CollectableFruits[1] = new CollectableFruit(LevelData.BananaSpawn, CollectableFruit.FruitTypeEnum.Banana);
            CollectableFruits[2] = new CollectableFruit(LevelData.OrangeSpawn, CollectableFruit.FruitTypeEnum.Orange);

            for (int Index = 0; Index < LevelData.CheckPointSpawns.Count; Index++)
            {
                CheckPoints.Add(new CheckPoint(LevelData.CheckPointSpawns[Index]));
            }

            for (int Index = 0; Index < LevelData.CollisionObjects.Count; Index++)
            {
                if (LevelData.CollisionObjects[Index].Item1.Length == 3)
                    CollisionObjects.Add(new CollisionObjectTriangle(LevelData.CollisionObjects[Index].Item1, LevelData.CollisionObjects[Index].Item2));
                else if (LevelData.CollisionObjects[Index].Item1.Length == 4)
                    CollisionObjects.Add(new CollisionObjectRectangle(LevelData.CollisionObjects[Index].Item1, LevelData.CollisionObjects[Index].Item2));
                else
                    throw new Exception("Invalid vertice count on loaded collision object");
            }

            for (int Index = 0; Index < LevelData.EggSpawns.Count; Index++)
            {
                CollectableEggs.Add(new CollectableEgg(LevelData.EggSpawns[Index]));
            }

            for (int Index = 0; Index < LevelData.EnemySpawns.Count; Index++)
            {
                Enemies.Add(new Enemy(LevelData.EnemySpawns[Index].Item1, LevelData.EnemySpawns[Index].Item2));
            }

            for (int Index = 0; Index < LevelData.WorldObjects.Count; Index++)
            {
                WorldObjects.Add(new WorldObject(LevelData.WorldObjects[Index]));
            }

            BackGroundTiles.LoadTiles(CurrentEnvironment, LevelData.TilesBackGround);
            GroundTiles.LoadTiles(CurrentEnvironment, LevelData.GroundTiles);
            ForeGroundTiles.LoadTiles(CurrentEnvironment, LevelData.TilesForeGround);

            LevelExit = new LevelExit(LevelData.ExitSpawn);

            /*
            string LevelPath = String.Format("Content/Levels/{0}.txt", LevelID);

            BackGroundTiles.LoadTiles(CurrentEnvironment, LevelPath);
            GroundTiles.LoadTiles(CurrentEnvironment, LevelPath);
            ForeGroundTiles.LoadTiles(CurrentEnvironment, LevelPath);*/

            switch (CurrentEnvironment)
            {
                case EnvironmentEnum.Grass:
                    EnvironmentEmitterBG = new RainEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    EnvironmentEmitterFG = new RainEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    break;
                case EnvironmentEnum.Snow:
                    EnvironmentEmitterBG = new SnowEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    EnvironmentEmitterFG = new SnowEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    break;
                case EnvironmentEnum.Desert:
                    EnvironmentEmitterBG = new SandEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    EnvironmentEmitterFG = new SandEmitter(new Vector2(0, -24),
                       new Color(255, 255, 255, 255), new Vector2(-512, -32), new Vector2(Game.BackBufferWidth + 512, Game.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                       new Vector2(-512, 0), new Vector2(Game.BackBufferWidth + 512, 0));
                    break;
            }

            EnvironmentEmitterBG.StartEffect();
            EnvironmentEmitterFG.StartEffect();
        }

        protected LevelData GenerateLevelData()
        {
            LevelData LevelSaveData = new LevelData();

            LevelSaveData.LevelID = this.LevelID;
            LevelSaveData.PlayerSpawn = Player.Position;

            LevelSaveData.Environment = CurrentEnvironment;

            for (int Index = 0; Index < CollisionObjects.Count; Index++)
            {
                LevelSaveData.CollisionObjects.Add(new Tuple<Vector2[], CollisionObjectTypeEnum>(CollisionObjects[Index].Verticies, CollisionObjects[Index].CollisionObjectType));
            }

            for (int Index = 0; Index < Enemies.Count; Index++)
            {
                LevelSaveData.EnemySpawns.Add(new Tuple<Vector2, Enemy.EnemyTypeEnum>(Enemies[Index].Position, Enemies[Index].EnemyType));
            }

            for (int Index = 0; Index < CheckPoints.Count; Index++)
            {
                LevelSaveData.CheckPointSpawns.Add(CheckPoints[Index].Position);
            }

            for (int Index = 0; Index < CollectableEggs.Count; Index++)
            {
                LevelSaveData.EggSpawns.Add(CollectableEggs[Index].Position);
            }

            LevelSaveData.AppleSpawn = CollectableFruits[0].Position;
            LevelSaveData.BananaSpawn = CollectableFruits[1].Position;
            LevelSaveData.OrangeSpawn = CollectableFruits[2].Position;

            LevelSaveData.TilesBackGround = new byte[BackGroundTiles.Width, BackGroundTiles.Height];
            LevelSaveData.GroundTiles = new byte[GroundTiles.Width, GroundTiles.Height];
            LevelSaveData.TilesForeGround = new byte[ForeGroundTiles.Width, ForeGroundTiles.Height];

            for (int Y = 0; Y < GroundTiles.Height; Y++)
            {
                for (int X = 0; X < GroundTiles.Width; X++)
                {
                    LevelSaveData.TilesBackGround[X, Y] = (byte)BackGroundTiles.Tiles[X, Y].TileType;
                    LevelSaveData.GroundTiles[X, Y] = (byte)GroundTiles.Tiles[X, Y].TileType;
                    LevelSaveData.TilesForeGround[X, Y] = (byte)ForeGroundTiles.Tiles[X, Y].TileType;
                }
            }

            for (int Index = 0; Index < WorldObjects.Count; Index++)
            {
                LevelSaveData.WorldObjects.Add(WorldObjects[Index].SaveableFeatures);
            }

            LevelSaveData.ExitSpawn = LevelExit.Position;

            return LevelSaveData;
        }

        public static void LoadContent(ContentManager Content)
        {
            Tile.LoadContent(Content);
            CheckPoint.LoadContent(Content);
            LevelExit.LoadContent(Content);
            CollectableEgg.LoadContent(Content);
            CollectableFruit.LoadContent(Content);
            Enemy.LoadContent(Content);
            Projectile.LoadContent(Content);

            BreathEmitter.LoadContent(Content);
            CloudEmitter.LoadContent(Content);
            DeathEmitter.LoadContent(Content);
            HitEmitter.LoadContent(Content);
            MindPowerEmitter.LoadContent(Content);
            RainEmitter.LoadContent(Content);
            SandEmitter.LoadContent(Content);
            SnowEmitter.LoadContent(Content);

            SaveGame.LoadContent();
        }

        public void ResizeTileMap()
        {

        }

        public virtual void Update(GameTime GameTime)
        {
            if (InputManager.CheckJustPressed(InputManager.MenuKeys, InputManager.MenuButtons))
                MenuManager.OpenIngameMenu(true);

            Player.Update(GameTime, CollisionObjects);

            // Update enemies
            for (int EnemyIndex = 0; EnemyIndex < Enemies.Count; EnemyIndex++)
            {
                Enemies[EnemyIndex].Update(GameTime, CollisionObjects);
                if (!Enemies[EnemyIndex].IsAlive && Enemies[EnemyIndex].DeathTimer > Enemy.DeathTimerMax)
                {
                    Enemies.RemoveAt(EnemyIndex);
                }
            }

            for (int Index = 0; Index < CollectableEggs.Count; Index++)
            {
                CollectableEggs[Index].Update(GameTime);
            }

            for (int Index = 0; Index < CollectableFruits.Length; Index++)
            {
                CollectableFruits[Index].Update(GameTime);
            }

            EnvironmentEmitterBG.StartPosition = Camera.CameraPosition;
            EnvironmentEmitterFG.StartPosition = Camera.CameraPosition;
            EnvironmentEmitterBG.Update(GameTime);
            EnvironmentEmitterFG.Update(GameTime);

            Player.TestCollisionsVsEnemies(Enemies);
            Player.TestCollisionVsEggs(CollectableEggs);
            Player.TestCollisionVsCheckPoints(CheckPoints);

            for (int Index = 0; Index < CollectableFruits.Length; Index++)
            {
                if (Player.TestCollisionVsCollectable(CollectableFruits[Index]))
                {
                    CollectableFruits[Index].Collected = true;
                    switch (Index)
                    {
                        case 0:
                            AppleCollected = true;
                            break;
                        case 1:
                            BananaCollected = true;
                            break;
                        case 2:
                            OrangeCollected = true;
                            break;
                        default:
                            throw new Exception("This shouldn't be possible. Class Level.");
                    }
                }
            }

            if (Player.TestCollisionVsExit(LevelExit))
            {
                SaveGame.SaveLevelCompletion(LevelID, AppleCollected, BananaCollected, OrangeCollected);

                if (LevelID == 19)
                    MenuManager.OpenCreditsMenu(true);
                else
                    MenuManager.OpenLevelSelectMenu(true);

            }
        }

        // Note that due to the features of the level editor, this is fully overriden by the class
        public virtual void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            BackGroundTiles.Draw(GameTime, SpriteBatch);
            EnvironmentEmitterBG.Draw(GameTime, SpriteBatch);
            GroundTiles.Draw(GameTime, SpriteBatch);

            if (Game.DebugMode)
            {
                for (int Index = 0; Index < CollisionObjects.Count; Index++)
                {
                    CollisionObjects[Index].Draw(SpriteBatch, Camera.CameraPosition);
                }
            }

            LevelExit.Draw(SpriteBatch);

            for (int Index = 0; Index < CheckPoints.Count; Index++)
            {
                CheckPoints[Index].Draw(SpriteBatch);
            }

            for (int Index = 0; Index < CollectableEggs.Count; Index++)
            {
                CollectableEggs[Index].Draw(SpriteBatch);
            }

            for (int Index = 0; Index < CollectableFruits.Length; Index++)
            {
                CollectableFruits[Index].Draw(SpriteBatch);
            }

            for (int Index = 0; Index < Enemies.Count; Index++)
            {
                Enemies[Index].Draw(GameTime, SpriteBatch);
            }

            Player.Draw(GameTime, SpriteBatch);

            EnvironmentEmitterFG.Draw(GameTime, SpriteBatch);

            ForeGroundTiles.Draw(GameTime, SpriteBatch);

        }

        public enum LiquidType
        {
            Water,
            Lava,
        }
    }

    enum EnvironmentEnum
    {
        Grass,
        Snow,
        Desert,
    }
}
