using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XRProject.Boss
{
    public interface IActionListItem 
    {
        public Track ParentTrack { get; }
        public int Index { get; }
    }
    public class BeginTrack : IActionListItem
    {
        public Track ParentTrack { get; private set; }
        public Track Item { get; private set; }
        public int Index { get; private set; }
        public BeginTrack(Track parent, Track track, int index) 
        {
            this.ParentTrack = parent;
            this.Item = track;
            this.Index = index;
        }
    }

    public class EndTrack : IActionListItem
    {
        public Track ParentTrack { get; private set; }
        public Track Item { get; private set; }
        public int Index { get; private set; }
        public EndTrack(Track parent, Track track, int index) 
        {
            this.ParentTrack = parent;
            this.Item = track;
            this.Index = index;
        }
    }

    public class InAction : IActionListItem
    {
        public Track ParentTrack { get; private set; }
        public IAction Item { get; private set; }
        public int Index { get; private set; }

        public InAction(Track parent, IAction action, int index)
        {
            this.ParentTrack = parent;
            this.Item = action;
            this.Index = index;
        }
    }
    
    public class ActionList
    {
        public LinkedList<IActionListItem> _linkedList;
        private LinkedListNode<IActionListItem> _curNode;
        private LinkedListNode<IActionListItem> _curBeginNode;
        public bool Paused { get; set; } = false;
        private LinkedListNode<IActionListItem> _progressingNode;

        private void Inject(Track parent, Track track, int index)
        {
            _linkedList.AddLast(new BeginTrack(parent, track, index));
            for (int i = 0; i < track.ActionCount; i++)
            {
                var item = track[i];
                if (item is Track childTrack)
                    Inject(track, childTrack, i);
                else
                    _linkedList.AddLast(new InAction(track, item, i));
            }
            _linkedList.AddLast(new EndTrack(parent, track, index));
        }

        public void Create(Track track)
        {
            _linkedList = new LinkedList<IActionListItem>();
            Inject(null, track, 0);
            _curNode = _linkedList.First;

            while (true)
            {
                if (_curNode.Value is InAction)
                {
                    _curBeginNode = _curNode.Previous;
                    break;
                }

                _curNode = _curNode.Next;
            }
        }

        public bool IsActionEndedCurrentTrack
        {
            get
            {
                if (_progressingNode == null) return false;
                return _progressingNode.Value is EndTrack;
            }
        }
        public IActionListItem GetItemMoveNext()
        {
            if (_curNode == null) return null;
            
            var item = _curNode;
            _progressingNode = _curNode;
            _curNode = _curNode.Next;
            return item.Value;
        }
        public void NextTrack()
        {
            if (_curBeginNode == null) return;
            var current = _curBeginNode.Next;
            if (current == null) return;

            while (current.Value is not BeginTrack)
            {
                current = current.Next;
                if (current == null) return;
            }

            _curBeginNode = current;
            _curNode = _curBeginNode.Next;
        }
        public void GotoCursorOnBasedCurrentTrack(int index)
        {
            if (_curBeginNode == null) return;

            if (_curBeginNode.Value is BeginTrack beginTrack)
            {
                if (index >= beginTrack.Item.ActionCount || index < 0)
                {
                }
                else
                {
                    _curNode = _curBeginNode.Next;
                    for (int i = 0; i < index; i++)
                    {
                        _curNode = _curNode.Next;
                    }
                }
            }
        }
    }


    public class TrackPlayerComponent : MonoBehaviour
    {
        public void StartCo(Track track)
        {
            StartCoroutine(Co(track));
        }

        public ActionList ActionList { get; private set; } = new();

        private IEnumerator Co(Track track)
        {
            ActionList.Create(track);
            
            while (true)
            {
                if (ActionList.Paused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                
                IActionListItem item = ActionList.GetItemMoveNext();
                IEnumerator _context = null;

                if (item is InAction inAction)
                {
                    while (true)
                    {
                        //이벤트 연출 진행동안에는 보스의 행동을 수행하지 못하게 제한함 23/12/04 12:18 서범석
                        if (!TalkingEventManager.Instance._isEventEnd)
                        {
                            yield return new WaitForSeconds(Time.unscaledDeltaTime);
                            continue;
                        }
                        var temp = _context ?? inAction.Item.EValuate();
                        if (_context == null)
                        {
                            inAction.Item.Begin();
                            _context = temp;
                        }

                        bool pass = temp.MoveNext();

                        if (!pass)
                        {
                            inAction.Item.End();
                            inAction.Item.Predicate?.Process(ActionList);
                            inAction.ParentTrack.Predicate?.Process(ActionList);
                            break;
                        }
                        else
                        {
                            yield return temp.Current;
                        }
                    }
                }
                else if (item is BeginTrack beginTrack)
                {
                    beginTrack.Item.Predicate?.Process(ActionList);
                }
                else if (item is EndTrack endTrack)
                {
                    endTrack.Item.Predicate?.Process(ActionList);
                    yield return new WaitForEndOfFrame();
                    
                }
                else if(item == null)
                {
                    break;
                }
            }
        }
    }
    public class TrackPlayer
    {
        public Track CurrentTrack { get; private set; }
        public ActionList ActionList => _component.ActionList;

        private GameObject _playerObject;
        private TrackPlayerComponent _component;
        public TrackPlayer()
        {
            _playerObject = new GameObject("TrackPlayer");
            _component = _playerObject.AddComponent<TrackPlayerComponent>();
        }

        public void Play(Track track)
        {
            if (track == null)
            {
                Debug.LogError("track is null");
                return;
            }
            
            CurrentTrack = track;
            _component.StartCo(CurrentTrack);
        }
    }
}