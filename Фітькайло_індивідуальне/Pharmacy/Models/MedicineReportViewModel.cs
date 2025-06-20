using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pharmacy.Models
{
    public class MedicineReportViewModel
    {
        public string MedicineName { get; set; }
        public int PurchaseCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
