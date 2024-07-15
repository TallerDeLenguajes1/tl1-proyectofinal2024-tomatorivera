using Gui.Controladores;
using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
using Logica.Comandos;
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
        private bool partidoTerminado;

        public SimuladorPartidoHandler()
        {
            this.partidoTerminado = false;
        }

        /// <summary>
        /// Método encargado de manejar la lógica de un partido
        /// </summary>
        /// <param name="partido">Datos del partido a jugarse</param>
        public void IniciarPartido(Partido partido)
        {
            AnsiConsole.Clear(); 
            
            // Muestro un encabezado
            mostrarEncabezadoPartido(partido.Local.Nombre, partido.Visitante.Nombre, partido.TipoPartido);
            
            // El partido termina cuando alguno de los equipos llegue al puntaje necesario para ganar (establecido en la clase Partido)
            // con diferencia de dos puntos como establecen las reglas del volley
            while (!partidoTerminado)
            {

            }

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
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(3000);
                });

            // Limpio la consola
            AnsiConsole.Clear();
        }

    }
}