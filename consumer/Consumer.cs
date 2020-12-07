using System;
using System.IO;
using Arrowhead;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ArrowheadConsumer
{
    class Program
    {
        private Arrowhead.Admin admin;
        private Arrowhead.Client client;

        public Program(string serviceName, string systemName, string certPath)
        {
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings();
            settings.SetCertPath(certPath);
            settings.SetServiceSettings(serviceName, new string[] { "HTTPS-SECURE-JSON" }, "/");
            settings.SetSystemSettings(systemName, "127.0.0.1", "8080");
            this.client = new Arrowhead.Client(settings);
        }

        public Program(JObject consumerConfig, JObject adminConfig)
        {
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings(consumerConfig);
            this.client = new Arrowhead.Client(settings);

            Arrowhead.Utils.Settings adminSettings = new Arrowhead.Utils.Settings(adminConfig);
            this.admin = new Arrowhead.Admin(adminSettings);
            this.admin.StoreOrchestrate(this.client.GetSystemId());

            Console.WriteLine(this.client.Orchestrate());
        }

        static void Main(string[] args)
        {
            JObject consumerConfig = JObject.Parse(File.ReadAllText(@"consumer.json"));
            JObject adminConfig = JObject.Parse(File.ReadAllText(@"admin.json"));
            Program consumer = new Program(consumerConfig, adminConfig);
        }
    }
}
