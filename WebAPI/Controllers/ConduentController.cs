using Algorithmia;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ConduentController : ApiController
    {

        [HttpPost]
        public Response Post(Request request)
        {
            string responseText = "Default response";

           
            try
            {

                if (request.QueryResult != null)
                {
                    //Check the Intent Name
                    switch (request.QueryResult.Intent.DisplayName.ToLower())
                    {
                        case "compensation":

                            if (request.QueryResult.Parameters["variablepay"].ToString() != "")
                            {
                               // userRequest = request.QueryResult.Parameters["variablepay"].ToString();
                                responseText = GetTopicNumberandDocument("variable");
                            }
                            else {
                                responseText = "Only variable pay document is avialable";
                            }

                            
                            break;

                        case "innovation":

                            if (request.QueryResult.Parameters["ctiupclose"].ToString() != "")
                            {
                               
                                responseText = GetTopicNumberandDocument("cti");
                            }
                            else
                            {
                                responseText = "Only CtiUpClose document is avialable";
                            }


                            break;

                        case "social-network":

                            if (request.QueryResult.Parameters["yammer"].ToString() != "")
                            {
                                responseText = GetTopicNumberandDocument("yammer");
                            }
                            else
                            {
                                responseText = "Only yammer document is avialable";
                            }


                            break;

                        case "survey":

                            if (request.QueryResult.Parameters["survey"].ToString() != "")
                            {
                                responseText = GetTopicNumberandDocument("survey");
                            }
                            else{
                                responseText = "Only Engagement Survey document is avialable";
                            }

                            
                            break;

                        case "spam":

                            if (request.QueryResult.Parameters["phishing"].ToString() != "")
                            {
                                responseText = GetTopicNumberandDocument("phishing");
                            }
                            else{
                                responseText = "Only phishing document is avialable";
                            }

                            
                            break;

                        default:
                            responseText = "Results are not found";
                            break;
                    }
                }

                return new Response() { fulfillmentText = responseText, source = $"API.AI" };
            }

            catch (Exception ex)
            {
                return new Response() { fulfillmentText = "Api Error", source = $"API.AI" };
            }
        }


      

        public static string GetTopicNumberandDocument(string userRequest)
        {

            string responseText = "";
            try
            {
                string TopicNumber = null;

                string docsList = "["
     + "    \"Availability of variable compensation details under Personal Information. There was an enhancement to include current variable compensation components under Compensation tab. Navigation : Personal Information > Compensation>Additional Compensation\","
     + "    \"One of our goals in launching CTI Up Close is to help you learn more about what we do. Today we’re taking a deeper dive into data analytics. We see a tremendous opportunity to offer our clients insights and solutions derived from analysis of their data. Read the new issue of CTI Up Close to find out what we’re developing in each of the following areas so we can realize the opportunity before us: A common analytics platform for all Conduent analytics applications\","
     + "    \"Each of us has a voice, and a role, in building the company we aspire to be. Two weeks ago, we launched Engagement@Conduent, our first-ever companywide engagement survey, to gather insights into how you feel about working at Conduent. Our goal is simple: to gather your honest and candid feedback around a few key questions — the answers to which could create meaningful, positive impacts on our ability to make Conduent an employer of choice. Your feedback is so vital to our engagement and improvement efforts that we are extending the survey deadline to Wednesday, Sept. 19, to ensure everyone has the opportunity to participate. The survey takes about 10 to 15 minutes to complete, and all responses are confidential and anonymous to Conduent. You can access the survey directly here, as well as through the ConduentConnect homepage (Quick Links > Engagement@Conduent survey).\","
     + "    \"As part of our ongoing efforts to combat spam and phishing emails, Conduent’s spam filters are now quarantining incoming spam and phishing emails, as identified by our email system. The emails are being quarantined for up to 14 days and then are deleted automatically. To ensure valid emails are being delivered to our employees and not quarantined, an alerting feature will be enabled on September 14, 2018. Once implemented, the system will send an alert to employees whose email was quarantined. If you believe your quarantined email is valid, contact the ITO helpdesk at Helpdesk.ITO@atos.net within the 14-day quarantine period for assistance in releasing the email to you.\","
     + "    \"We’re excited to announce our next companywide YamJam session with four of our global leaders. The YamJam is an opportunity for everyone across Conduent to join in a conversation on our internal social network, Yammer, for a real-time, online discussion about building a unified, “One Conduent” culture.Each of us plays an important role in helping shape Conduent’s culture. Building on our core values YamJam in May, we are taking the conversation a step further for insights and suggestions about how we can create a unified culture. Make sure you have a profile on Yammer. Here’s a simple guide to Yammer, including how to create a profile.\","
     + "  ]";


                //           string basepath = "https://fullfillmentapiraj.azurewebsites.net/Files/";

                //           string docsList = "["
                //+ "\"" + File.ReadAllText(Path.Combine(basepath, "compensation.txt"), System.Text.Encoding.Default) + "\"" + ","
                //+ "\"" + File.ReadAllText(Path.Combine(basepath, "ctiUpClose.txt"), System.Text.Encoding.Default) + "\"" + ","
                //+ "\"" + File.ReadAllText(Path.Combine(basepath, "engagementsurvey.txt"), System.Text.Encoding.Default) + "\"" + ","
                //+ "\"" + File.ReadAllText(Path.Combine(basepath, "spam.txt"), System.Text.Encoding.Default) + "\"" + ","
                //+ "\"" + File.ReadAllText(Path.Combine(basepath, "yamjam.txt"), System.Text.Encoding.Default) + "\"" + ","
                //+ "]";


                //           var input = "{"
                //+ "  \"docsList\":" + docsList + ","
                //+ "  \"mode\": \"quality\""
                //+ "}";



                var input = "{"
    + "  \"docsList\":" + docsList + ","
    + " \"customSettings\": {"
    + "         \"numTopics\":" + 10 + ","
    + "         \"numIterations\":" + 300 + ","
    + "         \"numWords\":" + 10
    + "         }"
    + "}";

                var client = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                var algorithm = client.algo("nlp/LDA/1.0.0");
                var response = algorithm.pipeJson<object>(input);

                var data = response.result.ToString().Replace("{[", "[").Replace("}]", "]");

                var jresult = JArray.Parse(data).Children().ToList();

                for (int i = 0; i < jresult.Count; i++)
                {

                    if (jresult[i][userRequest] != null)
                    {
                        TopicNumber = Convert.ToString(i);
                        break;
                    }
                    else
                    {
                        TopicNumber = null;
                    }

                }

                

                if (TopicNumber != null)
                {

                    var secondinput = "{"
                   + " \"topics\":" + response.result + ","
                   + " \"docsList\":" + docsList
                   + "}";

                    var client1 = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                    var algorithm1 = client1.algo("nlp/LDAMapper/0.1.1");
                    var response1 = algorithm1.pipeJson<object>(secondinput);

                    var data1 = response1.result.ToString().Replace("{[", "[").Replace("}]", "]");

                    var jresult1 = JObject.Parse(data1)["topic_distribution"].Children().ToList();

                    Dictionary<int, decimal> docProbListByTopic = new Dictionary<int, decimal>();

                    for (int i = 0; i < jresult1.Count; i++)
                    {
                        var value = Convert.ToDecimal(jresult1[i]["freq"][TopicNumber]);
                        docProbListByTopic.Add(i, value);
                    }

                    var docIndex = docProbListByTopic.FirstOrDefault(x => x.Value == docProbListByTopic.Values.Max()).Key;

                    responseText = jresult1[docIndex]["doc"].ToString();
                }
                else
                {
                    responseText = "No documents is found for user requests";
                }

                return responseText;
            }
            catch (Exception ex)
            {
                responseText = ex.Message;

                return responseText;
            }

        }
      
    }
}

