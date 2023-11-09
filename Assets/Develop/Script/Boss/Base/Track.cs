using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public interface IAction
    {
        public void Begin();
        public IEnumerator EValuate();
        public bool IsEnd();
        public ITrackPredicate Predicate { get; set; }
    }
    public class Track : IAction
    {
        private List<IAction> _table = new();
        public ITrackPredicate Predicate { get; set; }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                _context = null;
                if (_currentIndex >= ActionCount)
                {
                    Debug.LogError("Track: current index >= ActionCount");
                    _currentIndex = -1;
                }
                if (_currentIndex < -1)
                {
                    Debug.LogError("Track: current index < -1");
                    _currentIndex = -1;
                }
            }
        }
        public bool IsEnded => CurrentIndex == -1;
        public int ActionCount => _table.Count;

        public Track AddAction(IAction action)
        {
            _table.Add(action);
            return this;
        }
        public void Begin()
        {
            _context = null;
            _currentIndex = 0;
        }
        
        private bool _isActionBegin;
        private IEnumerator _context;
        public IEnumerator EValuate()
        {
            yield break;
        }

        public bool IsEnd()
        {
            return IsEnded;
        }

        public IAction this[int index] => _table[index];
    }
}
