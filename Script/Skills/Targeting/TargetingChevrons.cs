using System.Collections.Generic;
using UnityEngine;

public class TargetingChevrons : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite;

    private CharacterData _owner;
    private TargetingFlags _flags;

    private List<Token> _collisions;
    private BoxCollider _chevronsCollider;

    public void SetTargetingInfo(CharacterData owner, TargetingFlags flags)
    {
        _owner = owner;
        _flags = flags;
        _collisions = new List<Token>();
    }

    public void Init(float width, float length)
    {
        Debug.Assert(width > 0, $"[TargetingShape] InitChevrons - width was invalid: {width}");
        Debug.Assert(length > 0, $"[TargetingShape] InitChevrons - length was invalid: {length}");

        float halfWidth = width / 2;
        float halfLength = length / 2;
        Vector3 pos = new Vector3(0, 0, halfLength);

        _sprite.size = new Vector3(1, length);
        _sprite.transform.localScale = new Vector3(width / 2, 1, 1);
        _sprite.transform.localPosition = pos;

        if (_chevronsCollider == null)
        {
            _chevronsCollider = gameObject.AddComponent<BoxCollider>();
            BoardControl.Instance.TryGetToken(_owner.CharacterId, out var owner);
            Physics.IgnoreCollision(_chevronsCollider, owner.Collider);
        }

        _chevronsCollider.size = new Vector3(width, 1, length + halfWidth);
        _chevronsCollider.center = pos + new Vector3(0, 0, halfWidth / 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Token token))
        {
            return;
        }

        if (!TargetingSystem.VerifyTarget(_owner, token.Data, _flags))
        {
            return;
        }

        _collisions.Add(token);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Token token))
        {
            return;
        }

        _collisions.Remove(token);
    }
}
