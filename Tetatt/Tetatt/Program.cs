using System;

namespace Tetatt
{
#if WINDOWS || XBOX || __MonoCS__
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TetattGame game = new TetattGame())
            {
                game.Run();
            }
        }
    }
#endif
}

