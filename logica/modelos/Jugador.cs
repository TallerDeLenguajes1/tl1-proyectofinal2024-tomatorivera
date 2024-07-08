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
    }
    
    public enum TipoJugador
    {
        PUNTA,
        OPUESTO,
        ARMADOR,
        LIBERO,
        CENTRAL
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