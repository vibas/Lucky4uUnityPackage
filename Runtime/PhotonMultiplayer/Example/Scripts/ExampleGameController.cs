using UnityEngine;
using Lucky4u.PhotonMultiplayer;

public class ExampleGameController : MonoBehaviour
{
    private PUNHelper punHelper = new PUNHelper();

    private void Start()
    {
        StartMultiplayerGame();
    }
    public void StartMultiplayerGame()
    {
        punHelper.InitMultiplayerConnection(userName: "Vibas", "PUNGameplay", 0);
    }

    private void OnEnable()
    {
        PUNEvents.OnRaisedEventDataReceived -= ProcessRaisedEventData;
        PUNEvents.OnRaisedEventDataReceived += ProcessRaisedEventData;
    }

    public void TestRaiseEvent()
    {
        punHelper.RaiseEvent(199, new object[] { "Test" });
    }

    void ProcessRaisedEventData(byte eventCode, object eventData)
    {
        Debug.LogError("Raised Event : " + eventCode);
        object[] eventDataArray = (object[])eventData;
        foreach (var item in eventDataArray)
        {
            Debug.LogError(item.ToString());
        }
    }
}
