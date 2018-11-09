using SIS.MvcFramework;

namespace Exam.App
{
    public class Launcher
    {
        public static void Main()
        {
            WebHost.Start(new StartUp());
        }
    }
}
