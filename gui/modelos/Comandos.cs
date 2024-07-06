using Gui.Util;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

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
        private Action? accionPersonalizada;

        public string titulo => tipoMenu.Descripcion();
        public Action? AccionPersonalizada { get => accionPersonalizada; set => accionPersonalizada = value; } 

        public ComandoSalir(TipoMenu tipoMenu)
        {
            this.tipoMenu = tipoMenu;
        }

        public void ejecutar()
        {
            System.Console.WriteLine();

            // Realizo una verificación para salir del menú
            // Cualquier primer caracter distinto de "s" será tomado como "n" para evitar bugs en la vista del menú  
            VistasUtil.MostrarCentradoSinSalto("¿Está seguro que desea salir? [si/no]: ");
            string seleccion = Console.ReadLine() ?? string.Empty;

            // Si el usuario ingresó una opción afirmativa, se debe cerrar el juego o volver al menú
            // anterior dependiendo del tipo del menú en el que estemos
            if (primerCaracter(seleccion).Equals('s'))
            {
                System.Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;

                if (tipoMenu == TipoMenu.PRINCIPAL)
                {
                    Console.Clear();

                    string mensajeDespedida = @"
    ___       ___            __
   /   | ____/ (_)___  _____/ /
  / /| |/ __  / / __ \/ ___/ / 
 / ___ / /_/ / / /_/ (__  )_/  
/_/  |_\__,_/_/\____/____(_)   
-*-
Espero que te hayas divertido :)
-*-
";
                    VistasUtil.MostrarCentrado(VistasUtil.ObtenerLineasSeparadas(mensajeDespedida));
                }
                else
                {
                    VistasUtil.MostrarCentrado("Volviendo al menu anterior...");

                    // Espero 500 ms antes de borrar este mensaje y mostrar la vista anterior
                    Thread.Sleep(500);
                    Console.Clear();
                }

                if (accionPersonalizada != null) accionPersonalizada.Invoke();
            }
        }

        /// <summary>
        /// Obtiene el primer caracter de una cadena
        /// </summary>
        /// <param name="input">Cadena</param>
        /// <returns>Primer caracter de <paramref name="input"/></returns>
        private char primerCaracter(string input) 
        {
            return String.IsNullOrWhiteSpace(input) ? ' ' : input.Trim().ToLower()[0];
        }
    }

    public class ComandoNuevaPartida : IComando
    {
        public string titulo => "Crear nueva partida";

        public void ejecutar()
        {
            /* EL USERNAME NO SE PIDE EN BUCLE PARA EVITAR BUGS VISUALES EN EL MENÚ */
            System.Console.WriteLine();

            // Solicito los datos al usuario
            Console.ForegroundColor = ConsoleColor.Green;
            VistasUtil.MostrarCentrado("+" + new string('=', Console.WindowWidth - 2) + "+");

            Console.ForegroundColor = ConsoleColor.White;
            VistasUtil.MostrarCentradoSinSalto("Ingrese su nombre de DT: ");

            var usuarioServicio = new UsuarioServicioImpl();

            // Leo el nombre borrando los espacios con el trim
            string nombreUsuario = (Console.ReadLine() ?? string.Empty).Trim();

            // Verifico si el nombre de usuario es correcto, si no lo fuese se
            // lanzará una excepción y mostrará el mensaje de error por pantalla
            usuarioServicio.ValidarNombreUsuario(nombreUsuario);

            // Creo la nueva partida y la inicio automáticamente
            var partidaServicio = new PartidaServicioImpl();
            int id = partidaServicio.ObtenerNuevoIdPartida();
            Usuario nuevoUsuario = new Usuario(nombreUsuario);
            Partida nuevaPartida = new Partida(id, DateTime.Now, nuevoUsuario);

            partidaServicio.CrearPartida(nuevaPartida);

            // Obtengo la partida desde el repositorio una vez persistida, para asegurarme de que todos su datos hayan sido creados
            // Si por alguna razón la partida creada no se guarda en el repositorio, el método ObtenerDatosPartida lanzará una excepción
            var partidaSeleccionada = partidaServicio.ObtenerDatosPartida();
            var manejadorPartida = partidaServicio.ObtenerManejadorPartida(partidaSeleccionada);

            manejadorPartida.IniciarPartida();
        }

    }

    public class ComandoCargarPartida : IComando
    {
        public string titulo => "Cargar partida";

        public void ejecutar()
        {
            System.Console.WriteLine();

            // Obtengo una lista de partidas guardadas
            PartidaServicio servicio = new PartidaServicioImpl();
            List<Partida> partidasGuardadas = servicio.ObtenerPartidas();

            // Verifico si hay al menos una partida, de lo contrario muestro un mensaje por pantalla
            if (!partidasGuardadas.Any())
            {
                VistasUtil.MostrarError("No hay partidas guardadas");
                VistasUtil.PausarVistas(2);
            }
            else
            {
                // Solicito la partida a cargar al usuario, no la solicito en bucle para evitar bugs visuales del menú

                // Muestro los datos de las partidas por pantalla
                VistasUtil.MostrarCentrado(partidasGuardadas.Select(p => p.ToString()).ToArray());

                System.Console.WriteLine();
                VistasUtil.MostrarCentradoSinSalto("► Ingrese el ID de la partida a cargar: ");

                string strOpcion = Console.ReadLine() ?? string.Empty;
                int intOpcion;

                // Verifico si la opción ingresada es un entero válido
                bool opcionValida = int.TryParse(strOpcion, out intOpcion);
                if (!opcionValida)
                {
                    VistasUtil.MostrarError("Debe ingresar un número entero");
                    VistasUtil.PausarVistas(2);
                }
                else
                {
                    Partida partidaCargar = obtenerPartida(partidasGuardadas, intOpcion);

                    // Si el ID retornado es -1, quiere decir que no se encontré una partida con el id indicado por el usuario
                    if (partidaCargar.Id == -1)
                    {
                        VistasUtil.MostrarError("El ID ingresado no corresponde a ninguna partida guardada");
                        VistasUtil.PausarVistas(2);
                    }
                    else
                    {
                        // Si se seleccionó una partida correcta, se obtienen TODOS sus datos desde el servicio y se crea
                        // un manejador para dicha partida, desde donde se inicia automáticamente luego de ser cargada
                        var manejadorPartida = servicio.ObtenerManejadorPartida(servicio.ObtenerDatosPartida(partidaCargar.Id));
                        manejadorPartida.IniciarPartida();
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene una partida de una lista de partidas según un ID específico
        /// </summary>
        /// <param name="partidas">Lista de partidas</param>
        /// <param name="id">ID de la partida a filtrar</param>
        /// <returns>Objeto <c>Partida</c> encontrado u objeto <c>Partida</c> con id -1 en caso de no encontrarse</returns>
        private Partida obtenerPartida(List<Partida> partidas, int idSeleccionado)
        {
            return partidas.Find(p => p.Id == idSeleccionado) ?? new Partida(-1);
        }
    }
}