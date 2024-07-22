using Logica.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Persistencia.Infraestructura;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que representa un jugador manejado por el usuario
    /// </summary>
    public class Jugador
    {
        private string? nombre;
        private int numeroCamiseta;
        [JsonConverter(typeof(StringEnumConverter))]
        private TipoJugador tipoJugador;
        private float experiencia;
        private float habilidadSaque;
        private float habilidadRemate;
        private float habilidadRecepcion;
        private float habilidadColocacion;
        private float habilidadBloqueo;
        private float cansancio;

        public Jugador()
        {
            nombre = string.Empty;
            numeroCamiseta = 0;
            tipoJugador = TipoJugador.PUNTA;
            experiencia = 0;
            habilidadSaque = 0;
            habilidadRemate = 0;
            habilidadRecepcion = 0;
            habilidadColocacion = 0;
            habilidadBloqueo = 0;
            cansancio = 0;
        }

        public Jugador(TipoJugador tipoJugador)
        {
            this.tipoJugador = tipoJugador;
        }

        public Jugador(string? nombre, int numeroCamiseta, TipoJugador tipoJugador, float experiencia)
        {
            this.nombre = nombre;
            this.numeroCamiseta = numeroCamiseta;
            this.tipoJugador = tipoJugador;
            this.experiencia = experiencia;

            habilidadSaque = 0;
            habilidadRemate = 0;
            habilidadRecepcion = 0;
            habilidadColocacion = 0;
            habilidadBloqueo = 0;
            cansancio = 0;
        }

        // Propiedades
        public string? Nombre { get => nombre; set => nombre = value; }
        public int NumeroCamiseta { get => numeroCamiseta; set => numeroCamiseta = value; }

        [JsonConverter(typeof(StringEnumConverter))] // Para guardar el ENUM en json con su nombre y no su ID
        public TipoJugador TipoJugador { get => tipoJugador; set => tipoJugador = value; }

        public float Experiencia { get => experiencia; set => experiencia = value; }
        public float HabilidadSaque { get => habilidadSaque; set => habilidadSaque = value; }
        public float HabilidadRemate { get => habilidadRemate; set => habilidadRemate = value; }
        public float HabilidadRecepcion { get => habilidadRecepcion; set => habilidadRecepcion = value; }
        public float HabilidadColocacion { get => habilidadColocacion; set => habilidadColocacion = value; }
        public float HabilidadBloqueo { get => habilidadBloqueo; set => habilidadBloqueo = value; }
        public float Cansancio { get => (float)Math.Round(cansancio, 2); set => cansancio = value; }
        
        // Métodos

        /// <summary>
        /// Aumenta el cansancio del jugador entre <paramref name="min"/> y <paramref name="max"/>
        /// teniendo en cuenta el factor experiencia del jugador.
        /// </summary>
        /// <param name="min">Mínimo a aumentar</param>
        /// <param name="max">Máximo a aumentar</param>
        public void AumentarCansancio(float min, float max)
        {
            float reduccionSegunExperiencia = 1 / experiencia;
            float incrementoCansancio = ProbabilidadesUtil.ValorAleatorioEntre(min, max) * reduccionSegunExperiencia;

            this.cansancio = Math.Min(cansancio + incrementoCansancio, 10.0f);
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(nombre) ? $"{nombre} ({numeroCamiseta}) - {tipoJugador} - {experiencia} pts. de experiencia en juego"
                                                      : $"Jugadores {tipoJugador}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            Jugador other = (Jugador)obj;
            return NumeroCamiseta == other.NumeroCamiseta;
        }
        
        public override int GetHashCode()
        {
            return NumeroCamiseta.GetHashCode();
        }
    }

    // Modelos para API's relacionadas al jugador
    public class Name
    {
        [JsonProperty("first", NullValueHandling = NullValueHandling.Ignore)]
        public string? First { get; set; }

        [JsonProperty("last", NullValueHandling = NullValueHandling.Ignore)]
        public string? Last { get; set; }
    }

    public class NameResultado
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public Name? Name { get; set; }
    }

    public class NameRaiz : IConsumido
    {
        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<NameResultado>? Results { get; set; }
    }

}