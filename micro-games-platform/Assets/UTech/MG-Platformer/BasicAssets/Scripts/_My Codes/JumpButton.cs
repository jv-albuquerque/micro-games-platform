using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent jumpEvent;
    [SerializeField] private UnityEvent stopJumpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        jumpEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stopJumpEvent.Invoke();
    }

}
