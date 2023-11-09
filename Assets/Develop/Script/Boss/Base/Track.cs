using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public interface IAction
    {
        public void Begin();
        public void End();
        public IEnumerator EValuate();
        public ITrackPredicate Predicate { get; set; }
    }
    public class Track : IAction
    {
        private List<IAction> _table = new();
        public ITrackPredicate Predicate { get; set; }
        public int ActionCount => _table.Count;

        public Track AddAction(IAction action)
        {
            _table.Add(action);
            return this;
        }
        public void Begin()
        {
        }

        public void End()
        {
        }
        public IEnumerator EValuate()
        {
            yield break;
        }

        public IAction this[int index] => _table[index];
    }
}
