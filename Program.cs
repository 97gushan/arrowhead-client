using System;
using ArrowHead;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace arrowhead_client
{
    class Program
    {
        private ArrowHead.Http http;
        private ArrowHead.Test t;
        public Program()
        {
            this.http = new ArrowHead.Http("https://postman-echo.com/");
            t = new ArrowHead.Test();

        }

        private void Run()
        {
            Console.WriteLine(this.t.GetHelloWorld());

            this.PostExample();

            while (true) ;
        }

        private async void PostExample()
        {
            Object json = new
            {
                a = "b",
                c = "d",
                e = new
                {
                    f = "g"
                }
            };
            HttpResponseMessage resp = await this.http.Post("post", json);
            string respMessage = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(JsonConvert.DeserializeObject(respMessage).data);
        }

        private async void GetExample()
        {
            HttpResponseMessage resp = await this.http.Get("get");
            Console.WriteLine(resp);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }
    }
}
