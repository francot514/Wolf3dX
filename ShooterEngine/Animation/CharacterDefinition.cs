using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Nexxt.Engine.Animations.Script;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Common;
using Nexxt.Engine.Entities.Actors;
using Nexxt.Engine.GameObjects;
using Nexxt.Common.Enums;

namespace Nexxt.Engine.Animations
{
    public class CharacterDefinition
    {

        #region Fields

        public String Path;
        public ActorType ActorTypeID;

        //fields related to animation
        Animation[] animation;
        public int TextureIndex;

        //actor attributes
        public int[] HitPoints = new int[Constants.NUMBER_DIFFICULTY_LEVELS];
        public float MaxSpeed = 0;
        public float MaxForce = 0;
        public bool HasPain = false;
        public bool MustGetCloseEnoughToAttack = false;
        public int Score = 0;
        public Weapons WeaponType;

        #endregion

        #region Constructors
        public CharacterDefinition(String loadPath, ActorType _defID)
        {
            Reset();

            //TODO: Use the path to load the correct definition depending on the enemy type
            Path = loadPath;

            ActorTypeID = _defID;
            Read();
        }

        public CharacterDefinition()
        {
            Reset();
        }
        #endregion

        private void Reset()
        {
            animation = new Animation[64];
            for (int i = 0; i < animation.Length; i++)
                animation[i] = new Animation();
            Path = "char";
        }

        public Animation GetAnimation(int idx)
        {
            return animation[idx];
        }

        public void SetAnimation(int idx, Animation _animation)
        {
            animation[idx] = _animation;
        }

        public Animation[] GetAnimationArray()
        {
            return animation;
        }

        public void DoScript(int animIdx, int animFrame)
        {
            KeyFrame keyFrame = animation[animIdx].GetKeyFrame(animFrame);

        }

        public void Read()
        {
            //TODO: Read it from file

            int startTexture = ActorCollection<Actor>.START_TEXTURE;

            if (ActorTypeID == ActorType.Guard)
            {
                #region Guard
                //atributes

                HitPoints[(int)Difficulty.None] = 25;
                HitPoints[(int)Difficulty.Easy] = 25;
                HitPoints[(int)Difficulty.Medium] = 25;
                HitPoints[(int)Difficulty.Hard] = 25;
                MaxSpeed = 0.012f;
                MaxForce = 0.030f;
                HasPain = true;
                Score = 5000;
                WeaponType = Weapons.Pistol;

                //idle animation
                animation[0].Name = "idle";
                animation[0].MustBeAnimatedFromDistinctAngles = true;
                KeyFrame keyFrame = new KeyFrame();
                keyFrame.Duration = 1;
                keyFrame.Frame = 0;
                animation[0].SetKeyFrame(0, keyFrame);


                //walk animation
                animation[1].Name = "walk";
                animation[1].MustBeAnimatedFromDistinctAngles = true;

                float duration = 0.2f;
                KeyFrame keyFrameRun1 = new KeyFrame();
                keyFrameRun1.Duration = duration;
                keyFrameRun1.Frame = 58 - startTexture;
                animation[1].SetKeyFrame(0, keyFrameRun1);


                KeyFrame keyFrameRun2 = new KeyFrame();
                keyFrameRun2.Duration = duration;
                keyFrameRun2.Frame = 66 - startTexture;
                animation[1].SetKeyFrame(1, keyFrameRun2);


                KeyFrame keyFrameRun3 = new KeyFrame();
                keyFrameRun3.Duration = duration;
                keyFrameRun3.Frame = 74 - startTexture;
                animation[1].SetKeyFrame(2, keyFrameRun3);

                KeyFrame keyFrameRun4 = new KeyFrame();
                keyFrameRun4.Duration = duration;
                keyFrameRun4.Frame = 82 - startTexture;
                animation[1].SetKeyFrame(3, keyFrameRun4);

                //attack
                float durationAttack = 0.4f;

                animation[2].Name = "attack";
                animation[2].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun411 = new KeyFrame();
                keyFrameRun411.Duration = durationAttack;
                keyFrameRun411.SetScript(0, "playsound Sound/Sfx/049");
                keyFrameRun411.Frame = 97 - startTexture;
                animation[2].SetKeyFrame(0, keyFrameRun411);


                KeyFrame keyFrameRun51 = new KeyFrame();
                keyFrameRun51.Duration = durationAttack;
                keyFrameRun51.SetScript(0, "playsound Sound/Sfx/049");
                keyFrameRun51.Frame = 98 - startTexture;
                animation[2].SetKeyFrame(1, keyFrameRun51);

                //pain animation
                animation[3].Name = "pain";
                animation[3].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun41111 = new KeyFrame();
                keyFrameRun41111.Duration = 1;
                keyFrameRun41111.Frame = 90 - startTexture;
                animation[3].SetKeyFrame(0, keyFrameRun41111);


                //falling animation
                float durationKilled = 0.1f;

                animation[4].Name = "falling";
                animation[4].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun4111 = new KeyFrame();
                keyFrameRun4111.Duration = durationKilled;
                keyFrameRun4111.Frame = 90 - startTexture;
                animation[4].SetKeyFrame(0, keyFrameRun4111);

                KeyFrame keyFrameRun411111 = new KeyFrame();
                keyFrameRun411111.Duration = durationKilled;
                keyFrameRun411111.Frame = 91 - startTexture;
                animation[4].SetKeyFrame(1, keyFrameRun411111);

                KeyFrame keyFrameRun41112 = new KeyFrame();
                keyFrameRun41112.Duration = durationKilled;
                keyFrameRun41112.Frame = 92 - startTexture;
                animation[4].SetKeyFrame(2, keyFrameRun41112);

                KeyFrame keyFrameRun41113 = new KeyFrame();
                keyFrameRun41113.Duration = durationKilled;
                keyFrameRun41113.Frame = 93 - startTexture;
                keyFrameRun41113.SetScript(0, "playsound Sound/Sfx/086");
                animation[4].SetKeyFrame(3, keyFrameRun41113);


                KeyFrame keyFrameRun41114 = new KeyFrame();
                keyFrameRun41114.Duration = durationKilled;
                keyFrameRun41114.Frame = 95 - startTexture;
                keyFrameRun41114.SetScript(0, "killme");//very important to sync with state machine
                animation[4].SetKeyFrame(4, keyFrameRun41114);


                //killed animation
                float durationBones = 1;

                animation[5].Name = "killed";
                animation[5].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun511 = new KeyFrame();
                keyFrameRun511.Duration = durationBones;
                keyFrameRun511.Frame = 95 - startTexture;
                animation[5].SetKeyFrame(0, keyFrameRun511); 
                #endregion
            }
            else if (ActorTypeID == ActorType.Dog)
            {
                #region Dog
		        //atributes
                HitPoints[(int)Difficulty.None] = 1;
                HitPoints[(int)Difficulty.Easy] = 1;
                HitPoints[(int)Difficulty.Medium] = 1;
                HitPoints[(int)Difficulty.Hard] = 1;
                MaxSpeed = 0.018f;
                MaxForce = 0.040f;
                HasPain = false;
                MustGetCloseEnoughToAttack = true;
                Score = 200;
                WeaponType = Weapons.Knife;

                //idle animation
                animation[0].Name = "idle";
                animation[0].MustBeAnimatedFromDistinctAngles = false;
                KeyFrame keyFrame = new KeyFrame();
                keyFrame.Duration = 1;
                keyFrame.Frame = 115 - startTexture;
                animation[0].SetKeyFrame(0, keyFrame);


                //walk animation
                animation[1].Name = "walk";
                animation[1].MustBeAnimatedFromDistinctAngles = true;

                float duration = 0.2f;
                KeyFrame keyFrameRun1 = new KeyFrame();
                keyFrameRun1.Duration = duration;
                keyFrameRun1.Frame = 99 - startTexture;
                animation[1].SetKeyFrame(0, keyFrameRun1);


                KeyFrame keyFrameRun2 = new KeyFrame();
                keyFrameRun2.Duration = duration;
                keyFrameRun2.Frame = 107 - startTexture;
                animation[1].SetKeyFrame(1, keyFrameRun2);


                KeyFrame keyFrameRun3 = new KeyFrame();
                keyFrameRun3.Duration = duration;
                keyFrameRun3.Frame = 115 - startTexture;
                animation[1].SetKeyFrame(2, keyFrameRun3);

                KeyFrame keyFrameRun4 = new KeyFrame();
                keyFrameRun4.Duration = duration;
                keyFrameRun4.Frame = 123 - startTexture;
                animation[1].SetKeyFrame(3, keyFrameRun4);

                //attack
                float durationAttack = 0.4f;

                animation[2].Name = "attack";
                animation[2].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun411 = new KeyFrame();
                keyFrameRun411.Duration = durationAttack;
                keyFrameRun411.Frame = 135 - startTexture;
                animation[2].SetKeyFrame(0, keyFrameRun411);

                KeyFrame keyFrameRun51 = new KeyFrame();
                keyFrameRun51.Duration = durationAttack;
                keyFrameRun51.SetScript(0, "playsound Sound/Sfx/002");
                keyFrameRun51.Frame = 136 - startTexture;
                animation[2].SetKeyFrame(1, keyFrameRun51);

                KeyFrame keyFrameRun517 = new KeyFrame();
                keyFrameRun517.Duration = durationAttack;
                keyFrameRun517.SetScript(0, "playsound Sound/Sfx/002");
                keyFrameRun517.Frame = 137 - startTexture;
                animation[2].SetKeyFrame(2, keyFrameRun517);

                //falling animation
                float durationKilled = 0.1f;

                animation[4].Name = "falling";
                animation[4].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun4111 = new KeyFrame();
                keyFrameRun4111.SetScript(0, "playsound Sound/Sfx/035");
                keyFrameRun4111.Duration = durationKilled;
                keyFrameRun4111.Frame = 131 - startTexture;
                animation[4].SetKeyFrame(0, keyFrameRun4111);

                KeyFrame keyFrameRun411111 = new KeyFrame();
                keyFrameRun411111.Duration = durationKilled;
                keyFrameRun411111.Frame = 132 - startTexture;
                animation[4].SetKeyFrame(1, keyFrameRun411111);

                KeyFrame keyFrameRun41112 = new KeyFrame();
                keyFrameRun41112.Duration = durationKilled;
                keyFrameRun41112.Frame = 133 - startTexture;
                animation[4].SetKeyFrame(2, keyFrameRun41112);

                KeyFrame keyFrameRun41113 = new KeyFrame();
                keyFrameRun41113.Duration = durationKilled;
                keyFrameRun41113.Frame = 134 - startTexture;
                keyFrameRun41113.SetScript(0, "killme");//very important to sync with state machine
                animation[4].SetKeyFrame(3, keyFrameRun41113);


                //killed animation
                float durationBones = 1;

                animation[5].Name = "killed";
                animation[5].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun511 = new KeyFrame();
                keyFrameRun511.Duration = durationBones;
                keyFrameRun511.Frame = 134 - startTexture;
                animation[5].SetKeyFrame(0, keyFrameRun511);

	            #endregion            
            }
            else if (ActorTypeID == ActorType.Gretel_Grosse)
            {
                #region Boss
		        HitPoints[(int)Difficulty.None] = 850;
                HitPoints[(int)Difficulty.Easy] = 950;
                HitPoints[(int)Difficulty.Medium] = 1050;
                HitPoints[(int)Difficulty.Hard] = 1200;
                MaxSpeed = 0.010f;
                MaxForce = 0.025f;
                HasPain = false;
                Score = 5000;
                WeaponType = Weapons.GatlingGun;


                //walk animation
                animation[0].Name = "walk";
                animation[0].MustBeAnimatedFromDistinctAngles = false;

                float duration = 0.2f;
                KeyFrame keyFrameRun1 = new KeyFrame();
                keyFrameRun1.Duration = duration;
                keyFrameRun1.Frame = 385 - startTexture;
                keyFrameRun1.SetScript(0, "playsound xyz");
                animation[0].SetKeyFrame(0, keyFrameRun1);


                KeyFrame keyFrameRun2 = new KeyFrame();
                keyFrameRun2.Duration = duration;
                keyFrameRun2.Frame = 386 - startTexture;
                animation[0].SetKeyFrame(1, keyFrameRun2);


                KeyFrame keyFrameRun3 = new KeyFrame();
                keyFrameRun3.Duration = duration;
                keyFrameRun3.Frame = 387 - startTexture;
                animation[0].SetKeyFrame(2, keyFrameRun3);

                KeyFrame keyFrameRun4 = new KeyFrame();
                keyFrameRun4.Duration = duration;
                keyFrameRun4.Frame = 388 - startTexture;
                animation[0].SetKeyFrame(3, keyFrameRun4);


                //attack animation
                float durationAttack = 0.4f;

                animation[1].Name = "attack";
                animation[1].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun41 = new KeyFrame();
                keyFrameRun41.Duration = durationAttack;
                keyFrameRun41.Frame = 389 - startTexture;
                keyFrameRun41.SetScript(0, "playsound Sound/Sfx/013");
                animation[1].SetKeyFrame(0, keyFrameRun41);


                KeyFrame keyFrameRun5 = new KeyFrame();
                keyFrameRun5.Duration = durationAttack;
                keyFrameRun5.SetScript(0, "playsound Sound/Sfx/013");
                keyFrameRun5.Frame = 390 - startTexture;
                animation[1].SetKeyFrame(1, keyFrameRun5);


                KeyFrame keyFrameRun6 = new KeyFrame();
                keyFrameRun6.Duration = durationAttack;
                keyFrameRun6.Frame = 391 - startTexture;
                animation[1].SetKeyFrame(2, keyFrameRun6);


                //idle animation
                float durationIdle = 0.4f;

                animation[2].Name = "idle";
                animation[2].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun42 = new KeyFrame();
                keyFrameRun42.Duration = durationIdle;
                keyFrameRun42.Frame = 385 - startTexture;
                keyFrameRun42.SetScript(0, "playsound Sound/Sfx/013");
                animation[2].SetKeyFrame(0, keyFrameRun41);


                //falling animation
                float durationKilled = 0.1f;

                animation[3].Name = "falling";
                animation[3].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun4111 = new KeyFrame();
                keyFrameRun4111.Duration = durationKilled;
                keyFrameRun4111.Frame = 393 - startTexture;
                animation[3].SetKeyFrame(0, keyFrameRun4111);

                KeyFrame keyFrameRun411111 = new KeyFrame();
                keyFrameRun411111.Duration = durationKilled;
                keyFrameRun411111.Frame = 394 - startTexture;
                animation[3].SetKeyFrame(1, keyFrameRun411111);

                KeyFrame keyFrameRun41114 = new KeyFrame();
                keyFrameRun41114.Duration = durationKilled;
                keyFrameRun41114.Frame = 395 - startTexture;
                animation[3].SetKeyFrame(2, keyFrameRun41114);


                KeyFrame keyFrameRun5115 = new KeyFrame();
                keyFrameRun5115.Duration = durationKilled;
                keyFrameRun5115.Frame = 392 - startTexture;
                keyFrameRun5115.SetScript(0, "killme"); //very important to sync with state machine
                animation[3].SetKeyFrame(3, keyFrameRun5115);

                //killed animation
                float durationBones = 0.1f;

                animation[4].Name = "killed";
                animation[4].MustBeAnimatedFromDistinctAngles = false;

                KeyFrame keyFrameRun511 = new KeyFrame();
                keyFrameRun511.Duration = durationBones;
                keyFrameRun511.Frame = 392 - startTexture;
                animation[4].SetKeyFrame(0, keyFrameRun511);

	            #endregion            
            }

        }
    }
}
