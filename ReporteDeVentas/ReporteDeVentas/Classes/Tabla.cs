using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ReporteDeVentas.Classes
{
    public class Tabla
    {
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public decimal Unidades { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Total_Compra { get; set; }
        public decimal Total_Venta { get; set; }
        public decimal Total_Utilidad { get; set; }
        public decimal Total_Porcentaje { get; set; }
    }
}