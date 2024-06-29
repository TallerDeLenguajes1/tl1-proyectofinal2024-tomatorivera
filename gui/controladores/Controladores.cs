using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;

namespace Gui.Controladores
{
    /// <summary>
    /// Representa una clase controladora. Dichas clases
    /// se encargan de controlar el funcionamiento de una vista
    /// </summary>
    public abstract class Controlador<V> where V : Vista
    {
        protected V vista;

        /// <summary>
        /// Constructor de una clase controladora
        /// </summary>
        /// <param name="vista">Instancia de la clase vista a controlar</param>
        public Controlador(V vista)
        {
            this.vista = vista;
        }
    }

    public class InicioControlador : Controlador<Inicio>
    {
        public InicioControlador(Inicio vista) : base(vista)
        {}

        /// <summary>
        /// Muestra la vista de inicio y solicita al usuario su nombre de DT
        /// </summary>
        public void MostrarEncabezado()
        {
            vista.Dibujar();

            // Falta persistir el nombre de usuario
            string nombreUsuario = string.Empty;
            do
            {
                vista.SolicitarNombre();
                nombreUsuario = Console.ReadLine() ?? string.Empty;

                if (estaVacio(nombreUsuario))
                {
                    VistasUtil.MostrarError("El nombre de usuario no puede estar vacío");
                }
                else if (!cumpleLongitud(nombreUsuario))
                {
                    VistasUtil.MostrarError("El nombre de usuario debe tener de 3 a 15 caracteres");
                }

            } while (estaVacio(nombreUsuario) || !cumpleLongitud(nombreUsuario));
        }

        /// <summary>
        /// Verifica si un nombre de usuario está vacío
        /// </summary>
        /// <param name="nombre">Nombre de usuario a validar</param>
        /// <returns><c>True</c> si el nombre es NULL o solo espacios, <c>False</c> en caso contrario</returns>
        private bool estaVacio(string nombre)
        {
            return nombre.Length == 0 || nombre.Trim().Equals("");
        }

        /// <summary>
        /// Verifica si un nombre cumple con la longitud requerida
        /// </summary>
        /// <param name="nombre">Nombre de usuario a validar</param>
        /// <returns><c>True</c> si el nombre de usuario tiene de 3 a 15 caracteres, <c>False</c> en caso contrario</returns>
        private bool cumpleLongitud(string nombre)
        {
            return nombre.Length >= 3 && nombre.Length <= 15;
        }
    }

    public class MenuControlador : Controlador<Menu>
    {

        private bool estaSeleccionando;
        private int indiceSeleccionado;

        public MenuControlador(Menu vista) : base(vista)
        {
            this.indiceSeleccionado = 0;
            this.estaSeleccionando = true;
        }

        /// <summary>
        /// Muestra lo necesario del menu y controla su funcionamiento
        /// </summary>
        public void MostrarMenu()
        {
            ConsoleKeyInfo teclaPresionada;
            vista.IndiceSeleccionado = this.indiceSeleccionado;
            vista.Dibujar();

            // El menú se mostrará mientras no se seleccione la opción Salir
            while (estaSeleccionando)
            {
                // Este while mantiene al usuario en el menú hasta que presione enter
                while ((teclaPresionada = Console.ReadKey(true)).Key != ConsoleKey.Enter)
                {
                    switch (teclaPresionada.Key)
                    {
                        case ConsoleKey.DownArrow:
                            if (indiceSeleccionado == (vista.Comandos.Count() - 1))
                                continue;
                            indiceSeleccionado++;
                            break;

                        case ConsoleKey.UpArrow:
                            if (indiceSeleccionado == 0) 
                                continue;
                            indiceSeleccionado--;
                            break;

                        default:
                            break;
                    }

                    // Actualizo el indice seleccionado y vuelvo a dibujar el menu
                    vista.IndiceSeleccionado = this.indiceSeleccionado;
                    vista.Dibujar();
                }

                // Ejecuto el comando seleccionado
                vista.Comandos.ElementAt(indiceSeleccionado).ejecutar();

                if (vista.Comandos.ElementAt(indiceSeleccionado) is ComandoSalir)
                {
                    this.estaSeleccionando = false;
                }
            }
        }

    }
}