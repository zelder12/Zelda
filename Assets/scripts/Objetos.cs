using UnityEngine;

public class Objetos : MonoBehaviour
{
    public enum ObjetosEquipo
    {
        gato,
        fiera,
        conejo
    };

    [SerializeField] private ObjetosEquipo objetosEquipo;

    public ObjetosEquipo GetObjetosEquipo()
    {
        return objetosEquipo;
    }

    public void UsarObjeto()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement no encontrado.");
            return;
        }

        Debug.Log("Usando objeto de tipo: " + objetosEquipo);

        switch (objetosEquipo)
        {
            case ObjetosEquipo.gato:
                playerMovement.Curar(1, false); // Cura 1 punto de vida
                break;
            case ObjetosEquipo.fiera:
                playerMovement.Curar(3, true); // Cura completamente
                break;
            case ObjetosEquipo.conejo:
                playerMovement.AumentarVelocidad(1); // Aumenta la velocidad
                break;
        }

        Destroy(gameObject); // Destruye el objeto despu�s de usarlo
    }
}
