using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Engine.Animations;
using Microsoft.Xna.Framework;
using Nexxt.Engine.GameObjects;
using Nexxt.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Engine.Sound;  

namespace Nexxt.Engine.Entities.Actors
{
    public class ActorCollection<T> : List<T> where T : Actor
    {
        CharacterDefinition guardDefinition;
        CharacterDefinition bossDefinition;
        CharacterDefinition dogDefinition;

        //start and end texture numbers associated to all the enemies
        public static Texture2D[] Textures;
        public static int START_TEXTURE = 50;
        public static int END_TEXTURE = 407;
        public static bool TexturesLoaded = false;

        public Actor[] Actors = new Actor[33];

        /// <summary>
        /// Load the actors' textures
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager content)
        {
           //todo : should be part of the character definition file
            Textures = new Texture2D[END_TEXTURE - START_TEXTURE + 1];
            for (int i = START_TEXTURE; i < 100; i++)
                Textures[i - START_TEXTURE] = content.Load<Texture2D>(@"Sprites/Enemies/0" + i.ToString());

            for (int i = 100; i < END_TEXTURE + 1; i++)
                Textures[i - START_TEXTURE] = content.Load<Texture2D>(@"Sprites/Enemies/" + i.ToString());

            TexturesLoaded = true;

            SoundManager.LoadSound("Sound/Sfx/049"); //guard shooting effect
            SoundManager.LoadSound("Sound/Sfx/001"); //guard alert
            SoundManager.LoadSound("Sound/Sfx/025"); //guard die 1
            SoundManager.LoadSound("Sound/Sfx/086"); //guard die 2
            SoundManager.LoadSound("Sound/Sfx/002"); //dog alert
            SoundManager.LoadSound("Sound/Sfx/035"); //dog die 2

        }

        /// <summary>
        /// Load character definitions
        /// </summary>
        public void LoadDefinitions()
        {
            //TODO: Read character definitions from file

            guardDefinition = new CharacterDefinition("", ActorType.Guard);
            bossDefinition = new CharacterDefinition("", ActorType.Gretel_Grosse);
            dogDefinition = new CharacterDefinition("", ActorType.Dog);

        }

        /// <summary>
        /// Load all the actors
        /// </summary>
        /// <param name="map"></param>
        public void LoadActors(Map map)
        {
            //TODO: Load all the actors from file

            Actor guard = new Actor(map, new Vector2(61.5f, 31.5f), guardDefinition, 1);
            guard.IsActive = true;
            guard.IsCollectable = false;
            guard.IsMovable = false;
            guard.Name = "GUARD1";
            guard.Direction = new Vector2(0, -1);
            guard.SetAnim("idle");
            guard.Texture = Textures[START_TEXTURE];
            guard.StartPatrolling = false;
            map.ObjectDatabase.Add(guard);


            Actor guard2 = new Actor(map, new Vector2(61.5f, 39.5f), guardDefinition, 1);
            guard2.IsActive = true;
            guard2.IsCollectable = false;
            guard2.IsMovable = false;
            guard2.Name = "GUARD2";
            guard2.SetAnim("idle");
            guard2.Texture = Textures[START_TEXTURE];
            guard2.Direction = new Vector2(1, 0);
            guard2.StartPatrolling = false;
            map.ObjectDatabase.Add(guard2);

            Actor guard3 = new Actor(map, new Vector2(45.5f, 34.5f), guardDefinition, 1);
            guard3.IsActive = true;
            guard3.IsCollectable = false;
            guard3.IsMovable = false;
            guard3.Name = "GUARD3";
            guard3.SetAnim("idle");
            guard3.Texture = Textures[START_TEXTURE];
            guard3.Direction = new Vector2(1, 0);
            guard3.StartPatrolling = false;
            map.ObjectDatabase.Add(guard3);


            Actor guard4 = new Actor(map, new Vector2(33.5f, 38.5f), guardDefinition, 1);
            guard4.IsActive = true;
            guard4.IsCollectable = false;
            guard4.IsMovable = false;
            guard4.Name = "GUARD4";
            guard4.SetAnim("idle");
            guard4.Texture = Textures[START_TEXTURE];
            guard4.Direction = new Vector2(1, 1);
            guard4.StartPatrolling = false;
            map.ObjectDatabase.Add(guard4);


            Actor guard5 = new Actor(map, new Vector2(30.5f, 29.5f), guardDefinition, 1);
            guard5.IsActive = true;
            guard5.IsCollectable = false;
            guard5.IsMovable = false;
            guard5.Name = "GUARD5";
            guard5.SetAnim("idle");
            guard5.Texture = Textures[START_TEXTURE];
            guard5.Direction = new Vector2(1, 1);
            guard5.StartPatrolling = true;
            map.ObjectDatabase.Add(guard5);


            Actor dog = new Actor(map, new Vector2(34.5f, 45.5f), dogDefinition, 1);
            dog.IsActive = true;
            dog.IsCollectable = false;
            dog.IsMovable = false;
            dog.Name = "DOG1";
            dog.SetAnim("idle");
            dog.Texture = Textures[START_TEXTURE];
            dog.Direction = new Vector2(-1, 0);
            dog.StartPatrolling = true;
            map.ObjectDatabase.Add(dog);

            Actor guard6 = new Actor(map, new Vector2(41.5f, 48.5f), guardDefinition, 1);
            guard6.IsActive = true;
            guard6.IsCollectable = false;
            guard6.IsMovable = false;
            guard6.Name = "GUARD7";
            guard6.SetAnim("idle");
            guard6.Texture = Textures[START_TEXTURE];
            guard6.Direction = new Vector2(1, 1);
            guard6.StartPatrolling = true;
            map.ObjectDatabase.Add(guard6);


            Actor guard7 = new Actor(map, new Vector2(45.5f, 48.5f), guardDefinition, 1);
            guard7.IsActive = true;
            guard7.IsCollectable = false;
            guard7.IsMovable = false;
            guard7.Name = "GUARD7";
            guard7.SetAnim("idle");
            guard7.Texture = Textures[START_TEXTURE];
            guard7.Direction = new Vector2(1, 1);
            guard7.StartPatrolling = true;
            map.ObjectDatabase.Add(guard7);

            Actor dog2 = new Actor(map, new Vector2(45.5f, 54.5f), dogDefinition, 1);
            dog2.IsActive = true;
            dog2.IsCollectable = false;
            dog2.IsMovable = false;
            dog2.Name = "DOG2";
            dog2.SetAnim("idle");
            dog2.Texture = Textures[START_TEXTURE];
            dog2.Direction = new Vector2(-1, 0);
            dog2.StartPatrolling = true;
            map.ObjectDatabase.Add(dog2);

            Actor dog3 = new Actor(map, new Vector2(43.5f, 56.5f), dogDefinition, 1);
            dog3.IsActive = true;
            dog3.IsCollectable = false;
            dog3.IsMovable = false;
            dog3.Name = "DOG3";
            dog3.SetAnim("idle");
            dog3.Texture = Textures[START_TEXTURE];
            dog3.Direction = new Vector2(-1, 0);
            dog3.StartPatrolling = true;
            map.ObjectDatabase.Add(dog3);

            Actor dog4 = new Actor(map, new Vector2(44.5f, 59.5f), dogDefinition, 1);
            dog4.IsActive = true;
            dog4.IsCollectable = false;
            dog4.IsMovable = false;
            dog4.Name = "DOG3";
            dog4.SetAnim("idle");
            dog4.Texture = Textures[START_TEXTURE];
            dog4.Direction = new Vector2(-1, 0);
            dog4.StartPatrolling = true;
            map.ObjectDatabase.Add(dog4);

            Actor guard8 = new Actor(map, new Vector2(22.5f, 31.5f), guardDefinition, 1);
            guard8.IsActive = true;
            guard8.IsCollectable = false;
            guard8.IsMovable = false;
            guard8.Name = "GUARD7";
            guard8.SetAnim("idle");
            guard8.Texture = Textures[START_TEXTURE];
            guard8.Direction = new Vector2(1, 0);
            guard8.StartPatrolling = false;
            map.ObjectDatabase.Add(guard8);

            Actor guard9 = new Actor(map, new Vector2(17.5f, 34.5f), guardDefinition, 1);
            guard9.IsActive = true;
            guard9.IsCollectable = false;
            guard9.IsMovable = false;
            guard9.Name = "GUARD7";
            guard9.SetAnim("idle");
            guard9.Texture = Textures[START_TEXTURE];
            guard9.Direction = new Vector2(1, 0);
            guard9.StartPatrolling = false;
            map.ObjectDatabase.Add(guard9);

            Actor guard10 = new Actor(map, new Vector2(11.5f, 38.5f), guardDefinition, 1);
            guard10.IsActive = true;
            guard10.IsCollectable = false;
            guard10.IsMovable = false;
            guard10.Name = "GUARD7";
            guard10.SetAnim("idle");
            guard10.Texture = Textures[START_TEXTURE];
            guard10.Direction = new Vector2(1, 0);
            guard10.StartPatrolling = false;
            map.ObjectDatabase.Add(guard10);

            Actor guard11 = new Actor(map, new Vector2(9.5f, 33.5f), guardDefinition, 1);
            guard11.IsActive = true;
            guard11.IsCollectable = false;
            guard11.IsMovable = false;
            guard11.Name = "GUARD7";
            guard11.SetAnim("idle");
            guard11.Texture = Textures[START_TEXTURE];
            guard11.Direction = new Vector2(1, 0);
            guard11.StartPatrolling = false;
            map.ObjectDatabase.Add(guard11);

            Actor guard12 = new Actor(map, new Vector2(11.5f, 42.5f), guardDefinition, 1);
            guard12.IsActive = true;
            guard12.IsCollectable = false;
            guard12.IsMovable = false;
            guard12.Name = "GUARD7";
            guard12.SetAnim("idle");
            guard12.Texture = Textures[START_TEXTURE];
            guard12.Direction = new Vector2(1, 0);
            guard12.StartPatrolling = false;
            map.ObjectDatabase.Add(guard12);

            Actor guard13 = new Actor(map, new Vector2(16.5f, 19.5f), guardDefinition, 1);
            guard13.IsActive = true;
            guard13.IsCollectable = false;
            guard13.IsMovable = false;
            guard13.Name = "GUARD7";
            guard13.SetAnim("idle");
            guard13.Texture = Textures[START_TEXTURE];
            guard13.Direction = new Vector2(1, 0);
            guard13.StartPatrolling = false;
            map.ObjectDatabase.Add(guard13);

            Actor guard14 = new Actor(map, new Vector2(18.5f, 21.5f), guardDefinition, 1);
            guard14.IsActive = true;
            guard14.IsCollectable = false;
            guard14.IsMovable = false;
            guard14.Name = "GUARD7";
            guard14.SetAnim("idle");
            guard14.Texture = Textures[START_TEXTURE];
            guard14.Direction = new Vector2(1, 0);
            guard14.StartPatrolling = false;
            map.ObjectDatabase.Add(guard14);

            Actor guard15 = new Actor(map, new Vector2(15.5f, 14.5f), guardDefinition, 1);
            guard15.IsActive = true;
            guard15.IsCollectable = false;
            guard15.IsMovable = false;
            guard15.Name = "GUARD7";
            guard15.SetAnim("idle");
            guard15.Texture = Textures[START_TEXTURE];
            guard15.Direction = new Vector2(1, 0);
            guard15.StartPatrolling = false;
            map.ObjectDatabase.Add(guard15);

            Actor guard16 = new Actor(map, new Vector2(19.5f, 14.5f), guardDefinition, 1);
            guard16.IsActive = true;
            guard16.IsCollectable = false;
            guard16.IsMovable = false;
            guard16.Name = "GUARD7";
            guard16.SetAnim("idle");
            guard16.Texture = Textures[START_TEXTURE];
            guard16.Direction = new Vector2(1, 0);
            guard16.StartPatrolling = false;
            map.ObjectDatabase.Add(guard16);


            Actor guard17 = new Actor(map, new Vector2(16.5f, 8.5f), guardDefinition, 1);
            guard17.IsActive = true;
            guard17.IsCollectable = false;
            guard17.IsMovable = false;
            guard17.Name = "GUARD7";
            guard17.SetAnim("idle");
            guard17.Texture = Textures[START_TEXTURE];
            guard17.Direction = new Vector2(1, 0);
            guard17.StartPatrolling = false;
            map.ObjectDatabase.Add(guard17);

            Actor guard18 = new Actor(map, new Vector2(19.5f, 8.5f), guardDefinition, 1);
            guard18.IsActive = true;
            guard18.IsCollectable = false;
            guard18.IsMovable = false;
            guard18.Name = "GUARD7";
            guard18.SetAnim("idle");
            guard18.Texture = Textures[START_TEXTURE];
            guard18.Direction = new Vector2(1, 0);
            guard18.StartPatrolling = false;
            map.ObjectDatabase.Add(guard18);

            Actor guard19 = new Actor(map, new Vector2(27.5f, 10.5f), guardDefinition, 1);
            guard19.IsActive = true;
            guard19.IsCollectable = false;
            guard19.IsMovable = false;
            guard19.Name = "GUARD7";
            guard19.SetAnim("idle");
            guard19.Texture = Textures[START_TEXTURE];
            guard19.Direction = new Vector2(1, 0);
            guard19.StartPatrolling = false;
            map.ObjectDatabase.Add(guard19);

            Actor guard20 = new Actor(map, new Vector2(29.5f, 6.5f), guardDefinition, 1);
            guard20.IsActive = true;
            guard20.IsCollectable = false;
            guard20.IsMovable = false;
            guard20.Name = "GUARD7";
            guard20.SetAnim("idle");
            guard20.Texture = Textures[START_TEXTURE];
            guard20.Direction = new Vector2(1, 0);
            guard20.StartPatrolling = false;
            map.ObjectDatabase.Add(guard20);

            Actor guard21 = new Actor(map, new Vector2(33.5f, 7.5f), guardDefinition, 1);
            guard21.IsActive = true;
            guard21.IsCollectable = false;
            guard21.IsMovable = false;
            guard21.Name = "GUARD7";
            guard21.SetAnim("idle");
            guard21.Texture = Textures[START_TEXTURE];
            guard21.Direction = new Vector2(1, 0);
            guard21.StartPatrolling = false;
            map.ObjectDatabase.Add(guard21);

            Actor guard22 = new Actor(map, new Vector2(37.5f, 13.5f), guardDefinition, 1);
            guard22.IsActive = true;
            guard22.IsCollectable = false;
            guard22.IsMovable = false;
            guard22.Name = "GUARD7";
            guard22.SetAnim("idle");
            guard22.Texture = Textures[START_TEXTURE];
            guard22.Direction = new Vector2(1, 0);
            guard22.StartPatrolling = false;
            map.ObjectDatabase.Add(guard22);


            Actor guard23 = new Actor(map, new Vector2(45.5f, 10.5f), guardDefinition, 1);
            guard23.IsActive = true;
            guard23.IsCollectable = false;
            guard23.IsMovable = false;
            guard23.Name = "GUARD7";
            guard23.SetAnim("idle");
            guard23.Texture = Textures[START_TEXTURE];
            guard23.Direction = new Vector2(1, 0);
            guard23.StartPatrolling = false;
            map.ObjectDatabase.Add(guard23);


            Actor guard24 = new Actor(map, new Vector2(45.5f, 2.5f), guardDefinition, 1);
            guard24.IsActive = true;
            guard24.IsCollectable = false;
            guard24.IsMovable = false;
            guard24.Name = "GUARD7";
            guard24.SetAnim("idle");
            guard24.Texture = Textures[START_TEXTURE];
            guard24.Direction = new Vector2(1, 0);
            guard24.StartPatrolling = false;
            map.ObjectDatabase.Add(guard24);


            Actor guard25 = new Actor(map, new Vector2(41.5f, 5.5f), guardDefinition, 1);
            guard25.IsActive = true;
            guard25.IsCollectable = false;
            guard25.IsMovable = false;
            guard25.Name = "GUARD7";
            guard25.SetAnim("idle");
            guard25.Texture = Textures[START_TEXTURE];
            guard25.Direction = new Vector2(1, 0);
            guard25.StartPatrolling = false;
            map.ObjectDatabase.Add(guard25);

            Actor guard26 = new Actor(map, new Vector2(26.5f, 7.5f), guardDefinition, 1);
            guard26.IsActive = true;
            guard26.IsCollectable = false;
            guard26.IsMovable = false;
            guard26.Name = "GUARD7";
            guard26.SetAnim("idle");
            guard26.Texture = Textures[START_TEXTURE];
            guard26.Direction = new Vector2(1, 0);
            guard26.StartPatrolling = false;
            map.ObjectDatabase.Add(guard26);

            Actor guard27 = new Actor(map, new Vector2(46.5f, 23.5f), guardDefinition, 1);
            guard27.IsActive = true;
            guard27.IsCollectable = false;
            guard27.IsMovable = false;
            guard27.Name = "GUARD7";
            guard27.SetAnim("idle");
            guard27.Texture = Textures[START_TEXTURE];
            guard27.Direction = new Vector2(1, 0);
            guard27.StartPatrolling = false;
            map.ObjectDatabase.Add(guard27);


            Actor guard28 = new Actor(map, new Vector2(47.5f, 22.5f), guardDefinition, 1);
            guard28.IsActive = true;
            guard28.IsCollectable = false;
            guard28.IsMovable = false;
            guard28.Name = "GUARD7";
            guard28.SetAnim("idle");
            guard28.Texture = Textures[START_TEXTURE];
            guard28.Direction = new Vector2(1, 0);
            guard28.StartPatrolling = false;
            map.ObjectDatabase.Add(guard28);


            Actor guard29 = new Actor(map, new Vector2(48.5f, 23.5f), guardDefinition, 1);
            guard29.IsActive = true;
            guard29.IsCollectable = false;
            guard29.IsMovable = false;
            guard29.Name = "GUARD7";
            guard29.SetAnim("idle");
            guard29.Texture = Textures[START_TEXTURE];
            guard29.Direction = new Vector2(1, 0);
            guard29.StartPatrolling = false;
            map.ObjectDatabase.Add(guard29);

            Actors[0] = guard;
            Actors[1] = guard2;
            Actors[2] = guard3;
            Actors[3] = guard4;
            Actors[4] = guard5;
            Actors[5] = dog;
            Actors[6] = guard6;
            Actors[7] = guard7;
            Actors[8] = dog2;
            Actors[9] = dog3;
            Actors[10] = dog4;
            Actors[11] = guard8;
            Actors[12] = guard9;
            Actors[13] = guard10;
            Actors[14] = guard11;
            Actors[15] = guard12;
            Actors[16] = guard13;
            Actors[17] = guard14;
            Actors[18] = guard15;
            Actors[19] = guard16;
            Actors[20] = guard17;
            Actors[21] = guard18;
            Actors[22] = guard19;
            Actors[23] = guard20;
            Actors[24] = guard21;
            Actors[25] = guard22;
            Actors[26] = guard23;
            Actors[27] = guard24;
            Actors[28] = guard25;
            Actors[29] = guard26;
            Actors[30] = guard27;
            Actors[31] = guard28;
            Actors[32] = guard29;

        }

        //Animate every actor
        public void AnimateActors(Camera camera, GameTime gameTime)
        {
            for (int i = 0; i < Actors.Length; i++)
            {
                if (Actors[i] != null)
                {
                    if (Actors[i].IsActive && !Actors[i].IsKilled)
                        Actors[i].Animate(camera, gameTime);
                }
            }
        }

        //Update the state of every actor
        public void UpdateStates(GameTime gameTime, ref Map map, Camera camera)
        {
            for (int actorIndex = 0; actorIndex < Actors.Length; actorIndex++)
            {
                Actor actor = Actors[actorIndex];
                if (actor != null)
                {
                    if (!actor.IsKilled)
                    {
                        actor.Target = camera.Position;
                        actor.GameTime = gameTime;
                        actor.Tick();
                    }
                }
            }
        }

    }
}
