using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XRProject.Boss
{
    public interface IActionListItem 
    {
        public Track Parent { get; }
        public int Index { get; }
    }
    public class BeginTrack : IActionListItem
    {
        public Track Parent { get; private set; }
        public Track Item { get; private set; }
        public int Index { get; private set; }
        public BeginTrack(Track parent, Track track, int index) 
        {
            this.Parent = parent;
            this.Item = track;
            this.Index = index;
        }
    }

    public class EndTrack : IActionListItem
    {
        public Track Parent { get; private set; }
        public Track Item { get; private set; }
        public int Index { get; private set; }
        public EndTrack(Track parent, Track track, int index) 
        {
            this.Parent = parent;
            this.Item = track;
            this.Index = index;
        }
    }

    public class InAction : IActionListItem
    {
        public Track Parent { get; private set; }
        public IAction Item { get; private set; }
        public int Index { get; private set; }

        public InAction(Track parent, IAction action, int index)
        {
            this.Parent = parent;
            this.Item = action;
            this.Index = index;
        }
    }
    
    public class ActionList
    {
        public LinkedList<IActionListItem> _linkedList;
        private LinkedListNode<IActionListItem> _curNode;
        private LinkedListNode<IActionListItem> _curBeginNode;

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
                if (_curNode == null)
                    break;
            }
        }

        public IActionListItem GetItemMoveNext()
        {
            if (_curNode == null) return null;
            
            var item = _curNode;
            _curNode = _curNode.Next;
            return item.Value;
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

        private ActionList _list = new ActionList();

        private IEnumerator Co(Track track)
        {
            _list.Create(track);

            while (true)
            {
                IActionListItem item = _list.GetItemMoveNext();
                IEnumerator _context = null;

                if (item is InAction inAction)
                {
                    while (true)
                    {
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
                            inAction.Item.Predicate?.Process(_list, inAction.Parent, inAction.Index);
                            break;
                        }
                        else
                        {
                            yield return temp.Current;
                        }
                    }
                }
                else if (item is EndTrack endTrack)
                {
                    endTrack.Item.Predicate?.Process(_list, endTrack.Item,  -1);
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