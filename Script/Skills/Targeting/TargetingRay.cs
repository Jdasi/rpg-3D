using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TargetingRay : MonoBehaviour
{
    [SerializeField] LineRenderer _line;

    private CharacterData _owner;
    private TargetingTypes.Ray.TargetingConfig _config;
    private TargetingFlags? _targetingFlags;

    private List<Token> _collisions;
    private List<Vector3> _linePoints;
    private RaycastHit[] _hits;

    public void SetTargetingInfo(CharacterData owner, TargetingFlags? overrideFlags = null)
    {
        _owner = owner;
        _collisions = new List<Token>();
        _targetingFlags ??= overrideFlags;
    }

    public void Init(TargetingTypes.Ray.TargetingConfig config)
    {
        _config = config;
        _targetingFlags ??= config.Flags;
        _hits = new RaycastHit[2 + config.MaxBounces]; // minimum user & first target
        transform.hasChanged = true;

        if (_line != null)
        {
            _line.enabled = false;
            _line.startWidth = _line.endWidth = Mathf.Max(0.1f, config.Width);
            _linePoints = new List<Vector3>(2 + config.MaxBounces);
        }
    }

    private void FixedUpdate()
    {
        if (!transform.hasChanged)
        {
            return;
        }

        transform.hasChanged = false;

        LayerMask hitLayers = BoardControl.Instance.LayerWall | BoardControl.Instance.LayerToken;
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;

        _collisions.Clear();
        _linePoints?.Clear();
        _linePoints?.Add(start);

        CharacterId lastCharacterHit = _owner.CharacterId;
        int remainingRays = 1 + _config.MaxBounces;
        float remainingLength = _config.Length;

        while (remainingRays > 0 && remainingLength > 0 && _collisions.Count < _config.MaxTargets)
        {
            Ray ray = new Ray(start, direction);
            float hitDistance = 0;

            int hitCount = _config.Width > 0
                ? Physics.SphereCastNonAlloc(ray, _config.Width, _hits, remainingLength, hitLayers)
                : Physics.RaycastNonAlloc(ray, _hits, remainingLength, hitLayers);

            Array.Sort(_hits, 0, hitCount, JComparer.RaycastHitClosest);

            for (int i = 0; i < hitCount; ++i)
            {
                ref RaycastHit hit = ref _hits[i];

                if (hit.transform.gameObject.layer == BoardControl.Instance.LayerWall)
                {
                    hitDistance = hit.distance;
                    _linePoints?.Add(hit.point);
                    Reflect(ref start, ref direction, ref hit, ref remainingRays);
                    break;
                }

                if (!hit.collider.TryGetComponent(out Token token))
                {
                    continue;
                }

                if (lastCharacterHit == token.Data.CharacterId)
                {
                    continue;
                }

                if (!TargetingSystem.VerifyTarget(_owner, token.Data, _targetingFlags.Value))
                {
                    continue;
                }

                hitDistance = hit.distance;
                lastCharacterHit = token.Data.CharacterId;

                _linePoints?.Add(hit.point);
                _collisions.Add(token);

                if (!_config.Penetrate)
                {
                    Reflect(ref start, ref direction, ref hit, ref remainingRays);
                    break;
                }
            }

            bool hitSomething = hitDistance > 0;

            if (!hitSomething)
            {
                _linePoints?.Add(start + direction * remainingLength);
                remainingLength = 0;
                break;
            }

            remainingLength -= hitDistance;
        }

        if (_line != null)
        {
            _line.positionCount = _linePoints.Count;
            _line.enabled = true;

            for (int i = 0; i < _linePoints.Count; ++i)
            {
                _line.SetPosition(i, _linePoints[i]);
            }
        }

        return;

        void Reflect(ref Vector3 start, ref Vector3 direction, ref RaycastHit hit, ref int remainingRays)
        {
            start = hit.point;

            Vector3 prevDirection = direction;
            direction = Vector3.Reflect(direction, hit.normal);

            if (_config.MinAngle > 0 || _config.MaxAngle < 180)
            {
                float angle = Vector3.Angle(prevDirection, direction);
                if (angle < _config.MinAngle || angle > _config.MaxAngle)
                {
                    remainingRays = 0;
                    return;
                }
            }

            --remainingRays;
        }
    }
}
