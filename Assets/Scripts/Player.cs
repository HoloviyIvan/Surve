using UnityEngine;

public class Player : Character
{
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var forward = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += forward * moveSpeed * Time.deltaTime;
    }
}
