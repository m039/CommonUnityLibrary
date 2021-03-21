/*
 * 
// Popup list created by Eric Haines
// ComboBox Extended by Hyungseok Seo.(Jerry) sdragoon@nate.com
// Refactored by zhujiangbo jumbozhu@gmail.com
// Slight edit for button to show the previously selected item AndyMartin458 www.clubconsortya.blogspot.com
// Furhter adapted by m039 m0391n@gmail.com
// 
// -----------------------------------------------
// This code working like ComboBox Control.
// I just changed some part of code, 
// because I want to seperate ComboBox button and List.
// ( You can see the result of this code from Description's last picture )
// -----------------------------------------------
//
// === usage ======================================
using UnityEngine;
using System.Collections;
 
public class ComboBoxTest : MonoBehaviour
{
	private ComboBox _comboBox;
 
	private void Start()
	{
        var rect = new Rect(50, 400, 100, 20);

        var comboBoxList = new GUIContent[5];
        comboBoxList[0] = new GUIContent("Thing 1");
        comboBoxList[1] = new GUIContent("Thing 2");
        comboBoxList[2] = new GUIContent("Thing 3");
        comboBoxList[3] = new GUIContent("Thing 4");
        comboBoxList[4] = new GUIContent("Thing 5");

        var buttonStyle = new GUIStyle("button");

        var boxStyle = new GUIStyle("box");

        var listStyle = new GUIStyle();
        listStyle.normal.textColor = Color.white;
        listStyle.onHover.background =
        listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left =
        listStyle.padding.right =
        listStyle.padding.top =
        listStyle.padding.bottom = 4;

        _comboBox = new ComboBox(
            rect,
            comboBoxList,
            buttonStyle,
            boxStyle,
            listStyle
            );
        _comboBox.OnItemSelected += (i, userHasClicked) => {
            if (!userHasClicked) return;

            // ...
        };
        _comboBox.Direction = ComboBox.PopupDirection.FromBottomToTop;
        _comboBox.SelectedItemIndex = 0;
	}
 
	private void OnGUI () 
	{
        _comboBox.Show();
	}
}
 
*/

using UnityEngine;

namespace m039.Common
{

    public class ComboBox
    {
        public enum PopupDirection
        {
            FromTopToBottom, FromBottomToTop
        }

        private static bool _sForceToUnShow = false;

        private static int _sUseControlID = -1;

        public Rect rect;

        GUIStyle _buttonStyle;

        GUIStyle _boxStyle;

        GUIStyle _listStyle;

        GUIContent _buttonContent;

        readonly GUIContent[] _listContent;

        int _selectedItemIndex = -1;

        PopupDirection _direction = PopupDirection.FromTopToBottom;

        bool _isClickedComboButton = false;

        public ComboBox(
            Rect rect,
            GUIContent[] listContent
            ) :
            this(
                rect,
                listContent,
                new GUIStyle("button"),
                new GUIStyle("box"),
                new GUIStyle("label")
                )
        {
        }

        public ComboBox(
            Rect rect,
            GUIContent[] listContent,
            GUIStyle buttonStyle,
            GUIStyle boxStyle,
            GUIStyle listStyle
            )
        {
            this.rect = rect;
            this._buttonContent = listContent[0];
            this._listContent = listContent;
            this._buttonStyle = buttonStyle;
            this._boxStyle = boxStyle;
            this._listStyle = listStyle;
        }

        public void Show()
        {
            if (_sForceToUnShow)
            {
                _sForceToUnShow = false;
                _isClickedComboButton = false;
            }

            bool done = false;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseUp:
                    {
                        if (_isClickedComboButton)
                        {
                            done = true;
                        }
                    }
                    break;
            }

            if (GUI.Button(rect, _buttonContent, _buttonStyle))
            {
                if (_sUseControlID == -1)
                {
                    _sUseControlID = controlID;
                    _isClickedComboButton = false;
                }

                if (_sUseControlID != controlID)
                {
                    _sForceToUnShow = true;
                    _sUseControlID = controlID;
                }

                _isClickedComboButton = true;
            }

            if (_isClickedComboButton)
            {
                var lineHeight = _listStyle.CalcHeight(_listContent[0], 1.0f);
                Rect listRect = new Rect(rect.x, rect.y + rect.height,
                         rect.width, lineHeight * _listContent.Length);

                if (_direction == PopupDirection.FromBottomToTop)
                {
                    listRect.y = rect.y - listRect.height;
                }

                GUI.Box(listRect, string.Empty, _boxStyle);
                int newSelectedItemIndex = GUI.SelectionGrid(listRect, _selectedItemIndex, _listContent, 1, _listStyle);
                if (newSelectedItemIndex != _selectedItemIndex)
                {
                    SetSelectedItemIndex(newSelectedItemIndex, _selectedItemIndex >= 0);
                }
            }

            if (done)
                _isClickedComboButton = false;
        }

        #region Parameters

        public PopupDirection Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                SetDirection(value);
            }
        }

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }

            set
            {
                SetSelectedItemIndex(value, false);
            }
        }

        public delegate void OnItemSelectedCallback(int index, bool userHasClicked);

        public OnItemSelectedCallback OnItemSelected;

        #endregion

        void SetSelectedItemIndex(int index, bool userHasClicked)
        {
            var itemIndexChanged = _selectedItemIndex != index;

            _selectedItemIndex = index;
            _buttonContent = _listContent[_selectedItemIndex];

            if (itemIndexChanged)
            {
                OnItemSelected?.Invoke(_selectedItemIndex, userHasClicked);
            }
        }

        void SetDirection(PopupDirection direction)
        {
            _direction = direction;
        }
    }

}
