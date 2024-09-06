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
                playerMovement.RegenerarCorazon();
                break;
            case ObjetosEquipo.fiera:
                playerMovement.AumentarVida(2);
                break;
            case ObjetosEquipo.conejo:
                playerMovement.AumentarVelocidad(1); 
                break;
        }

        Destroy(this.gameObject);
    }
}
