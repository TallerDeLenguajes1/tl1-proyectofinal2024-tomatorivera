using Logica.Modelo;

namespace Logica.Acciones
{
    /// <summary>
    /// Acción de Saque. Ocurre cuando un jugador realiza un saque
    /// </summary>
    public class Saque : Accion
    {
        public Saque(Rally rally) : base(rally)
        {
            IncrementarCansancio(0.1f, 0.3f);
        }

        public override ResultadoAccion Realizar()
        {
            rally.Log(new ResultadoAccion($"{realizador.Nombre} va a realizar el saque", realizador));

            // Si el saque no pasa la red, el rally termina
            if (!pasaRed())
                return new ResultadoAccion($"El saque de {realizador.Nombre} no pasó la red", realizador);

            // Hay un 70% de chances de que el saque caiga en la zona de defensa, según donde caiga, se obtiene
            // un receptor aleatorio del equipo rival que se encuentre en dicha zona
            var caidaPelota = rnd.Next(1, 11);
            var receptor = (caidaPelota <= 7) ? rally.ObtenerEquipoRival().ObtenerDefensas().ElementAt(rnd.Next(3))
                                              : rally.ObtenerEquipoRival().ObtenerAtacantes().ElementAt(rnd.Next(3));

            // La pelota pasa al campo contrario y el cansancio del jugador aumenta
            rally.IntercambiarPosesionPelota();

            // Si es un punto directo, el rally termina
            if (esPuntoDirecto(receptor))
                return new ResultadoAccion($"¡{realizador.Nombre} hizo un punto directo!", realizador);

            // En caso de que no sea punto directo, quiere decir que un defensa del equipo rival recepcionó el balón
            rally.JugadorActual = receptor;
            return new ResultadoAccion(new Recepcion(rally), $"{receptor.Nombre} logra recepcionar el saque de {realizador.Nombre}", receptor);
        }

        /// <summary>
        /// Determina si el saque del jugador pasa la red
        /// </summary>
        /// <returns><c>True</c> si pasa la red, <c>False</c> en caso contrario</returns>
        private bool pasaRed()
        {
            // La probabilidad de fallar un saque es, de base, del 10% más lo cansado que esté el jugador
            var probabilidadFallo = Math.Round(Math.Clamp(0.1f + realizador.Cansancio / 10, 0.0f, 1.0f), 3);
            var rndNum = Math.Round(rnd.NextDouble(), 3);

            //System.Console.WriteLine($"PASA RED: {rndNum} - {probabilidadFallo} ({realizador.Cansancio})");

            return rndNum > probabilidadFallo;
        }

        /// <summary>
        /// Determina si el servicio del jugador que realiza el saque es punto directo
        /// </summary>
        /// <param name="receptor">Jugador que recibe el saque</param>
        /// <returns><c>True</c> si es punto directo, <c>False</c> en caso contrario</returns>
        private bool esPuntoDirecto(Jugador receptor)
        {
            // Si la pelota cae en la línea de ataque del equipo rival, es probable que vaya con poca potencia
            // en ese caso, los jugadores que se encuentren allí tienen un porcentaje bonus para poder recibirla
            // de entre 15% y 25%
            var zonaReceptor = rally.ObtenerEquipoPropio().DeterminarZonaJugador(receptor); // Obtengo el equipo PROPIO porque la pelota ya está en campo del receptor
            var bonusBajaPotencia = 0.0d;
            if (zonaReceptor >= 2 && zonaReceptor <= 4)
            {
                bonusBajaPotencia = 1.5f + rnd.NextDouble();
            }

            // Calculo la diferencia de habilidades
            var diferenciaHabilidades = realizador.HabilidadSaque - (receptor.HabilidadRecepcion + bonusBajaPotencia);
            // La probabilidad de un punto directo es, de base, 30% más o menos la diferencia entre las habilidades
            // de saque del realizador, y de recepción del jugador que reciba el saque
            var probabilidadPunto = 0.3f + (diferenciaHabilidades / 20.0f);

            var rndNum =  Math.Round(rnd.NextDouble(), 2);
            var skill = Math.Clamp(probabilidadPunto, 0.0f, 1.0f);

            //System.Console.WriteLine($"ES PTO. DIREC.: {receptor.Nombre} ({receptor.Cansancio}) - {diferenciaHabilidades} - {probabilidadPunto} - {rndNum} - {skill}");

            return rndNum <= skill;
        }
    }
}