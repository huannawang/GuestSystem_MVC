using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Web.Security;
using visitor_MVC.Models;
using static visitor_MVC.Models.Login;
using System.Reflection;

namespace visitor_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            TempData["Account"] = account;
            #region 登入Controller
            string adPath = "LDAP://TYNTEK";
            bool isLogin = false;
            string PIUSER_IP = CustomClass.GetIP4Address(); //HttpContext.Current.Request.UserHostAddress;
            string[] UserIPSP = PIUSER_IP.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            //if ((UserIPSP[0].ToString() == "192") || (UserIPSP[0].ToString() == "10") || (UserIPSP[0].ToString() == "127"))
            //{

            //  TempData["IP"] = PIUSER_IP;

            if (User.Identity.Name == "")
            {
                LdapAuthentication adAuth = new LdapAuthentication(adPath);
                try
                {
                    if (true == adAuth.IsAuthenticated("TYNTEK", account, password))
                    {
                        // string groups = adAuth.GetGroups(txtDomain.Text, txtUsername.Text, txtPassword.Text);
                        string groups = adAuth.GetGroups();

                        //Create the ticket, and add the groups.
                        // bool isCookiePersistent = chkPersist.Checked;
                        bool isCookiePersistent = false;
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1,
                                  account, DateTime.Now, DateTime.Now.AddMinutes(60), isCookiePersistent, groups);

                        //Encrypt the ticket.
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        //Create a cookie, and then add the encrypted ticket to the cookie as data.
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                        if (true == isCookiePersistent)
                            authCookie.Expires = authTicket.Expiration;

                        //Add the cookie to the outgoing cookies collection.
                        Response.Cookies.Add(authCookie);

                        //You can redirect now.
                        // Response.Redirect(FormsAuthentication.GetRedirectUrl(txtUsername.Text, false));

                        isLogin = true;
                    }
                    else
                    {
                        // errorLabel.Text = "Authentication did not succeed. Check user name and password.";
                    }
                }
                catch (Exception ex)
                {
                    // errorLabel.Text = "Error authenticating. " + ex.Message;
                }
            }
            else
            {
                isLogin = true;
            }
            //}
            if (isLogin)
            {
                using (OracleConnection SYSINFO = new OracleConnection("Data Source=***********;User ID=*******;Password=*******"))
                {
                    SYSINFO.Open();

                    string SQL = @"Select * From RESERAD_FILE Where USERNO = :UserNo";

                    OracleCommand Cmd = new OracleCommand(SQL, SYSINFO);
                    Cmd.Parameters.Add(param: new OracleParameter(parameterName: "UserNo", type: OracleDbType.Varchar2, size: 50)).Value = account;

                    OracleDataReader Rd = Cmd.ExecuteReader();

                    if (Rd.Read())
                    {
                        return RedirectToAction("GuardQuery", "Home");
                    }
                    else
                    {
                        SYSINFO.Close();
                        TempData["ErrMsg"] = "帳號或密碼錯誤!";
                        return RedirectToAction("Login");
                    }


                }
            }
            else
            {
                TempData["ErrMsg"] = "帳號或密碼錯誤!";
                return RedirectToAction("Login");
            }
            #endregion
        }


        public ActionResult GuardQuery()
        {
            #region 警衛室_初始自動顯示當天訪客資料
            DBmanager dBManager = new DBmanager();
            DateTime date = DateTime.Now;
            ViewBag.Date = date;
            List<GuestInfo> guests = dBManager.GetGuestinfos();
            ViewBag.guests = guests;
            return View();
            #endregion
        }

        [HttpPost]
        public ActionResult GuardQuery(string Category)
        {
            #region 警衛室_使用來訪類別查詢
            DBmanager dBManager = new DBmanager();
            DateTime date = DateTime.Now;
            ViewBag.Date = date;
            List<GuestInfo> guests = dBManager.GetGuestinfosByCategory(Category);
            ViewBag.guests = guests;
            return View();
            #endregion
        }

        public ActionResult GuardEditGuest(string OrderNo)
        {
            #region 警衛室_進去編輯畫面時顯示該筆資料
            DBmanager dBManager = new DBmanager();
            DateTime date = DateTime.Now;
            ViewBag.Date = date;

            GuestInfoNameViewModel viewModel = new GuestInfoNameViewModel();
            viewModel.GuestInfo = dBManager.GetGuestinfoByOrderNo(OrderNo);
            viewModel.GuestName = dBManager.GetGuestinfoByOrderNoGuestNames(OrderNo);

            return View(viewModel);
            #endregion
        }

        [HttpPost]
        public ActionResult GuardEditGuest(GuestInfoNameViewModel guestinfoname)
        {
            #region 警衛室_修改資料
            DBmanager dBManager = new DBmanager();
            
            dBManager.GuardUpdateGuestInfoName(guestinfoname);
            
            return RedirectToAction("GuardQuery");
            #endregion
        }

        
        public ActionResult EmployeeCreateGuest()
        {
            #region 員工_新增資料時先帶預設的資料(部門、工號...)
            DBmanager dBManager = new DBmanager();
            DateTime date = DateTime.Now;
            ViewBag.Date = date;
            string IP = CustomClass.GetIP4Address();
            string[] IP_split = IP.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            var account = TempData["Account"] as string;
            account = "T230082";
            Employee emp = dBManager.GetEmployeeInfo(account);

            return View(emp);
            #endregion
            
        }
        
    }
}
