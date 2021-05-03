using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintAPI.Models
{
    public class PrintJobFullModel
    {
        public int JobId { get; set; }
        public string PrinterName { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
        public string FileToBePrinted { get; set; }
        public string PrintStatus { get; set; } = " ";
    }
}
