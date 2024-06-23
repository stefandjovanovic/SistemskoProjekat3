namespace SistemskoProjekat3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FixtureService fixtureService = new FixtureService();

            WebServer server = new WebServer(fixtureService.GetFixturesObservable, "http://localhost:5050/");
            server.Run();
            Console.WriteLine("Press any key to stop web server");
            Console.ReadKey();
            server.Stop();
        }
    }
}
