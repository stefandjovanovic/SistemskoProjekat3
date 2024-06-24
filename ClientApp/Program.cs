using System;
using System.Net.Http;
using System.Threading;



namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Lista utakmica
            List<string> fixtures = new List<string> {
                "19116497",
                "19116467",
                "19116470",
                "19116472",
                "19116468",
                "19116498",
                "19116497",
                "19104334",
                "19116469",
                "19104335",
                "19104336",
                "19104337",
                "19116485"
            };

            Thread[] threads = new Thread[fixtures.Count];

            int i = 0;
            foreach (string fixture in fixtures)
            {

                threads[i] = new Thread(() => SendRequest(fixture));
                threads[i].Start();
                i++;
            }


            foreach (Thread thread in threads)
            {
                thread.Join();
            }

        }

        static void SendRequest(string fixture)
        {
            string url = $"http://localhost:5050/{fixture}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Odgovor servera za utakmicu {fixture}:");
                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Došlo je do greške prilikom slanja zahteva za utakmicu {fixture}: {ex.Message}");
                }
            }
        }

    }
}