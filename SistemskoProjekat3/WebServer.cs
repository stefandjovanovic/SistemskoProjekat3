using SistemskoProjekat3.Modules;
using SistemskoProjekat3.Services;
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
        private FixtureService fixtureService;
        private IDisposable? observableSubscription;


        public WebServer(params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("URI ne sadrzi adekvatan broj parametara");
            }

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            this.fixtureService = new FixtureService();

            listener.Start();
        }

        private IObservable<HttpListenerContext> GetContextObservableFromTask()
        {
            return Observable.Create<HttpListenerContext>(observer =>
            {

                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        while (listener.IsListening)
                        {
                            var context = await listener.GetContextAsync();

                            TaskPoolScheduler.Default.Schedule(() =>
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
                }, TaskCreationOptions.LongRunning);
                return Disposable.Empty;
            });
        }

        private IObservable<HttpListenerContext> GetContextObservableFromAsync()
        {
            return Observable
                .FromAsync(listener.GetContextAsync)
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Repeat()
                .TakeWhile( _ => listener.IsListening);
        }


        public void Run()
        {
            Console.WriteLine("Webserver je pokrenut...");

            IObservable<HttpListenerContext> obs = this.GetContextObservableFromAsync();

            //IObservable<HttpListenerContext> obs = this.GetContextObservableFromTask();

            this.observableSubscription = obs.Subscribe(
                context =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    if (request.RawUrl == "/favicon.ico")
                    {
                        return;
                    }

                    this.fixtureService.GetFixturesObservable(request)
                        .Take(1)
                        .Subscribe(
                          fixture =>
                          {
                              Console.WriteLine("Vracanje podataka za: " + request.RawUrl + " Thread: " + Thread.CurrentThread.ManagedThreadId);

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
