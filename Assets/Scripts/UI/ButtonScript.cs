using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISubmitHandler, ICancelHandler, ISelectHandler {
  public MenuButtonHandler buttonHandler;
  public GameObject cancelObject;

  public void OnSelect(BaseEventData eventData) {
    SFXHandler.PlaySound("generic_shot");
  }

  public void OnSubmit(BaseEventData eventData) {
    buttonHandler.HandleButton(name);
    SFXHandler.PlaySound("shot_special1");
  }

  public void OnCancel(BaseEventData eventData) {
    EventSystem.current.SetSelectedGameObject(cancelObject);
    SFXHandler.PlaySound("hit");
  }
}
