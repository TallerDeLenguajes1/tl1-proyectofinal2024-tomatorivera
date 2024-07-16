using Logica.Acciones;

namespace Logica.Modelo
{
    public class ResultadoAccion
    {
        public bool Continuar { get; set; }
        public bool Punto { get; set;}
        public Accion AccionSiguiente { get; set; }

        public ResultadoAccion(bool punto, Accion accionSiguiente)
        {
            Punto = punto;
            AccionSiguiente = accionSiguiente;
        }
    }
}