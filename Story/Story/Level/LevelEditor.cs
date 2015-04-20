using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    class LevelEditor : Level
    {
        private static Texture2D Panel;
        private static Texture2D CrossHair;
        private static Texture2D HorizontalSelector;
        private static Texture2D PlayerTexture;
        private static Texture2D[] CollectableTextures = new Texture2D[4];
        private static Texture2D[] EnemyTextures = new Texture2D[3];
        private static Texture2D CheckPointTexture;
        private static Texture2D ExitPointTexture;

        private static Vector2 CrossHairSizeOver2;
        private static Vector2 PanelSize;
        private static Vector2 HorizontalSelectorSize;
        private static Vector2 PlayerTextureSize;
        private static Vector2 CollectableTextureSize;
        private static Vector2 EnemyTextureSize;
        private static Vector2 CheckPointTextureSize;
        private static Vector2 ExitPointTextureSize;

        private static CollisionObjectRectangle PreviewCollisionObject;

        private static SpriteFont Font;

        private Vector2 EditorCameraPosition = Vector2.Zero;
        private Vector2 CameraMovement;

        private LayerDepthEnum CurrentLayerDepth = LayerDepthEnum.None;
        private EditorModeEnum EditorMode = EditorModeEnum.Tiles;
        private CollectableTypesEnum SelectedCollectable = CollectableTypesEnum.Egg;
        private Enemy.EnemyTypeEnum SelectedEnemy = Enemy.EnemyTypeEnum.Velociraptor;

        private CollisionObject CandidateCollisionObject;

        private const int PreviewCount = 13;
        private TileTypesEnum SelectedTileType = (TileTypesEnum)0;
        private Tile[] PreviewTile = new Tile[PreviewCount];

        public LevelEditor(int LevelID = 1)
        {
            LoadLevel(LevelID);

            EditorCameraPosition = Player.Position;
            LoadPreviewTiles();

            PreviewCollisionObject = new CollisionObjectRectangle((Camera.CameraPosition + Game.ScreenSize) * (Vector2.UnitY + Vector2.UnitX / 2) - Vector2.UnitY * 96.0f - Vector2.UnitX * 32.0f);
            PreviewCollisionObject.Resize((Camera.CameraPosition + Game.ScreenSize) * (Vector2.UnitY + Vector2.UnitX / 2) - Vector2.UnitY * 32.0f + Vector2.UnitX * 32.0f);
            PreviewCollisionObject.SelectedVertex = -1;
        }

        new public static void LoadContent(ContentManager Content)
        {
            // Load assets
            CollisionObject.LoadContent(Content);
            Panel = Content.Load<Texture2D>("Editor/LevelEditPanel");
            HorizontalSelector = Content.Load<Texture2D>("Editor/HorizontalSelector");
            CrossHair = Content.Load<Texture2D>("Editor/CrossHair");
            PlayerTexture = Content.Load<Texture2D>("Sprites/Player/Idle");
            CheckPointTexture = Content.Load<Texture2D>("Objects/CheckPoint/OuthouseClosed");
            ExitPointTexture = Content.Load<Texture2D>("Objects/Exit/Exit");

            CollectableTextures[0] = Content.Load<Texture2D>("Objects/Collectables/Egg");
            CollectableTextures[1] = Content.Load<Texture2D>("Objects/Collectables/Apple");
            CollectableTextures[2] = Content.Load<Texture2D>("Objects/Collectables/Banana");
            CollectableTextures[3] = Content.Load<Texture2D>("Objects/Collectables/Orange");

            EnemyTextures[0] = Content.Load<Texture2D>("Editor/RaptorEditor");
            EnemyTextures[1] = Content.Load<Texture2D>("Editor/PterodactylEditor");
            EnemyTextures[2] = Content.Load<Texture2D>("Editor/DunkleosteusEditor");

            // Grab the biggest ones and call that our size (hacky and ghetto)
            EnemyTextureSize.X = EnemyTextures[2].Width;
            EnemyTextureSize.Y = EnemyTextures[0].Height;

            CollectableTextureSize.X = CollectableTextures[0].Width;
            CollectableTextureSize.Y = CollectableTextures[0].Height;

            Font = Content.Load<SpriteFont>("Font/GameFont");

            // Calculate sizes as vectors
            CrossHairSizeOver2.X = CrossHair.Width / 2;
            CrossHairSizeOver2.Y = CrossHair.Height / 2;

            HorizontalSelectorSize.X = HorizontalSelector.Width;
            HorizontalSelectorSize.Y = HorizontalSelector.Height;

            PlayerTextureSize.X = PlayerTexture.Width;
            PlayerTextureSize.Y = PlayerTexture.Height;

            PanelSize.X = Panel.Width;
            PanelSize.Y = Panel.Height;

            CheckPointTextureSize.X = CheckPointTexture.Width;
            CheckPointTextureSize.Y = CheckPointTexture.Height;

            ExitPointTextureSize.X = ExitPointTexture.Width;
            ExitPointTextureSize.Y = ExitPointTexture.Height;
        }

        public override void Update(GameTime GameTime)
        {
            UpdateGeneral(GameTime);

            switch (EditorMode)
            {
                case EditorModeEnum.Tiles:
                    UpdateTileEditor(GameTime);
                    break;
                case EditorModeEnum.Collision:
                    UpdateBeginCollisionObject(GameTime);
                    UpdateDragCollisionObject(GameTime);
                    UpdateReleaseCollisionObject(GameTime);
                    break;
                case EditorModeEnum.Objects:
                    break;
                case EditorModeEnum.CheckPoints:
                    UpdatePlaceCheckPoints(GameTime);
                    break;
                case EditorModeEnum.Collectables:
                    UpdatePlaceCollectables(GameTime);
                    break;
                case EditorModeEnum.ExitPoint:
                    UpdatePlaceExitPoint(GameTime);
                    break;
                case EditorModeEnum.Enemies:
                    UpdatePlaceEnemies(GameTime);
                    break;
                case EditorModeEnum.SpawnPoint:
                    UpdatePlaceSpawnPoint(GameTime);
                    break;
                case EditorModeEnum.Misc:
                    UpdateMisc(GameTime);
                    break;

            }

        }

        private void UpdateGeneral(GameTime GameTime)
        {
            float ThumbStickMovement;
            CameraMovement = Vector2.Zero;

            // Saving (Ctrl+S)
            if (InputManager.CheckInputDown(InputManager.ControlKeys, InputManager.ControlButtons, InputManager.NoInputDelay) &&
                InputManager.CheckJustPressed(InputManager.SaveKeys, InputManager.SaveButtons))
            {
                SaveGame.SaveLevelData(GenerateLevelData());
            }

            if (InputManager.CheckSecondaryDirectionDown(InputDirectionEnum.Left))
                EditorMode = (EditorModeEnum)CycleEditorMode((int)EditorMode, -1);

            if (InputManager.CheckSecondaryDirectionDown(InputDirectionEnum.Right))
                EditorMode = (EditorModeEnum)CycleEditorMode((int)EditorMode, 1);

            // Camera movement
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Left, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                CameraMovement.X += ThumbStickMovement;
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Right, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                CameraMovement.X += ThumbStickMovement;
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Up, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                CameraMovement.Y -= ThumbStickMovement;
            if (InputManager.CheckPrimaryDirectionDown(InputDirectionEnum.Down, InputManager.ThumbStickPressThreshold, InputManager.NoInputDelay, out ThumbStickMovement))
                CameraMovement.Y -= ThumbStickMovement;

            EditorCameraPosition += GameTime.ElapsedGameTime.Milliseconds * CameraMovement;
            Camera.UpdateCameraPan(EditorCameraPosition, Camera.PanWindowSize.X, Camera.PanWindowSize.Y);
        }

        private void UpdateMisc(GameTime GameTime)
        {
            if (InputManager.CheckLeftTriggerDown())
            {
                BackGroundTiles.Resize(BackGroundTiles.Width - 8, BackGroundTiles.Height);
                GroundTiles.Resize(GroundTiles.Width - 8, GroundTiles.Height);
                ForeGroundTiles.Resize(ForeGroundTiles.Width - 8, ForeGroundTiles.Height);
            }
            if (InputManager.CheckRightTriggerDown())
            {
                BackGroundTiles.Resize(BackGroundTiles.Width + 8, BackGroundTiles.Height);
                GroundTiles.Resize(GroundTiles.Width + 8, GroundTiles.Height);
                ForeGroundTiles.Resize(ForeGroundTiles.Width + 8, ForeGroundTiles.Height);
            }
            if (InputManager.CheckJustPressed(InputManager.LeftBumperKeys, InputManager.LeftBumperButtons))
            {
                BackGroundTiles.Resize(BackGroundTiles.Width, BackGroundTiles.Height - 8);
                GroundTiles.Resize(GroundTiles.Width, GroundTiles.Height - 8);
                ForeGroundTiles.Resize(ForeGroundTiles.Width, ForeGroundTiles.Height - 8);
            }
            if (InputManager.CheckJustPressed(InputManager.RightBumperKeys, InputManager.RightBumperButtons))
            {
                BackGroundTiles.Resize(BackGroundTiles.Width, BackGroundTiles.Height + 8);
                GroundTiles.Resize(GroundTiles.Width, GroundTiles.Height + 8);
                ForeGroundTiles.Resize(ForeGroundTiles.Width, ForeGroundTiles.Height + 8);
            }

            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.NoInputDelay))
            {
                CurrentEnvironment = (EnvironmentEnum)CycleEnvironmentType((int)CurrentEnvironment, 1);
                BackGroundTiles.ReloadTiles(CurrentEnvironment);
                GroundTiles.ReloadTiles(CurrentEnvironment);
                ForeGroundTiles.ReloadTiles(CurrentEnvironment);
                LoadPreviewTiles();
            }

            if (InputManager.CheckJustPressed(InputManager.DeclineKeys, InputManager.DeclineButtons, InputManager.NoInputDelay))
            {
                CurrentEnvironment = (EnvironmentEnum)CycleEnvironmentType((int)CurrentEnvironment, -1);
                BackGroundTiles.ReloadTiles(CurrentEnvironment);
                GroundTiles.ReloadTiles(CurrentEnvironment);
                ForeGroundTiles.ReloadTiles(CurrentEnvironment);
                LoadPreviewTiles();
                LoadPreviewTiles();
            }
        }

        private void UpdatePlaceSpawnPoint(GameTime GameTime)
        {
            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu))
            {
                Player = new Player(Camera.CameraPosition + Game.HalfScreenSize);
            }
        }

        private void UpdatePlaceCollectables(GameTime GameTime)
        {

            if (InputManager.CheckJustPressed(InputManager.LeftBumperKeys, InputManager.LeftBumperButtons))
            {

                SelectedCollectable = (CollectableTypesEnum)CycleSelectedCollectable((int)SelectedCollectable, -1);
            }
            if (InputManager.CheckJustPressed(InputManager.RightBumperKeys, InputManager.RightBumperButtons))
            {
                SelectedCollectable = (CollectableTypesEnum)CycleSelectedCollectable((int)SelectedCollectable, 1);
            }

            // Both placing and removing collectables share many operations
            if (InputManager.CheckInputDown(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.SmallInputDelay) ||
                InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons, InputManager.SmallInputDelay))
            {
                Vector2 PlacePosition = Camera.CameraPosition + Game.HalfScreenSize;

                PlacePosition.X = (float)Math.Round((PlacePosition.X / Tile.Size.X));
                PlacePosition.X *= Tile.Size.X;
                PlacePosition.Y = (float)Math.Round((PlacePosition.Y / Tile.Size.Y));
                PlacePosition.Y *= Tile.Size.Y;

                for (int Index = 0; Index < CollectableEggs.Count; Index++)
                {
                    if (!CollectableEggs[Index].IsOnScreen())
                        continue;

                    // Remove collectables too close to the one being placed  (arbitrary bounds)
                    if (Math.Abs(CollectableEggs[Index].Position.X - PlacePosition.X) < Tile.SizeOver2.X &&
                        Math.Abs(CollectableEggs[Index].Position.Y - PlacePosition.Y) < Tile.SizeOver2.Y)
                    {
                        CollectableEggs.RemoveAt(Index);
                        break;
                    }
                }

                // Don't allow overwriting of collectable fruits
                for (int Index = 0; Index < CollectableFruits.Length; Index++)
                {
                    if (Math.Abs(CollectableFruits[Index].Position.X - PlacePosition.X) < Tile.SizeOver2.X &&
                        Math.Abs(CollectableFruits[Index].Position.Y - PlacePosition.Y) < Tile.SizeOver2.Y)
                    {
                        return;
                    }
                }

                // Place the new object
                if (InputManager.CheckInputDown(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.SmallInputDelay))
                {
                    switch (SelectedCollectable)
                    {
                        case CollectableTypesEnum.Egg:
                            CollectableEggs.Add(new CollectableEgg(PlacePosition));
                            break;
                        case CollectableTypesEnum.Apple:
                            CollectableFruits[0] = new CollectableFruit(PlacePosition, CollectableFruit.FruitTypeEnum.Apple);
                            break;
                        case CollectableTypesEnum.Banana:
                            CollectableFruits[1] = new CollectableFruit(PlacePosition, CollectableFruit.FruitTypeEnum.Banana);
                            break;
                        case CollectableTypesEnum.Orange:
                            CollectableFruits[2] = new CollectableFruit(PlacePosition, CollectableFruit.FruitTypeEnum.Orange);
                            break;
                    }
                }
            }
        }

        private void UpdatePlaceEnemies(GameTime GameTime)
        {
            if (InputManager.CheckJustPressed(InputManager.LeftBumperKeys, InputManager.LeftBumperButtons) ||
                InputManager.CheckLeftTriggerJustPressed())
            {
                SelectedEnemy = (Enemy.EnemyTypeEnum)CycleSelectedEnemy((int)SelectedEnemy, -1);
            }
            if (InputManager.CheckJustPressed(InputManager.RightBumperKeys, InputManager.RightBumperButtons) ||
                InputManager.CheckRightTriggerJustPressed())
            {
                SelectedEnemy = (Enemy.EnemyTypeEnum)CycleSelectedEnemy((int)SelectedEnemy, 1);
            }

            // Both placing and removing collectables share many operations
            if (InputManager.CheckInputDown(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.SmallInputDelay) ||
                InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons, InputManager.SmallInputDelay))
            {
                Vector2 PlacePosition = Camera.CameraPosition + Game.HalfScreenSize - Vector2.One * 64f;

                PlacePosition.X = (float)Math.Round((PlacePosition.X / (Tile.Size.X * 2)));
                PlacePosition.X *= (Tile.Size.X * 2);
                PlacePosition.Y = (float)Math.Round((PlacePosition.Y / (Tile.Size.Y * 2)));
                PlacePosition.Y *= (Tile.Size.Y * 2);

                for (int Index = 0; Index < Enemies.Count; Index++)
                {
                    if (!Enemies[Index].IsOnScreen())
                        continue;

                    // Remove enemies too close to the one being placed  (arbitrary bounds)
                    if (Math.Abs(Enemies[Index].Position.X - PlacePosition.X) < Tile.Size.X * 2 &&
                        Math.Abs(Enemies[Index].Position.Y - PlacePosition.Y) < Tile.Size.Y * 2)
                    {
                        Enemies.RemoveAt(Index);
                        break;
                    }
                }

                // Place the new enemy
                if (InputManager.CheckInputDown(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.SmallInputDelay))
                {
                    Enemies.Add(new Enemy(PlacePosition, SelectedEnemy));
                }
            }
        }

        private void UpdatePlaceCheckPoints(GameTime GameTime)
        {
            // Both placing and removing collectables share many operations
            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu) ||
                InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons))
            {
                Vector2 PlacePosition = Camera.CameraPosition + Game.HalfScreenSize - CheckPoint.Size / 2;

                PlacePosition.X = (float)Math.Round(PlacePosition.X / (Tile.Size.X * 2));
                PlacePosition.X *= (Tile.Size.X * 2);
                PlacePosition.Y = (float)Math.Round(PlacePosition.Y / (Tile.Size.Y * 1.15f));
                PlacePosition.Y *= (Tile.Size.Y * 1.15f);

                for (int Index = 0; Index < CheckPoints.Count; Index++)
                {
                    if (!CheckPoints[Index].IsOnScreen())
                        continue;

                    // Remove CheckPoints too close to the one being placed (arbitrary bounds)
                    if (Math.Abs(CheckPoints[Index].Position.X - PlacePosition.X) < Tile.Size.X * 3 &&
                        Math.Abs(CheckPoints[Index].Position.Y - PlacePosition.Y) < Tile.Size.Y * 8.0f)
                    {
                        CheckPoints.RemoveAt(Index);
                        break;
                    }
                }

                // Place the new object
                if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu))
                    CheckPoints.Add(new CheckPoint(PlacePosition));
            }
        }

        private void UpdatePlaceExitPoint(GameTime GameTime)
        {
            // Both placing and removing collectables share many operations
            if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu) ||
                InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons))
            {
                Vector2 PlacePosition = Camera.CameraPosition + Game.HalfScreenSize - ExitPointTextureSize / 2;

                PlacePosition.X = (float)Math.Round(PlacePosition.X / (Tile.Size.X * 2));
                PlacePosition.X *= (Tile.Size.X * 2);
                PlacePosition.Y = (float)Math.Round(PlacePosition.Y / (Tile.Size.Y * 1.15f));
                PlacePosition.Y *= (Tile.Size.Y * 1.15f);

                // Place the new object
                if (InputManager.CheckJustPressed(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu))
                    LevelExit = new LevelExit(PlacePosition);
            }
        }

        private void UpdateBeginCollisionObject(GameTime GameTime)
        {
            // Mouse click!
            if (InputManager.CheckMouseJustClicked())
            {
                Vector2 ClickCoordinates = InputManager.MouseCoords + Camera.CameraPosition;

                //if (!Camera.IsOnScreen(ClickCoordinates))
                //    return;

                // Check if modifying an existing collision object
                for (int index = 0; index < CollisionObjects.Count; index++)
                {
                    //if (!CollisionObjects[index].IsOnScreen())
                    //    continue;

                    if (CollisionObjects[index].TrySelectVertex(ClickCoordinates))
                    {
                        CandidateCollisionObject = CollisionObjects[index];
                        CollisionObjects.RemoveAt(index);
                        return;
                    }

                }
                if (InputManager.CheckInputDown(InputManager.ShiftKeys, InputManager.ShiftButtons))
                {
                    CandidateCollisionObject = new CollisionObjectTriangle(InputManager.MouseCoords + Camera.CameraPosition);
                }
                else
                {
                    CandidateCollisionObject = new CollisionObjectRectangle(InputManager.MouseCoords + Camera.CameraPosition);
                }

            }

        }

        private void UpdateDragCollisionObject(GameTime GameTime)
        {
            if (CandidateCollisionObject == null)
                return;

            if (InputManager.CheckJustPressed(InputManager.ShiftKeys, InputManager.ShiftButtons))
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.NPCOnly;

            if (InputManager.CheckJustPressed(InputManager.AltKeys, InputManager.AltButtons))
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.Liquid;

            if (InputManager.CheckJustPressed(InputManager.ControlKeys, InputManager.ControlButtons))
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.Passable;

            if (InputManager.CheckJustPressed(InputManager.CAPSKeys, InputManager.CAPSButtons))
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.Kill;

            if (InputManager.CheckJustPressed(InputManager.TabKeys, InputManager.TabButtons))
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.Damaging;

            if (InputManager.CheckJustReleased(InputManager.ShiftKeys, InputManager.ShiftButtons) ||
                InputManager.CheckJustReleased(InputManager.AltKeys, InputManager.AltButtons) ||
                InputManager.CheckJustReleased(InputManager.ControlKeys, InputManager.ControlButtons) ||
                InputManager.CheckJustReleased(InputManager.CAPSKeys, InputManager.CAPSButtons) ||
                InputManager.CheckJustReleased(InputManager.TabKeys, InputManager.TabButtons))
            {
                CandidateCollisionObject.CollisionObjectType = CollisionObjectTypeEnum.Normal;
            }

            if (InputManager.CheckMouseJustMoved() || CameraMovement != Vector2.Zero)
            {
                Vector2 ResizePosition = InputManager.MouseCoords + Camera.CameraPosition;

                // Apply vertex snapping
                for (int index = 0; index < CollisionObjects.Count; index++)
                {
                    if (!CollisionObjects[index].IsOnScreen())
                        continue;

                    bool Snap = false;
                    for (int VertexIndex = 0; VertexIndex < CollisionObjects[index].Verticies.Length; VertexIndex++)
                    {
                        if (Vector2.Distance(CollisionObjects[index].Verticies[VertexIndex], ResizePosition) < 8.0f)
                        {
                            ResizePosition = CollisionObjects[index].Verticies[VertexIndex];
                            Snap = true;
                            break;
                        }
                    }
                    if (Snap)
                        break;
                }

                CandidateCollisionObject.Resize(ResizePosition);
            }
        }

        private void UpdateReleaseCollisionObject(GameTime GameTime)
        {
            if (InputManager.CheckMouseJustReleased())
            {
                // Only add the collision object if it is large enough
                if (CandidateCollisionObject != null && !CandidateCollisionObject.TooSmall())
                {
                    CandidateCollisionObject.SelectedVertex = -1;
                    CollisionObjects.Add(CandidateCollisionObject);
                }
                CandidateCollisionObject = null;
            }
        }

        private void UpdateTileEditor(GameTime GameTime)
        {
            if (InputManager.CheckJustPressed(InputManager.LeftBumperKeys, InputManager.LeftBumperButtons))
                CurrentLayerDepth = (LayerDepthEnum)CycleLayerDepth((int)CurrentLayerDepth, -1);

            if (InputManager.CheckJustPressed(InputManager.RightBumperKeys, InputManager.RightBumperButtons))
                CurrentLayerDepth = (LayerDepthEnum)CycleLayerDepth((int)CurrentLayerDepth, 1);

            if (InputManager.CheckInputDown(InputManager.AcceptKeysNoMenu, InputManager.AcceptButtonsNoMenu, InputManager.SmallInputDelay))
            {
                switch (CurrentLayerDepth)
                {
                    case LayerDepthEnum.BackGround:
                        BackGroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, SelectedTileType);
                        break;
                    case LayerDepthEnum.Base:
                        GroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, SelectedTileType);
                        break;
                    case LayerDepthEnum.ForeGround:
                        ForeGroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, SelectedTileType);
                        break;
                }
            }
            if (InputManager.CheckInputDown(InputManager.DeclineKeys, InputManager.DeclineButtons, InputManager.SmallInputDelay))
            {
                switch (CurrentLayerDepth)
                {
                    case LayerDepthEnum.BackGround:
                        BackGroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, TileTypesEnum.None);
                        break;
                    case LayerDepthEnum.Base:
                        GroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, TileTypesEnum.None);
                        break;
                    case LayerDepthEnum.ForeGround:
                        ForeGroundTiles.UpdateTileAtCameraCenter(CurrentEnvironment, TileTypesEnum.None);
                        break;
                }
            }

            // Cycling selected/preview tiles
            if (InputManager.CheckLeftTriggerDown())
            {
                SelectedTileType = (TileTypesEnum)CycleTileSelection((int)SelectedTileType, -1);
                LoadPreviewTiles();
            }
            else if (InputManager.CheckRightTriggerDown())
            {
                SelectedTileType = (TileTypesEnum)CycleTileSelection((int)SelectedTileType, 1);
                LoadPreviewTiles();
            }
        }

        private void LoadPreviewTiles()
        {
            for (int Index = 0; Index < PreviewCount; Index++)
            {
                int ActIndex = CycleTileSelection((int)SelectedTileType, Index - PreviewCount / 2);
                PreviewTile[Index] = Tile.LoadTile(CurrentEnvironment, (byte)ActIndex);
            }
        }

        private int CycleTileSelection(int Value, int ShiftValue)
        {
            // This is a convoluted way of simply shifting an enum and keeping it in bounds

            if (ShiftValue == 0)
                return Value;

            int Sign = ShiftValue / Math.Abs(ShiftValue);

            while (ShiftValue != 0)
            {
                Value += Sign;

                if (Value < 0)
                    Value = Enum.GetValues(typeof(TileTypesEnum)).Cast<int>().Max();
                if (Value > Enum.GetValues(typeof(TileTypesEnum)).Cast<int>().Max())
                    Value = Enum.GetValues(typeof(TileTypesEnum)).Cast<int>().Min();

                ShiftValue -= Sign;
            }

            return Value;
        }

        private int CycleLayerDepth(int Value, int ShiftValue)
        {
            // Will not work for shift values greater than +/-1
            Value += ShiftValue;

            if (Value < 0)
                Value = Enum.GetValues(typeof(LayerDepthEnum)).Cast<int>().Max();
            if (Value > Enum.GetValues(typeof(LayerDepthEnum)).Cast<int>().Max())
                Value = Enum.GetValues(typeof(LayerDepthEnum)).Cast<int>().Min();

            return Value;
        }

        private int CycleSelectedCollectable(int Value, int ShiftValue)
        {
            // Will not work for shift values greater than +/-1
            Value += ShiftValue;

            if (Value < 0)
                Value = Enum.GetValues(typeof(CollectableTypesEnum)).Cast<int>().Max();
            if (Value > Enum.GetValues(typeof(CollectableTypesEnum)).Cast<int>().Max())
                Value = Enum.GetValues(typeof(CollectableTypesEnum)).Cast<int>().Min();

            return Value;
        }

        private int CycleSelectedEnemy(int Value, int ShiftValue)
        {
            // Will not work for shift values greater than +/-1
            Value += ShiftValue;

            if (Value < 0)
                Value = Enum.GetValues(typeof(Enemy.EnemyTypeEnum)).Cast<int>().Max();
            if (Value > Enum.GetValues(typeof(Enemy.EnemyTypeEnum)).Cast<int>().Max())
                Value = Enum.GetValues(typeof(Enemy.EnemyTypeEnum)).Cast<int>().Min();

            return Value;
        }

        private int CycleEnvironmentType(int Value, int ShiftValue)
        {
            // Will not work for shift values greater than +/-1
            Value += ShiftValue;

            if (Value < 0)
                Value = Enum.GetValues(typeof(EnvironmentEnum)).Cast<int>().Max();
            if (Value > Enum.GetValues(typeof(EnvironmentEnum)).Cast<int>().Max())
                Value = Enum.GetValues(typeof(EnvironmentEnum)).Cast<int>().Min();

            return Value;
        }

        private int CycleEditorMode(int Value, int ShiftValue)
        {
            // Will not work for shift values greater than +/-1
            Value += ShiftValue;

            if (Value < 0)
                Value = Enum.GetValues(typeof(EditorModeEnum)).Cast<int>().Max();
            if (Value > Enum.GetValues(typeof(EditorModeEnum)).Cast<int>().Max())
                Value = Enum.GetValues(typeof(EditorModeEnum)).Cast<int>().Min();

            return Value;
        }



        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            //base.Draw(GameTime, SpriteBatch);

            // Background
            if (EditorMode != EditorModeEnum.Tiles || CurrentLayerDepth == LayerDepthEnum.None || CurrentLayerDepth == LayerDepthEnum.BackGround)
            {
                BackGroundTiles.Draw(GameTime, SpriteBatch);
                if (CurrentLayerDepth != LayerDepthEnum.None)
                    BackGroundTiles.DrawSelectedCenteredTile(GameTime, SpriteBatch);
            }
            else
            {
                BackGroundTiles.Draw(GameTime, SpriteBatch, true);
            }
            // Base
            if (EditorMode != EditorModeEnum.Tiles || CurrentLayerDepth == LayerDepthEnum.None || CurrentLayerDepth == LayerDepthEnum.Base)
            {
                GroundTiles.Draw(GameTime, SpriteBatch);
                if (CurrentLayerDepth != LayerDepthEnum.None)
                    GroundTiles.DrawSelectedCenteredTile(GameTime, SpriteBatch);

            }
            else
            {
                GroundTiles.Draw(GameTime, SpriteBatch, true);
            }

            for (int Index = 0; Index < Enemies.Count; Index++)
            {
                Enemies[Index].Draw(GameTime, SpriteBatch);
            }

            // Player Spawn
            Player.Draw(GameTime, SpriteBatch);

            // Check points
            for (int Index = 0; Index < CheckPoints.Count; Index++)
            {
                CheckPoints[Index].Draw(SpriteBatch);
            }

            // Collectable objects
            for (int Index = 0; Index < CollectableEggs.Count; Index++)
            {
                CollectableEggs[Index].Draw(SpriteBatch);
            }

            for (int Index = 0; Index < CollectableFruits.Length; Index++)
            {
                CollectableFruits[Index].Draw(SpriteBatch);
            }

            LevelExit.Draw(SpriteBatch);

            // Foreground
            if (EditorMode != EditorModeEnum.Tiles || CurrentLayerDepth == LayerDepthEnum.None || CurrentLayerDepth == LayerDepthEnum.ForeGround)
            {
                ForeGroundTiles.Draw(GameTime, SpriteBatch);
                if (CurrentLayerDepth != LayerDepthEnum.None)
                    ForeGroundTiles.DrawSelectedCenteredTile(GameTime, SpriteBatch);
            }
            else
            {
                ForeGroundTiles.Draw(GameTime, SpriteBatch, true);
            }

            // Candidate collision object
            if (CandidateCollisionObject != null)
                CandidateCollisionObject.Draw(SpriteBatch, Camera.CameraPosition);

            // Existing collision objects
            for (int index = 0; index < CollisionObjects.Count; index++)
            {
                CollisionObjects[index].Draw(SpriteBatch, Camera.CameraPosition);
            }

            // UI
            DrawUICompontents(SpriteBatch);
        }

        private void DrawUICompontents(SpriteBatch SpriteBatch)
        {
            // Draw editor UI compontents
            SpriteBatch.Draw(CrossHair, Game.HalfScreenSize - CrossHairSizeOver2, Color.White);
            SpriteBatch.Draw(Panel, (Game.ScreenSize - PanelSize) * Vector2.UnitY, Color.White);
            SpriteBatch.DrawString(Font, "EditorMode: " + EditorMode.ToString(), (Game.ScreenSize * 0.25f - Font.MeasureString("EditorMode: " + EditorMode.ToString()) / 2) * Vector2.UnitX
                + (Game.ScreenSize - Font.MeasureString("F") * 1.5f) * Vector2.UnitY, Color.White);

            switch (EditorMode)
            {
                case EditorModeEnum.Collectables:
                    DrawCollectablePreview(SpriteBatch);
                    break;
                case EditorModeEnum.Tiles:
                    DrawTilePreviews(SpriteBatch);
                    break;
                case EditorModeEnum.SpawnPoint:
                    DrawPlayerPreview(SpriteBatch);
                    break;
                case EditorModeEnum.CheckPoints:
                    DrawCheckPointPreview(SpriteBatch);
                    break;
                case EditorModeEnum.Collision:
                    PreviewCollisionObject.Draw(SpriteBatch, Vector2.Zero, true, true);
                    break;
                case EditorModeEnum.Objects:

                    break;
                case EditorModeEnum.ExitPoint:
                    DrawExitPointPreview(SpriteBatch);
                    break;
                case EditorModeEnum.Misc:
                    DrawMiscPreview(SpriteBatch);
                    break;
                case EditorModeEnum.Enemies:
                    DrawEnemyPreview(SpriteBatch);
                    break;
            }

            SpriteBatch.Draw(HorizontalSelector, (Game.HalfScreenSize - HorizontalSelectorSize / 2.0f) * Vector2.UnitX + (Game.ScreenSize - HorizontalSelectorSize * 1.5f) * Vector2.UnitY, Color.White);

        }

        private void DrawMiscPreview(SpriteBatch SpriteBatch)
        {
            SpriteBatch.DrawString(Font, "Environment: " + CurrentEnvironment.ToString(), (Game.ScreenSize * 0.25f - Font.MeasureString("Environment: " + CurrentEnvironment.ToString()) / 2) * Vector2.UnitX +
                (Game.ScreenSize - Font.MeasureString("S") * 3.0f) * Vector2.UnitY, Color.White);

            SpriteBatch.DrawString(Font, "Screen Width: " + GroundTiles.Width.ToString(), (Game.ScreenSize * 0.75f - Font.MeasureString("Layer Screen Width: " + GroundTiles.Width.ToString()) / 2) * Vector2.UnitX +
                (Game.ScreenSize - Font.MeasureString("S") * 3.0f) * Vector2.UnitY, Color.White);
            SpriteBatch.DrawString(Font, "Screen Height: " + GroundTiles.Height.ToString(), (Game.ScreenSize * 0.75f - Font.MeasureString("Layer Screen Height: " + GroundTiles.Height.ToString()) / 2) * Vector2.UnitX +
                (Game.ScreenSize - Font.MeasureString("S") * 1.5f) * Vector2.UnitY, Color.White);
        }

        private void DrawPlayerPreview(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(PlayerTexture, Game.HalfScreenSize * Vector2.UnitX - Tile.SizeOver2 * Vector2.UnitX - Tile.Size * Vector2.UnitY * 2.0f +
                        Game.ScreenSize * Vector2.UnitY, Color.White);
        }

        private void DrawCheckPointPreview(SpriteBatch SpriteBatch)
        {
            const float Scale = 0.5f;
            SpriteBatch.Draw(CheckPointTexture, Game.HalfScreenSize * Vector2.UnitX - CheckPointTextureSize / 2 * Scale * Vector2.UnitX - CheckPointTextureSize * 1.15f * Vector2.UnitY * Scale +
                    Game.ScreenSize * Vector2.UnitY, null, Color.White, 0.0f, Vector2.Zero, Vector2.One * Scale, SpriteEffects.None, 0.0f);
        }
        private void DrawExitPointPreview(SpriteBatch SpriteBatch)
        {
            const float Scale = 0.25f;
            SpriteBatch.Draw(ExitPointTexture, Game.HalfScreenSize * Vector2.UnitX - ExitPointTextureSize / 2 * Scale * Vector2.UnitX - ExitPointTextureSize * 1.15f * Vector2.UnitY * Scale +
                    Game.ScreenSize * Vector2.UnitY, null, Color.White, 0.0f, Vector2.Zero, Vector2.One * Scale, SpriteEffects.None, 0.0f);
        }

        private void DrawCollectablePreview(SpriteBatch SpriteBatch)
        {
            Color DrawColor = Color.White;
            for (int Index = 0; Index < CollectableTextures.Length; Index++)
            {
                if (Index == (int)SelectedCollectable)
                    DrawColor = Color.White;
                else
                    DrawColor = Color.White * 0.25f;
                SpriteBatch.Draw(CollectableTextures[Index], Game.HalfScreenSize * Vector2.UnitX - CollectableTextureSize / 2 * Vector2.UnitX - CollectableTextureSize * Vector2.UnitY * 2.5f +
                    Game.ScreenSize * Vector2.UnitY + CollectableTextureSize * 2.0f * Vector2.UnitX * (Index - (int)SelectedCollectable), DrawColor);
            }

        }

        private void DrawEnemyPreview(SpriteBatch SpriteBatch)
        {
            Color DrawColor = Color.White;
            for (int Index = 0; Index < EnemyTextures.Length; Index++)
            {
                if (Index == (int)SelectedEnemy)
                    DrawColor = Color.White;
                else
                    DrawColor = Color.White * 0.25f;
                SpriteBatch.Draw(EnemyTextures[Index], Game.HalfScreenSize * Vector2.UnitX - EnemyTextureSize / 2 * Vector2.UnitX * 0.75f - EnemyTextureSize * Vector2.UnitY * 1.25f +
                    EnemyTextureSize * 0.25f * Vector2.UnitY * (float)(Index / 2) +
                    Game.ScreenSize * Vector2.UnitY + EnemyTextureSize * 1.5f * Vector2.UnitX * (Index - (int)SelectedEnemy), DrawColor);
            }

        }

        private void DrawTilePreviews(SpriteBatch SpriteBatch)
        {
            Texture2D DrawTexture;
            Color DrawColor = Color.White;
            for (int Index = -PreviewCount / 2; Index <= PreviewCount / 2; Index++)
            {
                DrawTexture = PreviewTile[Index + PreviewCount / 2].Texture;
                if (DrawTexture != null)
                {
                    if (Index == 0)
                        DrawColor = Color.White;
                    else
                        DrawColor = Color.White * 0.25f;
                    SpriteBatch.Draw(DrawTexture, Game.HalfScreenSize * Vector2.UnitX - Tile.SizeOver2 * Vector2.UnitX - Tile.Size * Vector2.UnitY * 2.0f +
                        Game.ScreenSize * Vector2.UnitY + Tile.Size * 1.5f * Vector2.UnitX * Index, DrawColor);
                }
            }

            SpriteBatch.DrawString(Font, "Layer Focus: " + CurrentLayerDepth.ToString(), (Game.ScreenSize * 0.75f - Font.MeasureString("Layer Focus: " + CurrentLayerDepth.ToString()) / 2) * Vector2.UnitX +
                (Game.ScreenSize - Font.MeasureString("F") * 1.5f) * Vector2.UnitY, Color.White);
        }

        enum CollectableTypesEnum
        {
            Egg,
            Apple,
            Banana,
            Orange
        }

        enum EditorModeEnum
        {
            Tiles,
            Collision,
            Objects,
            Collectables,
            CheckPoints,
            Enemies,
            SpawnPoint,
            ExitPoint,
            Misc,
        }

    }

}
