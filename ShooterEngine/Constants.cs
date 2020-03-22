using System;
using System.Collections.Generic;
using System.Text;

namespace Nexxt.Engine.GameObjects
{
    public class Constants
    {
        public const int DOOR_SIDES = 2;

        public const int COLLISION_BLOCK = 99;
        public const int ELEVATOR_SIDES = 9;


        public const string OPENING_DOOR_SOUND = "Sound/Sfx/010";
        public const string CLOSING_DOOR_SOUND = "Sound/Sfx/007";
        public const string SECRET_DOOR_SOUND = "Sound/Sfx/034";

        


        public const string NORMAL_DOOR_TEXTURE = "Textures/4";
        public const string EXIT_DOOR_TEXTURE = "Textures/7";
        public const string GOLD_KEY_DOOR_TEXTURE = "Textures/5"; // Why goodkey door hasn't a GOLD like color??!! it looks the same as silver key door (WHAT THEY WAS THINKING!!!)
        public const string SILVER_KEY_DOOR_TEXTURE = "Textures/6";
        public const string DOOR_SPRITE_IDENTIFIER = "D";

        public const int NUMBER_DIFFICULTY_LEVELS = 4;
        
        public const double PI = 3.14159;
        public const double TWO_PI = PI * 2;
        public const double HALF_PI = PI / 2;
        public const double QUARTER_PI = PI / 4;
    }
}
