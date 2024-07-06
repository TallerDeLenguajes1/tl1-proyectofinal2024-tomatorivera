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

                if (tipoMenu == TipoMenu.PRINCIPAL)
                {
                    AnsiConsole.Clear();

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

                    // Muestro el msg de salida centrado vertical y horizontalmente usando el componente 'layout' de Spectre.Console
                    var layout = new Layout("raiz");
                    layout["raiz"].Update(
                        new Panel(Align.Center(
                            new Markup("[red]" + mensajeDespedida + "[/]"), VerticalAlignment.Middle
                        ))
                        .Border(BoxBorder.None)
                        .Expand()
                    );

                    AnsiConsole.Write(layout);
                    //VistasUtil.MostrarCentrado(VistasUtil.ObtenerLineasSeparadas(mensajeDespedida));
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

            // Creo un separador
            var separador = new Rule("[red]Creando nueva partida[/]");
            separador.LeftJustified();
            separador.Style = Style.Parse("bold red dim");
            AnsiConsole.Write(separador);

            // Solicito los datos al usuario
            var nombreUsuario = AnsiConsole.Prompt(
            new TextPrompt<string>("> Ingrese su nombre de DT [gray](o inserte espacios en blanco para salir)[/]:")
                .PromptStyle("yellow")
                .AllowEmpty()
            ).Trim();

            // Solo si el usuario NO ingresó una cadena vacía continúo con la ejecución
            // Caso contrario, volverá al menú principal
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                var usuarioServicio = new UsuarioServicioImpl();

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
                // Creo un separador de contenido
                var separador = new Rule("[red]Cargando una partida[/]");
                separador.LeftJustified();
                separador.Style = Style.Parse("bold red dim");
                AnsiConsole.Write(separador);

                // Esta partida la agrego para que el usuario pueda volver al menú anterior
                partidasGuardadas.Add(new Partida(-1));

                // El usuario puede seleccionar la partida a cargar desde un prompt de selección
                var partidaSeleccionada = AnsiConsole.Prompt(
                    new SelectionPrompt<Partida>()
                        .Title("Seleccione la partida que desea cargar")
                        .HighlightStyle(Style.Parse("yellow"))
                        .AddChoices(partidasGuardadas)
                );

                // Si se seleccionó una partida, se obtienen TODOS sus datos desde el servicio y se crea
                // un manejador para dicha partida, desde donde se inicia automáticamente luego de ser cargada
                if (partidaSeleccionada.Id != -1)
                {
                    var manejadorPartida = servicio.ObtenerManejadorPartida(servicio.ObtenerDatosPartida(partidaSeleccionada.Id));
                    manejadorPartida.IniciarPartida();
                }
            }
        }
    }
}