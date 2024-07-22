using Gui.Controladores;
using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
using Logica.Comandos;
using Logica.Excepciones;
using Logica.Modelo;
using Persistencia.Infraestructura;
using Persistencia.Util;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Logica.Handlers
{
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
                    comandoEjecutar.ejecutar();
                }
                catch (Exception e)
                {
                    VistasUtil.MostrarError(e.Message);

                    Console.ForegroundColor = ConsoleColor.Red;
                    VistasUtil.MostrarCentrado("-*- Presione una tecla para volver al dashboard -*-");
                    Console.ResetColor();

                    Console.ReadKey();
                }

                AnsiConsole.Clear();
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

            var seleccion = AnsiConsole.Prompt(
                                new SelectionPrompt<IComando>()
                                    .Title("")
                                    .HighlightStyle("yellow")
                                    .AddChoices(new List<IComando>() {
                                        new ComandoJugarAmistoso(),
                                        new ComandoConsultarPlantilla(),
                                        new ComandoConsultarHistorial(),
                                        new ComandoSalir(TipoMenu.SECUNDARIO) { AccionSalida = () => this.deseaSalir = true }
                                    })
                                    .UseConverter(comando => comando.titulo)
                                );

            return seleccion;
        }
    }

    /// <summary>
    /// Clase encargada de manejar la lógica de simulación de partidos
    /// </summary>
    public class SimuladorPartidoHandler
    {
        private const int puntosParaSet = 5;

        private Partido partido;
        private int setsRestantes;
        private TipoEquipo posesionPelota;
        private Equipo equipoEnSaque;

        public SimuladorPartidoHandler(Partido partido)
        {
            this.partido = partido;

            // Valores iniciales por defecto
            this.partido.SetActual = 1;
            setsRestantes = partido.SetMaximos;

            // El equipo en posesión de la pelota siempre arranca siendo el local
            posesionPelota = TipoEquipo.LOCAL;
            equipoEnSaque = partido.Local;
        }

        /// <summary>
        /// Método encargado de inicializar la simulación de un partido, sus vistas y objetos necesarios
        /// </summary>
        public void IniciarSimulacionPartido()
        {
            // Muestro un encabezado
            mostrarEncabezadoPartido(partido.Local.Nombre, partido.Visitante.Nombre, partido.TipoPartido);

            // Genero los datos del set actual
            partido.ResultadoSets.Add(partido.SetActual, new ResultadoSet());

            // Determino qué equipo hará el saque (probabilidad de 50/50)
            equipoEnSaque = determinarSaque();

            // Muestro las vistas del partido
            var panelPartidoControlador = new PanelPartidoControlador(new PanelPartido(), partido);
            var layoutInformacion = panelPartidoControlador.ObtenerLayoutInformacion();
            panelPartidoControlador.MostrarVista();
            
            // Actualizo el panel de información con el componente Live para ir mostrando las acciones en una misma vista
            AnsiConsole.Live(layoutInformacion)
            .Start(contextoDisplay => {

                jugarPartido(contextoDisplay, panelPartidoControlador);

            });

            // Temporal: para que no finalice je
            Console.ReadKey();
        }

        /// <summary>
        /// Muestra una animación para el encabezado del partido
        /// </summary>
        /// <param name="nombreLocal">Nombre del equipo local</param>
        /// <param name="nombreVisitante">Nombre del equipo visitante</param>
        /// <param name="tipoPartido">Tipo de partido a disputarse</param>
        private void mostrarEncabezadoPartido(string nombreLocal, string nombreVisitante, TipoPartido tipoPartido)
        {
            AnsiConsole.Clear(); 

            var layout = new Layout("raiz")
                .SplitRows(new Layout("arriba"));

            // Los nombres del local y el visitante se mostrarán como texto figlet con una fuente de ascii personalizada
            // En caso de no poder cargar dicha fuente, se mostrarán con la fuente figlet por defecto.
            FigletText figletLocal, figletVisitante;
            try
            {
                RecursosUtil.VerificarArchivo(Config.DirectorioFuentes + @"\" + Config.NombreFuentePagga);

                var figletFont = FigletFont.Load(Config.DirectorioFuentes + @"\" + Config.NombreFuentePagga);
                figletLocal = new FigletText(figletFont, nombreLocal).Color(Color.Red1);
                figletVisitante = new FigletText(figletFont, nombreVisitante).Color(Color.Red1);
            }
            catch (Exception)
            {
                figletLocal = new FigletText(nombreLocal).Color(Color.Red);
                figletVisitante = new FigletText(nombreVisitante).Color(Color.Red);
            }

            // Textos a mostrarse por pantalla
            var textos = new IRenderable[] {
                new Markup("Se va a disputar..."),
                figletLocal,
                new Rule("[orange1 bold]VS[/]") { Style = Style.Parse("bold gray"), Justification = Justify.Center },
                figletVisitante,
                new Markup($"En un partido [orange1]{tipoPartido}[/]")
            };

            // Muestro el layout con el componente Live para que se vaya actualizando de a poco
            AnsiConsole.Live(layout)
                .Start(controladorDisplay => {
                    // En esta lista almaceno los objetos que voy mostrando por pantalla para dar el efecto
                    // de que van apareciendo uno por uno
                    var textosMostrados = new List<IRenderable>();

                    foreach (var linea in textos)
                    {
                        textosMostrados.Add(linea);
                        layout["arriba"].Update(
                            new Panel(Align.Center(
                                new Rows(textosMostrados),
                                VerticalAlignment.Middle
                            ))
                            .Expand()
                            .Border(BoxBorder.None)
                        );
                        
                        textosMostrados.Add(new Text(""));
                        controladorDisplay.Refresh();
                        VistasUtil.PausarVistas(1);
                    }

                    VistasUtil.PausarVistas(3);
                });
        }

        /// <summary>
        /// Método encargado de ejecutar la lógica de un partido
        /// </summary>
        private void jugarPartido(LiveDisplayContext ctx, PanelPartidoControlador controladorPanel)
        {
            // Verifico si por alguna razón las formaciones de los equipos en juego son nulas
            if (partido.Local.FormacionPartido == null)
                throw new FormacionInvalidaException($"La formación del equipo {partido.Local.Nombre} es nula", partido.Local);
            if (partido.Visitante.FormacionPartido == null)
                throw new FormacionInvalidaException($"La formación del equipo {partido.Visitante.Nombre} es nula", partido.Visitante);
            if (equipoEnSaque.FormacionPartido == null)
                throw new FormacionInvalidaException($"La formación del equipo {equipoEnSaque.Nombre} es nula", equipoEnSaque);

            // Inicializo la información del rally
            var rally = new Rally(partido.Local.FormacionPartido, partido.Visitante.FormacionPartido, posesionPelota, equipoEnSaque.FormacionPartido.ObtenerJugadorZona(1));

            // El partido termina cuando ya se hayan jugado todos los sets o cuando se pueda
            // determinar un ganador según el puntaje de los equipos tras cada ronda
            while (setsRestantes != 0 || !hayGanadorPartido(partido.ScoreLocal, partido.ScoreVisitante))
            {
                // Comienzo el rally
                rally.ComenzarRally();
                determinarResultadoRally(rally);

                // Muestro las acciones del rally
                controladorPanel.MostrarAcciones(ctx, rally.AccionesRally);
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

                    // Actualizo los sets
                    setsRestantes--;
                    partido.SetActual++;

                    // Agrego el siguiente set a los resultados
                    partido.ResultadoSets.Add(partido.SetActual, new ResultadoSet());
                }

                // Actualizo la información de las vistas
                controladorPanel.MostrarVista();

                // Temporal, para que no se borren las acciones
                Console.ReadKey(true);
            }
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
        /// Determina si un equipo ya sea local o visitante ha ganado el set
        /// </summary>
        /// <returns><c>True</c> si un equipo ya ha ganado el set, <c>False</c> en caso contrario</returns>
        private bool hayGanadorSet()
        {
            var puntosLocal = partido.ResultadoSets[partido.SetActual].PuntosLocal;
            var puntosVisitante = partido.ResultadoSets[partido.SetActual].PuntosVisitante;

            return (puntosLocal >= puntosParaSet && puntosLocal - puntosVisitante >= 2) ||
                   (puntosVisitante >= puntosParaSet && puntosVisitante - puntosLocal >= 2);
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
                    if (equipoEnSaque.Nombre.Equals(partido.Local.Nombre))
                    {
                        //System.Console.WriteLine("Recupera el saque y rota el equipo VISITANTE");
                        partido.Visitante.FormacionPartido!.JugadoresCancha.Rotar();
                        equipoEnSaque = partido.Visitante;
                    }

                    posesionPelota = TipoEquipo.VISITANTE;
                    //System.Console.WriteLine("Punto para VISITANTE");
                    break;

                // Si la última posesión de la pelota es del equipo visitante, quiere decir que fue punto del local
                case TipoEquipo.VISITANTE:
                    if (equipoEnSaque.Nombre.Equals(partido.Visitante.Nombre))
                    {
                        //System.Console.WriteLine("Recupera el saque y rota el equipo LOCAL");
                        partido.Local.FormacionPartido!.JugadoresCancha.Rotar();
                        equipoEnSaque = partido.Local;
                    }
                    posesionPelota = TipoEquipo.LOCAL;
                    //System.Console.WriteLine("Punto para LOCAL");
                    break;
            }

            // Actualizo la información del rally y del partido
            partido.ResultadoSets[partido.SetActual].IncrementarPuntos(posesionPelota);
            rallyActual.PosesionPelota = posesionPelota;
            rallyActual.JugadorActual = equipoEnSaque.FormacionPartido!.ObtenerJugadorZona(1);
        }

        /// <summary>
        /// Determina qué equipo comenzará el saque según una probabilidad 50/50
        /// </summary>
        /// <returns>Equipo que comenzará sacando</returns>
        private Equipo determinarSaque()
        {
            return (new Random().NextDouble() >= 0.5) ? partido.Local : partido.Visitante;
        }
    }
}