using Gui.Controladores;
using Gui.Util;
using Gui.Vistas;
using Logica.Excepciones;
using Logica.Handlers;
using Logica.Modelo;
using Logica.Servicios;
using Spectre.Console;

namespace Logica.Comandos;

public class ComandoJugarAmistoso : IComando
{
    public string Titulo => "Jugar partido amistoso";
    private bool seJugaraAmistoso;
    private IEquipoJugadoresServicio servicioEquipos;
    private IUsuarioServicio servicioUsuario;

    public ComandoJugarAmistoso()
    {
        seJugaraAmistoso = false;
        servicioEquipos = new EquipoJugadoresServicioImpl();
        servicioUsuario = new UsuarioServicioImpl();
    }

    public bool SeJugaraAmistoso { get => seJugaraAmistoso; }

    public void Ejecutar()
    {
        var equipoJugador = seleccionarJugadoresConvocados();

        Partido? p = null;
        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBall)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("[yellow]Buscando rival...[/]", ctx => 
            {
               p = generarDatosPartidoAsync(equipoJugador).GetAwaiter().GetResult();
            }
        );

        // Si los datos del partido se generaron exitosamente, puedo iniciar el partido
        if (p != null) 
        {
            // Solicito la cantidad de sets a jugarse. No lo hago en el live porque dicho componente no soporta TextPrompts
            var nSets = solicitarRondas();
            p.SetMaximos = nSets;

            seJugaraAmistoso = true;

            var simuladorPartido = new SimuladorPartidoHandler(p);
            simuladorPartido.IniciarSimulacionPartido();
        }
    }

    /// <summary>
    /// Genera los datos necesarios para comenzar un partido
    /// </summary>
    /// <returns>Objeto <c>Partido</c></returns>
    private async Task<Partido> generarDatosPartidoAsync(Equipo equipoJugador)
    {
        // Genero el equipo rival
        var tipoEquipoConsola = (new Random().Next(2) < 0.5) ? TipoEquipo.LOCAL : TipoEquipo.VISITANTE;
        var equipoRival = await servicioEquipos.GenerarEquipoAsync();
        equipoRival.FormacionPartido = obtenerFormacionConsola(equipoRival, tipoEquipoConsola);

        // Las probabilidades del jugador de jugar de local o visitante son 50/50
        Partido datosPartido = (tipoEquipoConsola == TipoEquipo.VISITANTE) ? new Partido(equipoJugador, equipoRival, TipoPartido.AMISTOSO) 
                                                                           : new Partido(equipoRival, equipoJugador, TipoPartido.AMISTOSO);
        
        return datosPartido;
    }

    /// <summary>
    /// A partir del equipo del usuario, solicita una plantilla de 14 jugadores
    /// a ser convocados para el partido a disputarse
    /// </summary>
    /// <returns>Objeto <c>Equipo</c> solo con los jugadores convocados</returns>
    /// <exception cref="PlantillaInvalidaException">Cuando la seleccion de jugadores sea inválida</exception>
    private Equipo seleccionarJugadoresConvocados()
    {
        System.Console.WriteLine();

        // Obtengo el equipo del usuario
        var equipoJugador = servicioUsuario.ObtenerDatosUsuario().Equipo;

        // A partir de su equipo actual, le solicito 14 jugadores a convocar
        var equipoConvocado = AnsiConsole.Prompt(
            new MultiSelectionPrompt<Jugador>()
                .Title(":backhand_index_pointing_down: [orange3]Seleccione los jugadores a convocar[/]")
                .PageSize(6)
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
            throw new PlantillaInvalidaException("Debe seleccionar 14 jugadores para convocar");

        if (equipoConvocado.Where(j => j.TipoJugador == TipoJugador.LIBERO).Count() > 2)
            throw new PlantillaInvalidaException("No se pueden convocar más de dos jugadores líberos");
        
        // Solicito al usuario su equipo titular
        var titularesJugador = seleccionarEquipoTitularJugador(new List<Jugador>(equipoJugador.Jugadores));
        
        return new Equipo()
        {
            Nombre = equipoJugador.Nombre,
            Jugadores = equipoConvocado,
            EsEquipoJugador = true,
            FormacionPartido = new Formacion(titularesJugador, obtenerSuplentes(equipoConvocado, titularesJugador))
        };
    }

    /// <summary>
    /// A partir de los 14 jugadores convocados por el usuario, se le permite seleccionar
    /// los 6 jugadores que jugarán de titulares y en qué zona de la cancha comenzarán
    /// </summary>
    /// <param name="equipoConvocado">Lista de jugadores convocados</param>
    /// <returns>Objeto <c>ListaCircular</c> de <c>Jugador</c> con los titulares</returns>
    private ListaCircular<Jugador> seleccionarEquipoTitularJugador(List<Jugador> jugadoresSeleccionables)
    {
        var equipoTitular = new ListaCircular<Jugador>();
        var nJugadoresCancha = 6;
        for (int i=nJugadoresCancha ; i>0 ; i--) // Para que la lista circular quede en orden, hay que ingresar los datos de atrás para adelante
        {
            var jugador = AnsiConsole.Prompt(
                new SelectionPrompt<Jugador>()
                    .Title($":backhand_index_pointing_down: [orange3]Seleccione al jugador que iniciará en zona [/][red]{i}[/]")
                    .HighlightStyle(Style.Parse("orange1"))
                    .PageSize(6)
                    .MoreChoicesText("[grey italic](( Desplázese por los jugadores utilizando las flechas del teclado ))[/]\n\n")
                    .UseConverter(jugador => $"{jugador.Nombre} :volleyball: [gray]Saque:[/] {jugador.HabilidadSaque} pts. [gray]Remate:[/] {jugador.HabilidadRemate} pts. [gray]Recepcion:[/] {jugador.HabilidadRecepcion} pts. [gray]Colocación:[/] {jugador.HabilidadColocacion} pts. [gray]Bloqueo:[/] {jugador.HabilidadBloqueo} pts. [gray]Experiencia:[/] {jugador.Experiencia} pts.")
                    .AddChoices(jugadoresSeleccionables)
            );

            equipoTitular.Insertar(jugador);
            jugadoresSeleccionables.Remove(jugador);
        }

        return equipoTitular;
    }
    
    /// <summary>
    /// Selecciona el equipo titular de la consola
    /// </summary>
    /// <param name="equipoConvocado">Equipo generado para la consola</param>
    /// <param name="tipoEquipo">Tipo de equipo de la consola (LOCAL O VISITANTE)</param>
    /// <returns>Objeto <c>ListaCircular</c> de <c>Jugador</c> con los titulares</returns>
    private Formacion obtenerFormacionConsola(Equipo equipoConvocado, TipoEquipo tipoEquipo)
    {
        var titularesConsola = new ListaCircular<Jugador>();

        // Selecciono el equipo de la consola según sus habilidades
        var libero = equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadRecepcion).First();
        equipoConvocado.Jugadores.Remove(libero);
        var zaguero = equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadRecepcion).First();
        equipoConvocado.Jugadores.Remove(zaguero);

        titularesConsola.Insertar(zaguero); // zona 6
        titularesConsola.Insertar(libero);  // zona 5

        var lateral = equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadRemate).First();
        equipoConvocado.Jugadores.Remove(lateral);
        var armador = equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadColocacion).First();
        equipoConvocado.Jugadores.Remove(armador);
        var opuesto = (tipoEquipo == TipoEquipo.LOCAL) ? equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadBloqueo).First()
                                                       : equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadSaque).First();
        equipoConvocado.Jugadores.Remove(opuesto);

        titularesConsola.Insertar(lateral); // zona 4 
        titularesConsola.Insertar(armador); // zona 3
        titularesConsola.Insertar(opuesto); // zona 2

        var servidor = (tipoEquipo == TipoEquipo.LOCAL) ? equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadSaque).First()
                                                        : equipoConvocado.Jugadores.OrderByDescending(jugador => jugador.HabilidadRecepcion).First();
        equipoConvocado.Jugadores.Remove(servidor);

        titularesConsola.Insertar(servidor); // zona 1

        return new Formacion(titularesConsola, obtenerSuplentes(equipoConvocado.Jugadores, titularesConsola));
    }

    /// <summary>
    /// Obtiene los jugadores suplentes de una plantilla
    /// </summary>
    /// <param name="jugadores">Lista completa de jugadores</param>
    /// <param name="titulares">Lista de titulares</param>
    /// <returns>Lista Circular con jugadores suplentes</returns>
    private ListaCircular<Jugador> obtenerSuplentes(List<Jugador> jugadores, ListaCircular<Jugador> titulares)
    {
        var listaSuplentes = new ListaCircular<Jugador>();
        foreach (var jugadorSuplente in jugadores.Where(jugador => !titulares.Contains(jugador)).ToList())
        {
            listaSuplentes.Insertar(jugadorSuplente);
        }

        return listaSuplentes;
    }

    /// <summary>
    /// Solicita al usuario el número de rondas que se jugarán
    /// </summary>
    /// <returns>Número de rondas (verificado)</returns>
    private int solicitarRondas()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>(":backhand_index_pointing_right: [orange3]Indique al mejor de cuantas rondas se va a jugar:[/]")
                .PromptStyle("yellow")
                .DefaultValue(3)
                .HideDefaultValue()
                .ValidationErrorMessage("[red]Debe ingresar un numero entero[/]")
                .Validate(input => {
                    if (input % 2 == 0)
                        return ValidationResult.Error("[red]Debe ingresar un número impar[/]");
                    if (input < 1)
                        return ValidationResult.Error("[red]Debe jugarse al menos 1 set[/]");

                    return ValidationResult.Success();
                })
        );
    }
}

public class ComandoConsultarPlantilla : IComando
{
    public string Titulo => "Plantilla de jugadores";

    private List<Jugador> jugadores;
    private string nombreEquipo;

    public ComandoConsultarPlantilla(List<Jugador> jugadores, string nombreEquipo)
    {
        this.jugadores = jugadores;
        this.nombreEquipo = nombreEquipo;
    }

    public void Ejecutar()
    {
        // Genero el panel de información de la plantilla y lo muestro al usuario
        var controladorPanelPlantilla = new PanelPlantillaControlador(new PanelPlantilla(jugadores, nombreEquipo));
        controladorPanelPlantilla.MostrarVista();
    }
}

public class ComandoConsultarHistorial : IComando
{
    public string Titulo => "Historial de partidos";
    private string nombreEquipoJugador;
    private IHistorialServicio servicioHistorial;

    public ComandoConsultarHistorial(string nombreEquipoJugador)
    {
        this.nombreEquipoJugador = nombreEquipoJugador;
        servicioHistorial = new HistorialServicioImpl();
    }

    public void Ejecutar()
    {
        // Obtengo el historial de la partida actual
        var historial = servicioHistorial.ObtenerDatosHistorial();
        
        // Genero un panel de información del historial y lo muestro por pantalla
        var controladorPanelHistorial = new PanelHistorialControlador(new PanelHistorial(historial, nombreEquipoJugador));
        controladorPanelHistorial.MostrarVista();
    }
}

public class ComandoEliminarPartida : IComando
{
    public string Titulo => "Eliminar partida";
    public Action? AccionCancelacion { get; set; }
    private IPartidaServicio partidaServicio;

    public ComandoEliminarPartida()
    {
        partidaServicio = new PartidaServicioImpl();
    }

    public void Ejecutar()
    {
        if (pregunta("¿Está seguro de que desea eliminar esta partida? [si/no]: "))
        {
            partidaServicio.EliminarPartida();

            if (AccionCancelacion != null) AccionCancelacion.Invoke();
        }
    }

    /// <summary>
    /// Realiza una pregunta de sí o no al usuario
    /// </summary>
    /// <param name="textoPregunta">Pregunta a realizar</param>
    /// <returns><c>True</c> si el primer caracter de la cadena es 's', <c>False</c> en caso contrario</returns>
    private bool pregunta(string textoPregunta) 
    {
        System.Console.WriteLine();

        VistasUtil.MostrarCentradoSinSalto(textoPregunta);
        string respuesta = Console.ReadLine() ?? string.Empty;

        return !string.IsNullOrWhiteSpace(respuesta) && respuesta.ToLower()[0].Equals('s');
    }
}

public class ComandoMercadoJugadores : IComando
{
    public string Titulo => "Mercado de jugadores";
    private IMercadoServicio mercadoServicio;
    private List<Jugador> jugadoresUsuario;

    public ComandoMercadoJugadores(List<Jugador> jugadoresUsuario)
    {
        this.jugadoresUsuario = jugadoresUsuario;
        mercadoServicio = new MercadoServicioImpl();
    }

    public void Ejecutar()
    {
        var mercadoActual = mercadoServicio.ObtenerDatosMercado();
        // Si ya pasaron 12 horas desde la última actualización del mercado, se regenera
        if (debeRenerarMercado(mercadoActual)) mercadoActual = mercadoServicio.RegenerarMercadoAsync().GetAwaiter().GetResult();
        
        var controladorPanelMercado = new PanelMercadoControlador(new PanelMercado(mercadoActual, jugadoresUsuario));

        // Ejecuto el menú del mercado mientras el usuario no seleccione 'volver al dashboard'
        Jugador jugadorComprar;
        do
        {
            controladorPanelMercado.MostrarVista();

            jugadorComprar = mostrarMenuSeleccion(mercadoActual);

            // Si el jugador seleccionó que desea cancelar la compra, vuelvo atrás
            if (string.IsNullOrWhiteSpace(jugadorComprar.Nombre)) return;

            if (pregunta("¿Seguro de que desea realizar esta compra? [si/no]: "))
            {
                mercadoServicio.RealizarCompraJugador(jugadorComprar);
            }   
        }
        while (!string.IsNullOrEmpty(jugadorComprar.Nombre));
    }

    /// <summary>
    /// Muestra el menú de selección de jugadores disponibles para comprar
    /// </summary>
    /// <param name="mercado"></param>
    /// <returns></returns>
    private Jugador mostrarMenuSeleccion(Mercado mercado)
    {
        var jugadoresMostrar = new List<Jugador>(mercado.Jugadores){ new Jugador() /* opción de salida */ };

        return AnsiConsole.Prompt(
            new SelectionPrompt<Jugador>()
                .Title("[orange1 bold]Seleccione el jugador a comprar:[/]")
                .HighlightStyle("navajowhite1")
                .AddChoices(jugadoresMostrar.Where(j => !esJugadorComprado(j)))
                .UseConverter(j => string.IsNullOrWhiteSpace(j.Nombre) ? "[red3]:right_arrow_curving_left: Volver al dashboard[/]"
                                                                       : j.DescripcionMercado())
        );
    }

    /// <summary>
    /// Realiza una pregunta de sí o no al usuario
    /// </summary>
    /// <param name="textoPregunta">Pregunta a realizar</param>
    /// <returns><c>True</c> si el primer caracter de la cadena es 's', <c>False</c> en caso contrario</returns>
    private bool pregunta(string textoPregunta) 
    {
        System.Console.WriteLine();

        VistasUtil.MostrarCentradoSinSalto(textoPregunta);
        string respuesta = Console.ReadLine() ?? string.Empty;

        return !string.IsNullOrWhiteSpace(respuesta) && respuesta.ToLower()[0].Equals('s');
    }

    /// <summary>
    /// Verifica si ya pasaron las 12 horas desde la ultima actualizacion del mercado
    /// </summary>
    /// <param name="mercado">Mercado a evaluar</param>
    /// <returns>True o false</returns>
    private bool debeRenerarMercado(Mercado mercado)
    {
        return mercado.UltimaActualizacion.AddHours(12) < DateTime.Now;
    }

    /// <summary>
    /// Verifica si el usuario ya posee al jugador
    /// </summary>
    /// <param name="jugador">Jugador a verificar</param>
    /// <returns><c>True</c> si el usuario ya tiene un jugador con el mismo nombre, habilidades y experiencia que <paramref name="jugador"/>, <c>False</c> en caso contrario</returns>
    private bool esJugadorComprado(Jugador jugador)
    {
        return jugadoresUsuario.Where(j => j.Nombre.Equals(jugador.Nombre) &&
                                           j.HabilidadSaque == jugador.HabilidadSaque &&
                                           j.HabilidadBloqueo == jugador.HabilidadBloqueo &&
                                           j.HabilidadColocacion == jugador.HabilidadColocacion &&
                                           j.HabilidadRecepcion == jugador.HabilidadRecepcion &&
                                           j.HabilidadRemate == jugador.HabilidadRemate &&
                                           j.Experiencia == jugador.Experiencia
                                    )
                                    .Any();
    }
}
