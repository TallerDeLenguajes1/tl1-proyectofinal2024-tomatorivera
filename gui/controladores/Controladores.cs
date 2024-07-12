using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
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
        private IUsuarioServicio servicio;

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
        private IUsuarioServicio servicio;

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

                    Console.ForegroundColor = ConsoleColor.Red;
                    VistasUtil.MostrarCentrado("-*- Presione una tecla para volver al menú -*-");
                    Console.ResetColor();

                    Console.ReadKey();
                }

                // Solo si el menú aún se sigue ejecutando, borro las lineas de lo escrito por los comandos
                if (estaSeleccionando) VistasUtil.BorrarDesdeLinea(lineaInicio);
            }
        }

        /// <summary>
        /// Agrega la acción de modificar el atributo "estaSeleccionado" en el comando salir
        /// para que detenga la ejecución del WHILE que mantiene activo el menú
        /// </summary>
        private void configurarComandoSalida() {
            var cmdSalir = (ComandoSalir) vista.Comandos.Where(cmd => cmd is ComandoSalir).First();
            cmdSalir.AccionPersonalizada = () => { this.estaSeleccionando = false; };
        }
    }

    public class DashboardControlador : Controlador<Dashboard>
    {
        public DashboardControlador(Dashboard vista) : base(vista)
        {
        }

        public override void MostrarVista()
        {
            vista.Dibujar();

            // Para que no se cierre el programa
            Console.ReadKey();
        }
    }
}