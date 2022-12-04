using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReporteDeVentas.Classes
{
    public class ResumenPeriodo
    {
        public decimal TotalPriceB { get; set; }
        public decimal TotalPriceS { get; set; }
        public decimal TotalPriceU { get; set; }
        public decimal PricePercent { get; set; }
        public string totalElementos { get; set; }
        public string totalCosto { get; set; }
        public string totalGanancia { get; set; }
    }
}