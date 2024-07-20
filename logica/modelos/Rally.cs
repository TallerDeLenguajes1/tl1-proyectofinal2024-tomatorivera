using Gui.Util;
using Logica.Acciones;

namespace Logica.Modelo
{
    public class Rally
    {
        public Formacion FormacionLocal { get; set; }
        public Formacion FormacionVisitante { get; set;}
        public TipoEquipo PosesionPelota { get; set; }
        public Jugador JugadorActual { get; set; }

        public Rally(Formacion formacionLocal, Formacion formacionVisitante, TipoEquipo posesionPelota, Jugador jugadorActual)
        {
            this.FormacionLocal = formacionLocal;
            this.FormacionVisitante = formacionVisitante;
            this.PosesionPelota = posesionPelota;
            this.JugadorActual = jugadorActual;
        }

        /// <summary>
        /// Intercambia la posesión de la pelota según donde se encuentre, si está en el campo local
        /// se cambia al visitante y viceversa
        /// </summary>
        public void IntercambiarPosesionPelota()
        {
            this.PosesionPelota = (PosesionPelota == TipoEquipo.LOCAL) ? TipoEquipo.VISITANTE : TipoEquipo.LOCAL;
        }

        /// <summary>
        /// Obtiene los jugadores del equipo en posesión de la pelota
        /// </summary>
        /// <returns>Objeto <c>Formacion</c></returns>
        public Formacion ObtenerEquipoPropio()
        {
            return (this.PosesionPelota == TipoEquipo.LOCAL) ? FormacionLocal : FormacionVisitante;
        }

        /// <summary>
        /// Obtiene los jugadores del equipo que NO se encuentra en posesión de la pelota
        /// </summary>
        /// <returns>Objeto <c>Formacion</c></returns>
        public Formacion ObtenerEquipoRival()
        {
            return (this.PosesionPelota == TipoEquipo.LOCAL) ? FormacionVisitante : FormacionLocal;
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
                // realizo la acción
                resultado = accion.Realizar();

                // FALTA IMPLEMENTAR UN LOGGER A LA VISTA DE LA PARTIDA
                // muestro el mensaje que produzca la acción
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(resultado.MensajeAccion);
                Console.ResetColor();
                
                // Delay de 1 segundo entre las acciones
                VistasUtil.PausarVistas(1.5f);

                // Si el resultado tiene una acción siguiente, la ejecuto, caso contrario el rally terminó
                if (resultado.AccionSiguiente != null) accion = resultado.AccionSiguiente;

            } while (resultado.AccionSiguiente != null);
        }
    }
}