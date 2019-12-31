using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class CommonExt
    {

        public static string Decorate(this string str) {
            return $"<<< {str} >>>";
        }

    }

}