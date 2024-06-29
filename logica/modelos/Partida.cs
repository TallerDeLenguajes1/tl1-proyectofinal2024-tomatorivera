namespace Logica.Modelo
{
    public class Partida
    {
        private int id;
        private DateTime fechaGuardado;
        private Usuario usuario;

        public Partida(int id, DateTime fechaGuardado, Usuario usuario)
        {
            this.id = id;
            this.fechaGuardado = fechaGuardado;
            this.usuario = usuario;
        }

        // Propiedades
        public int Id { get => id; set => id = value; }
        public DateTime FechaGuardado { get => fechaGuardado; set => fechaGuardado = value; }
        public Usuario Usuario { get => usuario; set => usuario = value; }

        // Métodos
        public void Iniciar() {
            // Falta implementar la lógica
            System.Console.WriteLine("Iniciando partida...");
        }

        public override string ToString()
        {
            return "Partida ID " + id + " - " + fechaGuardado + " - DT: " + usuario.NombreUsuario;
        }
    }
}