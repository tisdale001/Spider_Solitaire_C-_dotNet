using System;

namespace Spider_Solitaire
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press D: Deal a new hand.");
            Console.WriteLine("Press Z: Undo.");
            Console.WriteLine("Press Y: Redo.");
            Console.WriteLine("Press M: Win animation.");
            Console.WriteLine("Press N: Win animation.");
            Console.WriteLine("Press ESC: Close window.");

            Controller controller = new Controller();
            controller.playGame();
        }
    }
}
