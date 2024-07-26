using Logica.Modelo;

namespace Logica.Excepciones
{
    public class FormacionInvalidaException : Exception
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

    public class SustitucionInvalidaException : Exception
    {
        public SustitucionInvalidaException(string message) : base(message)
        {}
    }
}