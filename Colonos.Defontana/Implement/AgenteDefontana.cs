using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Defontana
{
    public class AgenteDefontana
    {
        Logger logger;
        cnnDF cnndf;
        public AgenteDefontana(Logger _logger, cnnDF _cnnDF)
        {
            logger = _logger;
            cnndf = _cnnDF;
        }
        public string RenovarToken(string metodo)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            logger.Info("RenovarToken...");
            string url = String.Format("{0}{1}?client={2}&company={3}&user={4}&password={5}", cnndf.baseurl,  metodo, cnndf.cliente, cnndf.company, cnndf.user, cnndf.pass);
            logger.Info("RenovarToken, url: {0}",url);
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            logger.Info("response.ErrorMessage: {0}", response.ErrorMessage);
            logger.Info("response.ErrorException: {0}", response.ErrorException);
            logger.Info("response.ProtocolVersion: {0}", response.ProtocolVersion);
            logger.Info("response.StatusCode: {0}", response.StatusCode);
            logger.Info("response.StatusDescription: {0}", response.StatusDescription);
            logger.Info("response.Content: {0}", response.Content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var auth = JsonConvert.DeserializeObject<TokenResult>(response.Content);
                if (auth != null)
                {
                    if (auth.success)
                    {
                        var token = String.Format("{0} {1}", auth.token_type, auth.access_token);
                        return token;
                    }
                    else
                    {
                        logger.Error("RenovarToken: {0}", auth.message);
                    }
                }
            }
            logger.Error("RenovarToken: {0}", response.Content);
            return "";
        }

        public string ExecutePost(string metodo, string token, string json, ref bool success)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var url = String.Format("{0}{1}", cnndf.baseurl, metodo);

            var client = new RestClient(url);
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                success = true;
                return response.Content;
            }
            success = false;

            logger.Error("DF ExecutePost: {0} {1}", response.StatusDescription, response.Content);
            return "";
        }

        public string ExecuteGet(string metodo, string token, string json, ref bool success)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var url = String.Format("{0}{1}", cnndf.baseurl, metodo);
            var client = new RestClient(url);
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-type", "application/json");

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                success = true;
                return response.Content;
            }
            success = false;
            return response.Content;
        }
    }
}
