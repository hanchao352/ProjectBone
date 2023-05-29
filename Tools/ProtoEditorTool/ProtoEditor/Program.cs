using Newtonsoft.Json;

namespace ProtoEditor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ProtoManager.Instance.Initialize();
          //  TreeDateManager.Instance.Initialize();
            
            EventManager.Instance.Initialize();
            ApplicationConfiguration.Initialize();
            Application.Run(new ProtoWindow());
        }
       
    }
}