#region File Description
//-----------------------------------------------------------------------------
// MapTypeReader.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Framework;
using Nexxt.Engine.GameObjects;
using Microsoft.Xna.Framework;
using Nexxt.Common.Enums;
using Nexxt.Engine.Entities;
using Nexxt.Engine.Entities.Actors;
#endregion



namespace Nexxt.Engine.TypeReaders
{
    public class MapTypeReader : ContentTypeReader<Map>
    {


        DoorCollection<Door> doors = new DoorCollection<Door>();
        ActorCollection<Actor> actors = new ActorCollection<Actor>();
        SecretCollection<Secret> secrets = new SecretCollection<Secret>();
 
        protected override Map Read(ContentReader input, Map existingInstance)
        {
            //then we can read in all the textures
            int tCount = input.ReadInt32();
            List<Texture2D> textures = new List<Texture2D>();
            for (int i = 0; i < tCount; i++)
                textures.Add(input.ReadExternalReference<Texture2D>());

            //next the background texture
            Texture2D background = input.ReadExternalReference<Texture2D>();

            // next read the ambient audio resource name
            string AmbientAudio = input.ReadString();

            // Read the Spawning Point
            Vector2 spawnPoint = input.ReadVector2();

            List<GameObject> objectDataBase = new List<GameObject>();
            //Reads all the sprites for the game
            tCount = input.ReadInt32();

            for (int i = 0; i < tCount; i++)
            {
                objectDataBase.Add(input.ReadObject<GameObject>());
            }

            //and finally the map layout
            int rCount = input.ReadInt32();
            List<int[]> rows = new List<int[]>();
            for (int i = 0; i < rCount; i++)
                rows.Add(input.ReadObject<int[]>());

            //now we cram that map layout into the proper int[,] structure
            int[,] map = new int[rCount, rows[0].Length];
            for (int i = 0; i < rCount; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    PutDoors(input, objectDataBase, rows, i, j);
                    map[i, j] = rows[i][j];
                }
            }

            // TODO : Read Secrets from map file

            Secret secret1 = new Secret();
            secret1.SecretSprite.Texture = input.ContentManager.Load<Texture2D>("Textures/30");
            secret1.SecretSprite.Orientation = Orientation.Horizontal;
            secret1.SecretSprite.Position = new Vector2(22.5f, 30.9f);
            secret1.SecretSprite.Name = "Secret";
            secret1.Position = new Vector2(22, 30);
            secret1.finalPosition = secret1.Position.Y - 3;   
            secret1.doorType = DoorType.Secret;
            objectDataBase.Add(secret1.SecretSprite);

            Secret secret2 = new Secret();
            secret2.SecretSprite.Texture = input.ContentManager.Load<Texture2D>("Textures/19");
            secret2.SecretSprite.Orientation = Orientation.Vertical;
            secret2.SecretSprite.Position = new Vector2(13.9f, 10.5f);
            secret2.SecretSprite.Name = "Secret";
            secret2.Position = new Vector2(13, 10);
            secret2.finalPosition = secret2.Position.X - 3;
            secret2.doorType = DoorType.Secret;
            objectDataBase.Add(secret2.SecretSprite);

            Secret secret3 = new Secret();
            secret3.SecretSprite.Texture = input.ContentManager.Load<Texture2D>("Textures/13");
            secret3.SecretSprite.Orientation = Orientation.Vertical;
            secret3.SecretSprite.Position = new Vector2(49.9f, 18.5f);
            secret3.SecretSprite.Name = "Secret";
            secret3.Position = new Vector2(49, 18);
            secret3.finalPosition = secret3.Position.X - 3;    
            secret3.doorType = DoorType.Secret;
            objectDataBase.Add(secret3.SecretSprite);

           
            secrets.Add(secret1);
            secrets.Add(secret2);
            secrets.Add(secret3);
   
            //TODO: Read actors from file
            //and return the new map object
            Map mapResult = new Map(textures, background, map, objectDataBase, AmbientAudio, doors, null, secrets, spawnPoint);
            //TODO: Read actors from file
            actors.LoadContent(input.ContentManager);   
            actors.LoadDefinitions();
            //actors.LoadActors(mapResult);
            mapResult.Actors = actors;  
            //and return the new map object
            return mapResult;



        }

        private void PutDoors(ContentReader input, List<GameObject> objectDataBase, List<int[]> rows, int i, int j)
        {
            // calculate doorSide coords
            if (
                rows[i][j] == (int)DoorType.Normal
                |
                rows[i][j] == (int)DoorType.Exit
                |
                rows[i][j] == (int)DoorType.Gold_Key
                |
                rows[i][j] == (int)DoorType.Silver_Key
                )
            {
                Door door = new Door();

                if (rows[i - 1][j] > 0)
                {
                    door.putDoorXSides(i, j);
                    door.orientation = Orientation.Horizontal;
                }
                else if (rows[i][j - 1] > 0)
                {
                    door.putDoorYSides(i, j);
                    door.orientation = Orientation.Vertical;
                }
                switch (rows[i][j])
                {
                    case (int)DoorType.Normal:
                        door.doorType = DoorType.Normal;
                        door.DoorSprite.Texture = input.ContentManager.Load<Texture2D>(Constants.NORMAL_DOOR_TEXTURE);
                        break;
                    case (int)DoorType.Exit:
                        door.DoorSprite.Texture = input.ContentManager.Load<Texture2D>(Constants.EXIT_DOOR_TEXTURE);
                        door.doorType = DoorType.Exit;
                        break;
                    case (int)DoorType.Gold_Key:
                        door.DoorSprite.Texture = input.ContentManager.Load<Texture2D>(Constants.GOLD_KEY_DOOR_TEXTURE);
                        door.doorType = DoorType.Gold_Key;
                        break;
                    case (int)DoorType.Silver_Key:
                        door.DoorSprite.Texture = input.ContentManager.Load<Texture2D>(Constants.SILVER_KEY_DOOR_TEXTURE);
                        door.doorType = DoorType.Silver_Key;
                        break;
                    default:
                        door.DoorSprite.Texture = input.ContentManager.Load<Texture2D>(Constants.NORMAL_DOOR_TEXTURE);
                        door.doorType = DoorType.Normal;
                        break;
                }
                door.Position = new Vector2(i, j);
                door.DoorSprite.Position = new Vector2((float)i + 0.5f, (float)j + 0.5f);

                // horizontal doors
                if (door.orientation == Orientation.Horizontal)
                {
                    door.startPosition = i + 0.5f;
                    door.finalPosition = i + 1.5f;
                }
                else //Vertical Doors
                {
                    door.startPosition = j + 0.5f;
                    door.finalPosition = j + 1.5f;
                }


                door.originalPosition = door.DoorSprite.Position;
                door.DoorSprite.Orientation = door.orientation;
                // add the door to the map doors collection
                doors.Add(door);
                door.DoorSprite.Name = Constants.DOOR_SPRITE_IDENTIFIER;
                //// add the doorSprites to the objectDatabase
                objectDataBase.Add(door.DoorSprite);
            }
        }
    }
}
