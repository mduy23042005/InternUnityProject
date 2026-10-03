using UnityEngine;
using UnityEngine.EventSystems;

public class BackClickEvent { }

public class BackButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ObserverManager.Notify(new BackClickEvent());
    }
}
