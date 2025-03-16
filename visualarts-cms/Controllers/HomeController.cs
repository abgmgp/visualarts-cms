using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace visualarts_cms.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var viewmodel = new InquiryViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitInquiry(InquiryViewModel viewModel)
        {
            try
            {

                #region sql
                conn.Open();

                SqlCommand insertQuery = new SqlCommand("INSERT INTO Inquiries " +
                    "(Name, Email, Message, IsActive, CreatedDate) " +
                    "VALUES (@Name, @Email, @Message, @IsActive, @CreatedDate)", conn);
                insertQuery.Parameters.Add(new SqlParameter("@Name", viewModel.Name));
                insertQuery.Parameters.Add(new SqlParameter("@Email", viewModel.Email));
                insertQuery.Parameters.Add(new SqlParameter("@Message", viewModel.Message ?? ""));
                insertQuery.Parameters.Add(new SqlParameter("@IsActive", true));
                insertQuery.Parameters.Add(new SqlParameter("@CreatedDate", DateTime.Now));

                insertQuery.ExecuteNonQuery();

                #endregion

                #region email sending

                var toEmail = ConfigurationManager.AppSettings["SmtpEmailRecipient"];
                var email = ConfigurationManager.AppSettings["SmtpEmailUser"];
                var pass = ConfigurationManager.AppSettings["SmtpEmailPass"];

                try
                {
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(email, pass),
                        EnableSsl = true
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(email),
                        Subject = "Inquiry from " + viewModel.Name + "(" + viewModel.Email + ") - ICT Visual Arts",
                        Body = viewModel.Message,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("Email could not be sent. " + ex.Message);
                }
                #endregion

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}