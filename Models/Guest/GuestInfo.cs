using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace visitor_MVC.Models
{
    public class GuestInfo
    {
        [Column("FactoryArea")]
        public string FactoryArea { get; set; }
        [Column("OrderNo")]
        public string OrderNo { get; set; }
        [Column("RegisterTime")]
        public DateTime RegisterTime { get; set; }
        [Column("RegisterIP")]
        public string RegisterIP { get; set; }
        [Column("EmployeeID")]
        public string EmployeeID { get; set; }
        [Column("EntryTime")]
        public DateTime EntryTime { get; set; }
        [Column("LeaveTime")]
        public DateTime LeaveTime { get; set; }
        [Column("ETA")]
        public DateTime ETA { get; set; }
        [Column("GuardIP")]
        public string GuardIP { get; set; }
        [Column("WorkID")]
        public string WorkID { get; set; }
        [Column("CompanyName")]
        public string CompanyName { get; set; }
        [Column("Category")]
        public string Category { get; set; }
        [Column("HeadCount")]
        public int HeadCount { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("PhoneNo")]
        public string PhoneNo { get; set; }
        [Column("PCNumber")]
        public int PCNumber { get; set; }
        [Column("PhoneNumber")]
        public int PhoneNumber { get; set; }
        [Column("Other3CNumber")]
        public int Other3CNumber { get; set; }
        [Column("LicensePlateNo")]
        public string LicensePlateNo { get; set; }
        [Column("Construction")]
        public string Construction { get; set; }
        [Column("MeetingRoom")]
        public string MeetingRoom { get; set; }
        [Column("Reason")]
        public int Reason { get; set; }
        [Column("Remark")]
        public string Remark { get; set; }

        //public List<Guestname> Guestnames { get; set; }
    }
}