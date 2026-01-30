using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : Selectable, IPointerClickHandler
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [Header("Effects")]
    [SerializeField] Color _baseColor = Color.white;
    [SerializeField] Color _hoverColor = Color.grey;
    [SerializeField] Color _pressedColor = Color.cyan;

    [Header("References")]
    [SerializeField] Image _image;
    [SerializeField] Text _text;

    private SkillIds _id;

    public void Init(SkillIds id)
    {
        _id = id;
        _text.text = id.ToString().SpaceOut(); // TODO - name lookup
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (_image == null)
        {
            return;
        }

        Color target = _baseColor;

        switch (state)
        {
            case SelectionState.Normal:
                target = _baseColor;
                break;

            case SelectionState.Highlighted:
                target = _hoverColor;

                break;
            case SelectionState.Pressed:
                target = _pressedColor;
                break;

            case SelectionState.Disabled:
                target = Color.black; // or any disabled color
                break;
        }

        _image.color = target;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BoardControl.Instance.SetSkill(_id);
    }
}
