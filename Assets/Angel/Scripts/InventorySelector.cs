using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class InventorySelector : MonoBehaviour
{
    public RectTransform selectorRectTransform;
    public float moveSpeed = 5f;

    private RectTransform currentSlot;

    private void Start()
    {
        if (selectorRectTransform == null)
        {
            Debug.LogError("Selector RectTransform no está asignado.");
        }
    }

    public void MoveToSlot(RectTransform newSlot)
    {
        currentSlot = newSlot;
        StopAllCoroutines();
        StartCoroutine(MoveSelector());
    }

    private IEnumerator MoveSelector()
    {
        Vector3 targetPosition = currentSlot.position;
        while (Vector3.Distance(selectorRectTransform.position, targetPosition) > 0.1f)
        {
            selectorRectTransform.position = Vector3.Lerp(selectorRectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        selectorRectTransform.position = targetPosition;
    }
}
