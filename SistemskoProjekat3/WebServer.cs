using SistemskoProjekat3.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
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
        private readonly Func<HttpListenerRequest, IObservable<Fixtures>> responderObservable;
        private IDisposable? observableSubscription;


        public WebServer(Func<HttpListenerRequest, IObservable<Fixtures>> responderMethod, params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("URI ne sadrzi adekvatan broj parametara");
            }

            if (responderMethod == null)
            {
                throw new ArgumentException("Potreban je odgovarajuci responderMethod");
            }

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            this.responderObservable = responderMethod;

            listener.Start();
        }

        private IObservable<HttpListenerContext> GetContextAsync()
        {
            return Observable.Create<HttpListenerContext>(observer =>
            {
                
                Task.Run(async () =>
                {
                    try
                    {
                        while (listener.IsListening)
                        {
                            var context = await listener.GetContextAsync();

                            ThreadPoolScheduler.Instance.Schedule(() =>
                            {
                                try
                                {
                                    observer.OnNext(context);
                                }
                                catch (Exception ex)
                                {
                                    observer.OnError(ex);
                                }

                            });

                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
                return Disposable.Empty;
            });
        }

        public void Run()
        {
            Console.WriteLine("Webserver je pokrenut...");
            IObservable<HttpListenerContext> obs = this.GetContextAsync();

            this.observableSubscription = obs.Subscribe(
                context =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    if (request.RawUrl == "/favicon.ico")
                    {
                        return;
                    }

                    this.responderObservable(request).Subscribe(
                          fixture =>
                          {
                              Console.WriteLine("Vracanje podataka za: " + request.RawUrl);

                              byte[] buf = Encoding.UTF8.GetBytes(fixture.ToString());
                              response.ContentLength64 = buf.Length;
                              response.OutputStream.Write(buf, 0, buf.Length);

                              context.Response.OutputStream.Close();
                          },
                          err =>
                          {
                              Console.WriteLine("Greska za: " + request.RawUrl);

                              byte[] buf = Encoding.UTF8.GetBytes(err.Message);
                              response.ContentLength64 = buf.Length;
                              response.OutputStream.Write(buf, 0, buf.Length);

                              context.Response.OutputStream.Close();

                              Console.WriteLine("Error: " + err.Message);
                          });
                    

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
