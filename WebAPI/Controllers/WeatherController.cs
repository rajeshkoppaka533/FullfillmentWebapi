using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class WeatherController : ApiController
    {

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Post api/Product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Response Post(Request request)
        {
            string responseText = "Default response";

            if (request.QueryResult != null)
            {
                //Check the Intent Name
                switch (request.QueryResult.Intent.DisplayName.ToLower())
                {
                    case "checkweather":
                        var location = request.QueryResult.Parameters["geo-city"].ToString();
                        responseText = GetTemperature(location);
                        break;
                    case "bookmovie":
                        var movienames = "Sure, Please find the latest movies: Incredibles 2, Sherlock Gnomes, Peter-rabbit, Isle of Dogs, Hanuman.";
                        responseText = movienames;
                        break;
                }
            }

            return new Response() { fulfillmentText = responseText, source = $"API.AI" };
        }

        /// <summary>
        /// Method to get Temparature by city
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static string GetTemperature(string city)
        {
            string temp = string.Empty;
            if (city == null)
            {
                return "City missing (but called the api)";
            }
            switch (city.ToLower())
            {
                case "washington":
                    temp = "36.3 degree";
                    break;
                case "delhi":
                    temp = "38.7 degree";
                    break;
                case "bangalore":
                    temp = "21.11 degree";
                    break;
                default:
                    temp = "0 degree";
                    break;
            }

            return "Temperature in " + city + " is " + temp;
        }
    }
}
