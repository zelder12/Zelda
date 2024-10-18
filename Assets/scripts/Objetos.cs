using UnityEngine;

public class Objetos : MonoBehaviour
{
    //ANGEL
    public Item weaponItem;
    //FIN
    public enum ObjetosEquipo
    {
        gato,
        fiera,
        conejo,
        escudoBurbuja
    };

    [SerializeField] public ObjetosEquipo objetosEquipo;

    public ObjetosEquipo GetObjetosEquipo()
    {
        return objetosEquipo;
    }

    public void UsarObjeto(Inventario inventario, int slotIndex)
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
            case ObjetosEquipo.escudoBurbuja:
                playerMovement.ActivarEscudoBurbuja(5.0f); // Activa el escudo por 5 segundos
                break;
        }

        inventario.VaciarSlot(slotIndex);

        //switch (weaponItem.objetosEquipo)
        //{
        //    case Item.ObjetosEquipo.Espada:
                
        //        break;
        //    default:
        //        break;
        //}
    }
}