namespace Logica.Comandos;

/// <summary>
/// Interfaz que representa un comando de un menú
/// </summary>
public interface IComando
{
    /// <value>La propiedad nombre es como se muestra el comando en un menú</value>
    public string Titulo { get; }

    /// <summary>
    /// Realiza todas las acciones que este comando deba realizar
    /// al ser seleccionado mediante un menú
    /// </summary>
    public void Ejecutar();
}