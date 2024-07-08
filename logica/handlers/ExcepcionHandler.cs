namespace Logica.Handlers
{
    public class ErroresIgnorablesHandler 
    {
        public event EventHandler<string>? ErrorOcurrido;

        public void ManejarError(string mensaje)
        {
            ErrorOcurrido?.Invoke(this, mensaje);
        }
    }
}