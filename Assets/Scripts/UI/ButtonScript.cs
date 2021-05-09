using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISubmitHandler, ICancelHandler {
  public MenuButtonHandler buttonHandler;
  public GameObject cancelObject;

  public void OnSubmit(BaseEventData eventData) {
    buttonHandler.HandleButton(name);
  }

  public void OnCancel(BaseEventData eventData) {
    EventSystem.current.SetSelectedGameObject(cancelObject);
  }
}
