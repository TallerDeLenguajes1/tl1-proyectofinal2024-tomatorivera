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
>>======================================================<<   
 
";
            MostrarCentrado(ObtenerLineasSeparadas(error));
            MostrarCentrado("× " + mensaje + " ×");

            System.Console.WriteLine("\n");
        }
    }
}