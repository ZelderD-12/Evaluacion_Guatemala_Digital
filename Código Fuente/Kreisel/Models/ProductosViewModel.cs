namespace Kreisel.Models
{
    public class ProductosViewModel
    {
        public int AnioSeleccionado { get; set; }
        public string CategoriaSeleccionada { get; set; }
        public List<Categoria> Categorias { get; set; }
        public List<Producto> Productos { get; set; }
    }
}
