using UnityEngine;
using UnityEngine.EventSystems;

public class CreateClickEvent { }

public class CreateButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ObserverManager.Notify(new CreateClickEvent());
    }
}
