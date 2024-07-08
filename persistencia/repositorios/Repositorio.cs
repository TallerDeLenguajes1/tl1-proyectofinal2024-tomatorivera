namespace Persistencia
{
    /// <summary>
    /// Clase interfaz repositorio para el manejo de la persistencia de los datos
    /// </summary>
    /// <typeparam name="T">Tipo de dato trabajado por el repositorio</typeparam>
    public interface IRepositorio<T>
    {
        void Crear(T obj);
        void Guardar(T obj);
        T ObtenerActual();
        T Cargar(int id);
    }

    /// <summary>
    /// Interfaz para aquellos repositorios que contengan directorios navegables
    /// </summary>
    /// <typeparam name="T">Tipo de dato trabajado por el repositorio</typeparam>
    public interface IRepositorioNavegable<T> : IRepositorio<T>
    {
        List<string> ObtenerDirectorios();
    }
}