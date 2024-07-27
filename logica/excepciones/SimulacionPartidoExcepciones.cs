using Logica.Modelo;

namespace Logica.Excepciones;

public class FormacionInvalidaException : VoleyballManagerRuntimeException
{
    public Equipo? EquipoEnPosesion { get; set; }

    public FormacionInvalidaException(string message) : base(message)
    {
        this.EquipoEnPosesion = null;
    }
    
    public FormacionInvalidaException(string message, Equipo EquipoEnPosesion) : base(message)
    {
        this.EquipoEnPosesion = EquipoEnPosesion;
    }
}

public class SustitucionInvalidaException : VoleyballManagerRuntimeException
{
    public SustitucionInvalidaException(string message) : base(message)
    {}
}

public class PlantillaInvalidaException : VoleyballManagerRuntimeException
{
    public PlantillaInvalidaException(string message) : base(message)
    {}
}