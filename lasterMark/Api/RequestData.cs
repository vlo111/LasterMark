namespace lasterMark.Api
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class RequestData
    {
        public static async Task<string> GetRequestAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return await task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        public static async Task<string> GetAllEventsAsync(string url, string headerParam)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Headers["Authorization"] = headerParam;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return await task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responseStream ?? throw new InvalidOperationException()))
                {
                    string strContent = sr.ReadToEnd();
                    return strContent;
                }
            }
        }

    }
}
