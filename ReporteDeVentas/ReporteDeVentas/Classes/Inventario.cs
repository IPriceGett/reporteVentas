using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReporteDeVentas.Classes
{
    public class Inventario
    {
        public double stockInicial { get; set; }
        public string codigo { get; set; }
        public double stockActual { get; set; }
        public double unidadesVendidas { get; set; }
        public string producto { get; set; }
        public string categoria { get; set; }
    }
}