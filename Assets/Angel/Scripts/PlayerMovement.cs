using UnityEngine;

public class PlayerMovementA : MonoBehaviour
{
    public float speed = 5f;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer leftHandRenderer;
    public SpriteRenderer rightHandRenderer;
    public SpriteRenderer leftWeaponRenderer;
    public SpriteRenderer rightWeaponRenderer;

    private Vector2 moveDirection;

    private void Update()
    {
        HandleMovement();
        UpdateSpriteDirections();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        // Movimiento del jugador
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void UpdateSpriteDirections()
    {
        // Cambiar dirección del cuerpo
        if (moveDirection.x < 0)
        {
            bodyRenderer.flipX = true;

        }
        else if (moveDirection.x > 0)
        {
            bodyRenderer.flipX = false;

        }
    }
}
