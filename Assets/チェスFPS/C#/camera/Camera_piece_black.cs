using UnityEngine;

public class Camera_piece_black : MonoBehaviour
{
    public Camera camera_piece;

    void Update()
    {
        Show_BlackOnly();
    }

    public void Show_BlackOnly()
    {
        camera_piece.cullingMask = LayerMask.GetMask("Stage_Ground", "black","UI");
    }
}
