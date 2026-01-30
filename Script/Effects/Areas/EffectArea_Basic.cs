using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectArea_Basic : EffectArea
{
    private class MoveOrder
    {
        public Vector3 Start;
        public Vector3 Direction;
        public float Speed;
        public float TotalDistance;
        public float Progress;
    }

    [SerializeField] float _speed;

    private SkillContext _skillContext;
    private Skill.ProcessTargetHandler _processTarget;
    private TargetingFlags _targetingFlags;
    private TickTiming _tickTiming;

    private MoveOrder _move;
    private List<Token> _overlappingTokens;

    public void Init(SkillContext context, Skill.ProcessTargetHandler processTarget, float size,
        TargetingFlags targetingFlags, TickTiming tickTiming = TickTiming.EndOfTurn)
    {
        _skillContext = context;
        _processTarget = processTarget;
        _targetingFlags = targetingFlags;
        _tickTiming = tickTiming;

        if (TryGetComponent(out CapsuleCollider capsule))
        {
            capsule.radius = size / 2;
        }
        else if (TryGetComponent(out BoxCollider box))
        {
            box.size = Vector3.one * (size / 2);
        }

        // temp until rounds are implemented
        Destroy(this.gameObject, 8);
    }

    public void SetDestination(Vector3 destination, float speed)
    {
        Vector3 diff = destination - transform.position;
        float distance = diff.magnitude;

        _move = new MoveOrder
        {
            Start = transform.position,
            Direction = diff.normalized,
            Speed = speed,
            TotalDistance = distance,
        };

        transform.LookAt(destination, Vector3.up);

        AffectOverlappingTokens();
    }

    public IEnumerator MoveSequence(Vector3 destination)
    {
        SetDestination(destination, _speed);
        yield return Wait.Until(() => _move == null);
    }

    public void Tick(TickTiming timing)
    {
        if (_tickTiming == timing)
        {
            AffectOverlappingTokens();
        }
    }

    private void FixedUpdate()
    {
        if (_move == null)
        {
            return;
        }

        _move.Progress = Mathf.Clamp(_move.Progress + (Time.fixedDeltaTime * _move.Speed), 0, _move.TotalDistance);
        transform.position = _move.Start + (_move.Direction * _move.Progress);

        if (_move.Progress >= _move.TotalDistance)
        {
            _move = null;
        }
    }

    private void AffectOverlappingTokens()
    {
        for (int i = 0; i < _overlappingTokens?.Count; ++i)
        {
            _skillContext.Target = _overlappingTokens[i];
            _processTarget?.Invoke(_skillContext);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Token>(out var token))
        {
            return;
        }

        if (!TargetingSystem.VerifyTarget(_skillContext.User.Data, token.Data, _targetingFlags))
        {
            return;
        }

        _overlappingTokens ??= new List<Token>();
        _overlappingTokens.Add(token);

        _skillContext.Target = token;
        _processTarget?.Invoke(_skillContext);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Token>(out var token))
        {
            return;
        }

        if (!TargetingSystem.VerifyTarget(_skillContext.User.Data, token.Data, _targetingFlags))
        {
            return;
        }

        _overlappingTokens?.Remove(token);
    }
}
