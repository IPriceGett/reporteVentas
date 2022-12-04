using ReporteDeVentas.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Razor.Configuration;

namespace ReporteDeVentas.Models
{
    public class Busqueda
    {
        public Busqueda() {
            Lista_Productos = new List<ResumenPeriodo>();
            Product = new List<Tabla>();
            Criterios = new List<Cajas>();
            fechas = new List<Fechas>();
            categories = new List<Categories>();
        }
        public List<ResumenPeriodo> Lista_Productos { get; set; }
        public List<Tabla> Product { get; set; }
        public List<Cajas> Criterios{ get; set; }
        public List<Fechas> fechas { get; set; }
        public List<Categories> categories { get; set; }
        public bool HayErrores { get; set; }

    }
}