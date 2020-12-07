using System;
using System.IO;
using Arrowhead;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ArrowheadProducer
{
    class Program
    {
        private Arrowhead.Client client;

        public Program(JObject config)
        {
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings(config);
            this.client = new Arrowhead.Client(settings);
        }

        static void Main(string[] args)
        {
            JObject config = JObject.Parse(File.ReadAllText(@"producer.json"));
            Program producer = new Program(config);
        }
    }
}
