using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace SIS.HTTP
{
    public class HttpServer : IHttpServer
    {
        private readonly TcpListener tcpListener;
        //TODO : actions
        public HttpServer(int port)
        {
            this.tcpListener = new TcpListener(IPAddress.Loopback,port);
            
        }
        public async Task ResetAsync()
        {
            this.Stop();
            await this.StartAsync();
        }

        public async Task StartAsync()
        {
            this.tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                await Task.Run(() => ProcessClientAsync(tcpClient));
            }
        }

        public void Stop()
        {
            this.tcpListener.Stop();
        }
        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            using NetworkStream networkStream = tcpClient.GetStream();//poluchava danni/info vuv formata na baitove
            byte[] requestBytes = new byte[1000000]; //4096 e obshtoprieto da e edin bufer - 4kb
            int bytesRead = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length); //zapishi mi ot 0 , tolkova na broi baitove, kolkoto mi e masiva i gi zapishi v requestBytes(1voto)
                                                                                          //chete dannite/preobrazuva gi ot baitove kum tekst primerno
            string requestAsString = Encoding.UTF8.GetString(requestBytes, 0, bytesRead); //zapochni ot 0 i vzemi tolkova na broi baita kolkoto si prochel, ot tqh konventirai string s pravilata s utf-8
            
            var request = new HttpRequest(requestAsString);
            string content = "<h1>random page</h1>";
            if(request.Path == "/")
            {
                content = "<h1>home page</h1>";
            }
            else if(request.Path == "/users/login")
            {
                content = "<h1>login page</h1>";
            }

            byte[] stringContent = Encoding.UTF8.GetBytes(content);
            var response = new HttpResponse(HttpResponseCode.Ok, stringContent);
            response.Headers.Add(new Header("Server", "SoftUniServer/1.0"));
            response.Headers.Add(new Header("Content-Type", "text/html"));
            response.Cookies.Add(new ResponseCookie("sid", Guid.NewGuid().ToString()) { HttpOnly = true, MaxAge = 3600, });

            byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
            await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            await networkStream.WriteAsync(response.Body, 0, response.Body.Length);


            Console.WriteLine(requestAsString); //request
            Console.WriteLine(new string('=', 60));
            }
        }
}

