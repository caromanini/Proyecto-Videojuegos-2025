using UnityEngine;
using System; // Necesario para el evento Action

public class Timer_Script : MonoBehaviour
{
    // EVENTO: Usado para notificar a otros scripts (como un MusicController) cuando el tiempo se acaba.
    public static event Action OnTimerStop;

    [Header("Audio Settings")]
    [Tooltip("Arrastra aqu� el AudioSource que contiene la m�sica de fondo.")]
    public AudioSource bgmAudioSource;

    void Start()
    {
        // Al iniciar la escena, asumimos que el contador empieza a 'andar'.
        StartMusic();
    }

    // M�todo para iniciar la m�sica
    public void StartMusic()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            // Asegura que la m�sica se repita y la reproduce
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            Debug.Log("M�sica de fondo iniciada.");
        }
    }

    // M�todo para detener la m�sica (debe ser llamado por tu l�gica de derrota/fin del tiempo)
    public void StopMusic()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            // Llama al evento para que otros scripts sepan que el temporizador se detuvo.
            OnTimerStop?.Invoke();
            Debug.Log("M�sica detenida, el contador ha parado.");
        }
    }

    // Nota: El m�todo Update() se mantiene vac�o seg�n tu solicitud.
    void Update()
    {

    }
}