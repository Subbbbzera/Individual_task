using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pharmacy.Models
{
    public class MedicinesListViewModel
    {
        public List<Medicine> Medicines { get; set; }
        public SelectList MedicineNames { get; set; }
        public SelectList Manufacturers { get; set; }

        public string SelectedMedicine { get; set; }
        public string SelectedManufacturer { get; set; }
        public PageInfo PageInfo { get; set; }

        public string SortOrder { get; set; }  


    }
}
