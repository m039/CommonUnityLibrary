using UnityEngine;

namespace m039.Common
{

    public static class UIUtils 
    {
        public const float UIReferenceHeight = 1920;

        public static float UICoeff => Screen.height / UIReferenceHeight;
    }

}
