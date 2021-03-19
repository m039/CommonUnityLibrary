using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace m039.Common
{
    /// <summary>
    /// A class that helps to use button or key presses like axes
    /// which changes gradually over time based on gravity and sensitivity settings.
    /// 
    /// The idea is borrowed from here <see href="https://guavaman.com/projects/rewired/docs/RewiredEditor.html"/>.
    /// </summary>
    public class DigitalAxisHelper
    {
        #region Settings

        public GetSettingValue<bool> snap = () => true;

        public GetSettingValue<float> gravity = () => 3;

        public GetSettingValue<float> sensitivity = () => 3;

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

            // If snapping is enabled, reset the axis value if the player pressed an opposite key.

            if (snap() && Mathf.Sign(_value) != Mathf.Sign(_valuePrevious))
            {
                _axisValue = 0;
            }

            var dg = Time.deltaTime * gravity();
            var ds = Time.deltaTime * sensitivity();

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
                    var direction = Mathf.Sign(buttonValue - axis);
                    value = axis + direction * ds;

                    if (direction > 0)
                    {
                        if (value > buttonValue)
                        {
                            value = buttonValue;
                        }
                    } else
                    {
                        if (value < buttonValue)
                        {
                            value = buttonValue;
                        }
                    }
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

        public void SetAxis(float value)
        {
            _axisValue = value;
        }
    }

}
