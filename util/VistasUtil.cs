using Spectre.Console;

namespace Gui.Util
{
    /// <summary>
    /// Esta clase se encarga de aportar funciones 
    /// aisladas útiles para las vistas
    /// </summary>
    public class VistasUtil
    {
        /// <summary>
        /// Calcula el padding necesario para centrar una linea
        /// </summary>
        /// <param name="linea">Linea a la cual centrar</param>
        /// <returns>Un entero con el padding necesario para que una línea se vea centrada</returns>
        private static int calcularPadding(string linea)
        {
            return ((Console.WindowWidth - linea.Length) / 2) + linea.Length;
        }

        /// <summary>
        /// Muestra una línea centrada en la consola
        /// </summary>
        /// <param name="linea">Linea a mostrar</param>
        public static void MostrarCentrado(string linea)
        {
            System.Console.WriteLine(linea.PadLeft(calcularPadding(linea)));
        }

        /// <summary>
        /// Muestra un arreglo de lineas centradas en la consola
        /// </summary>
        /// <param name="lineas">Lineas a mostrar</param>
        public static void MostrarCentrado(string[] lineas)
        {
            foreach(string linea in lineas)
            {
                MostrarCentrado(linea);
            }
        }

        /// <summary>
        /// Muestra una línea centrada sin dar un salto de linea posterior
        /// </summary>
        /// <param name="linea">Linea a mostrar</param>
        public static void MostrarCentradoSinSalto(string linea)
        {
            System.Console.Write(linea.PadLeft(calcularPadding(linea)));
        }

        /// <summary>
        /// Obtiene un arreglo de lineas a partir de un string literal de muchas lineas
        /// </summary>
        /// <param name="linea">Linea a separar</param>
        /// <returns>Arreglo con cada fila de la linea separada</returns>
        public static string[] ObtenerLineasSeparadas(string linea)
        {
            return linea.Split(new [] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Muestra un mensaje de error por pantalla con decoración ascii art
        /// </summary>
        /// <param name="mensaje">Mensaje de error a mostrar</param>
        public static void MostrarError(string mensaje)
        {
            System.Console.WriteLine("\n");
            
            string error = @"
>>======================================================<<
|| ______     ______     ______     ______     ______   ||
||/\  ___\   /\  == \   /\  == \   /\  __ \   /\  == \  ||
||\ \  __\   \ \  __<   \ \  __<   \ \ \/\ \  \ \  __<  ||
|| \ \_____\  \ \_\ \_\  \ \_\ \_\  \ \_____\  \ \_\ \_\||
||  \/_____/   \/_/ /_/   \/_/ /_/   \/_____/   \/_/ /_/||
||                                                      ||
>>======================================================<<
 
";

            AnsiConsole.Write(new Panel(
                Align.Center(
                    new Markup("[bold red]" + error + "[/]")
                )
            ).Border(BoxBorder.None));

            AnsiConsole.Write(new Panel(
                Align.Center(
                    new Markup("[red]× " + mensaje + " ×[/]")
                )
            ).Border(BoxBorder.None));

            System.Console.WriteLine("\n");
        }

        /// <summary>
        /// Pausa la vista durante <paramref name="segundos"/> para evitar algún
        /// barrido de consola rápido en algún momento preciso del código
        /// </summary>
        /// <param name="segundos">Cantidad de segundos a pausar la vista</param>
        public static void PausarVistas(int segundos)
        {
            try
            {
                // Pauso el hilo
                Thread.Sleep(segundos * 1000);

                // Limpio el buffer para evitar bugs con las teclas que haya presionado
                // el usuario durante el período de sleep
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            catch (Exception)
            {
                // Si por alguna razón la interrupción del thread lanza un error
                // solo sigue su ejecución sin pausar la vista
            }
        }

        /// <summary>
        /// Borra la consola a partir de la linea <paramref name="lineaInicio"/>
        /// </summary>
        /// <param name="lineaInicio">Linea desde la cual se borrará la consola</param>
        public static void BorrarDesdeLinea(int lineaInicio)
        {
            int lineaFin = Console.CursorTop;

            for (int i = lineaInicio ; i < lineaFin ; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, lineaInicio);
        }
    }
}