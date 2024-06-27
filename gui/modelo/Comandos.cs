using Gui.Util;

namespace Gui.Modelo
{
    /// <summary>
    /// Interfaz que representa un comando de un menú
    /// </summary>
    public interface IComando
    {
        /// <value>La propiedad nombre es como se muestra el comando en un menú</value>
        string titulo { get; }

        /// <summary>
        /// Realiza todas las acciones que este comando deba realizar
        /// al ser seleccionado mediante un menú
        /// </summary>
        public void ejecutar();
    }

    public class ComandoSalir : IComando
    {
        private TipoMenu tipoMenu;
        public string titulo => tipoMenu.Descripcion();

        public ComandoSalir(TipoMenu tipoMenu)
        {
            this.tipoMenu = tipoMenu;
        }

        public void ejecutar()
        {
            System.Console.WriteLine("Sistema finalizado");
        }
    }

    // COMANDOS DE EJEMPLO PARA PRUEBAS, SERÁN ELIMINADOS
    public class prueba1 : IComando
    {
        public string titulo => "Prueba 1";

        public void ejecutar()
        {
            System.Console.WriteLine("Ejecutando prueba 1");
        }
    }

    public class prueba2 : IComando
    {
        public string titulo => "Prueba 2";

        public void ejecutar()
        {
            System.Console.WriteLine("Ejecutando prueba 2");
        }
    }
}