using Gui.Util;
using Logica.Modelo;
using Logica.Servicios;

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
            string seleccion;
            
            VistasUtil.MostrarCentradoSinSalto("¿Está seguro que desea salir? [si/no]: ");
            seleccion = Console.ReadLine() ?? string.Empty;

            // Si el usuario ingresó una opción afirmativa, se debe cerrar el juego o volver al menú
            // anterior dependiendo del tipo del menú en el que estemos
            if (primerCaracter(seleccion).Equals('s'))
            {
                System.Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;

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

    public class NuevaPartida : IComando
    {
        public string titulo => "Crear nueva partida";

        public void ejecutar()
        {
            // Creo una nueva partida
            PartidaServicio servicio = new PartidaServicioImpl();
            servicio.CrearPartida();

            // Inicio la partida creada
            Partida partidaActual = servicio.ObtenerDatosPartida();
            partidaActual.Iniciar();
        }
    }

    public class CargarPartida : IComando
    {
        public string titulo => "Cargar partida";

        public void ejecutar()
        {
            // Obtengo una lista de partidas guardadas
            PartidaServicio servicio = new PartidaServicioImpl();
            List<Partida> partidasGuardadas = servicio.ObtenerPartidas();

            Partida? partidaCargar = null;
            string strOpcion = string.Empty;
            bool opcionValida = false;
            do
            {
                System.Console.WriteLine();

                // Muestro los datos de las partidas por pantalla
                VistasUtil.MostrarCentrado(partidasGuardadas.Select(p => p.ToString()).ToArray());
                VistasUtil.MostrarCentradoSinSalto("► Ingrese el ID de la partida a cargar: ");

                strOpcion = Console.ReadLine() ?? string.Empty;
                int intOpcion;

                // Verifico si la opción ingresada es un entero válido
                opcionValida = int.TryParse(strOpcion, out intOpcion);
                if (!opcionValida)
                    VistasUtil.MostrarError("Debe ingresar un número entero");

                else
                {
                    partidaCargar = obtenerPartida(partidasGuardadas, intOpcion);

                    // Verifico si el ID ingresado corresponde a alguna partida
                    if (partidaCargar == null)
                        VistasUtil.MostrarError("El ID ingresado no corresponde a ninguna partida guardada");
                }

            } while (partidaCargar == null || !opcionValida);

            // Inicio la partida seleccionada
            partidaCargar.Iniciar();
        }

        /// <summary>
        /// Obtiene una partida de una lista de partidas según un ID específico
        /// </summary>
        /// <param name="partidas">Lista de partidas</param>
        /// <param name="id">ID de la partida a filtrar</param>
        /// <returns>Objeto <c>Partida</c></returns>
        private Partida obtenerPartida(List<Partida> partidas, int id)
        {
            return partidas.Find(p => p.Id == id);
        }
    }
}