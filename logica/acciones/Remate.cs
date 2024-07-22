using Logica.Modelo;

namespace Logica.Acciones
{
    /// <summary>
    /// Acción que representa un remate. Ocurre cuando un jugador
    /// realiza un remate desde cualquier zona de la cancha
    /// </summary>
    public class Remate : Accion
    {
        private CalidadAccion calidadColocacion;
        private double bonificacionRemate;

        public Remate(Rally rally, CalidadAccion calidadColocacion) : base(rally)
        {
            this.calidadColocacion = calidadColocacion;
            this.bonificacionRemate = calcularBonificacionRemate();
        }

        public override ResultadoAccion Realizar()
        {
            // Si el jugador no llega al remate, termina el rally
            if (!llegaAlRemate())
                return new ResultadoAccion($"{realizador.Nombre} no llegó al remate", realizador);
            // Si el jugador tira afuera el remate, termina el rally
            if (tiraAfuera())
                return new ResultadoAccion($"{realizador.Nombre} lanza el remate afuera de la cancha", realizador);

            // Determino la calidad del remate
            var calidadRemate = calcularCalidadRemate();

            string accionSubMensaje = string.Empty;
            switch (calidadRemate)
            {
                case CalidadAccion.EXCELENTE:
                    accionSubMensaje = $"¡{realizador.Nombre} REMATA CON POTENCIA!";
                    realizador.AumentarCansancio(0.5f, 0.7f);
                    break;
                case CalidadAccion.MEDIA:
                    accionSubMensaje = $"{realizador.Nombre} realiza un buen remate";
                    realizador.AumentarCansancio(0.1f, 0.4f);
                    break;
                case CalidadAccion.MALA:
                    accionSubMensaje = $"{realizador.Nombre} no logró conectar bien con la pelota en el aire pero logra rematar";
                    realizador.AumentarCansancio(0.03f, 0.06f);
                    break;
            }

            rally.Log(new ResultadoAccion(accionSubMensaje, realizador));

            // Según la zona en la que se encuentre el rematador, obtengo los posibles bloqueadores y escojo uno random de ellos
            var zonaRematador = rally.ObtenerEquipoPropio().DeterminarZonaJugador(realizador);
            var posiblesBloqueadores = obtenerPosiblesBloqueadores(zonaRematador);
            var bloqueadorApuntado = posiblesBloqueadores.ElementAt(rnd.Next(posiblesBloqueadores.Count()));

            // Si el jugador es bloqueado, continúo con una acción de bloqueo
            if (esBloqueado(bloqueadorApuntado))
            {
                rally.IntercambiarPosesionPelota();
                rally.JugadorActual = bloqueadorApuntado;
                return new ResultadoAccion(new Bloqueo(rally, calidadRemate), $"¡{bloqueadorApuntado.Nombre} LOGRA LLEGAR AL BLOQUEO!", bloqueadorApuntado);
            }

            // Si nadie llega a bloquear, el jugador pasa el bloqueo y puede ser recepcionado por la defensa rival.
            // Obtengo cualquier defensor y determino si logra recibir el remate o no
            var defensasRival = rally.ObtenerEquipoRival().ObtenerDefensas();
            var receptor = defensasRival.ElementAt(rnd.Next(defensasRival.Count()));

            rally.IntercambiarPosesionPelota();
            rally.JugadorActual = receptor;
            return esPuntoDirecto(receptor) ? new ResultadoAccion($"{receptor.Nombre} no logró recepcionar el remate de {realizador.Nombre}", receptor)
                                            : new ResultadoAccion(new Recepcion(rally), $"{receptor.Nombre} pudo recepcionar el remate de {realizador.Nombre}", receptor);
        }

        /// <summary>
        /// Determina si el jugador logra llegar al remate
        /// </summary>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool llegaAlRemate()
        {
            // La probabilidad del jugador de llegar depende de su habilidad de remate más o menos
            // un bonus de remate que depende de la calidad de la colocación, todo esto restado por
            // el cansancio del jugador en este momento
            var probabilidadLlegar = Math.Round((realizador.HabilidadRemate + bonificacionRemate - realizador.Cansancio) / 10, 3);
            var rndNum = Math.Clamp(Math.Round(rnd.NextDouble(), 3), 0.0f, 1.0f);
            
            //System.Console.WriteLine($"LLEGA REMATE: {rndNum} - {Math.Clamp(probabilidadLlegar, 0.0f, 1.0f)}");

            return rndNum <= Math.Clamp(probabilidadLlegar, 0.0f, 1.0f);
        }

        /// <summary>
        /// Calcula la bonificación para el remate según la calidad de la colocación
        /// </summary>
        /// <returns>Número <c>double</c> de aumento o disminución de habilidad para el rematador</returns>
        private double calcularBonificacionRemate()
        {
            this.bonificacionRemate = calidadColocacion switch
            {
                CalidadAccion.EXCELENTE => 2 + rnd.NextDouble(),
                CalidadAccion.MEDIA => 1 + rnd.NextDouble(),
                CalidadAccion.MALA => 0 - rnd.NextDouble(),
                _ => realizador.HabilidadRemate
            };

            return this.bonificacionRemate;
        }

        /// <summary>
        /// Calcula la calidad del remate
        /// </summary>
        /// <returns>Objeto <c>CalidadAccion</c></returns>
        private CalidadAccion calcularCalidadRemate()
        {
            // El nivel de remate depende de la habilidad del rematador, más o menos la bonificación
            // que recibío y todo esto restado por su cansancio
            var nivelRemate = realizador.HabilidadRemate + bonificacionRemate - realizador.Cansancio;

            // Si el nivel de remate es menor a tres, fue malo. Si es menor a 7, fue un remate normal
            // y si es mayor a 7 entonces fue excelente
            return (nivelRemate <= 3) ? CalidadAccion.MALA 
                                      : (nivelRemate <= 7) ? CalidadAccion.MEDIA
                                                           : CalidadAccion.EXCELENTE;
        }

        /// <summary>
        /// Obtiene los posibles bloqueadores para un remate según la zona desde
        /// la que se remate
        /// </summary>
        /// <param name="zonaRematador">Zona en la que se encuentra el rematador</param>
        /// <returns>Lista de <c>Jugador</c> que pueden bloquear al jugador</returns>
        private List<Jugador> obtenerPosiblesBloqueadores(int zonaRematador)
        {
            var equipoRival = rally.ObtenerEquipoRival();
            var posiblesBloqueadores = new List<Jugador>
            {
                equipoRival.ObtenerJugadorZona(3) // EL 3 SIEMPRE LLEGA AL BLOQUEO
            };
            
            // Si el rematador está en zona 1 o 2, puede ser bloqueado por los rivales en zona 3 y 4
            if (zonaRematador == 1 || zonaRematador == 2)
            {
                posiblesBloqueadores.Add(equipoRival.ObtenerJugadorZona(4));
            }

            // Si el rematador está en zona 6 o 3, puede ser bloqueado por los rivales en zona 2, 3 y 4
            else if (zonaRematador == 6 || zonaRematador == 3)
            {
                posiblesBloqueadores.Add(equipoRival.ObtenerJugadorZona(2));
                posiblesBloqueadores.Add(equipoRival.ObtenerJugadorZona(4));
            }

            // Si el rematador está en zona 5 o 4, puede ser bloqueado por los rivales en zona 3 y 2
            else
            {
                posiblesBloqueadores.Add(equipoRival.ObtenerJugadorZona(2));
            }

            return posiblesBloqueadores;
        }

        /// <summary>
        /// Determina si el rematador es bloqueado
        /// </summary>
        /// <param name="bloqueador">Datos del jugador que lo bloquea</param>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool esBloqueado(Jugador bloqueador)
        {
            // Se calcula la diferencia de habilidades, teniendo en cuenta sus habilidades de remate y bloqueo
            // sumando y restando bonus y cansancio de los jugadores involucrados
            var diferenciaHabilidades = realizador.HabilidadRemate + bonificacionRemate - realizador.Cansancio - (bloqueador.HabilidadBloqueo - bloqueador.Cansancio);
            // La probabilidad de pasar el bloqueo es, de base, del 50% más o menos la diferencia de habilidades
            var probabilidadPasarBloqueo = 0.5f + (diferenciaHabilidades / 20.0f);

            var rndNum =  Math.Round(rnd.NextDouble(), 2);
            var skill = Math.Clamp(probabilidadPasarBloqueo, 0.0f, 1.0f);

            //System.Console.WriteLine($"ES BLOQUEADO: {bloqueador.Nombre} - {diferenciaHabilidades} - {probabilidadPasarBloqueo} - {rndNum} - {skill}");

            return rndNum <= skill;
        }

        /// <summary>
        /// Determina si el remate termina en punto directo
        /// </summary>
        /// <param name="receptor">Datos del jugador que recibe el remate</param>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool esPuntoDirecto(Jugador receptor)
        {
            // Se calcula la diferencia de habilidades, teniendo en cuenta sus habilidades de remate y recepción
            // sumando y restando bonus y cansancio de los jugadores involucrados
            var diferenciaHabilidades = realizador.HabilidadRemate + bonificacionRemate - realizador.Cansancio - (receptor.HabilidadRecepcion - receptor.Cansancio);
            // La probabilidad de punto directo es, de base, del 50% más o menos la diferencia de habilidades
            var probabilidadPunto = 0.5f + (diferenciaHabilidades / 20.0f);
            
            var rndNum =  Math.Round(rnd.NextDouble(), 2);
            var skill = Math.Clamp(probabilidadPunto, 0.0f, 1.0f);

            //System.Console.WriteLine($"ES PTO. DIREC. DE REMATE: {receptor.Nombre} - {diferenciaHabilidades} - {probabilidadPunto} - {rndNum} - {skill}");

            return rndNum <= skill;
        }

        /// <summary>
        /// Determina si el remate va afuera de la cancha
        /// </summary>
        /// <returns><c>True</c> en caso afirmativo, <c>False</c> en caso contrario</returns>
        private bool tiraAfuera()
        {
            // La probabilidad de fallar depende del cansancio del jugador, más o menos la bonificación del remate
            var probabilidadFallo = Math.Clamp((realizador.Cansancio - bonificacionRemate) / 10, 0.0f, 1.0f);
            var rndNum = Math.Round(rnd.NextDouble(), 3);

            //System.Console.WriteLine($"TIRA AFUERA: {probabilidadFallo} - {rndNum}");

            return rndNum <= probabilidadFallo;
        }
    }


}