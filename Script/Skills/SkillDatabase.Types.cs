using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class SkillDatabase
{
    private static class SkillTypes
    {
        public class Target : Skill<TargetingTypes.Target, TargetingTypes.Target.TargetingConfig>
        {
            protected override IEnumerator OnPreResolve(SkillContext context, TargetingTypes.Target targeting)
            {
                BoardControl.Instance.TryGetToken(context.TargetPosition, out context.Target);
                yield return BoardControl.Instance.Walker.WalkSequence(context.Target, targeting.Config.Range);
            }

            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Target targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        ProcessTargets(context);
                    } break;
                }
            }
        }

        public class Line : Skill<TargetingTypes.Line, TargetingTypes.Line.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Line targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Line targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        Vector3 origin = context.User.GetCastingPoint(context.TargetPosition);
                        List<Token> tokens = TargetingShape.TestLine(origin, context.CastDirection, targeting.Config.Width, targeting.Config.Length, context.User.Data, targeting.Config.Flags);
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Cone : Skill<TargetingTypes.Cone, TargetingTypes.Cone.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Cone targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Cone targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        Vector3 origin = context.User.GetCastingPoint(context.TargetPosition);
                        List<Token> tokens = TargetingShape.TestCone(origin, context.CastDirection, targeting.Config.Width, targeting.Config.Length, context.User.Data, targeting.Config.Flags);
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Circle : Skill<TargetingTypes.Circle, TargetingTypes.Circle.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Circle targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Circle targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        Vector3 origin = targeting.Config.Range > 0 ? context.TargetPosition : context.User.transform.position;
                        List<Token> tokens = TargetingShape.TestCircle(origin, targeting.Config.Radius, context.User.Data, targeting.Config.Flags);
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Arc : Skill<TargetingTypes.Arc, TargetingTypes.Arc.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Arc targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Arc targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        Vector3 origin = context.User.GetCastingPoint(context.TargetPosition, targeting.CastingPointFactor);
                        List<Token> tokens = TargetingShape.TestArc(origin, context.CastDirection, targeting.Config.Radius, targeting.Config.Angle, context.User.Data, targeting.Config.Flags);
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class FrontBack : Skill<TargetingTypes.FrontBack, TargetingTypes.FrontBack.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.FrontBack targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.FrontBack targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.CastFront:
                    {
                        ResolveFront();
                    } break;

                    case TokenActionEvents.CastBack:
                    {
                        ResolveBack();
                    } break;

                    case TokenActionEvents.Cast:
                    {
                        ResolveFront();
                        ResolveBack();
                    } break;
                }

                return;

                void ResolveFront()
                {
                    Vector3 frontPoint = context.User.GetCastingPoint(context.TargetPosition);
                    ResolveDir(frontPoint, context.CastDirection);
                }

                void ResolveBack()
                {
                    Vector3 backPoint = context.User.GetFlankingPoint(context.TargetPosition);
                    ResolveDir(backPoint, -context.CastDirection);
                }

                void ResolveDir(Vector3 pos, Vector3 dir)
                {
                    List<Token> tokens = TargetingShape.TestLine(pos, dir, targeting.Config.Width, targeting.Config.Length, context.User.Data, targeting.Config.Flags);
                    ProcessTargets(context, tokens);
                }
            }
        }

        public class LineSpawn : Skill<TargetingTypes.LineSpawn, TargetingTypes.LineSpawn.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.LineSpawn targeting)
            {
                ICoroutineHandle effectHandle = null;

                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting, ref effectHandle),
                });

                yield return Wait.ForHandle(effectHandle);
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.LineSpawn targeting, ref ICoroutineHandle effectHandle)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        EffectArea_Basic effectArea = EffectAreaManager.Spawn(Id) as EffectArea_Basic;
                        effectArea.Init(context, ProcessTarget, targeting.Config.Width, targeting.Config.Flags);

                        Vector3 point = context.User.GetCastingPoint(context.TargetPosition);
                        Vector3 destination = point + context.CastDirection * targeting.Config.Length;

                        effectArea.transform.position = point;
                        effectHandle = Coroutines.Start(effectArea.MoveSequence(destination));
                    } break;
                }
            }
        }

        public class Ray : Skill<TargetingTypes.Ray, TargetingTypes.Ray.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Ray targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Ray targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        Vector3 origin = context.User.GetCastingPoint(context.TargetPosition);
                        List<Token> tokens = TargetingRay.Test(origin, context.CastDirection, targeting.Config, context.User.Data);
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Rush : Skill<TargetingTypes.Rush, TargetingTypes.Rush.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Rush targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Rush targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        targeting.Test(out List<Token> tokens, out Vector3 userPos, false);
                        context.User.transform.position = userPos.SnapToTokenPlane();
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Tumble : Skill<TargetingTypes.Tumble, TargetingTypes.Tumble.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Tumble targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Tumble targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        targeting.Test(out List<Token> tokens, out Vector3 userPos, false);
                        context.User.transform.position = userPos.SnapToTokenPlane();
                        ProcessTargets(context, tokens);
                    } break;
                }
            }
        }

        public class Teleport : Skill<TargetingTypes.Teleport, TargetingTypes.Teleport.TargetingConfig>
        {
            protected override IEnumerator OnResolve(SkillContext context, TargetingTypes.Teleport targeting)
            {
                yield return context.User.Model.PlayAction(new TokenPlayActionConfig
                {
                    Action = TokenActions.Attack1,
                    TargetPosition = context.TargetPosition,
                    OnActionEvent = @event => OnActionEvent(@event, context, targeting),
                });
            }

            private void OnActionEvent(TokenActionEvents @event, SkillContext context, TargetingTypes.Teleport targeting)
            {
                switch (@event)
                {
                    case TokenActionEvents.Cast:
                    {
                        context.User.transform.position = context.TargetPosition.SnapToTokenPlane();
                        ProcessTargets(context);
                    } break;
                }
            }
        }
    }
}
