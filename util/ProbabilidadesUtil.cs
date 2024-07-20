namespace Logica.Util
{
    /// <summary>
    /// Clase de utilidad para ciertos cálculos referidos a las probabilidades
    /// </summary>
    public static class ProbabilidadesUtil
    {
        private readonly static Random rnd = new Random();

        /// <summary>
        /// Genera un número real aleatorio entre <paramref name="min"/> y <paramref name="max"/>
        /// redondeandolo a <paramref name="cifrasDecimales"/> cifras decimales
        /// </summary>
        /// <param name="min">Número mínimo a generar</param>
        /// <param name="max">Número máximo a generar</param>
        /// <param name="cifrasDecimales">Cifras decimales a redondear (por defecto 2)</param>
        /// <returns>Número <c>float</c> generado</returns>
        public static float ValorAleatorioEntre(float min, float max, int cifrasDecimales = 2)
        {
            return (float) Math.Round(rnd.NextDouble() * (max - min) + min, cifrasDecimales);
        }
    }
}