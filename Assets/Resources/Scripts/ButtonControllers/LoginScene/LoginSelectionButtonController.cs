using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectionClickEvent
{
    public int idSelection;
}

public class LoginSelectionButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        int id = transform.GetSiblingIndex();

        ObserverManager.Notify(new CharacterSelectionClickEvent { idSelection = id });
    }
}
