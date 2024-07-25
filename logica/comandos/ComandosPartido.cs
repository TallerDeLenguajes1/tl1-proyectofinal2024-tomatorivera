using Logica.Modelo;
using Spectre.Console;

namespace Logica.Comandos;

public class ComandoRealizarSustitucion : IComando
{
    public string Titulo => "Realizar una sustitución";
    private Equipo equipo;

    public ComandoRealizarSustitucion(Equipo equipo)
    {
        this.equipo = equipo;
    }

    public void Ejecutar()
    {
        var formacion = equipo.FormacionPartido!;
        
        // Si el equipo que realizará una sustitución es del usuario, se piden los datos requeridos
        // Si es un equipo de consola, se realiza un cambio automático (o esa es la idea)
        if (equipo.EsEquipoJugador)
        {
            ejecutarSustitucionUsuario(formacion);
        }
        else
        {
            ejecutarSustitucionConsola(formacion);
        }
    }

    private void ejecutarSustitucionUsuario(Formacion plantilla)
    {
        // Falta implementar
        var jugadorSale = AnsiConsole.Prompt(
            new SelectionPrompt<Jugador>()
                .Title("[orange1 bold]Seleccione el jugador a sustituir:[/]")
                .HighlightStyle(Style.Parse("yellow"))
                .AddChoices(plantilla.JugadoresCancha)
                .UseConverter(jugador => jugador.DescripcionPartido())
        );

        var jugadorIngresa = AnsiConsole.Prompt(
            new SelectionPrompt<Jugador>()
                .Title($"[orange1 bold]Seleccione el jugador que ingresará en lugar de[/] [yellow]{jugadorSale.Nombre}[/]")
                .HighlightStyle(Style.Parse("yellow"))
                .AddChoices(plantilla.JugadoresSuplentes)
                .UseConverter(jugador => jugador.DescripcionPartido())
        );
    }

    private void ejecutarSustitucionConsola(Formacion plantilla)
    {
        // Falta implementar
    }
}

public class ComandoVisualizarPlantilla : IComando
{
    public string Titulo => "Revisar estadísticas de los jugadores en cancha";
    private Formacion plantilla;

    public ComandoVisualizarPlantilla(Formacion plantilla)
    {
        this.plantilla = plantilla;
    }

    public void Ejecutar()
    {
        throw new NotImplementedException();
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
