using UnityEngine;
using System; // Necesario para el evento Action

public class Timer_Script : MonoBehaviour
{
    // EVENTO: Usado para notificar a otros scripts (como un MusicController) cuando el tiempo se acaba.
    public static event Action OnTimerStop;

    [Header("Audio Settings")]
    [Tooltip("Arrastra aquí el AudioSource que contiene la música de fondo.")]
    public AudioSource bgmAudioSource;

    void Start()
    {
        // Al iniciar la escena, asumimos que el contador empieza a 'andar'.
        StartMusic();
    }

    // Método para iniciar la música
    public void StartMusic()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            // Asegura que la música se repita y la reproduce
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            Debug.Log("Música de fondo iniciada.");
        }
    }

    // Método para detener la música (debe ser llamado por tu lógica de derrota/fin del tiempo)
    public void StopMusic()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            // Llama al evento para que otros scripts sepan que el temporizador se detuvo.
            OnTimerStop?.Invoke();
            Debug.Log("Música detenida, el contador ha parado.");
        }
    }

    // Nota: El método Update() se mantiene vacío según tu solicitud.
    void Update()
    {

    }
}