using System.Collections.Generic;
using UnityEngine;

public static class TargetingTypes
{
    public class Target : TargetingSystem<Target.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasRange
            , TargetingConfigAspects.IHasRadius
        {
            public TargetingFlags Flags { get; set; }
            public float Range { get; set; }
            public float Radius { get; set; }
        }

        protected override void OnPlayerUpdate()
        {
            if (!MousePosChangedThisFrame)
            {
                return;
            }

            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool test = Physics.Raycast(ray, out RaycastHit hit, 50, BoardControl.Instance.LayerToken);
            AllowResolve = false;

            if (!test || !hit.collider.TryGetComponent(out Token token))
            {
                return;
            }

            if (!VerifyTarget(Context.User.Data, token.Data, Config.Flags))
            {
                return;
            }

            Context.SetTargetPosition(Config.Range == SkillDatabase.BASE_MELEE_RANGE ? token.transform.position : hit.point);
            AllowResolve = true;
        }
    }

    public class Line : TargetingSystem<Line.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasWidth
            , TargetingConfigAspects.IHasLength
        {
            public TargetingFlags Flags { get; set; }
            public float Width { get; set; }
            public float Length { get; set; }
        }

        private TargetingShape _shape;

        protected override void OnPlayerUpdate()
        {
            if (_shape == null)
            {
                _shape = CreateShape();
                _shape.InitLine(Config.Width, Config.Length);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            _shape.transform.position = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _shape.transform.forward = Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class Cone : TargetingSystem<Cone.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasWidth
            , TargetingConfigAspects.IHasLength
        {
            public TargetingFlags Flags { get; set; }
            public float Width { get; set; }
            public float Length { get; set; }
        }

        private TargetingShape _shape;

        protected override void OnPlayerUpdate()
        {
            if (_shape == null)
            {
                _shape = CreateShape();
                _shape.InitTriangle(Config.Width, Config.Length);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            _shape.transform.position = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _shape.transform.forward = Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class Circle : TargetingSystem<Circle.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasRange
            , TargetingConfigAspects.IHasRadius
        {
            public TargetingFlags Flags { get; set; }
            public float Range { get; set; }
            public float Radius { get; set; }
        }

        private TargetingShape _shape;
        
        protected override void OnPlayerUpdate()
        {
            if (_shape == null)
            {
                _shape = CreateShape();
                _shape.InitCircle(Config.Radius);

                if (Config.Range == 0)
                {
                    _shape.transform.position = Context.User.transform.position.SnapToEffectPlane();
                }
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            if (Config.Range > 0)
            {
                _shape.transform.position = Context.TargetPosition.SnapToEffectPlane();
            }

            AllowResolve = Config.Range == 0 || Context.TargetPosition.IsInArea(Context.User.transform.position, Config.Range);
            _shape.gameObject.SetActive(AllowResolve);
        }
    }

    public class Arc : TargetingSystem<Arc.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasRadius
        {
            public TargetingFlags Flags { get; set; }
            public float Angle { get; set; }
            public float Radius { get; set; }
        }

        public float CastingPointFactor => 0;

        private TargetingShape _shape;

        protected override void OnPlayerUpdate()
        {
            if (_shape == null)
            {
                _shape = CreateShape();
                _shape.InitArc(Config.Radius, Config.Angle);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            _shape.transform.position = Context.User.GetCastingPoint(Context.TargetPosition, CastingPointFactor).SnapToEffectPlane();
            _shape.transform.forward = Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class FrontBack : TargetingSystem<FrontBack.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasWidth
            , TargetingConfigAspects.IHasLength
        {
            public TargetingFlags Flags { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
        }

        private TargetingShape _frontShape;
        private TargetingShape _backShape;

        protected override void OnPlayerUpdate()
        {
            if (_frontShape == null)
            {
                _frontShape = CreateShape();
                _frontShape.InitLine(Config.Width, Config.Length);

                _backShape = CreateShape();
                _backShape.InitLine(Config.Width, Config.Length);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            _frontShape.transform.position = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _frontShape.transform.forward = Context.CastDirection;

            _backShape.transform.position = Context.User.GetFlankingPoint(Context.TargetPosition).SnapToEffectPlane();
            _backShape.transform.forward = -Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class LineSpawn : TargetingSystem<LineSpawn.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasLength
            , TargetingConfigAspects.IHasWidth
        {
            public TargetingFlags Flags { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
        }

        private TargetingShape _shape;
        private TargetingChevrons _chevrons;

        protected override void OnPlayerUpdate()
        {
            if (_shape == null)
            {
                _shape = CreateShape();
                _shape.InitCircle(Config.Width / 2);

                _chevrons = CreateChevrons();
                _chevrons.Init(Config.Width, Config.Length - (Config.Width / 2));
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            Vector3 castingPoint = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _shape.transform.position = castingPoint + Context.CastDirection * Config.Length;
            _chevrons.transform.position = castingPoint;
            _chevrons.transform.forward = Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class Ray : TargetingSystem<Ray.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasWidth
            , TargetingConfigAspects.IHasLength
        {
            public TargetingFlags Flags { get; set; }
            public float Width { get; set; }
            public float Length { get; set; } = 50;
            public int MaxBounces { get; set; }
            public int MaxTargets { get; set; } = 50;
            public bool Penetrate { get; set; }
            public float MinAngle { get; set; }
            public float MaxAngle { get; set; } = 180;
        }

        private TargetingRay _ray;

        protected override void OnPlayerUpdate()
        {
            if (_ray == null)
            {
                _ray = CreateRay();
                _ray.Init(Config);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            _ray.transform.position = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _ray.transform.forward = Context.CastDirection;

            AllowResolve = true;
        }
    }

    public class Rush : TargetingSystem<Rush.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasRange
        {
            public TargetingFlags Flags { get; set; }
            public float Range { get; set; }
            public bool Penetrate { get; set; }
        }

        private float UserWidth => Context.User.Size;

        private TargetingShape _userShape;
        private TargetingChevrons _chevrons;

        private List<Token> _collisions;
        private RaycastHit[] _hits;
        private float _length;

        protected override void OnPlayerUpdate()
        {
            if (_userShape == null)
            {
                _userShape = CreateShape(TargetingFlags.PRESET_OTHERS);
                _userShape.InitCircle(UserWidth);

                _chevrons = CreateChevrons();
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            Test(out _collisions, out Vector3 userPos, true);
            _userShape.transform.position = userPos.SnapToEffectPlane();
            bool shouldRenderChevrons = !userPos.IsInArea(Context.User.transform.position, 0.05f);

            if (shouldRenderChevrons)
            {
                _chevrons.transform.position = Context.User.transform.position.SnapToEffectPlane();
                _chevrons.transform.forward = Context.CastDirection;
                _chevrons.Init(UserWidth * 2, _length);
            }

            _chevrons.gameObject.SetActive(shouldRenderChevrons);
        }

        public void Test(out List<Token> tokens, out Vector3 userPos, bool preResolve)
        {
            tokens = _collisions ??= new List<Token>();
            UnityEngine.Ray ray = new UnityEngine.Ray(Context.User.transform.position, Context.CastDirection);

            _length = Mathf.Min(Context.TargetPosition.FlatDist(ray.origin), Config.Range);
            _collisions.Clear();

            if (Config.Penetrate)
            {
                _hits ??= new RaycastHit[10];
                int hitCount = Physics.SphereCastNonAlloc(ray, UserWidth, _hits, _length + 0.2f, BoardControl.Instance.LayerToken);

                for (int i = 0; i < hitCount; ++i)
                {
                    if (!_hits[i].collider.TryGetComponent(out Token token))
                    {
                        continue;
                    }

                    if (VerifyTarget(Context.User.Data, token.Data, Config.Flags))
                    {
                        _collisions.Add(token);
                    }
                }
            }
            else
            {
                bool didHit = Physics.SphereCast(ray, UserWidth, out RaycastHit hit, _length, BoardControl.Instance.LayerToken);

                if (didHit && hit.collider.TryGetComponent(out Token token))
                {
                    _length = hit.distance - 0.05f;

                    if (VerifyTarget(Context.User.Data, token.Data, Config.Flags))
                    {
                        _collisions.Add(token);
                    }
                }
            }

            userPos = Context.User.transform.position + Context.CastDirection * _length;

            if (preResolve)
            {
                AllowResolve = !_userShape.AnyCollisions;
            }
        }
    }

    public class Tumble : TargetingSystem<Tumble.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasWidth
            , TargetingConfigAspects.IHasRange
        {
            public TargetingFlags Flags { get; set; }
            public float Width { get; set; }
            public float Range { get; set; }
        }

        private float UserWidth => Context.User.Size;

        private TargetingShape _userShape;
        private TargetingShape _attackShape;

        protected override void OnPlayerUpdate()
        {
            if (_userShape == null)
            {
                _userShape = CreateShape(TargetingFlags.PRESET_OTHERS);
                _userShape.InitCircle(UserWidth);

                _attackShape = CreateShape();
                _attackShape.InitLine(Config.Width, Config.Range);
                _attackShape.IgnoreCollision(Context.User.Collider);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            Test(out _, out Vector3 userPos, true);
            _userShape.transform.position = userPos.SnapToEffectPlane();
            _attackShape.transform.position = Context.User.GetCastingPoint(Context.TargetPosition).SnapToEffectPlane();
            _attackShape.transform.forward = Context.CastDirection;
        }

        public void Test(out List<Token> tokens, out Vector3 userPos, bool preResolve)
        {
            Vector3 castingPoint = Context.User.GetCastingPoint(Context.TargetPosition);
            userPos = Context.User.GetCastingPoint(Context.TargetPosition) + Context.CastDirection * (Config.Range + UserWidth);

            if (preResolve)
            {
                tokens = _attackShape.GetCollisions();
                AllowResolve = !_userShape.AnyCollisions;
            }
            else
            {
                tokens = TargetingShape.TestLine(castingPoint, Context.CastDirection, Config.Width, Config.Range, Context.User.Data, Config.Flags);
            }
        }
    }

    public class Teleport : TargetingSystem<Teleport.TargetingConfig>
    {
        public class TargetingConfig : ITargetingConfig
            , TargetingConfigAspects.IHasRange
        {
            public TargetingFlags Flags { get; set; }
            public float Range { get; set; }
        }

        private float UserWidth => Context.User.Size;

        private TargetingShape _userShape;

        protected override void OnPlayerUpdate()
        {
            if (_userShape == null)
            {
                _userShape = CreateShape(TargetingFlags.PRESET_OTHERS);
                _userShape.InitCircle(UserWidth);
            }

            if (!MousePosChangedThisFrame)
            {
                return;
            }

            AllowResolve = Context.TargetPosition.IsInArea(Context.User.transform.position, Config.Range * Config.Range)
                && !_userShape.AnyCollisions;
            _userShape.transform.position = Context.TargetPosition.SnapToEffectPlane();
        }
    }

    /* TODO
     * - target spawn
     * - ray spawn
    */
}
