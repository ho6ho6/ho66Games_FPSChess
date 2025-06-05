using UnityEngine;

public class Camera_piece : MonoBehaviour
{
    public Camera camera_piece;

    void Update()
    {
        Show_WhiteOnly();
    }

    public void Show_WhiteOnly()
    {
        camera_piece.cullingMask = LayerMask.GetMask("Stage_Ground", "white","UI");
    }
}
