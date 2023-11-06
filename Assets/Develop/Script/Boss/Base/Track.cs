using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public interface IAction
    {
        public void Begin();
        public void EValuate();
        public bool IsEnd();
        public Predicate Predicate { get; set; }
    }

    public delegate void Predicate(IAction target);
    public class Track : IAction
    {
        private List<IAction> _table = new();
        public Predicate Predicate { get; set; }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
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
            CurrentIndex = 0;
        }
        
        private bool _isActionBegin;
        public void EValuate()
        {
            if (IsEnded)
            {
                return;
            }
            
            IAction current = _table[_currentIndex];
            
            if(_isActionBegin == false)
            {
                current.Begin();
                _isActionBegin = true;
            }
            
            current.EValuate();
            
            if (current.IsEnd())
            {
                _currentIndex++;

                if (_currentIndex >= _table.Count)
                {
                    _currentIndex = -1;
                }
                
                _isActionBegin = false;
            }
            
            current.Predicate?.Invoke(this);
        }

        public bool IsEnd()
        {
            return IsEnded;
        }

        public IAction this[int index] => _table[index];
    }   
}
