using UnityEngine;
using UnityEngine.EventSystems;

public class CreateCharacterClickEvent { }
public class CreateCharacterButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ObserverManager.Notify(new CreateCharacterClickEvent());
    }
}