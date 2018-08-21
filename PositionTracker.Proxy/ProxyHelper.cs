using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using PositionTracker.Utility;

namespace PositionTracker.Proxy
{
    public static class ProxyHelper
    {
        public static HttpResponseMessage GetHttpResponse(this HttpClient httpClient, HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            try
            {
                response = httpClient.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var error = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode == (HttpStatusCode) 418)
                    {
//                        throw new BinanceIpBanException(error);
                    }
                    else if (response.StatusCode == (HttpStatusCode) 429)
                    {
//                        throw new BinanceRateExceededException(error);
                    }

                    throw new HttpRequestException($"Url: {request.RequestUri} " +
                                                   $"Status: {response.StatusCode} " +
                                                   $"Content: {response.Content}" +
                                                   $"Error: {error}");
                }

                string content;

                if (response.Content.Headers.ContentEncoding.All(x => x != "gzip"))
                {
                    content = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    using (var stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        using (var decompressed = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var reader = new StreamReader(decompressed))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                }

//                return content;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetHttp Error", ex);

//                if (ex is BinanceRateExceededException || ex is BinanceIpBanException)
                {
                    Logger.LogFatal("Binance IP BANNED!");

                    throw;
                }
            }

            return null;
        }

        public static string GetRawHttp(string pUrl, int timeout = 5000)
        {
            var data = string.Empty;

            var request = (HttpWebRequest) WebRequest.Create(pUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = timeout;

            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var stream = response?.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var rawResponse = string.Empty;

                if (ex.Response != null)
                {
                    using (var responseStream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            rawResponse = reader.ReadToEnd();
                        }
                    }
                }

                Logger.LogProxy($"Url: {pUrl} Response: {rawResponse}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Url: {pUrl}", ex);
            }

            return data;
        }
    }
}