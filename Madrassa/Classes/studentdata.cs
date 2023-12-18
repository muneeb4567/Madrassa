using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madrassa.Classes
{
    public class studentdata
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string fname { get; set; }
        public string sarfname { get; set; }
        public decimal sarfcon { get; set; }
        public decimal stdcnic { get; set; }
        public string sarfaresh { get; set; }
        public decimal sarfcnic { get; set; }
        public int fees { get; set; }
        public DateTime dateofbirth { get; set; }
        public DateTime dateofdahila { get; set; }
        public string darga { get; set; }
        public string nughait1 { get; set; }
        public decimal stdnum { get; set; }
        public string address { get; set; }
        public string nughait2 { get; set; }
        public byte[] image { get; set; }
        public string status { get; set; }
    }
}
