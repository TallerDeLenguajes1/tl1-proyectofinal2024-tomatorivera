using Logica.Modelo;

namespace Logica.Excepciones
{
    public class FormacionInvalidaException : Exception
    {
        public Equipo EquipoEnPosesion { get; set; }
        
        public FormacionInvalidaException(string message, Equipo EquipoEnPosesion) : base(message)
        {
            this.EquipoEnPosesion = EquipoEnPosesion;
        }
    }
}