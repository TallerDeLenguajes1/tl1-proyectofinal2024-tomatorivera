using System.Text.Json;
using System.Text.Json.Serialization;
using Logica.Modelo;

namespace Logica.Util
{
    public class UsuarioConverter : JsonConverter<Usuario>
    {
        public override Usuario Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Implementación de deserialización si es necesaria
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convertidor que utilizo para mostrar solo el nombre del usuario
        /// en el archivo de persistencia de las partidas
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Usuario value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Nombre);
        }
    }
}