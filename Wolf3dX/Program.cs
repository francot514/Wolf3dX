using System;
using Wolf3d;

namespace WW2Soldier
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new WW2Game())
                game.Run();
        }
    }
}
