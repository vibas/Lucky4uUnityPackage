using UnityEngine;
using TMPro;

public class RoomPlayerCard : MonoBehaviour
{
    public int userID;
    public TextMeshProUGUI UserIDText;
    public TextMeshProUGUI UserNameText;

    public void UpdateCardInfo(int ID, string userName)
    {
        userID = ID;
        UserIDText.text = ID.ToString();
        UserNameText.text = userName;
    }
}
