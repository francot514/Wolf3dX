#region File Description
//-----------------------------------------------------------------------------
// Map.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Nexxt.Framework;
using Nexxt.Engine.Entities;
using Nexxt.Engine.Entities.Actors;
using Nexxt.Engine.AI.Algorithms;
#endregion

namespace Nexxt.Engine.GameObjects
{
    public class Map
    {
        public DoorCollection<Door> Doors;
        public ActorCollection<Actor> Actors;
        public BulletCollection<Bullet> Bullets;
        public SecretCollection<Secret> Secrets;
        public List<Texture2D> Textures;
        public Vector2 SpawnPoint;
        public List<GameObject> ObjectDatabase = new List<GameObject>();
        public Texture2D Background;
        public int[,] WorldMap;
        public string AmbientAudio;
        public PathFinder PathFinder;

        internal Map(List<Texture2D> textures, Texture2D background, int[,] map, List<GameObject> objects, string ambientAudio, DoorCollection<Door> doors, ActorCollection<Actor> actors, SecretCollection<Secret> secrets, Vector2 spawnPoint)
        {
            Textures = textures;
            Background = background;
            this.ObjectDatabase = objects;
            WorldMap = map;
            AmbientAudio = ambientAudio;
            Doors = doors;
            SpawnPoint = spawnPoint;
            Actors = actors;
            Secrets = secrets;
            CreatePathFinder(map);
            Bullets = new BulletCollection<Bullet>();
        }

        private void CreatePathFinder(int[,] map)
        {
            PathFinder = new PathFinder(WorldMap);
            PathFinder.Formula = Nexxt.Engine.AI.Algorithms.PathFinder.HeuristicFormula.EuclideanNoSQR;
            PathFinder.Diagonals = false;
            PathFinder.HeavyDiagonals = true;
            PathFinder.HeuristicEstimate = (int)2;
            PathFinder.PunishChangeDirection = false;
            PathFinder.TieBreaker = false;
            PathFinder.SearchLimit = (int)100;
            PathFinder.DebugProgress = false;
            PathFinder.DebugFoundPath = false;
        }
    }
}
