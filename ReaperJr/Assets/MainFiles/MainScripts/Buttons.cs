
using UnityEngine.EventSystems;

public class Buttons : ReaperJr, IPointerEnterHandler, IPointerExitHandler
{

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _GAME.playerActive = false;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _GAME.playerActive = true;
    }
}
