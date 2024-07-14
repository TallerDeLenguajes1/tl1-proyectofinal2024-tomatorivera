using Gui.Controladores;
using Gui.Modelo;
using Gui.Vistas;
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
                System.Console.WriteLine();

                // Solicito un comando a ejecutar al usuario desde un menú de opciones
                var comandoEjecutar = mostrarMenu();

                // Si el usuario selecciona la opción de salir, rompo el bucle
                if (comandoEjecutar is ComandoSalir) 
                {
                    deseaSalir = true;
                    AnsiConsole.Clear();
                }
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

            System.Console.WriteLine();

            var seleccion = AnsiConsole.Prompt(
                                new SelectionPrompt<IComando>()
                                    .HighlightStyle("orange3")
                                    .AddChoices(new List<IComando>() {
                                        new ComandoJugarAmistoso(),
                                        new ComandoConsultarPlantilla(),
                                        new ComandoConsultarHistorial(),
                                        new ComandoSalir(TipoMenu.SECUNDARIO)
                                    })
                                );

            return seleccion;
        }
    }
}