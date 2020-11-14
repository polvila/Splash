using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.ScreenManagement
{
    public class PopupScreenView : MonoBehaviour, IPopupScreenView
    {
        public event Action<IPopupScreenView> PopupClosed;
        
        [SerializeField] protected Button CloseButton;
        
        public virtual GameObject OwnerGameObject => this != null ? gameObject : null;
        
        public virtual void SetParams(object paramsObject) {}

        protected virtual void OnDestroy()
        {
            PopupClosed?.Invoke(this);
            PopupClosed = null;
            
            CleanButtons();
        }
        
        protected virtual void CleanButtons()
        {
            if (CloseButton != null)
            {
                CloseButton.onClick.RemoveAllListeners();
            }
        }
        
        public virtual void ClosePopup()
        {
            Destroy(OwnerGameObject);
        }
    }
}
