using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;

    private void Update()
    {
        if (Mouse.current == null || gameCamera == null)
        {
            return;
        }

        // Vị trí con trỏ trên màn hình, tính bằng pixel.
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Khoảng cách từ camera tới mặt phẳng chứa súng.
        // Camera của bài này có Rotation = (0, 0, 0).
        float depth = transform.position.z - gameCamera.transform.position.z;

        // Đổi vị trí chuột thành tọa độ trong thế giới game.
        Vector3 mouseWorldPosition = gameCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                depth
            )
        );

        // Tính hướng từ trục súng đến chuột.
        Vector2 direction = mouseWorldPosition - transform.position;

        // Nếu chuột trùng tâm súng, giữ nguyên hướng hiện tại.
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}