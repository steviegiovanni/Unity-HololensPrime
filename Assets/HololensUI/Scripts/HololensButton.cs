using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HololensButton : MonoBehaviour,IInputClickHandler {
    public UnityEvent OnClick;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (OnClick != null)
            OnClick.Invoke();
    }
}
