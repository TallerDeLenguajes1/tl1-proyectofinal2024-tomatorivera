using Logica.Modelo;

namespace Logica.Acciones
{
    public class Bloqueo : Accion
    {
        private CalidadAccion calidadRemate;

        public Bloqueo(Rally rally, CalidadAccion calidadRemate) : base(rally)
        {
            this.calidadRemate = calidadRemate;
            IncrementarCansancio(0.4f, 0.7f);
        }

        public override ResultadoAccion Realizar()
        {
            // Si el bloqueo va hacia afuera de la cancha, termina el rally
            if (tiraAfuera())
                return new ResultadoAccion($"El bloqueo de {realizador.Nombre} no fue muy bueno y terminó afuera de la cancha", realizador);

            // Según la zona en la que se encuentre el bloqueador se determina qué jugadores
            // del equipo rival pueden llegar a recepcionar el bloqueo
            var zonaBloqueador = rally.ObtenerEquipoPropio().DeterminarZonaJugador(realizador);
            var receptor = determinarReceptor(zonaBloqueador, rally.ObtenerEquipoRival());

            // Si el bloqueo es efectivo y cae en la cancha rival, es punto y termina el rally
            if (hacePunto(receptor))
            {
                rally.IntercambiarPosesionPelota();
                return new ResultadoAccion($"¡BLOQUEO PERFECTO Y PUNTO DE {realizador.Nombre}!", realizador);
            }

            // Si el bloqueador llega a tocarla, la pelota puede ser salvada por un defensa de su mismo equipo
            else if (pasaBloqueo())
            {
                receptor = rally.ObtenerEquipoPropio().ObtenerDefensas().OrderByDescending(j => j.HabilidadRecepcion).First();
                rally.JugadorActual = receptor;
                return new ResultadoAccion(new Recepcion(rally), $"{realizador.Nombre} realiza un buen toque en el bloqueo para que {receptor.Nombre} pueda recepionar", receptor);
            }

            // Si el bloqueador llega a taparla pero el equipo anterior llega a salvarla, continúa el juego
            else
            {
                rally.IntercambiarPosesionPelota();
                rally.JugadorActual = receptor;
                return new ResultadoAccion(new Recepcion(rally), $"¡{realizador.Nombre} BLOQUEA Y {receptor.Nombre} LA SALVA!", receptor);
            }
        }

        /// <summary>
        /// Determina si el bloqueo va hacia afuera
        /// </summary>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool tiraAfuera()
        {
            // La probabilidad de que vaya afuera es, de base, del 25% más el cansancio del jugador pero restado
            // por la habilidad de bloqueo del mismo
            var probabilidadAfuera = Math.Round(Math.Clamp((2.5f + realizador.Cansancio - realizador.HabilidadBloqueo) / 10, 0.0f, 1.0f), 3);
            var rndNum = Math.Round(rnd.NextDouble(), 3);

            //System.Console.WriteLine($"TIRA AFUERA: {rndNum} - {probabilidadAfuera}");

            return rndNum <= probabilidadAfuera;
        }

        /// <summary>
        /// Determina si el bloqueo del jugador termina en punto directo
        /// </summary>
        /// <param name="receptor">Datos del jugador que puede llegar a recepcionar del equipo rival</param>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool hacePunto(Jugador receptor)
        {
            // Se calcula la diferencia de habilidades según la capacidad de bloqueo y recepción de cada jugador, restado por
            // sus niveles de cansancio. El bloqueador recibe una bonificación según el remate haya sido bueno o malo
            var diferenciaHabilidades = realizador.HabilidadBloqueo + calcularBonificacionBloqueo() - realizador.Cansancio - (receptor.HabilidadRecepcion - receptor.Cansancio);
            // Las probabilidades de hacer un punto son, de base, 50% más o menos la diferencia de habilidades de los jugadores
            var probabilidadPunto = 0.5f + (diferenciaHabilidades / 20.0f);
            
            var rndNum =  Math.Round(rnd.NextDouble(), 2);
            var skill = Math.Clamp(probabilidadPunto, 0.0f, 1.0f);

            //System.Console.WriteLine($"HACE PUNTO: {receptor.Nombre} - {diferenciaHabilidades} - {probabilidadPunto} - {rndNum} - {skill}");

            return rndNum <= skill;
        }
        
        /// <summary>
        /// Determina si el remate pasa el bloqueo
        /// </summary>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool pasaBloqueo()
        {
            // Las probabilidades de que pase su bloqueo son, de base, 25% más el cansancio del jugador
            var probabilidadPase = Math.Clamp(Math.Round(0.25f + (realizador.Cansancio / 10), 3), 0.0f, 1.0f);
            var rndNum = Math.Round(rnd.NextDouble(), 3);

            //System.Console.WriteLine($"PASA BLOQUEO: {probabilidadPase} - {rndNum}");

            return rndNum <= probabilidadPase;
        }

        /// <summary>
        /// Calcula la bonificación del bloqueo según la calidad del remate anterior a este
        /// </summary>
        /// <returns>Número <c>double</c> de aumento o disminución de habilidad para el bloqueador</returns>
        private double calcularBonificacionBloqueo()
        {
            return calidadRemate switch
            {
                CalidadAccion.EXCELENTE => 0 - 1.0f - rnd.NextDouble(),
                CalidadAccion.MEDIA => 0.5f - rnd.NextDouble(),
                CalidadAccion.MALA => 1 + rnd.NextDouble(),
                _ => 0
            };
        }

        /// <summary>
        /// Determina el posible receptor del bloqueo
        /// </summary>
        /// <param name="zonaBloqueador">Zona del bloqueador</param>
        /// <param name="equipoReceptor">Equipo que puede recibir el bloqueo</param>
        /// <returns>Objeto <c>Jugador</c></returns>
        private Jugador determinarReceptor(int zonaBloqueador, Formacion equipoReceptor)
        {
            (int min, int max) posiblesZonasRecepcion;

            switch (zonaBloqueador)
            {
                // Si el bloqueo es en zona 2, lo pueden salvar los jugadores de zona 3, 4 y 5
                case 2:
                    posiblesZonasRecepcion = (3, 5);
                    break;
                // Si el bloqueo es en zona 3, lo pueden salvar los jugadores de zonas 2, 3, 4 y 6
                case 3:
                    posiblesZonasRecepcion = (2, 5);
                    break;
                // Si el bloqueo es en zona 4, lo pueden salvar los jugadores de zona 1, 2 y 3
                case 4:
                    posiblesZonasRecepcion = (1, 3);
                    break;
                // Si por alguna razón el bloqueo viene de otra zona, lo puede salvar cualquiera
                default:
                    posiblesZonasRecepcion = (1, 6);
                    break;
            }

            var indexReceptor = rnd.Next(posiblesZonasRecepcion.min, posiblesZonasRecepcion.max + 1);

            // Hago la corrección del index si el bloqueo es en zona 3 y salva zona 5
            indexReceptor = (zonaBloqueador == 3 && indexReceptor == 5) ? 6 : indexReceptor;

            return equipoReceptor.JugadoresCancha.ElementAt(indexReceptor - 1);
        }
    
    }
}