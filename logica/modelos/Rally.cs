using Gui.Util;
using Logica.Acciones;
using Spectre.Console;

namespace Logica.Modelo
{
    public class Rally
    {
        private string colorAccionesLocal = Color.Yellow.ToMarkup();
        private string colorAccionesVisitante = Color.Red.ToMarkup();

        public Formacion FormacionLocal { get; set; }
        public Formacion FormacionVisitante { get; set;}
        public TipoEquipo PosesionPelota { get; set; }
        public Jugador JugadorActual { get; set; }
        public List<string> AccionesRally { get; set; }

        public Rally(Formacion FormacionLocal, Formacion FormacionVisitante, TipoEquipo PosesionPelota, Jugador JugadorActual)
        {
            this.FormacionLocal = FormacionLocal;
            this.FormacionVisitante = FormacionVisitante;
            this.PosesionPelota = PosesionPelota;
            this.JugadorActual = JugadorActual;

            AccionesRally = new List<string>();
        }

        /// <summary>
        /// Intercambia la posesión de la pelota según donde se encuentre, si está en el campo local
        /// se cambia al visitante y viceversa
        /// </summary>
        public void IntercambiarPosesionPelota()
        {
            PosesionPelota = (PosesionPelota == TipoEquipo.LOCAL) ? TipoEquipo.VISITANTE : TipoEquipo.LOCAL;
        }

        /// <summary>
        /// Obtiene los jugadores del equipo en posesión de la pelota
        /// </summary>
        /// <returns>Objeto <c>Formacion</c></returns>
        public Formacion ObtenerEquipoPropio()
        {
            return (PosesionPelota == TipoEquipo.LOCAL) ? FormacionLocal : FormacionVisitante;
        }

        /// <summary>
        /// Obtiene los jugadores del equipo que NO se encuentra en posesión de la pelota
        /// </summary>
        /// <returns>Objeto <c>Formacion</c></returns>
        public Formacion ObtenerEquipoRival()
        {
            return (PosesionPelota == TipoEquipo.LOCAL) ? FormacionVisitante : FormacionLocal;
        }

        /// <summary>
        /// Gestiona la lógica de un rally, es decir, desde un saque hasta que alguno
        /// de los equipos haga un punto
        /// </summary>
        public void ComenzarRally()
        {
            // La primera acción del partido siempre es un saque
            Accion accion = new Saque(this);
            ResultadoAccion resultado;

            do
            {
                // Realizo la acción
                resultado = accion.Realizar();

                // Almaceno el mensaje que produzca la acción
                Log(resultado);

                // Si el resultado tiene una acción siguiente, la ejecuto, caso contrario el rally terminó
                if (resultado.AccionSiguiente != null) accion = resultado.AccionSiguiente;

            } while (resultado.AccionSiguiente != null);
        }

        /// <summary>
        /// Realiza el log de una acción del rally
        /// </summary>
        /// <param name="mensajeAccion">Mensaje a almacenar</param>
        public void Log(ResultadoAccion resultadoAccion)
        {
            var mensajeAccion = resultadoAccion.MensajeAccion;
            var colorAccion = FormacionLocal.JugadoresCancha.Contains(resultadoAccion.Realizador) ? colorAccionesLocal : colorAccionesVisitante;
            AccionesRally.Add($"[white]►[/] [{colorAccion}]{mensajeAccion}[/]");
        }
    }
}