using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Win32;

namespace proxycheck
{
    public class CheckService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly string[] UrlsToCheck = {
            "https://www.here.com",
            "https://www.box.com",
            "https://logiseu.beyondtrustcloud.com/",
            "https://logis.bomgarcloud.com/",
            "https://intra.logis.dk/LogisWeb/Account/Login"

        };

        public CheckService(HttpClient httpClient) => _httpClient = httpClient;

        public string GetProxyConf()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(UrlsToCheck[0]);
                IWebProxy? proxy = req.Proxy;

                return proxy is not null
                    ? $"Proxy URI: '{proxy.GetProxy(req.RequestUri)}', a {proxy}"
                    : "No proxy configured by default.";
            } catch (Exception ex)
            {
                return $"Had an issue, {ex}";
            }
        }

        public async Task<string> CheckUrls()
        {
            try
            {
                var tasks = UrlsToCheck.Select(x => _httpClient.GetAsync(x));
                var results = await Task.WhenAll(tasks.ToArray());
                var fails = results.Where(x => !x.IsSuccessStatusCode);

                return fails.Count() > 0
                    ? $@"The following ({fails.Count()}) failed to get successfully; {String.Join(Environment.NewLine, fails.Select(x => $"{x.StatusCode} - {x.RequestMessage.RequestUri}"))}"
                    : $"All {UrlsToCheck.Count()} passed.";
            }
            catch (Exception ex)
            {
                return $"An error occured, {ex}";
            }
        }

        public string GetIEProxy()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
                if (key is null) { return "No IE proxy configured"; }
                var proxyEnable = (int)key.GetValue("ProxyEnable", 0) != 0;
                var proxyString = (string)key.GetValue("ProxyServer", "None");
                return proxyEnable ? $"WinINET Proxy set, and is {proxyString}" : "WinINET Proxy not set.";
            } catch (Exception ex)
            {
                return $"Could not check WinINET Proxy, {ex}";
            }
        }
    }

    public record Joke(int Id, string Type, string Setup, string Punchline);
}

