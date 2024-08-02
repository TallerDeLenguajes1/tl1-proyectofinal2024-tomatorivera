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