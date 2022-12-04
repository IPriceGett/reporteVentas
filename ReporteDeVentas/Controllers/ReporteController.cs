using MySql.Data.MySqlClient;
using ReporteDeVentas.Models;
using ReporteDeVentas.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Renci.SshNet.Security.Cryptography;
using System.Globalization;
using System.Web.Services.Protocols;
using Rotativa;
using System.Web.Configuration;
using Rotativa.Options;
using System.Configuration;

namespace ReporteDeVentas.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Report
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

        public ActionResult Index()
        {
            conn.Open();
            string caja = "SELECT DISTINCT closedcash.HOST FROM closedcash";
            var cajas = new MySqlCommand(caja, conn);
            var reader_cajas = cajas.ExecuteReader();
            var model = new Busqueda();
            model.fechas = new List<Fechas>();
            while (reader_cajas.Read())
            {
                var host = new Cajas();
                host.Caja = reader_cajas["HOST"].ToString();
                model.Criterios.Add(host);
            }
            conn.Close();
            conn.Open();
            string categoria = "SELECT categories.NAME FROM categories";
            var categorias = new MySqlCommand(categoria, conn);
            var reader_categorias = categorias.ExecuteReader();
            while (reader_categorias.Read())
            {
                var category = new Categories();
                category.Categorias = reader_categorias["NAME"].ToString();
                model.categories.Add(category);
            }
            conn.Close();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string Fecha_Inicio, string Fecha_fin, string punto_venta, string imprimir, string Hora_Inicio, string Hora_Fin, string categoria)
        {
                var model = new Busqueda();
                var fechas = new Fechas();
                fechas.Fecha_Inicio = Fecha_Inicio;
                fechas.Fecha_Fin = Fecha_fin;
                fechas.Caja_Seleccionada = punto_venta;
                fechas.Hora_Inicio = Hora_Inicio;
                fechas.Hora_Fin = Hora_Fin;
                fechas.Categorias = categoria;
                model.fechas.Add(fechas);
                conn.Open();
                string caja = "SELECT DISTINCT closedcash.HOST FROM closedcash";
                var cajas = new MySqlCommand(caja, conn);
                var reader_cajas = cajas.ExecuteReader();
                while (reader_cajas.Read())
                {
                    var host = new Cajas();
                    host.Caja = reader_cajas["HOST"].ToString();
                    model.Criterios.Add(host);
                }
                conn.Close();
                conn.Open();
                string cat = "SELECT categories.NAME FROM categories";
                var cate = new MySqlCommand(cat, conn);
                var reader_categorias = cate.ExecuteReader();
                while (reader_categorias.Read())
                {
                    var category = new Categories();
                    category.Categorias = reader_categorias["NAME"].ToString();
                    model.categories.Add(category);
                }
                conn.Close();
                conn.Open();
                string sql = $"SELECT products.name,products.CODE,products.PRICEBUY,ticketlines.PRICE as PRICESELL," +
                $"SUM(ticketlines.UNITS) AS UNIDADES, " +
                $"SUM(ticketlines.UNITS*ticketlines.PRICE) as Totalv, " +
                $"SUM(ticketlines.UNITS*products.PRICEBUY) as Totalc FROM  " +
                $"closedcash INNER JOIN receipts " +
                    $"ON closedcash.MONEY = receipts.MONEY " +
                $"INNER JOIN ticketlines " +
                    $"ON receipts.ID = ticketlines.TICKET " +
                $"INNER JOIN products " +
                    $"ON ticketlines.PRODUCT = products.ID " +
                $"INNER JOIN categories " +
                    $"ON products.CATEGORY = categories.ID " +
                $"WHERE receipts.DATENEW >= '{Fecha_Inicio} {Hora_Inicio}:00' AND receipts.DATENEW <= '{Fecha_fin} {Hora_Fin}:59' " +
                $"AND HOST = '{punto_venta}' " +
                $"AND categories.NAME = '{categoria}'" +
                $"GROUP BY products.name HAVING COUNT(*)>=1 ORDER BY UNIDADES DESC";
                if (categoria == "Todas") sql = sql.Replace("AND categories.NAME = 'Todas'", "");
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                var resumen = new ResumenPeriodo();
                try
                {
                    while (reader.Read())
                    {
                        resumen.TotalPriceB += Convert.ToDecimal(reader["Totalc"].ToString());
                        resumen.TotalPriceS += Convert.ToDecimal(reader["Totalv"].ToString()); 

                        var producto = new Tabla();
                        producto.Compra = Convert.ToDecimal(reader["PRICEBUY"].ToString());
                        producto.Venta = Convert.ToDecimal(reader["PRICESELL"].ToString());
                        producto.Code = reader["CODE"].ToString();
                        producto.Name = reader["NAME"].ToString();
                        producto.Unidades = decimal.Round(Convert.ToDecimal(reader["UNIDADES"].ToString()), 2);
                        producto.Total_Compra = Convert.ToDecimal(reader["Totalc"].ToString());
                        producto.Total_Venta = Convert.ToDecimal(reader["Totalv"].ToString());
                        producto.Total_Utilidad = producto.Total_Venta - producto.Total_Compra;
                        if (producto.Total_Compra == 0)
                        {
                            producto.Total_Compra = 1;
                            producto.Total_Porcentaje = producto.Total_Utilidad / producto.Total_Compra;
                            producto.Total_Compra = 0;
                        }
                        else producto.Total_Porcentaje = decimal.Round(Convert.ToDecimal((producto.Total_Utilidad / producto.Total_Compra) * 100), 2);
                        model.Product.Add(producto);
                    }
                    resumen.TotalPriceU = resumen.TotalPriceS - resumen.TotalPriceB;
                    resumen.PricePercent = decimal.Round(Convert.ToDecimal((resumen.TotalPriceU / resumen.TotalPriceB) * 100), 2);
                    model.Lista_Productos.Add(resumen);
                }catch
                {
                    model.HayErrores = true;
                    conn.Close();
                    ViewBag.Mensaje = "No se encontraron ventas en esa fecha o caja. Ingrese nuevamente";
                    return View("Index", model);
                }
                conn.Close();
                return View("Index", model);
        }

    }
}
   