using UnityEngine;
using UnityEngine.EventSystems;

public class PlayClickEvent { }

public class PlayButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ObserverManager.Notify(new PlayClickEvent());
    }
}
