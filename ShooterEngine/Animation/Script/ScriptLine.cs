using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Common;

namespace Nexxt.Engine.Animations.Script
{
    public class ScriptLine
    {
        Commands command;
        String sParam;
        int iParam;

        public ScriptLine(String line)
        {
            String[] split = line.Split(' ');
            try
            {
                switch (split[0])
                {
                    case "setanim":
                        command = Commands.SetAnim;
                        sParam = split[1];
                        break;
                    case "goto":
                        command = Commands.Goto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "ifupgoto":
                        command = Commands.IfUpGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "ifdowngoto":
                        command = Commands.IfDownGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;

                    case "playsound":
                        command = Commands.PlaySound;
                        sParam = split[1];
                        break;
                    case "ifdyinggoto":
                        command = Commands.IfDyingGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "killme":
                        command = Commands.KillMe;
                        break;
                    case "ai":
                        command = Commands.AI;
                        sParam = split[1];
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }

        public Commands GetCommand()
        {
            return command;
        }

        public int GetIParam()
        {
            return iParam;
        }

        public String GetSParam()
        {
            return sParam;
        }
    }
}
