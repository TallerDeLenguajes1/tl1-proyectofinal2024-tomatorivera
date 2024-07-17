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
            while (!deseaSalir)
            {
                // Muestro el dashboard
                var vistaDashboard = new Dashboard(partidaActual);
                var controladorDashboard = new DashboardControlador(vistaDashboard);
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
        private Jugador? jugadorAccion;

        public SimuladorPartidoHandler(Partido partido)
        {
            this.partido = partido;

            // Valores iniciales por defecto
            this.setsRestantes = partido.SetMaximos;
            this.partido.SetActual = 1;

            // El equipo en posesión de la pelota siempre arranca siendo el local
            this.posesionPelota = TipoEquipo.LOCAL;
        }

        /// <summary>
        /// Método encargado de manejar la lógica de un partido
        /// </summary>
        public void IniciarPartido()
        {
            // Muestro un encabezado
            mostrarEncabezadoPartido(partido.Local.Nombre, partido.Visitante.Nombre, partido.TipoPartido);

            // Inicializo datos del partido
            partido.ResultadoSets.Add(partido.SetActual, new ResultadoSet());

            // Muestro las vistas del partido
            var panelPartidoControlador = new PanelPartidoControlador(new PanelPartido(), partido);
            panelPartidoControlador.MostrarVista();
            
            /*
            // El partido termina cuando ya se hayan jugado todos los sets o cuando se pueda
            // determinar un ganador según el puntaje de los equipos tras cada ronda
            while (setsRestantes == 0 || !hayGanadorPartido(partido.ScoreLocal, partido.ScoreVisitante))
            {
                empezarRally();

                // Si después de un rally hay un ganador del set, se realizan algunas acciones para pasar al siguiente
                if (hayGanadorSet(partido.ResultadoSets[partido.SetActual].PuntosLocal, partido.ResultadoSets[partido.SetActual].PuntosVisitante))
                {
                    setsRestantes--;
                    partido.SetActual++;

                    partido.ResultadoSets.Add(partido.SetActual, new ResultadoSet());
                }
            }
            */

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
        /// Se encarga de manejar la lógica de ejecución de un rally, es decir, de un intercambio
        /// entre equipos sin que la pelota caiga al suelo
        /// </summary>
        private void empezarRally()
        {
            // Obtengo el equipo que va a sacar
            var equipoEnPosesion = ObtenerEquipoEnPosesion();

            // Si por alguna razón su formación fuese nula, no puedo continuar
            if (equipoEnPosesion.FormacionPartido == null)
                throw new FormacionInvalidaException("No se pudo iniciar el rally porque la formación del equipo en posesión es nula", equipoEnPosesion);

            // Establezco el jugador que va a sacar
            this.jugadorAccion = equipoEnPosesion.FormacionPartido.JugadoresCancha.ElementAt(0);

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
        /// <param name="puntosLocal">Puntos del equipo local</param>
        /// <param name="puntosVisitante">Puntos del equipo visitante</param>
        /// <returns><c>True</c> si un equipo ya ha ganado el set, <c>False</c> en caso contrario</returns>
        private bool hayGanadorSet(int puntosLocal, int puntosVisitante)
        {
            return (puntosLocal >= puntosParaSet && puntosLocal - puntosVisitante >= 2) ||
                   (puntosVisitante >= puntosParaSet && puntosVisitante - puntosLocal >= 2);
        }

        /// <summary>
        /// Retorna la instancia del equipo que tenga la pelota en posesión, ya sea el local o el visitante
        /// </summary>
        /// <returns>Objeto <c>Equipo</c> que posee la pelota</returns>
        private Equipo ObtenerEquipoEnPosesion()
        {
            return (posesionPelota == TipoEquipo.LOCAL) ? partido.Local : partido.Visitante;
        }
    }
}