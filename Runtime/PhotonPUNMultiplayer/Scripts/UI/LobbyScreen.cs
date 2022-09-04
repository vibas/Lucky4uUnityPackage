using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LobbyScreen : MonoBehaviour
{
    public Action OnStartButtonPressedEvent; 
    public void StartButtonClicked()
    {
        OnStartButtonPressedEvent?.Invoke();
    }
}
