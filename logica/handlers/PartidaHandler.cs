using Gui.Controladores;
using Gui.Modelo;
using Gui.Util;
using Gui.Vistas;
using Logica.Comandos;
using Logica.Modelo;
using Spectre.Console;

namespace Logica.Handlers
{
    public class PartidaHandler
    {
        private Partida partidaActual;
        private bool deseaSalir;

        public PartidaHandler(Partida partidaActual)
        {
            this.partidaActual = partidaActual;
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
        /// <returns></returns>
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
                                        new ComandoJugarAmistoso(this),
                                        new ComandoConsultarPlantilla(),
                                        new ComandoConsultarHistorial(),
                                        new ComandoSalir(TipoMenu.SECUNDARIO) { AccionPersonalizada = () => this.deseaSalir = true }
                                    })
                                    .UseConverter(comando => comando.titulo)
                                );

            return seleccion;
        }

        public void JugarPartido(Partido partido)
        {
            AnsiConsole.Clear(); 
            System.Console.WriteLine("**** PARTIDO ****");
            System.Console.WriteLine(partido.Local);
            System.Console.WriteLine(partido.Visitante);
            System.Console.WriteLine(partido.TipoPartido);
            Console.ReadKey();
        }
    }
}