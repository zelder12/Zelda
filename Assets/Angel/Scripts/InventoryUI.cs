using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public int numberOfSlots = 20;

    void Start()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            Instantiate(slotPrefab, transform);
        }
    }
}
