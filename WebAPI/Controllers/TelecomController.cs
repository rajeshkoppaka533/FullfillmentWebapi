using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class TelecomController : ApiController
    {

        [HttpPost]
        public Response Post(Request request)
        {
            string responseText = "Default response";

            var mobile = "";

            try
            {

                if (request.QueryResult != null)
                {
                    //Check the Intent Name
                    switch (request.QueryResult.Intent.DisplayName.ToLower())
                    {
                        case "authorize":

                            if (request.QueryResult.Parameters["phone-number"].ToString() != "")
                            {
                                mobile = request.QueryResult.Parameters["phone-number"].ToString();
                            }
                            else
                            {
                                mobile = request.QueryResult.Parameters["number"].ToString();
                            }

                            responseText = Authorize(mobile);
                            break;

                        case "total":

                            string result = "";

                            if (request.QueryResult.Parameters["customerTotalDetails"] != null)
                            {
                                var requestTotal = request.QueryResult.Parameters["customerTotalDetails"].ToString();

                                if (request.QueryResult.OutputContexts[0].Parameters["phone-number"].ToString() != "")
                                {
                                    mobile = request.QueryResult.OutputContexts[0].Parameters["phone-number"].ToString();
                                }
                                else
                                {
                                    mobile = request.QueryResult.OutputContexts[0].Parameters["number"].ToString();
                                }


                                if (requestTotal.ToLower().Contains("amount"))
                                {

                                    result = TotalAmount(mobile);
                                }
                                else if (requestTotal.ToLower().Contains("topup"))
                                {
                                    result = TotalTopup(mobile);
                                }
                                else
                                {
                                    responseText = "";
                                }
                            }

                            responseText = result;
                            break;



                    }
                }

                return new Response() { fulfillmentText = responseText, source = $"API.AI" };
            }

            catch (Exception ex) {
                return new Response() { fulfillmentText = "Api Error", source = $"API.AI" };
            }



            
        }


        public static string Authorize(string mobile)
        {
            try
            {

                string connectionString = "Data Source=conduentcti.database.windows.net,1433;Initial Catalog=TelecomDB;User Id=conduentadmin;Password=Jimmer01;MultipleActiveResultSets=true";

                SqlConnection con = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("select CustId from [TelecomDB].[dbo].[Customers] where mobile = " + mobile, con)
                {
                    CommandType = CommandType.Text
                };

                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                con.Open();
                sd.Fill(dt);
                con.Close();

                string response = "";

                if (dt.Rows.Count > 0)
                {
                    response = "Authorization successfull. Please tell us what do you want to know either Total Amount Payed/ Total Topup Added?";
                }
                else
                {
                    response = "Authorization is failed. Please provide correct number";
                }

                return response;
            }

            catch (Exception ex)
            {
                return "Authorization is failed. Please provide correct number";
            }

        }

        public static string TotalAmount(string mobile)
        {
            try
            {

                int custId = CheckAuthentication(mobile);

                string totalAmount = "";

                if (custId > 0)
                {

                    string connectionString = "Data Source=conduentcti.database.windows.net,1433;Initial Catalog=TelecomDB;User Id=conduentadmin;Password=Jimmer01;MultipleActiveResultSets=true";

                    SqlConnection con = new SqlConnection(connectionString);

                    SqlCommand cmd = new SqlCommand("select sum(Amount) as TotalAmount from [PaymentDetails] where CustomerId = " + custId, con)
                    {
                        CommandType = CommandType.Text
                    };

                    SqlDataAdapter sd = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    con.Open();
                    sd.Fill(dt);
                    con.Close();


                    var data = dt.Rows[0];

                    totalAmount = data["TotalAmount"].ToString();
                    return totalAmount;

                }
                else
                {
                    totalAmount = "Sorry, We are not able to fetch your data!";
                    return totalAmount;
                }
            }

            catch (Exception ex)
            {
                return "Sorry, We are not able to fetch your data!"; ;
            }

        }


        public static string TotalTopup(string mobile)
        {
            try
            {

                int custId = CheckAuthentication(mobile);

                string totalTopup = "";

                if (custId > 0)
                {

                    string connectionString = "Data Source=conduentcti.database.windows.net,1433;Initial Catalog=TelecomDB;User Id=conduentadmin;Password=Jimmer01;MultipleActiveResultSets=true";

                    SqlConnection con = new SqlConnection(connectionString);

                    SqlCommand cmd = new SqlCommand("select sum(TopupBalance) as TotalTopup from [TelecomDB].[dbo].[Topups] where TopupId in (select TopupId from [TelecomDB].[dbo].[PaymentDetails] where CustomerId = " + custId + ")", con)
                    {
                        CommandType = CommandType.Text
                    };

                    SqlDataAdapter sd = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    con.Open();
                    sd.Fill(dt);
                    con.Close();


                    var data = dt.Rows[0];

                    totalTopup = data["TotalTopup"].ToString();
                    return totalTopup;
                }
                else
                {
                    totalTopup = "Sorry, We are not able to fetch your data!";
                    return totalTopup;
                }

            }

            catch (Exception ex)
            {
                return "Sorry, We are not able to fetch your data!"; ;
            }

        }



        public static int CheckAuthentication(string mobile)
        {

            try
            {

                string connectionString = "Data Source=conduentcti.database.windows.net,1433;Initial Catalog=TelecomDB;User Id=conduentadmin;Password=Jimmer01;MultipleActiveResultSets=true";

                SqlConnection con = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("select CustId from [TelecomDB].[dbo].[Customers] where mobile = " + mobile, con)
                {
                    CommandType = CommandType.Text
                };

                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                con.Open();
                sd.Fill(dt);
                con.Close();

                int response = 0;

                if (dt.Rows.Count > 0)
                {
                    var data = dt.Rows[0];
                    response = Convert.ToInt32(data["CustId"]);
                }

                return response;
            }

            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
