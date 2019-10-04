using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private UnityEvent jumpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        jumpEvent.Invoke();
    }
}
