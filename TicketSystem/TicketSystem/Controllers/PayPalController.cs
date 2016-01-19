using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketSystem.Models;
using TicketSystem.ViewModel;

namespace TicketSystem.Controllers
{
    [RequireHttps]
    public class PayPalController : Controller
    {

        // change when live
       // [Authorize]
        // GET: PayPal
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult PaymentWithPaypal(OrderView orders)
        {

            int totalTicketsNum = orders.AmountOfTickets;


            if(totalTicketsNum > 4)
            {
                ViewBag.Error = "Maximum of 4 tickets per transaction. Thank you.";
                return View("Index");
            }

            string totalCost = Convert.ToString(totalTicketsNum * 20.50);

            using (var db = new DataContext())
            {
                var ticketAmount = db.Orders.SingleOrDefault();

                if (ticketAmount.AmountOfTickets < totalTicketsNum)
                {
                    ViewBag.Error = "There is " + ticketAmount.AmountOfTickets + " Ticekts left.";

                    return View("Index");
                }
            }






          

            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                Random rnd = new Random();
                int prefix = rnd.Next(1, 9999999);

                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist

                    //it is returned by the create function call of the payment class

                    // Creating a payment

                    // baseURL is the url on which paypal sendsback the data.

                    // So we have provided URL of this controller only

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session

                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url

                    //on which payer is redirected for paypal acccount payment
                    string totalTicketsString = Convert.ToString(totalTicketsNum);
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, totalTicketsString, totalCost);



                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("----------------");
                Debug.WriteLine(ex.InnerException);
                return View("FailureView");
            }

            using (var db = new DataContext())
            {
                var ticketAmount = db.Orders.SingleOrDefault();
                ticketAmount.AmountOfTickets = ticketAmount.AmountOfTickets - totalTicketsNum;
                db.SaveChanges();
            }

            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }


        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string totalTicketsNum,string totalCost)
        {
           // string fees = "0.50";

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = "Gary Moore Ticket",
                currency = "GBP",
                price = "20.50",
                quantity = totalTicketsNum,
                sku="Ticket"
               
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                fee= "0.00",
                shipping = "0.00",
                subtotal = totalCost
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "GBP",
                total = totalCost, // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = "your invoice number",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }


    }
}