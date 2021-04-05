using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class ApiOtherRegister
    {
        public string reqtype { get; set; }
        public string username { get; set; }
        public string mobile { get; set; }
        public string referralid { get; set; }
        public string side { get; set; }
        public string name { get; set; }
        public string fname { get; set; }
        public string dob { get; set; }
        public string ismarried { get; set; }
        public string marriagedate { get; set; }
        public string address { get; set; }
        public string statecode { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string mobl { get; set; }
        public string phoneno { get; set; }
        public string email { get; set; }
        public string nominee { get; set; }
        public string relation { get; set; }
        public string mpasswd { get; set; }
        public string areacode { get; set; }
        public string citycode { get; set; }
        public string districtcode { get; set; }
        public string frelation { get; set; }
        public string actype { get; set; }
        public string bankcode { get; set; }
        public string panno { get; set; }
        public string accountno { get; set; }
        public string ifsc { get; set; }
        public string branch { get; set; }
        public string aadharno { get; set; }
        // public string religion { get; set; }
         public string jointype { get; set; }
         public string religiontype { get; set; }
        public string religionid { get; set; }
         public string nonreligionid { get; set; }
         public string religionname { get; set; }
        public string nonreligionname { get; set; }
    }
     public class ReligionReqType
    {
        public string  reqtype { get; set; }
    }
     public  class Religion
    {
         public string id { get; set; }
         public string religion { get; set; }
    }
    public class RegligionResponse
    {
        public List<Religion> religions {get;set;}
         public List<Religion> nonreligions { get; set; }
    }
}