using Gui.Controladores;
using Gui.Vistas;
using Logica.Handlers;
using Logica.Modelo;
using Spectre.Console;

namespace Logica.Comandos;

public class ComandoRealizarSustitucion : IComando
{
    public string Titulo => "Realizar una sustitución";
    private SimuladorPartidoHandler simulador;
    private Equipo equipo;
    private TipoEquipo tipoEquipo;

    public ComandoRealizarSustitucion(SimuladorPartidoHandler simulador, Equipo equipo, TipoEquipo tipoEquipo)
    {
        this.simulador = simulador;
        this.equipo = equipo;
        this.tipoEquipo = tipoEquipo;
    }

    public void Ejecutar()
    {
        var separador = new Rule()
        {
            Style = Style.Parse("gray bold")
        };
        AnsiConsole.Write(separador);

        var formacion = equipo.FormacionPartido!;
        ejecutarSustitucionUsuario(formacion);
    }

    /// <summary>
    /// Solicita los datos necesarios al usuario para realizar una sustitución
    /// </summary>
    /// <param name="plantilla">Plantilla de jugadores titulares y suplentes</param>
    private void ejecutarSustitucionUsuario(Formacion plantilla)
    {
        var jugadoresCancha = new List<Jugador>(plantilla.JugadoresCancha) { new Jugador() /* Opción de salida */ };
        var jugadoresSuplentes = new List<Jugador>(plantilla.JugadoresSuplentes) { new Jugador() /* Opción de salida */ };

        var jugadorSale = AnsiConsole.Prompt(
            new SelectionPrompt<Jugador>()
                .Title("[orange1 bold]Seleccione el jugador a sustituir:[/]")
                .HighlightStyle(Style.Parse("yellow"))
                .AddChoices(jugadoresCancha)
                .UseConverter(jugador => jugador.DescripcionPartido())
        );

        // Si selecciona el jugador con id -1, es porque desea cancelar la sustitución
        if (jugadorSale.NumeroCamiseta == -1) return;

        var jugadorIngresa = AnsiConsole.Prompt(
            new SelectionPrompt<Jugador>()
                .Title($"[orange1 bold]Seleccione el jugador que ingresará en lugar de[/] [yellow]{jugadorSale.Nombre}[/]")
                .HighlightStyle(Style.Parse("yellow"))
                .AddChoices(jugadoresSuplentes)
                .UseConverter(jugador => jugador.DescripcionPartido())
        );

        // Si selecciona el jugador con id -1, es porque desea cancelar la sustitución
        if (jugadorIngresa.NumeroCamiseta == -1) return;

        simulador.Partido.SetActual.Sustituciones.VerificarSustitucion(tipoEquipo, jugadorIngresa, jugadorSale);
        simulador.RealizarSustitucion(tipoEquipo, jugadorIngresa, jugadorSale);
        simulador.Partido.SetActual.Sustituciones.VerificarCicloSustitucionCumplido(tipoEquipo, jugadorIngresa, jugadorSale);
    }
}

public class ComandoVisualizarPlantilla : IComando
{
    public string Titulo => "Revisar plantilla de jugadores";
    private Formacion plantilla;

    public ComandoVisualizarPlantilla(Formacion plantilla)
    {
        this.plantilla = plantilla;
    }

    public void Ejecutar()
    {
        var controladorPlantilla = new PanelPlantillaControlador(new PanelPlantilla(plantilla));
        controladorPlantilla.MostrarVista();
    }
}

public class ComandoContinuarPartido : IComando
{
    public string Titulo => "Continuar el partido";

    public void Ejecutar()
    {
        // Por el momento no es requerida ninguna acción para continuar el partido
    }
}
