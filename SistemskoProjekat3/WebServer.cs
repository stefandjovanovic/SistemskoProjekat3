using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProjekat3
{
    public class WebServer
    {
        private readonly HttpListener listener = new HttpListener();
        //private readonly Func<HttpListenerRequest, Task<string>> responderMethod;
        //private IObservable<HttpListenerContext>? httpListenerObservable;
        private IDisposable? observableSubscription;


        public WebServer(/*Func<HttpListenerRequest Task<string>> responderMethod,*/ params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("URI ne sadrzi adekvatan broj parametara");
            }

            //if (responderMethod == null)
            //{
            //    throw new ArgumentException("Potreban je odgovarajuci responderMethod");
            //}

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }


            //this.responderMethod = responderMethod;

            
        }

        public void Run()
        {
            Console.WriteLine("Webserver je pokrenut...");
            IObservable<HttpListenerContext> obs = Observable.Create<HttpListenerContext>(observer =>
            {
                listener.Start();
                Task.Run(async () =>
                {
                    try
                    {
                        while (listener.IsListening)
                        {
                            var context = await listener.GetContextAsync();
                            observer.OnNext(context);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
                return Disposable.Empty;
            }); 


            //IObservable<HttpListenerContext> obs = listener.GetContextAsync().ToObservable();
            this.observableSubscription = obs.Subscribe(
                context =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    Console.WriteLine("Request: " + request.RawUrl);

                    byte[] buf = Encoding.UTF8.GetBytes("Returning response");
                    response.ContentLength64 = buf.Length;
                    response.OutputStream.Write(buf, 0, buf.Length);

                    if (context != null)
                    {
                        context.Response.OutputStream.Close();
                    }

                },
                exception =>
                {
                    Console.WriteLine("Error: " + exception.Message);
                });



        }

        public void Stop()
        {
            if (observableSubscription != null)
                observableSubscription.Dispose();
            listener.Stop();
            listener.Close();
            
        }
    }
}
