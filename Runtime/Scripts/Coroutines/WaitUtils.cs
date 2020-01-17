using System;
using UnityEngine;

namespace m039.Common
{

	public static class WaitUtils
	{

        /// A version of WaitForSeconds but can be interrupted.
        public static CustomYieldInstruction WaitForSeconds(float seconds, Func<bool> isInterrupted)
        {
            if (isInterrupted == null || isInterrupted() || seconds < 0 )
                return null;

            var endTime = Time.time + seconds;
            return new WaitWhile(() => endTime > Time.time && !isInterrupted());
        }

	}

}