namespace Opuno.Brenn.Website.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    using global::Website.Tests.Opuno.Brenn.WindowsPhone.Helpers;

    using Opuno.Brenn.Models;

    public class ServiceHelper
    {
        private const string ServiceUrl = "http://localhost:5600/Brenn.svc/";
        private readonly WebClient webClient = new WebClient();

        public static readonly ServiceHelper Instance = new ServiceHelper();

        private ServiceHelper()
        {
            this.webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
        }

        public void Queue<TModel, T>(string action, Action<T> callback, TModel model, params KeyValuePair<string, string>[] arguments)
        {
            var argumentText = string.Join("&", arguments.Select(a => a.Key + "=" + a.Value).ToArray());
            var uri = new Uri(ServiceUrl + action + "?" + argumentText);
            var modelJson = JsonHelper.Serialize(model);
            var modelBytes = Encoding.UTF8.GetBytes(modelJson);

            Debug.WriteLine("Requesting (string) {0} async.", uri);

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            //request.Headers["Content-Length"] = modelBytes.Length.ToString(CultureInfo.InvariantCulture);

            Action<IAsyncResult> getResponse = ar =>
            {
                var response = request.EndGetResponse(ar);

                using (var stream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        var jsonString = streamReader.ReadToEnd();
                        var result = JsonHelper.Deserialize<T>(jsonString);
                        callback(result);
                    }
                }
            };

            Action<IAsyncResult> getRequestStream = ar =>
            {
                using (var stream = request.EndGetRequestStream(ar))
                {
                    stream.Write(modelBytes, 0, modelBytes.Length);
                    stream.Flush();
                    stream.Close();
                }

                request.BeginGetResponse(new AsyncCallback(getResponse), null);
            };


            request.BeginGetRequestStream(new AsyncCallback(getRequestStream), null);
        }

        public void Queue<T>(string action, Action<T> callback, params KeyValuePair<string, string>[] arguments)
        {
            var argumentText = string.Join("&", arguments.Select(a => a.Key + "=" + a.Value).ToArray());
            var uri = new Uri(ServiceUrl + action + "?" + argumentText);

            Debug.WriteLine("Requesting (string) {0} async.", uri);

            this.webClient.DownloadStringAsync(uri, callback);
        }

        static void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine("Completed web request (string) with exception: " + e.Error);
                throw new Exception("Failed to execute web request, see inner exception.", e.Error);
            }

            if (e.Cancelled)
            {
                Debug.WriteLine("Web request (string) was cancelled.");
                return;
            }

            Debug.WriteLine("Completed web request (string).");

            if (e.UserState is Action<List<Trip>>)
            {
                ((Action<List<Trip>>)e.UserState)(JsonHelper.Deserialize<List<Trip>>(e.Result));
            }
            else if (e.UserState is Action<List<Expense>>)
            {
                ((Action<List<Expense>>)e.UserState)(JsonHelper.Deserialize<List<Expense>>(e.Result));
            }
            else
            {
                throw new NotImplementedException("The response was not handled.");
            }
        }
    }
}