using System.Collections.Generic;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Battle
{
    public class BattleGrid
    {
        public List<GridSlot> Slots { get; }

        private BattleGridView _view;

        public BattleGrid(BattleGridView view)
        {
            _view = view;

            Slots = GetGridSlots();
        }

        private List<GridSlot> GetGridSlots()
        {
            var slots = new List<GridSlot>();

            var angleStep = 360f / _view.SlotAmount;
            var startRad = 0f;

            for (int i = 0; i < _view.SlotAmount; i++)
            {
                var currentAngle = startRad + i * angleStep * Mathf.Deg2Rad;
                var cos = Mathf.Cos(currentAngle);
                var sin = Mathf.Sin(currentAngle);

                Vector3 closePos = _view.transform.position + new Vector3(cos * _view.CloseRadius, 0, sin * _view.CloseRadius);
                Vector3 farPos = _view.transform.position + new Vector3(cos * _view.FarRadius, 0, sin * _view.FarRadius);

                slots.Add(CreateGridSlot(i, closePos, farPos));
            }

            return slots;
        }

        private GridSlot CreateGridSlot(int i, Vector3 closePos, Vector3 farPos)
        {
            GameObject slotGO = new GameObject($"Slot_{i}");
            slotGO.transform.SetParent(_view.transform);

            return new GridSlot(CreateSlotPoint("ClosePoint", closePos, slotGO.transform), CreateSlotPoint("FarPoint", farPos, slotGO.transform));
        }

        private Transform CreateSlotPoint(string gameObjectName, Vector3 pos, Transform parentTransform)
        {
            GameObject point = new GameObject(gameObjectName);
            point.transform.position = pos;
            point.transform.SetParent(parentTransform);

            return point.transform;
        }
    }
}
