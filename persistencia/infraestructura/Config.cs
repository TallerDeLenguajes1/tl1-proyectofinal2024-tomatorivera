using Logica.Modelo;

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

        public static string? ApiSportsKey { get; set; }
        public static string? ApiSportsUrl { get; set; }
        public static string? ApiRandomUserUrl { get; set; }

        // Datos cargados durante la ejecución
        public static string? DirectorioPartidaActual { get; set; }
        public static string? NombreUsuarioActual { get; set; }
        public static string? NombreEquipoUsuario { get; set; }
        public static List<Team>? EquiposArgentinos { get; set; }

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
            
            ApiSportsKey = "03028e54f1f00215f6bb821f0b260a6d";
            ApiSportsUrl = "https://v1.volleyball.api-sports.io";
            ApiRandomUserUrl = "https://randomuser.me/api/";
        }
    }
}