using Gui.Controladores;
using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
using Logica.Comandos;
using Logica.Excepciones;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

namespace Logica.Handlers;

/// <summary>
/// Clase encargada de manejar la lógica de la partida, despliega el dashboard
/// y se encarga de ejecutar los comandos que solicite el usuario
/// </summary>
public class PartidaHandler
{
    private Partida partidaActual;
    private bool deseaSalir;
    private bool partidaEliminada;
    private bool recargarNovedades;

    public PartidaHandler(Partida partidaActual)
    {
        this.partidaActual = partidaActual;
        deseaSalir = false;
        partidaEliminada = false;
        recargarNovedades = true;
    }
    
    /// <summary>
    /// Inicia la lógica de una partida
    /// </summary>
    public void IniciarPartida() {
        var servicioRecursos = new RecursoServicioImpl();
        var audioHandler = servicioRecursos.ObtenerManejadorAudio();
        audioHandler.Reproducir(Audio.MENU_SELECTION);
        
        var vistaDashboard = new Dashboard(partidaActual);
        var controladorDashboard = new DashboardControlador(vistaDashboard, partidaActual.Usuario.Dinero);
        
        IComando comandoEjecutar;
        while (!deseaSalir && !partidaEliminada)
        {
            // Las novedades se recargan solo si se ejecutaron algunos comandos o es la primera vez
            // que ingresa al bucle. Esto para evitar que se recarguen con comandos sencillos como
            // consultar la plantilla o consultar el historial, ya que implica una llamada a una API
            if (recargarNovedades)
            {
                controladorDashboard.CargarNovedades();
                recargarNovedades = false;
            }

            // Muestro el dashboard
            controladorDashboard.MostrarVista();
            controladorDashboard.DineroPrePartido = partidaActual.Usuario.Dinero;

            // Reproduce el audio del menú si es que se canceló su reproducción
            if (!audioHandler.EstaReproduciendose(Audio.MENU_BACKGROUND)) audioHandler.Reproducir(Audio.MENU_BACKGROUND, true);

            // Solicito un comando a ejecutar al usuario desde un menú de opciones
            comandoEjecutar = mostrarMenu();

            // Ejecuto la acción seleccionada por el usuario (partido amistoso, iniciar liga, iniciar torneo, consultar historial...)
            try
            {
                comandoEjecutar.Ejecutar();
            }
            catch (Exception e)
            {
                VistasUtil.MostrarError(e.Message);
                if (!(e is VoleyballManagerRuntimeException)) VistasUtil.MostrarDetallesExcepcion(e);

                Console.ForegroundColor = ConsoleColor.Red;
                VistasUtil.MostrarCentrado("-*- Presione una tecla para volver al dashboard -*-");
                Console.ResetColor();

                Console.ReadKey();
            }

            AnsiConsole.Clear();
        }

        audioHandler.Detener(Audio.MENU_BACKGROUND);

        // Cuando sale de la partida guardo todos los datos (solo en caso
        // de que la partida no haya sido eliminada) 
        if (!partidaEliminada)
        {
            var partidaServicio = new PartidaServicioImpl();
            partidaServicio.GuardarPartida(partidaActual);
        }
    }

    /// <summary>
    /// Solicita un comando a ejecutar al usuario
    /// </summary>
    /// <returns>Objeto <c>IComando</c> seleccionado desde el menú por el usuario</returns>
    private IComando mostrarMenu()
    {
        var separador = new Rule("[underline orange1]Ingrese una opción para continuar[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("bold gray")
        };
        AnsiConsole.Write(separador);

        var datosUsuario = partidaActual.Usuario;
        var seleccion = AnsiConsole.Prompt(
                            new SelectionPrompt<IComando>()
                                .Title("")
                                .HighlightStyle("yellow")
                                .AddChoices(new List<IComando>() {
                                    new ComandoJugarAmistoso(),
                                    new ComandoConsultarPlantilla(datosUsuario.Equipo.Jugadores, datosUsuario.Equipo.Nombre),
                                    new ComandoConsultarHistorial(datosUsuario.Equipo.Nombre),
                                    new ComandoEliminarPartida() { AccionCancelacion = () => { this.partidaEliminada = true; } },
                                    new ComandoSalir(TipoMenu.SECUNDARIO) { AccionSalida = () => this.deseaSalir = true }
                                })
                                .UseConverter(comando => comando.Titulo)
                            );

        // Si el comando seleccionado es uno de estos, la partida recargará las novedades luego de su ejecución
        recargarNovedades = seleccion is ComandoJugarAmistoso && ((ComandoJugarAmistoso)seleccion).SeJugaraAmistoso;

        return seleccion;
    }
}