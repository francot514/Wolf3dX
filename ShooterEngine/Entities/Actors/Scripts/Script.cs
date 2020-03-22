using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Common;
using Nexxt.Engine.Animations;
using Nexxt.Engine.Animations.Script;
using Nexxt.Engine.Sound;

namespace Nexxt.Engine.Entities.Actors.Scripts
{
    public class Script
    {
        Actor character;

        public Script(Actor _character)
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
                                SoundManager.PlaySound(line.GetSParam(), true);
                                break;
                            case Commands.IfDyingGoto:

                                break;
                            case Commands.KillMe:
                                character.Kill();
                                break;
                        }
                    }
                }
            }
        }
    }
}
