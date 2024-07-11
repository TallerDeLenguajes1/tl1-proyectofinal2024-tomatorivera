using Logica.Modelo;

namespace Logica.Fabricas
{
    /// <summary>
    /// Fábrica de jugadores encargada de generar los atributos
    /// del jugador según su posición en la cancha
    /// </summary>
    public abstract class JugadorFabrica
    {
        /// <value>Randomizador utilizado para generar valores aleatorios</value>
        private static Random random = new Random();

        /// <summary>
        /// Crea los atributos de un jugador según su tipo
        /// </summary>
        /// <returns>Objeto <c>Jugador</c></returns>
        public abstract Jugador CrearJugador();

        /// <summary>
        /// Genera un valor aleatorio de punto flotante de dos decimales
        /// </summary>
        /// <param name="min">Valor minimo a generar</param>
        /// <param name="max">Valor máximo a generar</param>
        /// <returns>Número real generado entre <paramref name="min"/> y <paramref name="max"/></returns>
        protected float valorAleatorioEntre(float min, float max)
        {
            return (float) Math.Round(random.NextDouble() * (max - min) + min, 2);
        }

        /// <summary>
        /// Genera la instancia de un nuevo jugador con las habilidades enviadas por parámetro
        /// </summary>
        /// <param name="habilidades">Habilidades del jugador a generar</param>
        /// <returns>Objeto <c>Jugador</c></returns>
        protected Jugador generarJugador(Dictionary<string, (float min, float max)> habilidades)
        {
            return
                new Jugador() 
                { 
                    HabilidadSaque = valorAleatorioEntre(habilidades["saque"].min, habilidades["saque"].max),
                    HabilidadRemate = valorAleatorioEntre(habilidades["remate"].min, habilidades["remate"].max),
                    HabilidadRecepcion = valorAleatorioEntre(habilidades["recepcion"].min, habilidades["recepcion"].max),
                    HabilidadColocacion = valorAleatorioEntre(habilidades["colocacion"].min, habilidades["colocacion"].max),
                    HabilidadBloqueo = valorAleatorioEntre(habilidades["bloqueo"].min, habilidades["bloqueo"].max)
                };
        }
    }

    public class JugadorLiberoFabrica() : JugadorFabrica
    {
        /// <summary>
        /// Crea un jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c> con atributos de Líbero</returns>
        public override Jugador CrearJugador()
        {
            return generarJugador(
                new Dictionary<string, (float min, float max)>()
                    {
                        { "saque", (2, 4) },
                        { "remate", (0.5f, 2) },
                        { "recepcion", (5, 7) },
                        { "colocacion", (1, 3) },
                        { "bloqueo", (0.5f, 2) }
                    }
                );
        }
    }

    public class JugadorPuntaFabrica() : JugadorFabrica
    {
        /// <summary>
        /// Crea un jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c> con atributos de Punta</returns>
        public override Jugador CrearJugador()
        {
            return generarJugador(
                new Dictionary<string, (float min, float max)>()
                {
                    { "saque", (2.5f, 4.5f) },
                    { "remate", (4.5f, 6.5f) },
                    { "recepcion", (2, 4) },
                    { "colocacion", (1, 3) },
                    { "bloqueo", (3, 5) }
                }
            );
        }
    }

    public class JugadorOpuestoFabrica() : JugadorFabrica
    {
        /// <summary>
        /// Crea un jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c> con atributos de Opuesto</returns>
        public override Jugador CrearJugador()
        {
            return generarJugador(
                new Dictionary<string, (float min, float max)>()
                {
                    { "saque", (3, 5) },
                    { "remate", (5, 7) },
                    { "recepcion", (1, 3) },
                    { "colocacion", (1.5f, 3.5f) },
                    { "bloqueo", (2, 4) }
                }
            );
        }
    }

    public class JugadorArmadorFabrica() : JugadorFabrica
    {
        /// <summary>
        /// Crea un jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c> con atributos de Armador</returns>
        public override Jugador CrearJugador()
        {
            return generarJugador(
                new Dictionary<string, (float min, float max)>()
                {
                    { "saque", (2.5f, 5.5f) },
                    { "remate", (3, 6) },
                    { "recepcion", (1, 4) },
                    { "colocacion", (5, 7) },
                    { "bloqueo", (2, 4) }
                }
            );
        }
    }

    public class JugadorCentralFabrica() : JugadorFabrica
    {
        /// <summary>
        /// Crea un jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c> con atributos de Central</returns>
        public override Jugador CrearJugador()
        {
            return generarJugador(
                new Dictionary<string, (float min, float max)>()
                {
                    { "saque", (1, 3) },
                    { "remate", (3, 5) },
                    { "recepcion", (0.5f, 2.5f) },
                    { "colocacion", (2, 4) },
                    { "bloqueo", (5, 7) }
                }
            );
        }
    }
}