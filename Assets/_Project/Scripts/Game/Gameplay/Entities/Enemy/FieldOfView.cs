using System;
using UnityEngine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class FieldOfView : IDisposable
    {
        public Observable<bool> CanSeeTarget => _canSeeTarget;
        private ReactiveProperty<bool> _canSeeTarget;

        private FieldOfViewParameters _parameters;
        private Transform _target;

        private IDisposable _updateDisposable;

        public FieldOfView(FieldOfViewParameters parameters, Transform target)
        {
            _parameters = parameters;
            _target = target;

            _canSeeTarget = new(false);

            _updateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                var canSeeTarget = IsTargetVisible();

                if (_canSeeTarget.Value != canSeeTarget) _canSeeTarget.Value = canSeeTarget;
            });
        }

        public void Dispose()
        {
            _updateDisposable?.Dispose();
        }

        private bool IsTargetVisible()
        {
            if (_target == null) return false;

            Vector3 eyePos = _parameters.EyePosition;
            Vector3 targetPoint = _target.position;
            Vector3 toTarget = targetPoint - eyePos;

            float radius = _parameters.Radius;
            
            if (toTarget.sqrMagnitude > radius * radius) return false;

            if (toTarget.sqrMagnitude > 0.000001f)
            {
                float angle = Vector3.Angle(_parameters.transform.parent.forward, toTarget);
                if (angle > _parameters.Angle * 0.5f) return false;
            }

            if (Physics.Linecast(eyePos, targetPoint, Root.LayerMasks.ViewObstacle, QueryTriggerInteraction.Ignore)) return false;

            return true;
        }

        
    }
}
