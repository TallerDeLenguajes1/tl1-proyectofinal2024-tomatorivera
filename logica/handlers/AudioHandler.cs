using System.Collections.Concurrent;
using Logica.Modelo;
using NAudio.Wave;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Logica.Handlers;

public class AudioHandler
{
    private readonly static AudioHandler instancia = new AudioHandler();
    private ConcurrentDictionary<Audio, CancellationTokenSource> audiosReproduciendose;

    private AudioHandler()
    {
        audiosReproduciendose = new ConcurrentDictionary<Audio,CancellationTokenSource>();
    }

    public static AudioHandler Instancia { get => instancia; }

    /// <summary>
    /// Obtiene la ruta de un audio
    /// </summary>
    /// <param name="audio">Audio a obtener</param>
    /// <returns><c>string</c> path del audio</returns>
    public string ObtenerRutaAudio(Audio audio)
    {
        try
        {
            var archivoAudio = Config.DirectorioAudios + @"\" + audio switch
            {
                Audio.MENU_TICK => Config.AudioMenu,
                Audio.MENU_SELECTION => Config.AudioMenuSelection,
                Audio.MENU_BACKGROUND => Config.AudioMenuBackground,
                Audio.PARTIDO_ENCABEZADO => Config.AudioPartidoEncabezado,
                Audio.PARTIDO_BACKGROUND => Config.AudioPartidoBackground,
                Audio.PARTIDO_GANADO => Config.AudioPartidoGanado,
                Audio.PARTIDO_PERDIDO => Config.AudioPartidoPerdido,
                _ => "no encontrado"
            };

            RecursosUtil.VerificarArchivo(archivoAudio);

            return archivoAudio;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Reproduce un audio
    /// </summary>
    /// <param name="audio">Audio a reproducir</param>
    /// <param name="loop">Si se reproducirá en loop o no (por defecto false)</param>
    public void Reproducir(Audio audio, bool loop = false)
    {
        var audioPath = ObtenerRutaAudio(audio);
        
        // si el path está vacío quiere decir que no se encontró la ruta y no se puede proseguir con su reproducción
        if (string.IsNullOrEmpty(audioPath)) return;

        var cancellationTokenSrc = new CancellationTokenSource();
        var token = cancellationTokenSrc.Token;

        var tarea = Task.Run(() =>
        {
            WaveStream audioFile = loop ? new LoopStream(new WaveFileReader(audioPath)) : new AudioFileReader(audioPath);
        
            using (audioFile)
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (token.IsCancellationRequested)
                    {
                        outputDevice.Stop();
                        break;
                    }

                    Thread.Sleep(500);
                }
            }
        }, token);

        audiosReproduciendose.TryAdd(audio, cancellationTokenSrc);
        tarea.ContinueWith(t => 
        {
            audiosReproduciendose.TryRemove(audio, out _);
        });
    }

    /// <summary>
    /// Detiene un audio si es que está reproduciendose
    /// </summary>
    /// <param name="audio">Audio a detener</param>
    public void Detener(Audio audio)
    {
        if (EstaReproduciendose(audio))
        {
            audiosReproduciendose.TryGetValue(audio, out var token);
            token?.Cancel();
        }
    }

    /// <summary>
    /// Detiene todas las pistas en reproducción
    /// </summary>
    public void DetenerTodos()
    {
        foreach (var token in audiosReproduciendose.Values)
        {
            token.Cancel();
        }
        audiosReproduciendose.Clear();
    }
    
    /// <summary>
    /// Verifica si un audio está reproduciéndose
    /// </summary>
    /// <param name="audio">Audio a verificar</param>
    /// <returns><c>True</c> si se está reproduciendo, <c>False</c> en caso contrario</returns>
    public bool EstaReproduciendose(Audio audio)
    {
        return audiosReproduciendose.ContainsKey(audio);
    }
}

/*
/// <summary>
/// Gestiona un audio en particular, permitiendo ejecutarlo en distintos
/// hilos y detenerlos cuando se necesite
/// </summary>
public class AudioHandler : IDisposable
{
    private string archivo;
    private ConcurrentDictionary<Task, CancellationTokenSource> reproducciones;
    private bool disposed;
    
    public AudioHandler(string archivo)
    {
        this.archivo = archivo;
        reproducciones = new ConcurrentDictionary<Task, CancellationTokenSource>();
        disposed = false;
    }

    /// <summary>
    /// Genera una nueva tarea de reproducción
    /// </summary>
    /// <param name="audioFile"><c>WaveStream</c> stream de reproducción</param>
    /// <returns>Tarea <c>Task</c> de reproducción</returns>
    private Task generarTareaReproduccion(WaveStream audioFile)
    {
        var tokenSrc = new CancellationTokenSource();
        var task = new Task(() =>
        {
            using (audioFile)
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (tokenSrc.Token.IsCancellationRequested)
                    {
                        outputDevice.Stop();
                        break;
                    }

                    Thread.Sleep(500);
                }
            }
        }, tokenSrc.Token);

        reproducciones.TryAdd(task, tokenSrc);
        return task;
    }

    /// <summary>
    /// Detiene todas las reproducciones de este audio
    /// </summary>
    public void DetenerReproduccion()
    {
        foreach (var cancellationTokenSrc in reproducciones.Values)
        {
            cancellationTokenSrc.Cancel();
        }

        reproducciones.Clear();
    }

    /// <summary>
    /// Reproduce un archivo de audio
    /// </summary>
    public void Reproducir()
    {
        var audioFile = new AudioFileReader(archivo);
        var tareaReproduccion = generarTareaReproduccion(audioFile);

        tareaReproduccion.Start();
        tareaReproduccion.ContinueWith(t =>
        {
            reproducciones.TryRemove(tareaReproduccion, out _);
        });
    }

    /// <summary>
    /// Reproduce el archivo de audio en Loop
    /// </summary>
    public void ReproducirLoop()
    {
        var audioFile = new LoopStream(new WaveFileReader(archivo));
        var tareaReproduccion = generarTareaReproduccion(audioFile);

        tareaReproduccion.Start();
        tareaReproduccion.ContinueWith(t =>
        {
            reproducciones.TryRemove(tareaReproduccion, out _);
        });
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Remuevo esta instancia del servicio de audios
                AudiosManager.Instancia.ListaReproducciones.Remove(this);

                // Detengo de manera segura todas las reproducciones y limpio la lista
                DetenerReproduccion();

                foreach (var kvp in reproducciones)
                {
                    kvp.Value.Dispose();
                }
                reproducciones.Clear();
            }

            disposed = true;
        }
    }
    
    ~AudioHandler()
    {
        Dispose(false);
    }
}

/// <summary>
/// Clase encargada de crear instancias de AudioHandler con sus respectivos
/// audios para poder reproducirlos y detenerlos arbitrariamente
/// </summary>
public class AudiosManager
{
    private readonly static AudiosManager instancia = new AudiosManager();
    private List<AudioHandler> listaReproducciones;

    private AudiosManager()
    {
        listaReproducciones = new List<AudioHandler>();
    }

    public static AudiosManager Instancia { get => instancia; }
    public List<AudioHandler> ListaReproducciones { get => listaReproducciones; set => listaReproducciones = value; }

    /// <summary>
    /// Genera un reproductor para un audio en especifico
    /// </summary>
    /// <param name="audio">Tipo de audio requerido</param>
    /// <returns><c>AudioHandler</c></returns>
    /// <exception cref="PathInvalidoException">En caso de que no se pueda obtener el archivo de audio correspondiente a <paramref name="audio"/></exception>
    public AudioHandler GenerarAudio(Audio audio)
    {
        var archivoAudio = Config.DirectorioAudios + @"\" + audio switch
        {
            Audio.MENU_TICK => Config.AudioMenu,
            Audio.MENU_SELECTION => Config.AudioMenuSelection,
            Audio.MENU_BACKGROUND => Config.AudioMenuBackground,
            _ => "no encontrado"
        };

        RecursosUtil.VerificarArchivo(archivoAudio);

        var reproductor = new AudioHandler(archivoAudio);
        ListaReproducciones.Add(reproductor);
         
        return reproductor; 
    }

    /// <summary>
    /// Detiene de manera segura todos los audios en reproducción
    /// </summary>
    public void DetenerTodos()
    {
        foreach (var audio in listaReproducciones)
        {
            audio.DetenerReproduccion();
        }
        listaReproducciones.Clear();
    }
}
*/