using System.Collections;

namespace Logica.Modelo
{
    /// <summary>
    /// Implementación de la estructura de datos Lista Circular
    /// </summary>
    /// <typeparam name="T">Tipo de dato manejado</typeparam>
    public class ListaCircular<T> : IEnumerable<T>
    {
        private int nElementos;
        private NodoListaCircular<T>? cabecera;

        public ListaCircular()
        {
            cabecera = new NodoListaCircular<T>();
            nElementos = 0;
        }

        // Métodos

        /// <summary>
        /// Inserta un nuevo nodo a la lista circular
        /// </summary>
        /// <param name="valor">Valor a insetar</param>
        public void Insertar(T valor)
        {
            var nuevoNodo = new NodoListaCircular<T> { Valor = valor };
            
            if (EstaVacia())
            {
                cabecera = nuevoNodo;
                cabecera.Siguiente = nuevoNodo;
            }
            else
            {
                nuevoNodo.Siguiente = cabecera!.Siguiente;
                cabecera.Siguiente = nuevoNodo;
            }

            nElementos++;
        }

        /// <summary>
        /// Reemplaza el valor de un nodo en particular si es que se encuentra en la lista
        /// </summary>
        /// <param name="valorAnterior">Valor a reemplazar</param>
        /// <param name="valorNuevo">Nuevo valor</param>
        /// <exception cref="InvalidOperationException">Si la lista está vacía o <paramref name="valorAnterior"/> no está en la lista</exception>
        public void Reemplazar(T valorAnterior, T valorNuevo)
        {
            if (EstaVacia() || !this.Contains(valorAnterior))  
                throw new InvalidOperationException($"{valorAnterior} no está en la lista");

            NodoListaCircular<T>? actual = cabecera!.Siguiente;

            do
            {
                if (EqualityComparer<T>.Default.Equals(actual!.Valor, valorAnterior))
                {
                    actual.Valor = valorNuevo;
                    break;
                }
                actual = actual.Siguiente;
            } 
            while (actual != cabecera!.Siguiente);
        }

        /// <summary>
        /// Verifica si la lista circular está vacía
        /// </summary>
        /// <returns></returns>
        public bool EstaVacia()
        {
            return nElementos == 0 || cabecera == null;
        }

        /// <summary>
        /// Borra el último nodo insertado en la lista circular
        /// </summary>
        public void Borrar()
        {
            if (EstaVacia()) return;
            
            if (nElementos == 1)
            {
                cabecera = null;
            }
            else
            {
                cabecera!.Siguiente = cabecera.Siguiente!.Siguiente;
            }

            nElementos--;
        }

        /// <summary>
        /// Obtiene el valor en la ventana de la lista circular
        /// </summary>
        /// <returns>Valor <c>T</c> contenido por el último nodo insertado a la lista</returns>
        /// <exception cref="InvalidOperationException">Si la lista está vacía</exception>
        public T Valor()
        {
            if (EstaVacia())
                throw new InvalidOperationException("La lista está vacía");
            
            return cabecera!.Siguiente!.Valor!;
        }
        
        /// <summary>
        /// Obtiene el objeto <c>NodoListaCircular</c> que corresponde a la ventana de la L.C.
        /// </summary>
        /// <returns>Nodo ventana <c>NodoListaCircular</c></returns>
        /// <exception cref="InvalidOperationException">Cuando la lista está vacía</exception>O
        public NodoListaCircular<T> ObtenerNodoVentana()
        {
            if (EstaVacia())
                throw new InvalidOperationException("La lista está vacía");
            
            return cabecera!.Siguiente!;
        }

        /// <summary>
        /// Realiza una rotación en la lista circular
        /// </summary>
        public void Rotar()
        {
            if (nElementos > 1)
            {
                cabecera = cabecera!.Siguiente!;
            }
        }
        
        /// <summary>
        /// Cuenta la cantidad de elementos en la lista circular
        /// </summary>
        /// <returns></returns>
        public int ContarElementos()
        {
            return nElementos;
        }

        /// <summary>
        /// Verifica si un elemento está contenido en la lista
        /// </summary>
        /// <param name="obj">Objeto a buscar</param>
        /// <returns><c>True</c> en caso de que sí esté, <c>False</c> en caso contrario</returns>
        public bool Contiene(T obj)
        {
            bool contenido = false;
            foreach (var item in this)
            {
                if (EqualityComparer<T>.Default.Equals(item, obj))
                {
                    contenido = true;
                    break;
                }
            }
            
            return contenido;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListaCircularEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Enumerador de la lista circular
    /// </summary>
    /// <typeparam name="T">Tipo de dato manejado por la L.C.</typeparam>
    public class ListaCircularEnumerator<T> : IEnumerator<T>
    {
        private readonly ListaCircular<T> lista;
        private NodoListaCircular<T>? current;
        private int elementosRestantes;

        public ListaCircularEnumerator(ListaCircular<T> lista)
        {
            this.lista = lista;
            this.current = null;
            this.elementosRestantes = lista.ContarElementos();
        }

        public T Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException();
                }
                return current.Valor!;
            }
        }

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (elementosRestantes == 0)
            {
                return false;
            }

            if (current == null)
            {
                current = lista.ObtenerNodoVentana();
            }
            else
            {
                current = current.Siguiente;
            }

            elementosRestantes--;
            return true;
        }

        public void Reset()
        {
            current = null;
            elementosRestantes = lista.ContarElementos();
        }

        /*** No requerido ***/
        public void Dispose() { }
    }

    /// <summary>
    /// Representa un nodo de una lista circular
    /// </summary>
    /// <typeparam name="T">Tipo de dato del valor contenido por el nodo</typeparam>
    public class NodoListaCircular<T>
    {
        public T? Valor { get; set; }
        public NodoListaCircular<T>? Siguiente { get; set; }
    }
}