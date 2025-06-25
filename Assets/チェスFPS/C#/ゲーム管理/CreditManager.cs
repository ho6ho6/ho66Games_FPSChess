using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public GameObject Panel_MainMenu;
    public GameObject Panel_Credit;

    public void Show_Credit()
    {
        Panel_MainMenu.SetActive(false);
        Panel_Credit.SetActive(true);
    }

    public void Back_to_Menu()
    {
        Panel_Credit.SetActive(false);
        Panel_MainMenu.SetActive(true);
    }
}
