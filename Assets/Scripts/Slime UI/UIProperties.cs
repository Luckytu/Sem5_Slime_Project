using UnityEngine;

namespace Slime_UI
{
    public class UIProperties : MonoBehaviour
    {
        [SerializeField] private Transform _uiBackground;
        public static Transform uiBackground { get; private set; }

        private void Awake()
        {
            uiBackground = _uiBackground;
        }
    }
}