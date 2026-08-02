using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities
{
    public class AnimatorEventsCollector : MonoBehaviour
    {
        public Subject<int> OnFootstep { get; private set;} = new();
        public Subject<int> OnAttackStart { get; private set;} = new();
        public Subject<int> OnAttackExecute { get; private set;} = new();
        public Subject<int> OnAttackFinish { get; private set;} = new();
        public Subject<Unit> OnEquipWeapon { get; private set;} = new();
        public Subject<Unit> OnDisarmWeapon { get; private set;} = new();

        public void OnStep(int legIndex)
        {
            OnFootstep?.OnNext(legIndex);
        }

        public void OnAttackStarted(int attackType)
        {
            OnAttackStart?.OnNext(attackType);
        }

        public void OnAttackExecuted(int attackType)
        {
            OnAttackExecute?.OnNext(attackType);
        }

        public void OnAttackFinished(int attackType)
        {
            OnAttackFinish?.OnNext(attackType);
        }

        public void OnEquip()
        {
            OnEquipWeapon?.OnNext(Unit.Default);
        }

        public void OnDisarm()
        {
            OnDisarmWeapon?.OnNext(Unit.Default);
        }
    }
}
