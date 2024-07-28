using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Persistencia.Util;

public class ExclusionPropiedadesJson : DefaultContractResolver
{
    private readonly HashSet<string> propiedadesExcluidas;

    public ExclusionPropiedadesJson(IEnumerable<string> propiedadesExcluidas)
    {
        this.propiedadesExcluidas = new HashSet<string>(propiedadesExcluidas);
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var propiedades = base.CreateProperties(type, memberSerialization);
        return propiedades.Where(p => !propiedadesExcluidas.Contains(p.PropertyName ?? string.Empty)).ToList();
    }
}