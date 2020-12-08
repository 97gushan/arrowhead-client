using System;
using System.IO;
using Arrowhead;
using Newtonsoft.Json.Linq;
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

            using (var server = new RestServer())
            {
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

    [RestResource]
    public sealed class TestResource
    {
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
            context.Response.SendResponse("Hello, world.");
            return context;
        }
    }
}
