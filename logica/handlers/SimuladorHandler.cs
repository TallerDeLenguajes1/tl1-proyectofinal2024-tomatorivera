using Spectre.Console.Rendering;
using Persistencia.Infraestructura;
using Persistencia.Util;
using Logica.Excepciones;
using Logica.Modelo;
using Spectre.Console;
using Logica.Comandos;
using Gui.Controladores;
using Gui.Util;
using Gui.Vistas;
using Logica.Servicios;

namespace Logica.Handlers;

/// <summary>
/// Clase encargada de manejar la lógica de simulación de partidos
/// </summary>
public class SimuladorPartidoHandler
{
    private Partido partido;
    private int setsRestantes;
    private TipoEquipo posesionPelota;
    private PanelPartidoControlador panelPartidoControlador;
    private bool partidoAbandonado;
    private AudioHandler audioHandler;

    public SimuladorPartidoHandler(Partido partido)
    {
        this.partido = partido;

        // Valores iniciales por defecto
        setsRestantes = partido.SetMaximos;
        posesionPelota = TipoEquipo.LOCAL;
        partidoAbandonado = false;

        // Inicializo el controlador del panel de la vista del partido
        panelPartidoControlador = new PanelPartidoControlador(new PanelPartido(), partido);
        audioHandler = new RecursoServicioImpl().ObtenerManejadorAudio();
    }

    /* Propiedades */
    public Partido Partido { get => partido; }

    /* Métodos */

    /// <summary>
    /// Método encargado de inicializar la simulación de un partido, sus vistas y objetos necesarios
    /// </summary>
    public void IniciarSimulacionPartido()
    {
        audioHandler.Detener(Audio.MENU_BACKGROUND);
        //audioHandler.Reproducir(Audio.MENU_SELECTION);

        // Muestro un encabezado
        mostrarEncabezadoPartido();

        // Determino qué equipo hará el saque (probabilidad de 50/50)
        partido.EquipoEnSaque = determinarSaque();

        // Muestro las vistas del partido
        panelPartidoControlador.MostrarVista();
        
        // Inicializo la lógica del partido
        jugarPartido();

        // Guardo el partido jugado en el historial
        var servicioHistorial = new HistorialServicioImpl();
        servicioHistorial.GuardarPartido(partido);

        // Muestro una pantalla final del partido
        mostrarPantallaFinal();
    }

    /// <summary>
    /// Muestra una animación para el encabezado del partido
    /// </summary>
    private void mostrarEncabezadoPartido()
    {
        // Los nombres del local y el visitante se mostrarán como texto figlet con una fuente de ascii personalizada
        // En caso de no poder cargar dicha fuente, se mostrarán con la fuente figlet por defecto.
        FigletText figletLocal, figletVisitante;
        try
        {
            RecursosUtil.VerificarArchivo(Config.DirectorioFuentes + @"\" + Config.FuentePagga);

            var figletFont = FigletFont.Load(Config.DirectorioFuentes + @"\" + Config.FuentePagga);
            figletLocal = new FigletText(figletFont, partido.Local.Nombre).Color(Color.Red1);
            figletVisitante = new FigletText(figletFont, partido.Visitante.Nombre).Color(Color.Red1);
        }
        catch (Exception)
        {
            figletLocal = new FigletText(partido.Local.Nombre).Color(Color.Red);
            figletVisitante = new FigletText(partido.Visitante.Nombre).Color(Color.Red);
        }

        // Solicito a la vista que muestre por pantalla el encabezado
        panelPartidoControlador.MostrarEncabezadoPartido(figletLocal, figletVisitante, partido.TipoPartido, audioHandler);
    }

    /// <summary>
    /// Despliega una pantalla final al terminar de jugar un partido
    /// </summary>
    private void mostrarPantallaFinal()
    {
        // Cargo los datos a desplegar en la pantalla final
        FigletFont? fontTitulo;
        try
        {
            var fontPath = Config.DirectorioFuentes + @"\" + Config.FuenteDosRebel;
            RecursosUtil.VerificarArchivo(fontPath);
            fontTitulo = FigletFont.Load(fontPath);
        }
        catch (Exception)
        {
            fontTitulo = null;
        }

        string nombreEquipoJugador;
        string nombreEquipoRival;
        bool esGanadorUsuario;
        TipoEquipo tipoEquipoJugador;
        if (partido.Local.EsEquipoJugador)
        {
            nombreEquipoJugador = partido.Local.Nombre;
            nombreEquipoRival = partido.Visitante.Nombre;
            esGanadorUsuario = partido.ScoreLocal > partido.ScoreVisitante;
            tipoEquipoJugador = TipoEquipo.LOCAL;
        }
        else
        {
            nombreEquipoJugador = partido.Visitante.Nombre;
            nombreEquipoRival = partido.Local.Nombre;
            esGanadorUsuario = partido.ScoreVisitante > partido.ScoreLocal;
            tipoEquipoJugador = TipoEquipo.VISITANTE;
        }

        // Paso los datos al controlador para que la vista se encargue de mostrar la información
        panelPartidoControlador.MostrarPantallaFinal(nombreEquipoJugador, nombreEquipoRival, tipoEquipoJugador, esGanadorUsuario, fontTitulo, partidoAbandonado, audioHandler, esGanadorUsuario ? Audio.PARTIDO_GANADO : Audio.PARTIDO_PERDIDO);
    }

    /// <summary>
    /// Método encargado de ejecutar la lógica de un partido
    /// </summary>
    private void jugarPartido()
    {
        audioHandler.Reproducir(Audio.PARTIDO_BACKGROUND, true);

        // El partido termina cuando ya se hayan jugado todos los sets o cuando se pueda
        // determinar un ganador según el puntaje de los equipos tras cada ronda, o bien cuando
        // el jugador decida abandonar el partido
        while (!partidoAbandonado && setsRestantes != 0 && !hayGanadorPartido(partido.ScoreLocal, partido.ScoreVisitante))
        {
            // Comienza un set
            jugarSet();

            // Actualizo la información necesaria
            setsRestantes--;

            almacenarResultadoSetActual();
            partido.SetActual.SiguienteSet();
        }

        audioHandler.Detener(Audio.PARTIDO_BACKGROUND);

        if (partidoAbandonado)
        {
            partido.NombreGanador = "ABANDONADO";
        }
        else
        {
            // Guardo el nombre del equipo ganador
            var ganador = (partido.ScoreLocal > partido.ScoreVisitante) ? partido.Local : partido.Visitante;
            partido.NombreGanador = ganador.Nombre;

            // Si el jugador gana el partido, puede generar de $15.000 a $30.000, caso contrario, genera de $1.000 a $10.000
            var rnd = new Random();
            var servicioJugador = new UsuarioServicioImpl();
            servicioJugador.ObtenerDatosUsuario().Dinero += ganador.EsEquipoJugador ? rnd.Next(15000, 30001)
                                                                                    : rnd.Next(1000, 10001);
        }

        /*
        // Inicializo la información del rally
        var rally = new Rally(partido.Local.FormacionPartido, partido.Visitante.FormacionPartido, posesionPelota, equipoEnSaque.FormacionPartido.ObtenerJugadorZona(1));

        // El partido termina cuando ya se hayan jugado todos los sets o cuando se pueda
        // determinar un ganador según el puntaje de los equipos tras cada ronda
        while (setsRestantes != 0 && !hayGanadorPartido(partido.ScoreLocal, partido.ScoreVisitante))
        {
            // Actualizo el panel de información con el componente Live para ir mostrando las acciones en una misma vista
            AnsiConsole.Live(controladorPanel.ObtenerLayoutInformacion())
                .Start(ctx => {
                    // Actualizo la información de las vistas
                    controladorPanel.MostrarVista();

                    // Comienzo el rally
                    rally.ComenzarRally();
                    determinarResultadoRally(rally);

                    // Muestro las acciones del rally
                    controladorPanel.MostrarAcciones(ctx, rally.AccionesRally);
                    controladorPanel.MostrarPunto(ctx, posesionPelota);
                    rally.AccionesRally.Clear();

                    // Si después de un rally hay un ganador del set, se realizan algunas acciones para pasar al siguiente
                    if (hayGanadorSet())
                    {
                        // El ganador del set siempre es el último que hizo el punto, entonces sumo el score
                        // a ese equipo si se determinó que hay un ganador del set
                        if (posesionPelota == TipoEquipo.LOCAL)
                            partido.ScoreLocal++;
                        else    
                            partido.ScoreVisitante++;

                        // Actualizo la información necesaria
                        cambiosRestantes = 12;
                        setsRestantes--;
                        partido.SetActual++;
                        partido.ResultadoSets.Add(partido.SetActual, new ResultadoSet());
                    }
                });
            
            // Muestro las opciones del partido al usuario y ejecuto la que elija
            ejecutarMenu();
            */
    }

    /// <summary>
    /// Método encargado de manejar la lógica de un set
    /// </summary>
    /// <exception cref="FormacionInvalidaException">Si alguna de las formaciones fuese nula</exception>
    private void jugarSet()
    {
        // Verifico si por alguna razón las formaciones de los equipos en juego son nulas
        if (partido.Local.FormacionPartido == null)
            throw new NullReferenceException($"La formación del equipo {partido.Local.Nombre} es nula");
        if (partido.Visitante.FormacionPartido == null)
            throw new NullReferenceException($"La formación del equipo {partido.Visitante.Nombre} es nula");
        if (partido.EquipoEnSaque.FormacionPartido == null)
            throw new NullReferenceException($"La formación del equipo {partido.EquipoEnSaque.Nombre} es nula");
        
        var set = partido.SetActual;
        var rally = new Rally(partido.Local.FormacionPartido, partido.Visitante.FormacionPartido, posesionPelota, partido.EquipoEnSaque.FormacionPartido.ObtenerJugadorZona(1));

        // El set termina cuando uno de los equipos cuente con el puntaje requerido
        // para ganarlo con una diferencia de dos puntos por encima del rival
        while (!partidoAbandonado && !set.HayGanadorSet())
        {
            rally.JugadorActual = partido.EquipoEnSaque.FormacionPartido.ObtenerJugadorZona(1);

            // Actualizo el panel de información con el componente Live para ir mostrando las acciones en una misma vista
            AnsiConsole.Live(panelPartidoControlador.ObtenerLayoutInformacion())
                .Start(ctx => {
                    // Actualizo la información general de la vista
                    panelPartidoControlador.MostrarVista();

                    // Comienzo el rally
                    rally.ComenzarRally();
                    determinarResultadoRally(rally);

                    // Muestro las acciones del rally
                    panelPartidoControlador.MostrarAcciones(ctx, rally.AccionesRally);
                    panelPartidoControlador.ActualizarMarcador(ctx);
                    //panelPartidoControlador.MostrarPunto(ctx, posesionPelota);
                    rally.AccionesRally.Clear();
                });

            ejecutarMenu();
        }

        incrementarScore();
    }

    /// <summary>
    /// Almacena el resultado de un set en la lista correspondiente de resultados de sets del partido
    /// </summary>
    private void almacenarResultadoSetActual()
    {
        partido.ResultadoSets.Add(partido.SetActual.NumeroSet, partido.SetActual.Resultado);
    }

    /// <summary>
    /// Determina si con los sets ganados actuales de los equipos se puede determinar que
    /// hay un ganador según los sets restantes y la cantidad de rondas a jugarse
    /// </summary>
    /// <param name="scoreLocal">Sets ganados del equipo local</param>
    /// <param name="scoreVisitante">Sets ganados del equipo visitante</param>
    /// <returns><c>True</c> si ya se puede decidir un ganador, <c>False</c> en caso contrario</returns>
    private bool hayGanadorPartido(int scoreLocal, int scoreVisitante)
    {
        return (scoreLocal + setsRestantes < scoreVisitante) ||
               (scoreVisitante + setsRestantes < scoreLocal);
    }

    /// <summary>
    /// Determina el resultado del rally y realiza acciones en consecuencias de ello
    /// </summary>
    /// <param name="rallyActual">Información del rally a evaluar</param>
    private void determinarResultadoRally(Rally rallyActual)
    {
        switch (rallyActual.PosesionPelota)
        {
            // Si la última posesión de la pelota es del equipo local, quiere decir que fue punto del visitante
            case TipoEquipo.LOCAL:
                if (partido.EquipoEnSaque.Nombre.Equals(partido.Local.Nombre))
                {
                    //System.Console.WriteLine("Recupera el saque y rota el equipo VISITANTE");
                    partido.Visitante.FormacionPartido!.JugadoresCancha.Rotar();
                    partido.EquipoEnSaque = partido.Visitante;
                }

                posesionPelota = TipoEquipo.VISITANTE;
                //System.Console.WriteLine("Punto para VISITANTE");
                break;

            // Si la última posesión de la pelota es del equipo visitante, quiere decir que fue punto del local
            case TipoEquipo.VISITANTE:
                if (partido.EquipoEnSaque.Nombre.Equals(partido.Visitante.Nombre))
                {
                    //System.Console.WriteLine("Recupera el saque y rota el equipo LOCAL");
                    partido.Local.FormacionPartido!.JugadoresCancha.Rotar();
                    partido.EquipoEnSaque = partido.Local;
                }
                posesionPelota = TipoEquipo.LOCAL;
                //System.Console.WriteLine("Punto para LOCAL");
                break;
        }

        // Actualizo la información del rally y del partido
        partido.SetActual.Resultado.IncrementarPuntos(posesionPelota);
        rallyActual.PosesionPelota = posesionPelota;
    }

    /// <summary>
    /// Determina qué equipo comenzará el saque según una probabilidad 50/50
    /// </summary>
    /// <returns>Equipo que comenzará sacando</returns>
    private Equipo determinarSaque()
    {
        if (new Random().NextDouble() >= 0.5)
        {
            posesionPelota = TipoEquipo.LOCAL;
            return partido.Local;
        }
        else
        {
            posesionPelota = TipoEquipo.VISITANTE;
            return partido.Visitante;
        }
    }

    /// <summary>
    /// Ejecuta el menú del partido. Solicita una opción al usuario y la ejecuta
    /// capturando cualquier error que pudiese ocurrir.
    /// </summary>
    private void ejecutarMenu()
    {
        IComando comando;

        Equipo equipoJugador;
        TipoEquipo tipoEquipoJugador;

        // El menú se ejecuta mientras el usuario no seleccione la opción para continuar el partido
        do
        {
            if (partido.Local.EsEquipoJugador)
            {
                equipoJugador = partido.Local;
                tipoEquipoJugador = TipoEquipo.LOCAL;
            }
            else
            {
                equipoJugador = partido.Visitante;
                tipoEquipoJugador = TipoEquipo.VISITANTE;
            }

            var cambiosRestantes = partido.SetActual.ObtenerSustitucionesRestantes(tipoEquipoJugador);
            var comandosDisponibles = new List<IComando>();
            
            // El comando para realizar una sustitución lo muestro al usuario solo si es que le quedan suficientes cambios en el set actual
            if (cambiosRestantes > 0) 
                comandosDisponibles.Add(new ComandoRealizarSustitucion(this, equipoJugador, tipoEquipoJugador));

            comandosDisponibles.Add(new ComandoVisualizarPlantilla(equipoJugador.FormacionPartido!));  
            comandosDisponibles.Add(new ComandoContinuarPartido());
            comandosDisponibles.Add(new ComandoSalir("Abandonar el partido")
            {
                MostrarMensajeSalida = false,
                AccionSalida = () => { this.partidoAbandonado = true; }
            });

            comando = AnsiConsole.Prompt(
                new SelectionPrompt<IComando>()
                    .Title("[orange1 bold]Seleccione una opción para continuar[/]")
                    .HighlightStyle("tan")
                    .AddChoices(comandosDisponibles)
                    .UseConverter(cmd => (cmd is ComandoRealizarSustitucion) ? $"{cmd.Titulo} [gray](Restantes: {cambiosRestantes})[/]"
                                                                             : cmd.Titulo)
            );

            // Ejecuto el comando controlando y mostrando por pantalla los posibles errores
            try
            {
                comando.Ejecutar();
            }
            catch (Exception ex)
            {
                VistasUtil.MostrarError($"{ex.Message}");
                if (!(ex is VoleyballManagerRuntimeException)) VistasUtil.MostrarDetallesExcepcion(ex);

                Console.ReadKey(true);
            }
        } 
        while (!(comando is ComandoContinuarPartido) && (!(comando is ComandoSalir) || !partidoAbandonado));
    }
    
    /// <summary>
    /// Incrementa el score al equipo que corresponda luego de finalizar un set. Si el partido
    /// fue abandonado, el resultado se queda como estaba previo al abandono.
    /// </summary>
    private void incrementarScore()
    {
        if (partidoAbandonado) return;

        // El ganador del set siempre es el último que hizo el punto, entonces sumo el score
        // a ese equipo si se determinó que hay un ganador del set
        if (posesionPelota == TipoEquipo.LOCAL)
            partido.ScoreLocal++;
        else    
            partido.ScoreVisitante++;
    }

    /// <summary>
    /// Realiza una sustitución en alguno de los equipos
    /// </summary>
    /// <param name="equipoRealizador">Equipo que realiza el cambio (Local o visitante)</param>
    /// <param name="entrante">Jugador que ingresa</param>
    /// <param name="saliente">Jugador que sale</param>
    public void RealizarSustitucion(TipoEquipo equipoRealizador, Jugador entrante, Jugador saliente)
    {
        var nuevaSustitucion = new Sustitucion(entrante, saliente);
        Formacion formacion;

        if (equipoRealizador == TipoEquipo.LOCAL)
        {
            partido.SetActual.SustitucionesLocal++;
            partido.SetActual.Sustituciones.SustitucionesLocal.Add(nuevaSustitucion);
            formacion = partido.Local.FormacionPartido!;
        }
        else
        {
            partido.SetActual.SustitucionesVisitantes++;
            partido.SetActual.Sustituciones.SustitucionesVisitante.Add(nuevaSustitucion);
            formacion = partido.Visitante.FormacionPartido!;
        }

        formacion.JugadoresCancha.Reemplazar(saliente, entrante);
        formacion.JugadoresSuplentes.Reemplazar(entrante, saliente);
    }
}