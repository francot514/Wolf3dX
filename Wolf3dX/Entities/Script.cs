using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Common;
using Nexxt.Engine.Sound;
using Nexxt.Engine.Animations;
using Nexxt.Engine.Animations.Script;

namespace Wolf3d.Entities.Enemies.Script
{
    public class Script
    {
        Enemy character;

        public Script(Enemy _character)
        {
            character = _character;
        }

        public void DoScript(int animIdx, int keyFrameIdx)
        {
            CharacterDefinition charDef = character.GetCharDef();
            Animation animation = charDef.GetAnimation(animIdx);
            KeyFrame keyFrame = animation.GetKeyFrame(keyFrameIdx);

            bool done = false;

            for (int i = 0; i < keyFrame.GetScriptArray().Length; i++)
            {
                if (done)
                {
                    break;
                }
                else
                {
                    ScriptLine line = keyFrame.GetScript(i);
                    if (line != null)
                    {
                        switch (line.GetCommand())
                        {

                            case Commands.SetAnim:
                                character.SetAnim(line.GetSParam());
                                break;
                            case Commands.Goto:
                                character.SetFrame(line.GetIParam());
                                done = true;
                                break;                     
                            case Commands.PlaySound:
                                SoundManager.PlaySound(line.GetSParam(), false);
                                break;
                            case Commands.IfDyingGoto:

                                break;
                            case Commands.KillMe:

                                break;
                        }
                    }
                }
            }
        }
    }
}
