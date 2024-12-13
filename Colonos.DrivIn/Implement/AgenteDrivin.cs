using Colonos.Entidades.Defontana;
using Colonos.Entidades.Drivin;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DrivIn
{
    public class AgenteDrivin
    {
        Logger logger;
        cnnDrivin cnndrivin;
        public AgenteDrivin(Logger _logger, cnnDrivin _cnndrivin)
        {
            logger = _logger;
            cnndrivin = _cnndrivin;
        }

        public string ExecutePost(string metodo, string token, string json, ref bool success)
        {
            var url = String.Format("{0}{1}", cnndrivin.baseurl, metodo);

            var client = new RestClient(url);
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-API-Key", token);
            request.AddHeader("Content-type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            logger.Info("Drivin ExecutePost. url: {0}", url);
            logger.Info("Drivin ExecutePost. json: {0}", json);
            logger.Info("X-API-Key: {0}", token);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            logger.Info("Drivin ExecutePost. response.Content: {0} {1}", response.StatusCode, response.Content);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                success = true;
                return response.Content;
            }
            success = false;


            return response.Content;
        }

        public string ExecuteGet(string metodo, string token, string json, ref bool success)
        {
            var url = String.Format("{0}{1}", cnndrivin.baseurl, metodo);
            var client = new RestClient(url);
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-API-Key", token);
            request.AddHeader("Content-type", "application/json");
            logger.Info("Drivin ExecuteGet. url: {0}", url);
            logger.Info("X-API-Key: {0}", token);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            logger.Info("Drivin ExecuteGet. response.Content: {0} {1}", response.StatusCode, response.Content);
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
