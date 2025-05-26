using UnityEngine;

public class camera_knight1 : MonoBehaviour
{   
    /*ナイトのカメラ*/
    public GameObject Camera_knight1, Camera_knight2, Camera_knight3, Camera_knight4, Camera_knight_up;

    void Start()
    {
        Camera_knight_up.gameObject.SetActive(false);
        Camera_knight1.gameObject.SetActive(false);
        Camera_knight2.gameObject.SetActive(false);
        Camera_knight3.gameObject.SetActive(false);
        Camera_knight4.gameObject.SetActive(false);
    }

    void Update()
    {   

        if(Input.GetKeyDown(KeyCode.A) && Camera_knight3.activeSelf && Camera_knight4.activeSelf){
        Camera_knight1.gameObject.SetActive(true);
        Camera_knight2.gameObject.SetActive(true);
        Camera_knight3.gameObject.SetActive(false);
        Camera_knight4.gameObject.SetActive(false);
        Camera_knight_up.gameObject.SetActive(true);
        } else if(Input.GetKeyDown(KeyCode.D) && Camera_knight2.activeSelf && Camera_knight1.activeSelf){
        Camera_knight1.gameObject.SetActive(false);
        Camera_knight2.gameObject.SetActive(false);
        Camera_knight3.gameObject.SetActive(true);
        Camera_knight4.gameObject.SetActive(true);
        Camera_knight_up.gameObject.SetActive(true);
        }

    }
}
