
namespace OMS.AlgoManager
{
    public class ColoredScreenWriter
    {
        private int debugLevel = 0;

        public ColoredScreenWriter(int DebugLevel)
        {
            debugLevel = DebugLevel;
        }

        public void Log(int level, string msg)
        {
            SetNormal();
            WriteMessage(msg, level);
        }


        public void LogRed(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;

            WriteMessage(msg, level);
            SetNormal();

        }


        public void LogRedTypeFast(int level, string msg)
        {
            Task task = new Task(() => LogRedType(level, msg));
            task.Start();
        }



        public void LogRedType(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;

            WriteMessage(msg, level);
            SetNormal();

        }


        public void LogCyanType(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;

            WriteMessage(msg, level);
            SetNormal();

        }

        public void LogMagentaType(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.Black;

            WriteMessage(msg, level);
            SetNormal();

        }



        public void LogGreen(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;

            WriteMessage(msg, level);

            SetNormal();
        }



        public void LogGreenType(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;

            WriteMessage(msg, level);

            SetNormal();
        }

        public void LogGreenTypeFast(int level, string msg)
        {
            Task task = new Task(() => LogGreenType(level, msg));
            task.Start();
        }



        public void LogYellow(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkYellow;

            WriteMessage(msg, level);

            SetNormal();
        }



        public void LogYellowType(int level, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;

            WriteMessage(msg, level);

            SetNormal();
        }







        private void WriteMessage(string msg, int level)
        {
            if (level > debugLevel)
            {
               Console.WriteLine(msg);    
            }
        }

        private void SetNormal()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray; 
        }




    }
}