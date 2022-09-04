using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerCard : MonoBehaviour
{
    public int userID;
    public Text UserIDText;
    public Text UserNameText;

    public void UpdateCardInfo(int ID, string userName)
    {
        userID = ID;
        UserIDText.text = ID.ToString();
        UserNameText.text = userName;
    }
}
