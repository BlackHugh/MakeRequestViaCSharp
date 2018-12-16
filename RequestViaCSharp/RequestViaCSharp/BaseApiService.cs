using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RequestViaCSharp
{
    public class BaseApiService
    {
        protected HttpClient httpClient;

        public BaseApiService()
        {
            httpClient = new HttpClient(new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Automatic });
            httpClient.DefaultRequestHeaders.Accept.Add((new MediaTypeWithQualityHeaderValue("application/json")));
        }

        public BaseApiService(string user, string password)
        {
            httpClient = new HttpClient(new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Automatic });
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", user, password));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Accept.Add((new MediaTypeWithQualityHeaderValue("application/json")));

        }

        public string Get(string url)
        {
            var tGet = httpClient.GetAsync(url);

            HttpStatusCode code = HttpStatusCode.OK;
            String result = null;

            GetStatusAndResult(tGet, ref code, ref result);

            return result;
        }

        public string Post(string url, string requestBody)
        {
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var tPost = httpClient.PostAsync(url, content);

            HttpStatusCode code = HttpStatusCode.OK;
            String result = null;

            GetStatusAndResult(tPost, ref code, ref result);

            return result;
        }

        public string Put(string url, string requestBody)
        {
            StringContent content = new StringContent(requestBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var tPut = httpClient.PutAsync(url, content);

            HttpStatusCode code = HttpStatusCode.OK;
            String result = null;

            GetStatusAndResult(tPut, ref code, ref result);

            return result;
        }

        public string PostFile(string url, List<string> files)
        {
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            foreach (var file in files)
            {
                StringContent sc = new StringContent((new System.IO.StreamReader(file)).ReadToEnd());
                multipartContent.Add(sc, "file", Path.GetFileName(file));
            }
            var tPostFile = httpClient.PostAsync(url, multipartContent);
            var result = this.GetResult(tPostFile);

            return result;
        }

        public string Delete(string url)
        {
            var tDelete = httpClient.DeleteAsync(url);
            String result = null;

            result = GetResult(tDelete);

            return result;
        }

        private void GetStatusAndResult(Task<HttpResponseMessage> task, ref HttpStatusCode statusCode, ref string result)
        {

            task.Wait();

            var response = task.Result;
            var responseContent = response.Content;

            var tRead = responseContent.ReadAsStringAsync();

            statusCode = response.StatusCode;
            result = tRead.Result;

        }

        private string GetResult(Task<HttpResponseMessage> task)
        {
            string result = null;
            var tGetStatus = task.ContinueWith((requestTask) =>
            {
                HttpResponseMessage response = requestTask.Result;
                response.EnsureSuccessStatusCode(); //will throw exception if it's not ok;
                var t = response.Content.ReadAsStringAsync().ContinueWith((contentTask) =>
                {
                    result = contentTask.Result;
                });
                t.Wait();
            });
            tGetStatus.Wait();

            //result = r;
            return result;
        }
    }
}
