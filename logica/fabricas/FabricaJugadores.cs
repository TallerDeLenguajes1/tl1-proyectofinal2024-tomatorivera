using Logica.Modelo;

namespace Logica.Fabricas
{
    public interface JugadorFabrica
    {
        Jugador CrearJugador();
    }

    public class JugadorLiberoFabrica() : JugadorFabrica
    {
        public Jugador CrearJugador()
        {
            return new Jugador();
        }
    }

    public class JugadorRematadorFabrica() : JugadorFabrica
    {
        public Jugador CrearJugador()
        {
            return new Jugador();
        }
    }

    public class JugadorArmadorFabrica() : JugadorFabrica
    {
        public Jugador CrearJugador()
        {
            return new Jugador();
        }
    }

    public class JugadorCentralFabrica() : JugadorFabrica
    {
        public Jugador CrearJugador()
        {
            return new Jugador();
        }
    }

    public class JugadorServidorFabrica() : JugadorFabrica
    {
        public Jugador CrearJugador()
        {
            return new Jugador();
        }
    }
}