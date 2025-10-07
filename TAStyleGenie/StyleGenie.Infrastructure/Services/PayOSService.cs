using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TryOn.Api.Services
{
    public class PayOSService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public PayOSService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        // ✅ Tạo link thanh toán
        public async Task<JObject> CreatePaymentLinkAsync(long orderCode, decimal amount, string description, string cancelUrl, string returnUrl)
        {
            var sorted = new SortedDictionary<string, string>
            {
                { "amount", amount.ToString("0") },
                { "cancelUrl", cancelUrl },
                { "description", description },
                { "orderCode", orderCode.ToString() },
                { "returnUrl", returnUrl }
            };

            var rawData = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
            var signature = ComputeHmacSha256(rawData, _config["PayOS:ChecksumKey"]);

            var body = new
            {
                orderCode,
                amount,
                description,
                cancelUrl,
                returnUrl,
                signature
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config["PayOS:BaseUrl"]}/payment-requests")
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-client-id", _config["PayOS:ClientId"]);
            request.Headers.Add("x-api-key", _config["PayOS:ApiKey"]);

            var response = await _http.SendAsync(request);
            var text = await response.Content.ReadAsStringAsync();

            return JObject.Parse(text);
        }

        // ✅ Xác minh chữ ký webhook
        public string ComputeSignature(string dataJson)
        {
            var dataObj = JObject.Parse(dataJson);
            var dict = dataObj.Properties().ToDictionary(p => p.Name, p => p.Value.ToString());
            var sorted = new SortedDictionary<string, string>(dict);
            var raw = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
            return ComputeHmacSha256(raw, _config["PayOS:ChecksumKey"]);
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
