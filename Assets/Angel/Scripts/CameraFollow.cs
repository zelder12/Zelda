using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Vector3 offset = new Vector3(0, 0, -10); // Desplazamiento de la c�mara respecto al jugador
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    void LateUpdate()
    {
        // Verificar si el jugador est� asignado
        if (player == null)
            return;

        // Calcular la posici�n deseada de la c�mara
        Vector3 desiredPosition = player.position + offset;
        // Suavizar la transici�n de la c�mara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Asignar la nueva posici�n a la c�mara
        transform.position = smoothedPosition;
    }
}
