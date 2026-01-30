using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BoardWalker : MonoBehaviour
{
    [SerializeField] NavMeshAgent _nav;

    private Token _token;
    private bool _isWalking;

    private Vector3[] _corners;
    private Vector3 _curr;
    private Vector3 _next;

    private float _distToNext;
    private float _progressToNext;
    private int _pathIndex;

    public bool IsCarrying()
    {
        return _token != null;
    }

    public bool IsCarrying(Token token)
    {
        return _token == token;
    }

    public void Carry(Token token)
    {
        Drop();

        _token = token;
        _nav.radius = _token.Model.NavRadius;
        this.enabled = true;
    }

    public void Drop()
    {
        if (_token == null)
        {
            return;
        }

        _token = null;
        _corners = null;
        this.enabled = false;
    }

    public bool IsWalking()
    {
        return _isWalking;
    }

    public IEnumerator WalkSequence(Vector3 position, float radius = 0)
    {
        Debug.Assert(IsCarrying(), "[BoardWalker] SetDestination - nothing being carried");
        Debug.Assert(!_isWalking, "[BoardWalker] SetDestination - already walking");

        float dist = transform.position.FlatDist(position);

        if (dist <= radius)
        {
            yield break;
        }

        transform.position = _token.transform.position;
        _nav.enabled = true;
        _nav.destination = position;
        _nav.stoppingDistance = radius;
        _isWalking = true;

        yield return Wait.Until(() => !_isWalking);
        _nav.enabled = false;

        if (Coroutines.IsCurrentCancelled())
        {
            _isWalking = false;
            ResetPathData();
        }
    }

    public IEnumerator WalkSequence(Token target, float radius)
    {
        if (radius == SkillDatabase.BASE_MELEE_RANGE)
        {
            // hacky fix for shuffling when in melee
            radius = target.Model.NavRadius / 10;
        }

        if (_token.Distance(target) <= radius)
        {
            yield break;
        }

        Vector3 direction = _token.transform.position.FlatDir(target.transform.position);
        Vector3 position = target.transform.position - direction * target.Size;

        yield return WalkSequence(position, radius);
    }

    private void Update()
    {
        if (!_isWalking)
        {
            return;
        }

        if (_nav.pathPending)
        {
            return;
        }

        if (_nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            _isWalking = false;
        }
        else
        {
            Walk();
        }

        if (_isWalking)
        {
            return;
        }

        ResetPathData();
    }

    private void ResetPathData()
    {
        _nav.ResetPath();
        _corners = null;
        _curr = default;
        _next = default;
        _distToNext = 0;
        _progressToNext = 0;
        _pathIndex = 0;
    }

    private void Walk()
    {
        if (_corners == null)
        {
            _corners = _nav.path.corners;
        }

        if (_progressToNext >= _distToNext)
        {
            if (_pathIndex + 1 == _corners.Length)
            {
                _isWalking = false;

                return;
            }

            NextWaypoint();
        }

        bool isLastIndex = _pathIndex + 1 == _corners.Length;
        float maxProgress = isLastIndex ? _distToNext - _nav.stoppingDistance : _distToNext;

        _progressToNext = Mathf.Clamp(_progressToNext + (_token.Model.MoveSpeed * Time.deltaTime), 0, maxProgress);
        Vector3 step = Vector3.Lerp(_curr, _next, _progressToNext / _distToNext);

        if (float.IsNaN(step.x) || float.IsNaN(step.y) || float.IsNaN(step.z))
        {
            _isWalking = false;
            return;
        }

        transform.position = step;
        _token.transform.position = step.SnapToTokenPlane();

        if (isLastIndex && _progressToNext == maxProgress)
        {
            // special case for stopping short
            _progressToNext = _distToNext;
        }

        return;

        void NextWaypoint()
        {
            ++_pathIndex;
            _progressToNext = 0;

            _curr = transform.position;
            _next = _corners[_pathIndex];
            _next = new Vector3(_next.x, _curr.y, _next.z);

            Vector3 diff = _next - _curr;
            _distToNext = diff.magnitude;
            _token.Model.transform.forward = diff;
        }
    }
}
