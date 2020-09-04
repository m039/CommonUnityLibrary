using UnityEngine;

namespace m039.Common
{

    public class GuardYieldInstruction : CustomYieldInstruction
    {
        bool _keep = false;

        public void Free()
        {
            _keep = true;
        }

        public void Restart()
        {
            _keep = false;
        }

        public override bool keepWaiting
        {
            get
            {
                return !_keep;
            }
        }
    }

}
