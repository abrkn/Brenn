namespace Opuno.Brenn.WindowsPhone
{
    using System.Collections.Generic;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Opuno.Brenn.Models;
    using System.Linq;

    using Opuno.Brenn.WindowsPhone.Helpers;

    public class ServiceHelper
    {
        private const string ServiceUrl = "http://192.168.70.1:5601/Brenn.svc/";
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
            ////request.Headers["Content-Length"] = modelBytes.Length.ToString(CultureInfo.InvariantCulture);

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
