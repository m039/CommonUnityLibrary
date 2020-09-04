using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    /// <summary>
    /// A class that helps use button or key presses like axes
    /// which changes gradually over time based on gravity and sensitivity settings.
    /// 
    /// The idea is borrowed from here <see href="https://guavaman.com/projects/rewired/docs/RewiredEditor.html"/>.
    /// </summary>
    [System.Serializable]
    public class DigitalAxisHelper
    {
        #region Inspector

        public bool snap = true;

        public float gravity = 3;

        public float sensitivity = 3;

        #endregion

        float _value = 0f;

        float _valuePrevious = 0f;

        float _axisValue = 0f;

        public void Update()
        {
            if (_value == 0 && _axisValue == 0)
            {
                return;
            }

            // If snapping is enabled, reset the axis value.

            if (snap && _value != _valuePrevious)
            {
                _axisValue = 0;
            }

            var dg = Time.deltaTime * gravity;
            var ds = Time.deltaTime * sensitivity;

            float getValue(float buttonValue, float axis)
            {
                float value;

                if (buttonValue == 0)
                {
                    if (axis < 0)
                    {
                        value = axis + dg;

                        if (value > 0)
                        {
                            value = 0;
                        }
                    }
                    else
                    {
                        value = axis - dg;

                        if (value < 0)
                        {
                            value = 0;
                        }
                    }
                }
                else
                {
                    value = Mathf.Clamp(axis + Mathf.Sign(buttonValue) * ds, -1f, 1f);
                }

                return value;
            }

            _axisValue = getValue(_value, _axisValue);
            _valuePrevious = _value;
        }

        public void SetValue(float value)
        {
            _value = value;
        }

        public float GetAxis()
        {
            return _axisValue;
        }
    }

}
