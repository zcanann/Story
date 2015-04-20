using System;

namespace Story
{
#if WINDOWS || XBOX
    static class Entry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game Game = new Game())
            {
                Game.Run();
            }
        }
    }
#endif
}

