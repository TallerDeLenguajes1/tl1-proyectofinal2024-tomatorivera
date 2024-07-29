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

    public PartidaHandler(Partida partidaActual)
    {
        this.partidaActual = partidaActual;
        this.deseaSalir = false;
    }
    
    /// <summary>
    /// Inicia la lógica de una partida
    /// </summary>
    public void IniciarPartida() {
        var vistaDashboard = new Dashboard(partidaActual);
        var controladorDashboard = new DashboardControlador(vistaDashboard);
        
        while (!deseaSalir)
        {
            // Muestro el dashboard
            controladorDashboard.MostrarVista();

            // Solicito un comando a ejecutar al usuario desde un menú de opciones
            var comandoEjecutar = mostrarMenu();

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

        // Cuando sale de la partida, guardo todos los datos
        var partidaServicio = new PartidaServicioImpl();
        partidaServicio.GuardarPartida(partidaActual);
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
                                    new ComandoConsultarPlantilla(datosUsuario.Equipo.Jugadores, datosUsuario.Nombre),
                                    new ComandoConsultarHistorial(datosUsuario.Equipo.Nombre),
                                    new ComandoSalir(TipoMenu.SECUNDARIO) { AccionSalida = () => this.deseaSalir = true }
                                })
                                .UseConverter(comando => comando.Titulo)
                            );

        return seleccion;
    }
}