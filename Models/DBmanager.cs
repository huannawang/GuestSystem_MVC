using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;


namespace visitor_MVC.Models
{
    public class DBmanager
    {
        private readonly string ConnStr = "Data Source=192.168.10.10;Initial Catalog=Guestsys;Persist Security Info=True;User ID=admin;Password=ZAQ!@#xcv";

        public class CustomClass
        {
            #region IP
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
            #endregion
        }

        public List<GuestInfo> GetGuestinfos()
        {
            #region GuardQuery抓出當天資料
            List<GuestInfo> guests = new List<GuestInfo>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM guestinfo");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    GuestInfo guest = new GuestInfo();

                    // 使用 reflection 取得 Guestinfo 類別的所有屬性
                    var properties = typeof(GuestInfo).GetProperties();
                    foreach (var property in properties)
                    {
                        // 取得屬性對應的 ColumnAttribute
                        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
                        Console.WriteLine($"columnAttribute:{columnAttribute}");
                        // 如果有指定 ColumnAttribute，則使用其值作為資料庫欄位名稱
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name;
                            // 使用 SqlDataReader 的 GetOrdinal 方法取得資料庫欄位的索引
                            int columnIndex = reader.GetOrdinal(columnName);
                            if (!reader.IsDBNull(columnIndex))
                            {
                                // 根據屬性的型別使用對應的 Get 方法
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(guest, reader.GetString(columnIndex));
                                }
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(guest, reader.GetInt32(columnIndex));
                                }
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(guest, reader.GetDateTime(columnIndex));
                                }
                            }
                        }
                        else
                        {
                            // 如果欄位值為 NULL
                            property.SetValue(guest, null); // 設定為默認值
                        }
                    }
                    guests.Add(guest);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return guests;
            #endregion
        }

        public List<GuestInfo> GetGuestinfosByCategory(string Category)
        {
            #region GuardQuery用來訪類別條件查詢時抓資料
            List<GuestInfo> guests = new List<GuestInfo>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr); //SQL連線物件
            SqlCommand sqlCommand = new SqlCommand("select * from guestinfo");
            
            if (Category != "ALL")
            {
                // 如果 Category 不是 "ALL"，添加 WHERE 條件
                sqlCommand.CommandText += " where Category = @Category";
                sqlCommand.Parameters.Add(new SqlParameter("@Category", Category));
            }

            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader(); //sql 資料讀取物件
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    GuestInfo guest = new GuestInfo();

                    // 使用 reflection 取得 Guestinfo 類別的所有屬性
                    var properties = typeof(GuestInfo).GetProperties();
                    foreach (var property in properties)
                    {
                        // 取得屬性對應的 ColumnAttribute
                        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
                        Console.WriteLine($"columnAttribute:{columnAttribute}");
                        // 如果有指定 ColumnAttribute，則使用其值作為資料庫欄位名稱
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name;
                            // 使用 SqlDataReader 的 GetOrdinal 方法取得資料庫欄位的索引
                            int columnIndex = reader.GetOrdinal(columnName);
                            if (!reader.IsDBNull(columnIndex))
                            {
                                // 根據屬性的型別使用對應的 Get 方法
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(guest, reader.GetString(columnIndex));
                                }
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(guest, reader.GetInt32(columnIndex));
                                }
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(guest, reader.GetDateTime(columnIndex));
                                }
                            }
                        }
                        else
                        {
                            // 如果欄位值為 NULL
                            property.SetValue(guest, null); // 設定為默認值
                        }
                    }
                    guests.Add(guest);
                }
            }
            else
            {
                Console.WriteLine("查詢不到符合的項目");
            }
            sqlConnection.Close();
            return guests;
            #endregion
        }

        public GuestInfo GetGuestinfoByOrderNo(string OrderNo)
        {
            # region GuardQuery點選編輯後，GuardEditGuest抓出該筆資料
            GuestInfo guest = new GuestInfo();
            SqlConnection sqlConnection = new SqlConnection(ConnStr); //SQL連線物件
            SqlCommand sqlCommand = new SqlCommand("select * " +
                "from guestinfo " +
                "where OrderNo = @OrderNo");
            
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@OrderNo", OrderNo));
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // 使用 reflection 取得 Guestinfo 類別的所有屬性
                    var properties = typeof(GuestInfo).GetProperties();
                    foreach (var property in properties)
                    {
                        // 取得屬性對應的 ColumnAttribute
                        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
                        
                        // 如果有指定 ColumnAttribute，則使用其值作為資料庫欄位名稱
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name;
                            // 使用 SqlDataReader 的 GetOrdinal 方法取得資料庫欄位的索引
                            int columnIndex = reader.GetOrdinal(columnName);
                            if (!reader.IsDBNull(columnIndex))
                            {
                                // 根據屬性的型別使用對應的 Get 方法
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(guest, reader.GetString(columnIndex));
                                }
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(guest, reader.GetInt32(columnIndex));
                                }
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(guest, reader.GetDateTime(columnIndex));
                                }
                            }
                        }
                        else
                        {
                            // 如果欄位值為 NULL
                            property.SetValue(guest, null); // 設定為默認值
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return guest;
            #endregion
        }

        public GuestName GetGuestinfoByOrderNoGuestNames(string OrderNo)
        {
            # region GuardQuery點選編輯後，GuardEditGuest抓出該筆資料的姓名電話
            GuestName guestname = new GuestName();
            SqlConnection sqlConnection = new SqlConnection(ConnStr); //SQL連線物件
            SqlCommand sqlCommand = new SqlCommand("select OrderNo, Name1, PhoneNo1, Name2, PhoneNo2, Name3, PhoneNo3, Name4, PhoneNo4, Name5, PhoneNo5, Name6, PhoneNo6, Name7, PhoneNo7, Name8, PhoneNo8, Name9, PhoneNo9, Name10, PhoneNo10, Name11, PhoneNo11, Name12, PhoneNo12, Name13, PhoneNo13, Name14, PhoneNo14, Name15, PhoneNo15, Name16, PhoneNo16, Name17, PhoneNo17, Name18, PhoneNo18, Name19, PhoneNo19, Name20, PhoneNo20, Name21, PhoneNo21, Name22, PhoneNo22, Name23, PhoneNo23, Name24, PhoneNo24, Name25, PhoneNo25, Name26, PhoneNo26, Name27, PhoneNo27, Name28, PhoneNo28, Name29, PhoneNo29, Name30, PhoneNo30 " +
                "from guestname " +
                "where OrderNo = @OrderNo");

            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@OrderNo", OrderNo));
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // 使用 reflection 取得 GuestName 類別的所有屬性
                    var properties = typeof(GuestName).GetProperties();
                    foreach (var property in properties)
                    {
                        // 取得屬性對應的 ColumnAttribute
                        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
                        
                        // 如果有指定 ColumnAttribute，則使用其值作為資料庫欄位名稱
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name;
                            // 使用 SqlDataReader 的 GetOrdinal 方法取得資料庫欄位的索引
                            int columnIndex = reader.GetOrdinal(columnName);
                            if (!reader.IsDBNull(columnIndex))
                            {
                                // 根據屬性的型別使用對應的 Get 方法
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(guestname, reader.GetString(columnIndex));
                                }
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(guestname, reader.GetInt32(columnIndex));
                                }
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(guestname, reader.GetDateTime(columnIndex));
                                }
                            }
                        }
                        else
                        {
                            // 如果欄位值為 NULL
                            property.SetValue(guestname, null); // 設定為默認值
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return guestname;
            #endregion
        }
        
        public void GuardUpdateGuestInfoName(GuestInfoNameViewModel guestinfoname)
        {
            #region GuardEditGuest點選儲存後，修改資料
            SqlConnection sqlConnection = new SqlConnection(ConnStr);

            string updateInfo = @"UPDATE guestinfo SET " +
                                 "HeadCount = @HeadCount, " +
                                 "PCNumber = @PCNumber, " +
                                 "PhoneNumber = @PhoneNumber, " +
                                 "Other3CNumber = @Other3CNumber, " +
                                 "EntryTime = @EntryTime";  

            if (guestinfoname.GuestInfo.LeaveTime != default(DateTime))
            {
                updateInfo += ", LeaveTime = @LeaveTime";
            }

            updateInfo += " WHERE OrderNo = @OrderNo";
            //更新guestinfo
            using (SqlCommand sqlCommand = new SqlCommand(updateInfo, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@HeadCount", guestinfoname.GuestInfo.HeadCount);
                sqlCommand.Parameters.AddWithValue("@PCNumber", guestinfoname.GuestInfo.PCNumber);
                sqlCommand.Parameters.AddWithValue("@PhoneNumber", guestinfoname.GuestInfo.PhoneNumber);
                sqlCommand.Parameters.AddWithValue("@Other3CNumber", guestinfoname.GuestInfo.Other3CNumber);
                sqlCommand.Parameters.AddWithValue("@EntryTime", guestinfoname.GuestInfo.EntryTime);

                if (guestinfoname.GuestInfo.LeaveTime != default(DateTime))
                {
                    sqlCommand.Parameters.AddWithValue("@LeaveTime", guestinfoname.GuestInfo.LeaveTime);
                }

                sqlCommand.Parameters.AddWithValue("@OrderNo", guestinfoname.GuestInfo.OrderNo);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            //更新guestname
            using (sqlConnection)
            {
                sqlConnection.Open();

                for (int i = 1; i <= guestinfoname.GuestInfo.HeadCount + 3; i++)
                {
                    // 獲取 Name 和 PhoneNo 的值
                    string nameValue = guestinfoname.GuestName.GetType().GetProperty($"Name{i}")?.GetValue(guestinfoname.GuestName)?.ToString();
                    string phoneNoValue = guestinfoname.GuestName.GetType().GetProperty($"PhoneNo{i}")?.GetValue(guestinfoname.GuestName)?.ToString();

                    // 判斷是否需要更新
                    if (!string.IsNullOrEmpty(nameValue) || !string.IsNullOrEmpty(phoneNoValue))
                    {
                        string updateGuestNameQuery = $@"UPDATE guestname SET " +
                                                       $"Name{i} = @Name{i}, " +
                                                       $"PhoneNo{i} = @PhoneNo{i} " +
                                                       "WHERE OrderNo = @OrderNo";

                        using (SqlCommand sqlCommand = new SqlCommand(updateGuestNameQuery, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue($"@Name{i}", nameValue);
                            sqlCommand.Parameters.AddWithValue($"@PhoneNo{i}", phoneNoValue);
                            sqlCommand.Parameters.AddWithValue("@OrderNo", guestinfoname.GuestInfo.OrderNo);

                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
                sqlConnection.Close();
            }

            #endregion
        }
        
        public Employee GetEmployeeInfo(string EmployeeID)
        {
            #region 新增資料預設畫面帶入基本資料
            Employee emp = new Employee();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT Department, DepartmentNo, EmployeeID, EmployeeName, ExtensionNo, Email, Permissions " +
                                                    "from employee " +
                                                    "where EmployeeID = @EmployeeID");

            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@EmployeeID", EmployeeID));
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // 使用 reflection 取得 GuestName 類別的所有屬性
                    var properties = typeof(Employee).GetProperties();
                    foreach (var property in properties)
                    {
                        // 取得屬性對應的 ColumnAttribute
                        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

                        // 如果有指定 ColumnAttribute，則使用其值作為資料庫欄位名稱
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name;
                            // 使用 SqlDataReader 的 GetOrdinal 方法取得資料庫欄位的索引
                            int columnIndex = reader.GetOrdinal(columnName);
                            if (!reader.IsDBNull(columnIndex))
                            {
                                // 根據屬性的型別使用對應的 Get 方法
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(emp, reader.GetString(columnIndex));
                                }
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(emp, reader.GetInt32(columnIndex));
                                }
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(emp, reader.GetDateTime(columnIndex));
                                }
                            }
                        }
                        else
                        {
                            // 如果欄位值為 NULL
                            property.SetValue(emp, null); // 設定為默認值
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();

            return emp;
            #endregion
        }
        

    }
}