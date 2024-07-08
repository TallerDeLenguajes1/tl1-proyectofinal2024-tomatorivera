namespace Logica.Handlers
{
    /// <summary>
    /// Clase encargada de manejar información de errores que
    /// se mostrarán solo en ciertas situaciones del programa
    /// </summary>
    public class ErroresIgnorablesHandler 
    {
        public Dictionary<string, Exception> Errores { get; set; }
        private static ErroresIgnorablesHandler? instancia;

        private ErroresIgnorablesHandler()
        {
            this.Errores = new Dictionary<string, Exception>();
        }

        /// <summary>
        /// Obtiene la instancia generada
        /// </summary>
        /// <returns>Instancia única de <c>ErroresIgnorablesHandler</c></returns>
        public static ErroresIgnorablesHandler ObtenerInstancia()
        {
            if (instancia == null)
                instancia = new ErroresIgnorablesHandler();

            return instancia;
        }

        /// <summary>
        /// Remueve los errores agregados en caso de que haya
        /// </summary>
        public void LimpiarErrores()
        {
            if (Errores.Any()) Errores.Clear();
        }
    }
}