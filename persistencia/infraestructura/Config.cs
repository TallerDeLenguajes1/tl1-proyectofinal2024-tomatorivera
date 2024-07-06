namespace Persistencia.Infraestructura
{
    /// <summary>
    /// Clase encargada de portar datos relevantes para el contexto de
    /// la partida y el juego en general
    /// </summary>
    public static class Config
    {
        // Datos constantes
        public static string? DirectorioPersistencia { get; set; }
        public static string? DirectorioPartidas { get; set; }
        public static string? DirectorioPartidasPrefix { get; set; }
        public static string? DirectorioRecursos { get; set; }
        public static string? DirectorioImagenes { get; set; }

        public static string? NombreJsonPartida { get; set; }
        public static string? NombreJsonUsuario { get; set; }
        public static string? NombreImgLogo { get; set; }

        // Datos cargados durante la ejecución
        public static string? DirectorioPartidaActual { get; set; }
        public static string? NombreUsuarioActual { get; set; }

        /// <summary>
        /// Carga los datos constantes de la configuración
        /// </summary>
        public static void CargarConfiguracion()
        {
            // Podría traerse desde un archivo externo
            DirectorioPersistencia = "archivos";
            DirectorioPartidas = DirectorioPersistencia + @"\partidas";
            DirectorioPartidasPrefix = "partida-";
            DirectorioRecursos = "recursos";
            DirectorioImagenes = DirectorioRecursos + @"\img";

            NombreJsonPartida = "partida.json";
            NombreJsonUsuario = "usuario.json";
            NombreImgLogo = "logo.png";
        }
    }
}