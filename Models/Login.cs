using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;

namespace visitor_MVC.Models
{
    public class Login
    {
        public class LdapAuthentication : IHttpModule
        {
            /// <summary>
            /// 您將需要在您 Web 的 Web.config 檔中設定此模組，
            /// 並且向 IIS 註冊該處理程式，才能使用它。如需詳細資訊，
            /// 參閱下列連結: https://go.microsoft.com/?linkid=8101007
            /// </summary>
            #region IHttpModule 成員

            private string _path;
            private string _filterAttribute;

            public void Dispose()
            {
                //清理程式碼在這裡。
            }

            public void Init(HttpApplication context)
            {
                // 以下是您能夠如何處理 LogRequest 事件的範例，並提供
                // 它的自訂記錄實作
                context.LogRequest += new EventHandler(OnLogRequest);
            }

            public LdapAuthentication(string path)
            {
                _path = path;
            }

            public bool IsAuthenticated(string domain, string username, string pwd)
            {
                string domainAndUsername = domain + @"\" + username;
                DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

                try
                {
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;

                    DirectorySearcher search = new DirectorySearcher(entry);

                    search.Filter = "(sAMAccountName=" + username + ")";
                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();

                    if (null == result)
                    {
                        return false;
                    }

                    //Update the new path to the user in the directory.
                    _path = result.Path;
                    _filterAttribute = (string)result.Properties["cn"][0];
                }
                catch (Exception ex)
                {
                    throw new Exception("Error authenticating user. " + ex.Message);
                }

                return true;
            }
            #endregion

            #region getgroup
            public string GetGroups()
            {
                DirectorySearcher search = new DirectorySearcher(_path);
                search.Filter = "(cn=" + _filterAttribute + ")";
                search.PropertiesToLoad.Add("memberOf");
                StringBuilder groupNames = new StringBuilder();

                try
                {
                    SearchResult result = search.FindOne();
                    int propertyCount = result.Properties["memberOf"].Count;
                    string dn;
                    int equalsIndex, commaIndex;

                    for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                    {
                        dn = (string)result.Properties["memberOf"][propertyCounter];
                        equalsIndex = dn.IndexOf("=", 1);
                        commaIndex = dn.IndexOf(",", 1);
                        if (-1 == equalsIndex)
                        {
                            return null;
                        }
                        groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                        groupNames.Append("|");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error obtaining group names. " + ex.Message);
                }
                return groupNames.ToString();
            }

            #endregion

            public void OnLogRequest(Object source, EventArgs e)
            {
                //自動記錄邏輯在這裡
            }
        }

        public class CustomClass
        {
            public static int Asc(string S)
            {
                return Convert.ToInt32(S[0]);
            }

            public static int Asc(char C)
            {
                return Convert.ToInt32(C);
            }

            public static char Chr(int Num)
            {
                return Convert.ToChar(Num);
            }

            public static string GetIP4Address()
            {
                string IP4Address = System.String.Empty;

                foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(System.Web.HttpContext.Current.Request.UserHostAddress))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                if (IP4Address != System.String.Empty)
                {
                    return IP4Address;
                }

                foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                return IP4Address;
            }

            public static int strLength(string myStr)
            {
                int Length = 0;
                int WordAsc;

                for (int i = 0; i < myStr.Length; i++)
                {
                    WordAsc = Asc(myStr.Substring(i, 1));

                    if (WordAsc > 0 && WordAsc < 256)
                    {
                        Length = Length + 1;
                    }
                    else
                    {
                        Length = Length + 2;
                    }
                }

                return Length;
            }

            public static string strCut(string myStr, int myNum)
            {
                if (strLength(myStr) > myNum)
                {
                    int Length = 1;

                    while (strLength(myStr.Substring(0, Length)) <= myNum)
                    {
                        Length++;
                    }

                    return myStr.Substring(0, Length - 1) + "...";
                }
                else
                {
                    return myStr;
                }
            }
        }
    }
}