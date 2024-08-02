using Logica.Modelo;

namespace Logica.Acciones
{
    /// <summary>
    /// Acción de recepción. Ocurre cuando un jugador recibe una pelota desde 
    /// cualquier jugada de ataque, ya sea un saque, un remate, u otro.
    /// </summary>
    public class Recepcion : Accion
    {
        public Recepcion(Rally rally) : base(rally)
        {
            IncrementarCansancio(0.3f, 0.5f);
        }

        public override ResultadoAccion Realizar()
        {
            // Obtengo los jugadores que se encuentran en posición de ataque para recibir un pase desde la zona de defensa
            var atacantes = rally.ObtenerEquipoPropio().ObtenerAtacantes().Where(j => j.NumeroCamiseta != realizador.NumeroCamiseta).ToList();

            Jugador colocador;
            string mensajeAccion;
            CalidadAccion calidadRecepcion;

            // Si el receptor logra hacer un pase al colocador, recibirá el jugador en la ofensiva
            // que tenga mayor habilidad de recepción
            if (paseAlColocador())
            {
                colocador = atacantes.OrderByDescending(j => j.HabilidadColocacion).First();
                mensajeAccion = $"¡La recepción de {realizador.Nombre} fue buena! El jugador {colocador.Nombre} va a colocar";
                calidadRecepcion = CalidadAccion.EXCELENTE;
            }
            else
            {
                colocador = atacantes[rnd.Next(atacantes.Count())];
                mensajeAccion = $"{realizador.Nombre} no pudo recepcionar muy bien, la línea de ataque puede salvarla";
                calidadRecepcion = CalidadAccion.MEDIA;
            }

            rally.JugadorActual = colocador;
            return new ResultadoAccion(new Colocacion(rally, calidadRecepcion), mensajeAccion, realizador);
        }

        /// <summary>
        /// Determina si el pase del receptor va o no al mejor colocador de la línea ofensiva
        /// </summary>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool paseAlColocador()
        {
            // Las probabilidades de un buen pase depende de las habilidades de recepcion
            // del jugador que realiza esta acción menos el cansancio de dicho jugador
            var probabilidadBuenPase = Math.Clamp((realizador.HabilidadRecepcion - realizador.Cansancio) / 10, 0.0f, 1.0f);
            var rndNum = Math.Clamp(Math.Round(rnd.NextDouble(), 3), 0.0f, 1.0f);

            //System.Console.WriteLine($"RECEP: {rndNum} - {probabilidadBuenPase} ({realizador.Cansancio})");

            return rndNum <= probabilidadBuenPase;
        }
    }


}