using UnityEngine;
using UnityEngine.EventSystems;

namespace m039.Common
{
    public class Unselectable : MonoBehaviour, ISelectHandler
    {
        bool _selected;

        public void OnSelect(BaseEventData eventData)
        {
            _selected = true;
        }

        void Update()
        {
            if (!_selected)
                return;

            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            _selected = false;
        }
    }

}
