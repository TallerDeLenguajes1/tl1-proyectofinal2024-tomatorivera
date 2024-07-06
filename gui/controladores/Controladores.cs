using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

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

        /// <summary>
        /// Muestra la vista vinculada al controlador y se encarga de manejar su lógica
        /// </summary>
        public abstract void MostrarVista();
    }

    public class InicioControlador : Controlador<Inicio>
    {
        private UsuarioServicio servicio;

        public InicioControlador(Inicio vista) : base(vista)
        {
            servicio = new UsuarioServicioImpl();
        }

        /// <summary>
        /// Muestra la vista de inicio y solicita al usuario su nombre de DT
        /// </summary>
        public override void MostrarVista()
        {
            var servicio = new RecursoServicioImpl();
            
            try
            {
                // El servicio me devuelve el Path de la imagen Logo, si hubiese un 
                // error con el directorio o el archivo lanza una excepción
                string logo = servicio.ObtenerLogo();
                vista.Logo = new CanvasImage(logo) { MaxWidth = 38 };
            }
            catch (Exception)
            {
                // En caso de no poder cargar la imagen, muestro el logo de respaldo en ASCII art para que quede bonito :)
                string logoRespaldoAscii = @"░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
                                        ██████████                                    ░░
                                    ██████▓▓▓▓▓▓▓▓████                                ░░
░░      ░░    ░░      ░░      ░░  ██▓▓██    ████▓▓▓▓▓▓██  ░░      ░░      ░░    ░░    ░░
        ░░      ░░    ░░      ░░████▓▓▓▓██      ████▓▓▓▓██  ░░    ░░      ░░      ░░  ░░
                                ██  ██▓▓▓▓██        ██▓▓██                            ░░
░░                            ██    ██▓▓▓▓▓▓████      ██▓▓██                          ░░
░░      ░░    ░░░░    ░░      ██      ████▓▓▓▓▓▓██      ████░░    ░░      ░░    ░░░░  ░░
        ░░      ░░    ░░      ██    ████▓▓██▓▓▓▓▓▓██    ████      ░░      ░░      ░░  ░░
                              ██  ██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓████  ██                          ░░
                              ██  ██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓██████                          ░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████▓▓▓▓▓▓██  ██▓▓▓▓██  ██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░      ░░      ░░    ░░      ░░██████▓▓██      ████  ████  ░░    ░░      ░░      ░░  ░░
                                  ████▓▓████        ████                                
░░                                  ████▓▓██      ████                                  
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ██████████      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
";

                vista.Logo = new Markup("[bold red]" + logoRespaldoAscii + "[/]");
            }

            vista.Dibujar();

            // Leo una tecla para iniciar el juego
            Console.ReadKey();
        }
    }

    public class MenuControlador : Controlador<Menu>
    {

        private bool estaSeleccionando;
        private int indiceSeleccionado;
        private UsuarioServicio servicio;

        public MenuControlador(Menu vista) : base(vista)
        {
            this.indiceSeleccionado = 0;
            this.estaSeleccionando = true;
            this.servicio = new UsuarioServicioImpl();
            configurarComandoSalida();
        }

        /// <summary>
        /// Muestra lo necesario del menu y controla su funcionamiento
        /// </summary>
        public override void MostrarVista()
        {
            ConsoleKeyInfo teclaPresionada;
            vista.IndiceSeleccionado = this.indiceSeleccionado;
            vista.MostrarTitulo();
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

                // Almaceno la posición del cursor para borrar desde ahí los registros visuales del comando
                int lineaInicio = Console.CursorTop;

                // Ejecuto el comando seleccionado manejando los posibles errores que pueden lanzar
                try
                {
                    vista.Comandos.ElementAt(indiceSeleccionado).ejecutar();
                }
                catch (Exception e)
                {
                    VistasUtil.MostrarError(e.Message);
                    VistasUtil.PausarVistas(2);
                }

                // Solo si el menú aún se sigue ejecutando, borro las lineas de lo escrito por los comandos
                if (estaSeleccionando) borrarDesdeLinea(lineaInicio);
            }
        }

        /// <summary>
        /// Borra la consola a partir de la linea <paramref name="lineaInicio"/>
        /// </summary>
        /// <param name="lineaInicio">Linea desde la cual se borrará la consola</param>
        private void borrarDesdeLinea(int lineaInicio)
        {
            int lineaFin = Console.CursorTop;

            for (int i = lineaInicio ; i < lineaFin ; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, lineaInicio);
        }

        /// <summary>
        /// Agrega la acción de modificar el atributo "estaSeleccionado" en el comando salir
        /// para que detenga la ejecución del WHILE que mantiene activo el menú
        /// </summary>
        private void configurarComandoSalida() {
            vista.Comandos.ForEach(cmd => {
                if (cmd is ComandoSalir)
                {
                    ((ComandoSalir) cmd).AccionPersonalizada = () => { this.estaSeleccionando = false; };
                }
            });
        }
    }
}