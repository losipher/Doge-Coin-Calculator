using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Doge_Coin_Calculator
{
    class VircurexConnection
    {
        /// <summary>
        /// Requests Last Traded Price
        /// </summary>
        /// <param name="Base">Base (Coin/Currency) Market</param>
        /// <param name="Alt">Alt (Coin/Currency) Market</param>>
        public decimal RequestLastPrice(string Base, string Alt)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "Nobody");

            var url = String.Format("https://api.vircurex.com/api/get_last_trade.json?base={0}&alt={1}", Base, Alt);

            var json = client.DownloadString(new Uri(url));
            var jObject = JObject.Parse(json);
            var value = jObject.GetValue("value");

            return (decimal)value;
        }
    }
}
