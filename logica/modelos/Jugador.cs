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
        private string nombre;
        private int numeroCamiseta;
        private TipoJugador tipoJugador;
        private float experiencia;
        private float habilidadSaque;
        private float habilidadRemate;
        private float habilidadRecepcion;
        private float habilidadColocacion;
        private float habilidadBloqueo;
        private float cansancio;
        private float precio;

        public Jugador()
        {
            nombre = string.Empty;
            numeroCamiseta = -1;
            tipoJugador = TipoJugador.PUNTA;
            experiencia = 0;
            habilidadSaque = 0;
            habilidadRemate = 0;
            habilidadRecepcion = 0;
            habilidadColocacion = 0;
            habilidadBloqueo = 0;
            cansancio = 0;
            precio = 0;
        }

        public Jugador(TipoJugador tipoJugador)
        {
            nombre = string.Empty;
            this.tipoJugador = tipoJugador;
        }

        public Jugador(float habilidadSaque, float habilidadRemate, float habilidadRecepcion, float habilidadColocacion, float habilidadBloqueo, float experiencia)
        {
            nombre = string.Empty;
            numeroCamiseta = -1;
            this.habilidadSaque = habilidadSaque;
            this.habilidadRemate = habilidadRemate;
            this.habilidadRecepcion = habilidadRecepcion;
            this.habilidadColocacion = habilidadColocacion;
            this.habilidadBloqueo = habilidadBloqueo;
            this.experiencia = experiencia;
            cansancio = 0;
            precio = CalcularPrecio();
        }

        // Propiedades

        [JsonProperty("nombre_jugador")]
        public string Nombre { get => nombre; set => nombre = value; }

        [JsonProperty("numero_camiseta")]
        public int NumeroCamiseta { get => numeroCamiseta; set => numeroCamiseta = value; }

        [JsonProperty("tipo_jugador")]
        [JsonConverter(typeof(StringEnumConverter))] // Para guardar el ENUM en json con su nombre y no su ID
        public TipoJugador TipoJugador { get => tipoJugador; set => tipoJugador = value; }

        [JsonProperty("experiencia")]
        public float Experiencia { get => experiencia; set => experiencia = value; }

        [JsonProperty("habilidad_saque")]
        public float HabilidadSaque { get => habilidadSaque; set => habilidadSaque = value; }

        [JsonProperty("habilidad_remate")]
        public float HabilidadRemate { get => habilidadRemate; set => habilidadRemate = value; }

        [JsonProperty("habilidad_recepcion")]
        public float HabilidadRecepcion { get => habilidadRecepcion; set => habilidadRecepcion = value; }

        [JsonProperty("habilidad_colocacion")]
        public float HabilidadColocacion { get => habilidadColocacion; set => habilidadColocacion = value; }

        [JsonProperty("habilidad_bloqueo")]
        public float HabilidadBloqueo { get => habilidadBloqueo; set => habilidadBloqueo = value; }

        [JsonProperty("precio_mercado")]
        public float Precio { get => (float) Math.Round(precio, 2); set => precio = value; }

        [JsonIgnore]
        public float Cansancio { get => (float)Math.Round(cansancio, 2); set => cansancio = value; }
        
        // Métodos

        /// <summary>
        /// Aumenta el cansancio del jugador según <paramref name="incremento"/> pero
        /// teniendo en cuenta el factor experiencia del jugador
        /// </summary>
        /// <param name="incremento">Incremento de cansancio</param>
        public void AumentarCansancio(float incremento)
        {
            float reduccionSegunExperiencia = 1 / experiencia;
            float incrementoCansancio = incremento * reduccionSegunExperiencia;

            cansancio = Math.Clamp(cansancio + incrementoCansancio, 0.0f, 10.0f);
        }

        /// <summary>
        /// Disminuye en <paramref name="decremento"/> el cansancio del jugador
        /// </summary>
        /// <param name="decremento">Cantidad de cansancio a decrementar</param>
        public void DecrementarCansancio(float decremento)
        {
            cansancio = Math.Clamp(cansancio - decremento, 0.0f, 10.0f);
        }

        /// <summary>
        /// Calcula la calificación del jugador para el mercado
        /// </summary>
        /// <returns><c>float</c> calificación</returns>
        public float CalcularCalificacion()
        {
            return (habilidadSaque + habilidadRecepcion + habilidadBloqueo + habilidadColocacion + habilidadRemate + experiencia) / 6;
        }

        /// <summary>
        /// Calcula el precio de este jugador según su calificacion
        /// </summary>
        /// <returns><c>float</c> precio del jugador</returns>
        public float CalcularPrecio()
        {
            var rnd = new Random();
            var promedioSkills = CalcularCalificacion();
            return promedioSkills switch
            {
                < 3 => rnd.Next(5000, 15000),
                < 7 => rnd.Next(25000, 50000),
                _ => rnd.Next(65000, 100000)
            };
        }

        /// <summary>
        /// Obtiene la descripción de los jugadores utilizada para los partidos
        /// </summary>
        /// <returns><c>string</c> Descripción del jugador con colores</returns>
        public string DescripcionPartido()
        {
            return numeroCamiseta == -1
                ? "[red3_1]:right_arrow_curving_left: Cancelar sustitución[/]"
                : $"[mistyrose3]{numeroCamiseta}[/] :t_shirt: [lightgoldenrod3]{nombre}[/] [tan]({tipoJugador})[/] [gray] - [/][mistyrose3]Cansancio {Math.Round(cansancio, 2)}[/]" +
                $"[gray] - SAQ:[/] {habilidadSaque} [gray]REM:[/] {habilidadRemate} [gray]REC:[/] {habilidadRecepcion} [gray]COL:[/] {habilidadColocacion} [gray]BLO:[/] {habilidadBloqueo}";
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
            return numeroCamiseta == other.NumeroCamiseta &&
                   nombre.Equals(other.Nombre);
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