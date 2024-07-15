using System.Text.RegularExpressions;
using Gui.Modelo;
using Gui.Util;
using Logica.Handlers;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

namespace Logica.Comandos
{
    /// <summary>
    /// Interfaz que representa un comando de un menú
    /// </summary>
    public interface IComando
    {
        /// <value>La propiedad nombre es como se muestra el comando en un menú</value>
        public string titulo { get; }

        /// <summary>
        /// Realiza todas las acciones que este comando deba realizar
        /// al ser seleccionado mediante un menú
        /// </summary>
        public void ejecutar();
    }

    public class ComandoSalir : IComando
    {
        private TipoMenu tipoMenu;
        private Action? accionSalida;

        public string titulo => tipoMenu.Descripcion();
        public Action? AccionSalida { get => accionSalida; set => accionSalida = value; } 

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
                }
                else
                {
                    AnsiConsole.Write(Align.Center(new Markup("[gray italic]Volviendo al menú anterior...[/]")));
                    VistasUtil.PausarVistas(1);
                }

                if (accionSalida != null) accionSalida.Invoke();
            }
        }

        /// <summary>
        /// Obtiene el primer caracter de una cadena
        /// </summary>
        /// <param name="input">Cadena</param>
        /// <returns>Primer caracter de <paramref name="input"/></returns>
        private char primerCaracter(string input) 
        {
            return string.IsNullOrWhiteSpace(input) ? ' ' : input.Trim().ToLower()[0];
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
                    .Validate(nombreInput => {
                        return ValidarNombre(nombreInput.Trim());
                    })
            ).Trim();

            // Solo si el usuario NO ingresó una cadena vacía continúo con la ejecución
            // Caso contrario, volverá al menú principal
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return; 

            // Solicito los datos del equipo
            var nombreEquipo = AnsiConsole.Prompt(
                new TextPrompt<string>("> Ingrese el nombre de su equipo [gray](o inserte espacios en blanco para generarlo automáticamente)[/]:")
                    .PromptStyle("yellow")
                    .AllowEmpty()
                    .Validate(nombreInput => {
                        return ValidarNombre(nombreInput.Trim());
                    })
            ).Trim();

            PartidaHandler? manejadorPartida = null;
            AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBall)
                .SpinnerStyle(Style.Parse("yellow bold"))
                .Start("[yellow]Creando nueva partida...[/]", ctx => 
                {
                    manejadorPartida = crearPartidaAsync(nombreUsuario, nombreEquipo).GetAwaiter().GetResult();
                }
            );

            // Muestro aquellos errores que se pueden haber producido pero que se puedan ignorar
            MostrarErroresIgnorables();

            // Si se ha cargado una partida y su manejador correctamente, inicio la partida
            if (manejadorPartida != null)
            {
                manejadorPartida.IniciarPartida();
            }
        }

        /// <summary>
        /// Crea los datos de una nueva partida
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario</param>
        /// <param name="nombreEquipo">Nombre del nuevo equipo</param>
        /// <returns>Objeto <c>PartidaHandler</c> para manejar la partida creada</returns>
        private async Task<PartidaHandler> crearPartidaAsync(string nombreUsuario, string nombreEquipo)
        {
            // Creo la nueva partida y la inicio automáticamente
            var partidaServicio = new PartidaServicioImpl();
            var equipoServicio = new EquipoJugadoresServicioImpl();

            // Genero los datos necesarios para crear una nueva partida
            int id = partidaServicio.ObtenerNuevoIdPartida();
            Equipo nuevoEquipo = await equipoServicio.GenerarEquipoAsync(nombreEquipo);
            Usuario nuevoUsuario = new Usuario(nombreUsuario, nuevoEquipo);
            Historial nuevoHistorial = new Historial();
            Partida nuevaPartida = new Partida(id, DateTime.Now, nuevoUsuario, nuevoHistorial);

            // Creo la partida
            partidaServicio.CrearPartida(nuevaPartida);
            // Guardo el nombre del equipo del usuario
            equipoServicio.AlmacenarNombreEquipoUsuario(nuevoEquipo.Nombre);

            // Obtengo la partida desde el repositorio una vez persistida, para asegurarme de que todos su datos hayan sido creados
            // Si por alguna razón la partida creada no se guarda en el repositorio, el método ObtenerDatosPartida lanzará una excepción
            var partidaCreada = partidaServicio.ObtenerDatosPartida();
            return partidaServicio.ObtenerManejadorPartida(partidaCreada);
        }

        /// <summary>
        /// Valida si un nombre es correcto
        /// </summary>
        /// <param name="nombre">Nombre a validar</param>
        /// <returns>Objeto <c>ValidationResult</c> que indica el estado de la validación para los prompts</returns>
        private ValidationResult ValidarNombre(string nombre)
        {
            if (nombre.Length == 0) return ValidationResult.Success();

            // Verifico si el nombre de usuario cumple con la longitud establecida
            if (nombre.Length < 3 || nombre.Length > 15)
                return ValidationResult.Error("[red]El nombre de usuario debe tener de 3 a 15 caracteres[/]");
            
            // Regex para validar si contiene solo carácteres alfanuméricos
            Regex rgx = new Regex("^[a-zA-Z]+$");
            
            if (!rgx.IsMatch(nombre))
                return ValidationResult.Error("[red]El nombre de usuario debe tener solo caracteres alfabeticos[/]");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Muestra por pantalla aquellos errores que se hayan producido durante el proceso de
        /// creación de la partida, pero que no detendrán la ejecución del juego
        /// </summary>
        private void MostrarErroresIgnorables()
        {   
            System.Console.WriteLine();

            if (ErroresIgnorablesHandler.ObtenerInstancia().Errores.Any())
            {  
                var titulo = new Rule("[yellow] Se han producido uno o más errores [/]");
                titulo.LeftJustified();
                titulo.Style = Style.Parse("yellow");
                AnsiConsole.Write(titulo);
                
                foreach (var error in ErroresIgnorablesHandler.ObtenerInstancia().Errores)
                {
                    AnsiConsole.Write(new Markup($"Durante la operación: [gray]{error.Key}[/] - {error.Value.InnerException?.Message}. {error.Value.Message}"));
                }

                ErroresIgnorablesHandler.ObtenerInstancia().LimpiarErrores();

                System.Console.WriteLine("\n");
                AnsiConsole.Write(new Markup("[yellow]Presione una tecla para continuar el juego...[/]"));
                Console.ReadKey();
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
            IPartidaServicio servicio = new PartidaServicioImpl();
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
                    PartidaHandler? manejadorPartida = null;
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.BouncingBall)
                        .SpinnerStyle(Style.Parse("yellow bold"))
                        .Start("[yellow]Cargando partida...[/]", ctx => 
                        {
                            // Cargo las novedades para ser mostradas en el dashboard
                            manejadorPartida = servicio.ObtenerManejadorPartida(servicio.ObtenerDatosPartida(partidaSeleccionada.Id));
                        }
                    );

                    if (manejadorPartida != null) manejadorPartida.IniciarPartida();
                }
            }
        }
    }

    public class ComandoJugarAmistoso : IComando
    {
        public string titulo => "Jugar partido amistoso";

        public void ejecutar()
        {
            Partido? p = null;
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Clock)
                .SpinnerStyle(Style.Parse("yellow bold"))
                .Start("[yellow]Esperando al jugador...[/]", ctx => 
                {
                    var equipoJugador = seleccionarEquipoTitular();

                    ctx.Status("[yellow]Buscando rival...[/]");

                    p = generarDatosPartidoAsync(equipoJugador).GetAwaiter().GetResult();
                }
            );

            // Si los datos del partido se generaron exitosamente, inicio el partido
            if (p != null) new SimuladorPartidoHandler().IniciarPartido(p);
        }

        /// <summary>
        /// Genera los datos necesarios para comenzar un partido
        /// </summary>
        /// <returns>Objeto <c>Partido</c></returns>
        private async Task<Partido> generarDatosPartidoAsync(Equipo equipoJugador)
        {
            var servicioEquipos = new EquipoJugadoresServicioImpl();
            
            // Genero el equipo rival
            var equipoRival = await servicioEquipos.GenerarEquipoAsync();

            // Las probabilidades del jugador de jugar de local o visitante son 50/50
            Partido datosPartido = (new Random().Next(2) < 0.5) ? new Partido(equipoJugador, equipoRival, TipoPartido.AMISTOSO) 
                                                                : new Partido(equipoRival, equipoJugador, TipoPartido.AMISTOSO);
            
            return datosPartido;
        }

        /// <summary>
        /// A partir del equipo del usuario, solicita una plantilla de 14 jugadores
        /// a ser convocados para el partido a disputarse
        /// </summary>
        /// <returns>Objeto <c>Equipo</c> solo con los jugadores convocados</returns>
        /// <exception cref="Exception">Cuando la seleccion de jugadores sea inválida</exception>
        private Equipo seleccionarEquipoTitular()
        {
            System.Console.WriteLine();

            // Muestro un separador
            var separador = new Rule("[underline orange3]Seleccione su equipo titular[/]")
            {
                Justification = Justify.Left,
                Style = Style.Parse("bold gray")
            };
            AnsiConsole.Write(separador);

            // Obtengo el equipo del usuario
            var servicioUsuario = new UsuarioServicioImpl();
            var equipoJugador = servicioUsuario.ObtenerDatosUsuario().Equipo;

            // A partir de su equipo actual, le solicito 14 jugadores a convocar
            var equipoConvocado = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Jugador>()
                    .Title("")
                    .PageSize(14)
                    .HighlightStyle(Style.Parse("orange1"))
                    .MoreChoicesText("[grey italic](( Desplázese por los jugadores utilizando las flechas del teclado ))[/]")
                    .InstructionsText("[gray italic]Presione[/] [tan]<< espacio >>[/] [gray italic]para seleccionar y[/] [tan]<< enter >>[/] [gray italic]para finalizar[/]\n")
                    .UseConverter(jugador => jugador.ToString())
                    .AddChoiceGroup(new Jugador(TipoJugador.PUNTA), equipoJugador.Jugadores.Where(j => j.TipoJugador == TipoJugador.PUNTA))
                    .AddChoiceGroup(new Jugador(TipoJugador.OPUESTO), equipoJugador.Jugadores.Where(j => j.TipoJugador == TipoJugador.OPUESTO))
                    .AddChoiceGroup(new Jugador(TipoJugador.ARMADOR), equipoJugador.Jugadores.Where(j => j.TipoJugador == TipoJugador.ARMADOR))
                    .AddChoiceGroup(new Jugador(TipoJugador.CENTRAL), equipoJugador.Jugadores.Where(j => j.TipoJugador == TipoJugador.CENTRAL))
                    .AddChoiceGroup(new Jugador(TipoJugador.LIBERO), equipoJugador.Jugadores.Where(j => j.TipoJugador == TipoJugador.LIBERO))
            );

            // Si la selección es inválida por algún motivo, le notifico al jugador
            // caso contrario retorno un equipo con los datos del equipo del usuario y una lista con los jugadores convocados
            if (equipoConvocado.Count() != 14)
                throw new Exception("Debe seleccionar 14 jugadores para convocar");

            if (equipoConvocado.Where(j => j.TipoJugador == TipoJugador.LIBERO).Count() > 2)
                throw new Exception("No se pueden convocar más de dos jugadores líberos");

            return new Equipo()
            {
                Nombre = equipoJugador.Nombre,
                Jugadores = equipoConvocado
            };

        }
    }

    public class ComandoConsultarPlantilla : IComando
    {
        public string titulo => "Consultar plantilla de jugadores";

        public void ejecutar()
        {
            throw new NotImplementedException("Esta función no ha sido implementada aún");
        }
    }

    public class ComandoConsultarHistorial : IComando
    {
        public string titulo => "Consultar historial de partidos";

        public void ejecutar()
        {
            throw new NotImplementedException("Esta función no ha sido implementada aún");
        }
    }
}