using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CoronaV2.Models;

namespace CoronaV2.Controllers
{
    public class UserController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("Index", "Home");
            }
            //ViewBag.CountryList = new SelectList(GetCountryList(),"CountryID","Country");
            return View();
        }
            
        //Registration POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")]User user)
        {
            bool Status= false;
            string message = "";
            //
            // Model Validation
            if (ModelState.IsValid)
            {
                #region Email is already Exist
                
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Action Code    
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;

                #region Save data to Database
                using (CoronaEntities dc=new CoronaEntities())
                {
                    dc.User.Add(user);
                    dc.SaveChanges();

                    #region Send Email to User

                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has beem sent to your email id:" + user.EmailID;
                    Status = true;
                    #endregion

                }
                #endregion


            }
            else
            {
                message = Resources.Language.InvalidRequest;
            }

            //

            //
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        //Verify Email

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (CoronaEntities dc=new CoronaEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; //This line I have added here to avoid
                                                                //Confirm password does not match issue on save change
                var v = dc.User.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v!=null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = Resources.Language.InvalidRequest;
                }
            }
            ViewBag.Status = Status;
            return View();
        }   
        
        //Login
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login,string ReturnUrl="")
        {
            string message = "";
            
            using (CoronaEntities dc = new CoronaEntities())
            {
                var v = dc.User.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                if (v!=null)
                {
                    if (string.Compare(Crypto.Hash(login.Password),v.Password)==0)
                    {
                        if (v.IsEmailVerified==true)
                        {
                            int timeout = login.RememberMe ? 525600 : 20; // 525600 min=year
                            var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            message = Resources.Language.VerifyEmail;
                        }
                    }
                    else
                    {
                        message = Resources.Language.InvalidPassword;
                    }
                }
                else
                {
                    message = Resources.Language.InvalidUser;
                }
            }
            ViewBag.Message = message;
            return View();
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (CoronaEntities dc = new CoronaEntities())
            {
                var v = dc.User.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        //Verify Email Link
        [NonAction]
        public void SendVerificationLinkEmail(string emailID,string activationCode, string emailFor="VerifyAccount")
        {
            //var scheme = Request.Url.Scheme;
            //var host = Request.Url.Host;
            //var port = Request.Url.Port;

            //string url=scheme + "://" + host +

            var verifyUrl = "/User/"+emailFor+"/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //var fromEmail = new MailAddress("emergencypandemic@gmail.com","Pandemic Emergency");
            var fromEmail = new MailAddress("theemergencymap@gmail.com", "The Emergency Map");
            var toEmail = new MailAddress(emailID);
            //var fromEmailPassword = "Jrc2020$";
            var fromEmailPassword = "Jrc2014$";

            string subject = "";
            string body = "";

            if(emailFor=="VerifyAccount")
            {
                subject = "Your account is succesfully created!";

                body = "<br/><br/>We are excited to tell you that your account is" +
                            " successfully created. Pleade click on the below link to verify your account" +
                            " <br/><br/><a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor=="ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for rest tour account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link +">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address,fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        //Forgot Password
        public ActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link
            //Send Email
            string message = "";
            bool status = false;

            using (CoronaEntities dc=new CoronaEntities())
            {
                var account = dc.User.Where(a => a.EmailID == EmailID).FirstOrDefault();
                if(account!=null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EmailID, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue, as we had added a confirm password property
                    //in cur model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = Resources.Language.ResetPasswordSent;
                    status = true;
                }
                else
                {
                    message = Resources.Language.InvalidUser;
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        //Forgot Password
        public ActionResult ResendVerification()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ResendVerification(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link
            //Send Email
            string message = "";
            bool status = false;

            using (CoronaEntities dc = new CoronaEntities())
            {
                var account = dc.User.Where(a => a.EmailID == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    SendVerificationLinkEmail(account.EmailID, account.ActivationCode.ToString());
                    //This line I have added here to avoid confirm password not match issue, as we had added a confirm password property
                    //in cur model class in part 1
                    message = Resources.Language.ActivationLinkSent;
                    status = true;
                }
                else
                {
                    message = Resources.Language.InvalidUser;
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        #region ListaPais
        public ActionResult Countries()
        {
            using (var dc = new CoronaEntities())
            {
                return PartialView(dc.Country.ToList());
            }
        }
        #endregion

        #region ListaEstado
        public ActionResult States(int CountryID)
        {
            using (var dc = new CoronaEntities())
            {
                return PartialView(dc.State.Where(x => x.CountryID == CountryID).ToList());
            }
        }
        #endregion

        #region ListaCiudad
        public ActionResult Cities(int StateID)
        {
            using (var dc=new CoronaEntities())
            {
                return PartialView(dc.City.Where(x => x.StateID == StateID).OrderBy(x => x.City1).ToList());
                //var result = dc.City.Where(x => x.StateID == StateID).ToList();
                //ViewBag.Ciudad = result;
                //return PartialView(result);
            }
        }
        #endregion


        #region 
        public int UserCountry()
        {
            using (var dc = new CoronaEntities())
            {
                var userCountry = 157;
                if (System.Web.HttpContext.Current.User.Identity.Name.ToString() != string.Empty)
                {
                    userCountry = dc.User.Where(x => x.EmailID == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().CountryID;
                }
                return userCountry;
            }
        }
        #endregion
        
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            using (CoronaEntities dc=new CoronaEntities())
            {
                var user = dc.User.Where(a=> a.ResetPasswordCode==id).FirstOrDefault();
                if (user!=null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }


         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult ResetPassword(ResetPasswordModel model)
         {
            var message = "";
            bool status = false;
            if (ModelState.IsValid)
            {
                using (CoronaEntities dc = new CoronaEntities())
                {
                    var user = dc.User.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user!=null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = Resources.Language.NewPasswordUpdate;
                        status = true;
                    }
                }
            }
            else
            {
                message = Resources.Language.InvalidRequest;
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(model);
         }
        
    }
}