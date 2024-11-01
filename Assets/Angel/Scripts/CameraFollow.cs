using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Vector3 offset = new Vector3(0, 0, -10); // Desplazamiento de la cámara respecto al jugador
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    void LateUpdate()
    {
        // Verificar si el jugador está asignado
        if (player == null)
            return;

        // Calcular la posición deseada de la cámara
        Vector3 desiredPosition = player.position + offset;
        // Suavizar la transición de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Asignar la nueva posición a la cámara
        transform.position = smoothedPosition;
    }
}
