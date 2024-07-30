using Gui.Util;
using Gui.Vistas;
using Logica.Comandos;
using Logica.Excepciones;
using Logica.Handlers;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

namespace Gui.Controladores;

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
    public InicioControlador(Inicio vista) : base(vista)
    {}

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
        Console.ReadKey(false);
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

        // El menú se mostrará mientras no se seleccione la opción Salir
        while (estaSeleccionando)
        {
            vista.MostrarTitulo();
            vista.Dibujar();

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
                vista.Comandos.ElementAt(indiceSeleccionado).Ejecutar();
            }
            catch (Exception e)
            {
                VistasUtil.MostrarError(e.Message);
                if (!(e is VoleyballManagerRuntimeException)) VistasUtil.MostrarDetallesExcepcion(e);

                Console.ForegroundColor = ConsoleColor.Red;
                VistasUtil.MostrarCentrado("-*- Presione una tecla para volver al menú -*-");
                Console.ResetColor();

                Console.ReadKey(true);
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
        cmdSalir.AccionSalida = () => { this.estaSeleccionando = false; };
    }
}

public class DashboardControlador : Controlador<Dashboard>
{
    public DashboardControlador(Dashboard vista, float dineroPrePartido) : base(vista)
    {
        vista.DineroPrePartido = dineroPrePartido;
    }

    public float DineroPrePartido { get => vista.DineroPrePartido; set => vista.DineroPrePartido = value; }

    public override void MostrarVista()
    {
        // Limpio la consola
        AnsiConsole.Clear();

        // Si ocurrieron errores durante la carga de novedades las muestro por pantalla
        if (ErroresIgnorablesHandler.ObtenerInstancia().Errores.Any()) MostrarErrores();
        
        vista.Dibujar();
    }

    /// <summary>
    /// Carga las novedades
    /// </summary>
    public void CargarNovedades()
    {
        // Muestro un spinner mientras cargan las novedades
        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBall)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("[yellow]Cargando últimos datos...[/]", ctx => 
            {
                // Cargo las novedades para ser mostradas en el dashboard
                var novedades = obtenerNovedades().GetAwaiter().GetResult();
                vista.InformacionNovedades = novedades;
            }
        );
    }
    
    /// <summary>
    /// Obtiene novedades para ser mostradas en el dashboard
    /// </summary>
    /// <returns>Diccionario con información de la liga y de sus respectivos partidos a mostrar en novedades</returns>
    private async Task<(LeagueResponse, List<GamesResponse>)> obtenerNovedades()
    {
        var random = new Random();
        var servicioNovedades = new NovedadesServicioImpl();
        var temporada = 2024;

        // Obtengo las ligas del servicio, si no devuelve nada entonces retorno
        // datos vacíos para luego mostrar el mensaje por pantalla
        var ligas = await servicioNovedades.ObtenerLigasAsync(temporada);
        if (!ligas.Any())
        {
            return (new LeagueResponse(), new List<GamesResponse>());
        }
        
        // Selecciono una liga aleatoria y obtengo datos de sus partidos recientes
        var ligaSeleccionada = ligas[random.Next(ligas.Count())];
        var partidos = await servicioNovedades.ObtenerPartidosAsync(ligaSeleccionada.Id, temporada);

        return (ligaSeleccionada, partidos);
    }

    /// <summary>
    /// Muestra los errores guardados en <c>ErroresIgnorablesHandler</c>
    /// </summary>
    private void MostrarErrores()
    {
        var titulo = new Rule("[bold red] Se han producido uno o más errores [/]");
        titulo.LeftJustified();
        titulo.Style = Style.Parse("bold red");
        AnsiConsole.Write(titulo);
        
        foreach (var error in ErroresIgnorablesHandler.ObtenerInstancia().Errores)
        {
            AnsiConsole.Write(new Markup($"[gray]Durante la operación [/][red underline]{error.Key}[/][gray]: [/][gray]{error.Value.Message}.[/]"));
        }

        System.Console.WriteLine("\n");
        ErroresIgnorablesHandler.ObtenerInstancia().LimpiarErrores();
    }
}

public class PanelPartidoControlador : Controlador<PanelPartido>
{
    private Partido informacionPartido;

    public PanelPartidoControlador(PanelPartido vista, Partido informacionPartido) : base(vista)
    {
        this.informacionPartido = informacionPartido;
    }

    public override void MostrarVista()
    {
        vista.InformacionPartido = informacionPartido;
        vista.Dibujar();
    }

    /// <summary>
    /// Obtiene el layout sobre el cual se despliega la información del partido
    /// </summary>
    /// <returns>Objeto <c>Layout</c></returns>
    public Layout ObtenerLayoutInformacion()
    {
        return vista.LayoutInformacion;
    }

    /// <summary>
    /// Muestra una secuencia de acciones sucedidas en el partido en el layout correspondiente
    /// </summary>
    /// <param name="ctx">Contexto del objeto Live que actualiza el Layout</param>
    /// <param name="acciones">Lista de acciones a mostrar por pantalla</param>
    public void MostrarAcciones(LiveDisplayContext ctx, List<string> acciones)
    {
        vista.ActualizarAcciones(ctx, acciones, 1);
    }

    /// <summary>
    /// Muestra en el panel correspondiente qué equipo ha marcado un punto
    /// </summary>
    /// <param name="ctx">Contexto del objeto Live que actualiza el Layout</param>
    /// <param name="equipo">Tipo de equipo que ha marcado el punto (Local o Visitante)</param>
    public void MostrarPunto(LiveDisplayContext ctx, TipoEquipo equipo)
    {
        vista.MarcarPunto(ctx, equipo);
    }

    /// <summary>
    /// Actualiza la tabla de puntajes en la vista
    /// </summary>
    /// <param name="ctx">Contexto del objeto Live que actualiza el Layout</param>
    public void ActualizarMarcador(LiveDisplayContext ctx)
    {
        vista.ActualizarMarcador();
        ctx.Refresh();
    }
    
    /// <summary>
    /// Muestra una animación para el encabezado del partido
    /// </summary>
    /// <param name="figletLocal">Figlet text a mostrar del equipo local</param>
    /// <param name="figletVisitante">Figlet text a mostrar del equipo visitante</param>
    /// <param name="tipoPartido">Tipo de partido a disputarse</param>
    public void MostrarEncabezadoPartido(FigletText figletLocal, FigletText figletVisitante, TipoPartido tipoPartido)
    {
        vista.MostrarEncabezadoPartido(figletLocal, figletVisitante, tipoPartido);
    }

    /// <summary>
    /// Solicita mostrar una pantalla de cierre de partido a la vista
    /// </summary>
    /// <param name="nombreEquipoJugador">Nombre del equipo del jugador</param>
    /// <param name="nombreEquipoRival">Nombre del equipo rival</param>
    /// <param name="tipoEquipoJugador">Tipo de equipo del jugador (Local o visitante)</param>
    /// <param name="usuarioEsGanador">Indica si el usuario ha ganado o no la partida</param>
    /// <param name="fontTitulo">Fuente utilizada para desplegar el titulo de la pantalla final</param>
    /// <param name="partidoAbandonado">Indica si el partido ha sido abandonado, en dado caso muestro un mensaje diferente</param>
    public void MostrarPantallaFinal(string nombreEquipoJugador, string nombreEquipoRival, TipoEquipo tipoEquipoJugador, bool usuarioEsGanador, FigletFont? fontTitulo, bool partidoAbandonado)
    {
        vista.MostrarPantallaFinal(nombreEquipoJugador, tipoEquipoJugador, nombreEquipoRival, usuarioEsGanador, fontTitulo, partidoAbandonado);

        // Pauso la pantalla final unos segundos antes de continuar
        VistasUtil.PausarVistas(3);
    }
}

public class PanelHistorialControlador : Controlador<PanelHistorial>
{
    public PanelHistorialControlador(PanelHistorial vista) : base(vista)
    {}

    public override void MostrarVista()
    {
        vista.Dibujar();

        // Solicito una tecla para que la vista no desaparezca
        Console.ReadKey(true);
    }
}

public class PanelPlantillaControlador : Controlador<PanelPlantilla>
{
    public PanelPlantillaControlador(PanelPlantilla vista) : base(vista)
    {}

    public override void MostrarVista()
    {
        vista.Dibujar();

        // Solicito una tecla para que la vista no desaparezca
        Console.ReadKey(true);
    }
}