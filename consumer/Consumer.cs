using System;
using System.IO;
using Arrowhead;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Grapevine.Client;
using Grapevine.Shared;


namespace ArrowheadConsumer
{
    class Program
    {
        private Arrowhead.Admin admin;
        private Arrowhead.Client client;

        private string producerHost = "";
        private string producerPort = "";
        private string serviceUri = "";
        private bool producerSSL = false;

        public Program(JObject consumerConfig, JObject adminConfig)
        {
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings(consumerConfig);
            this.client = new Arrowhead.Client(settings);

            Arrowhead.Utils.Settings adminSettings = new Arrowhead.Utils.Settings(adminConfig);
            this.admin = new Arrowhead.Admin(adminSettings);
            this.admin.StoreOrchestrate(this.client.GetSystemId());

            JArray orchestrations = this.client.Orchestrate();
            JObject orchestration = (JObject)orchestrations[0];

            this.producerHost = orchestration.SelectToken("provider.address").ToString();
            this.producerPort = orchestration.SelectToken("provider.port").ToString();
            this.serviceUri = orchestration.SelectToken("serviceUri").ToString();
            JArray interfaces = (JArray)orchestration.SelectToken("interfaces");

            if (interfaces[0].SelectToken("interfaceName").ToString() == "HTTPs-SECURE-JSON")
            {
                producerSSL = true;
            }
            else
            {
                producerSSL = false;
            }

            Console.WriteLine("Orchestration against http" + (producerSSL ? "s://" : "://") + this.producerHost + ":" + this.producerPort + this.serviceUri + " was started");
        }

        public void ConsumeService()
        {
            RestClient client = new RestClient();

            client.Host = this.producerHost;
            client.Port = Int32.Parse(this.producerPort);

            RestRequest req = new RestRequest(this.serviceUri + "/demo");
            req.HttpMethod = HttpMethod.GET;
            while (true)
            {
                RestResponse resp = (RestResponse)client.Execute(req);
                Console.WriteLine(JsonConvert.DeserializeObject<JObject>(resp.GetContent()));
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void Main(string[] args)
        {
            JObject consumerConfig = JObject.Parse(File.ReadAllText(@"consumer.json"));
            JObject adminConfig = JObject.Parse(File.ReadAllText(@"admin.json"));
            Program consumer = new Program(consumerConfig, adminConfig);
            consumer.ConsumeService();
        }
    }
}
