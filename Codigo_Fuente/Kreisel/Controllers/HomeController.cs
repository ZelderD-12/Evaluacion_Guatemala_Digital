using Kreisel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace Kreisel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Productos(int anio = 2019, string categoria = "NA")
        {
            var viewModel = new ProductosViewModel
            {
                AnioSeleccionado = anio,
                CategoriaSeleccionada = categoria
            };

            BasedeDatos basedeDatos = new BasedeDatos();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                SqlCommand cmdCategorias = new SqlCommand(basedeDatos.ObtenerCategorias, connection);
                cmdCategorias.CommandType = CommandType.StoredProcedure;
                cmdCategorias.Parameters.AddWithValue("@fecha", anio);

                viewModel.Categorias = new List<Categoria>();
                using (SqlDataReader reader = cmdCategorias.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        viewModel.Categorias.Add(new Categoria
                        {
                            Nombre = reader["Nombre"].ToString()
                        });
                    }
                }

                if (!string.IsNullOrEmpty(categoria))
                {
                    SqlCommand cmdProductos = new SqlCommand(basedeDatos.ObtenerProductos, connection);
                    cmdProductos.CommandType = CommandType.StoredProcedure;
                    cmdProductos.Parameters.AddWithValue("@fecha", anio);
                    cmdProductos.Parameters.AddWithValue("@categoria", categoria);

                    viewModel.Productos = new List<Producto>();
                    using (SqlDataReader reader = cmdProductos.ExecuteReader())
                    {
                        int contador = 1;
                        while (reader.Read())
                        {
                            viewModel.Productos.Add(new Producto
                            {
                                Numero = contador++,
                                Descripcion = reader["Nombre"].ToString(),
                                Cantidad = Convert.ToInt32(reader["CantidadVentas"])
                            });
                        }
                    }
                }
            }

            return View(viewModel);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Creditos()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
