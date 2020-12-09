using System;
using System.Linq;
using System.IO;
using Arrowhead;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Grapevine.Shared;
using Grapevine.Server;
using Grapevine.Interfaces.Server;
using Grapevine.Server.Attributes;


namespace ArrowheadProducer
{
    class Program
    {
        private Arrowhead.Client client;

        public Program(JObject config)
        {
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings(config);
            this.client = new Arrowhead.Client(settings);
            bool useSSL = settings.Interfaces.Any(i => i == "HTTPS-SECURE-JSON");
            using (var server = new RestServer())
            {
                server.UseHttps = useSSL;
                server.Host = config.SelectToken("system.ip").ToString();
                server.Port = config.SelectToken("system.port").ToString();
                server.LogToConsole().Start();
                Console.ReadLine();
                server.Stop();
            }
        }

        static void Main(string[] args)
        {
            JObject config = JObject.Parse(File.ReadAllText(@"producer.json"));
            Program producer = new Program(config);
        }
    }

    [RestResource(BasePath = "/producer")]  // base path must be set to the same value as the apiURI in the config
    public sealed class TestResource
    {
        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/demo")]
        public IHttpContext Demo(IHttpContext context)
        {
            Random rnd = new Random();

            JObject response = new JObject();
            response.Add("value", rnd.Next(1, 100));
            response.Add("timestamp", DateTime.Now);

            context.Response.ContentType = ContentType.JSON;
            context.Response.SendResponse(JsonConvert.SerializeObject(response));
            return context;
        }

        [RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/repeat")]
        public IHttpContext RepeatMe(IHttpContext context)
        {
            var word = context.Request.QueryString["word"] ?? "what?";
            context.Response.SendResponse(word);
            return context;
        }

        [RestRoute]
        public IHttpContext HelloWorld(IHttpContext context)
        {
            context.Response.SendResponse("Hello, world!");
            return context;
        }
    }
}
