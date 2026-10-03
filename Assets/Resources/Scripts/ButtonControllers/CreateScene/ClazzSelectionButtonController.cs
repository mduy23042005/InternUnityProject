using UnityEngine;
using UnityEngine.EventSystems;

public class ClazzSelectionClickEvent
{
    public int idSelection;
}

public class ClazzSelectionButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        int id = transform.GetSiblingIndex();

        ObserverManager.Notify(new ClazzSelectionClickEvent { idSelection = id });
    }
}
