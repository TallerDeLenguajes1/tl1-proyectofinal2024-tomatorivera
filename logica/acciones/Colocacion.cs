using Logica.Modelo;

namespace Logica.Acciones
{
    public class Colocacion : Accion
    {
        private CalidadAccion calidadPaseAnterior;

        public Colocacion(Rally rally, CalidadAccion calidadPaseAnterior) : base(rally)
        {
            this.calidadPaseAnterior = calidadPaseAnterior;
        }

        public override ResultadoAccion Realizar()
        {
            // Calculo la calidad de la colocación
            var calidad = calcularCalidad();

            // Obtengo los jugadores atacantes y defensores porque el pase puede ir a cualquiera
            // de estos según la calidad del pase del colocador. Por supuesto filtro dichos jugadores
            // para que el colocador no se lance un pase a sí mismo
            var atacantes = rally.ObtenerEquipoPropio().ObtenerAtacantes().Where(j => j.NumeroCamiseta != realizador.NumeroCamiseta).ToList();
            var defensores = rally.ObtenerEquipoPropio().ObtenerDefensas().Where(j => j.NumeroCamiseta != realizador.NumeroCamiseta).ToList();

            Jugador? atacante = null;
            string mensajeAccion = string.Empty;
            switch (calidad)
            {
                // Si la colocación es muy buena, irá hacia el mejor atacante del equipo
                case CalidadAccion.EXCELENTE:
                    atacante = atacantes.OrderByDescending(j => j.HabilidadRemate).First();
                    mensajeAccion = $"¡{realizador.Nombre} realiza una excelente colocación para {atacante.Nombre}!";
                    rally.JugadorActual.AumentarCansancio(0.4f, 0.6f);
                    break;
                
                // Si la colocación no es tan buena, irá hacia a cualquier atacante
                case CalidadAccion.MEDIA:
                    atacante = atacantes[rnd.Next(atacantes.Count())];
                    mensajeAccion = $"La colocación de {realizador.Nombre} fue buena. {atacante.Nombre} puede rematar";
                    rally.JugadorActual.AumentarCansancio(0.1f, 0.4f);
                    break;
                
                // Si la colocación es mala, puede ser salvada por cualquier atacante o por cualquier defensor
                case CalidadAccion.MALA:
                    atacante = (rnd.NextDouble() >= 0.5f) ? atacantes[rnd.Next(atacantes.Count())] : defensores[rnd.Next(defensores.Count())];
                    mensajeAccion = $"{realizador.Nombre} alcanza por poco la pelota y logra levantarla. ¡{atacante.Nombre} se acerca a salvarla!";
                    rally.JugadorActual.AumentarCansancio(0.03f, 0.06f);
                    break;
            }

            rally.JugadorActual = atacante!;
            return new ResultadoAccion(new Remate(rally, calidad), mensajeAccion);
        }

        /// <summary>
        /// Calcula la calidad de la colocación
        /// </summary>
        /// <returns>Objeto <c>CalidadAccion</c></returns>
        private CalidadAccion calcularCalidad()
        {
            // La facilidad para alcanzar la pelota es la 1.5 parte de la habilidad de colocación del jugador más o
            // menos una bonificación que puede recibir según la calidad del pase anterior menos el cansancio del colocador
            var facilidadAlcancePelota = (realizador.HabilidadColocacion + calcularBonificacionColocacion() - realizador.Cansancio) / 1.5f / 10;
            var rndNum = Math.Round(rnd.NextDouble(), 3);

            //System.Console.WriteLine($"CALIDAD COL.: {realizador.Nombre} - {rndNum} - {facilidadAlcancePelota}");
        
            CalidadAccion calidad;
            // Si el número aleatorio es menor que la facilidad de alcance, la colocación es muy buena
            if (rndNum <= facilidadAlcancePelota)
            {
                calidad = CalidadAccion.EXCELENTE;
            }

            // Si el número aleatorio es mayor que la facilidad de alcance pero menor que la habilidad
            // de colocación del jugador, la colocación es no tan buena
            else if (rndNum <= (realizador.HabilidadColocacion / 10))
            {
                calidad = CalidadAccion.MEDIA;
            }

            // Si el número random es mayor, entonces la colocación es mala
            else
            {
                calidad = CalidadAccion.MALA;
            }

            return calidad;
        }

        /// <summary>
        /// Calcula una bonificación para las habilidades de colocación del jugador
        /// según la calidad del pase que haya recibido el jugador. Si el pase es bueno,
        /// es más fácil de colocar, de lo contrario es más difícil
        /// </summary>
        /// <returns>Número <c>double</c> de aumento o disminución de habilidad para el colocador</returns>
        private double calcularBonificacionColocacion()
        {
            return calidadPaseAnterior switch
            {
                CalidadAccion.EXCELENTE => 1.5f + rnd.NextDouble(),
                CalidadAccion.MEDIA => 0.5f + rnd.NextDouble(),
                _ => 0 - rnd.NextDouble()
            };
        }
    }


}