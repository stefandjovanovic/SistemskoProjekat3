namespace SistemskoProjekat3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebServer server = new WebServer("http://localhost:5050/");
            server.Run();
            Console.WriteLine("Pritisnite bilo koji taster da prekinete...");
            Console.ReadKey();
            server.Stop();
        }
    }
}
