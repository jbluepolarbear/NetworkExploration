/*using System.Collections;
using NetLib.Messages.Generated;
using NetLib.NetMath;
using JSL.NetTypes;
using NetLib.Unity.Client;
using UnityEngine;

namespace Systems
{
    public class ClientInputSystem: NetClientSystemBehaviour<ClientInputSystem>
    {
        public override int Priority { get; } = 10;
        public override IEnumerator NetStart()
        {
            yield break;
        }

        public override void NetDestroy()
        {
        }

        public override void NetUpdate()
        {
            if (!_dirty)
            {
                return;
            }

            if (_released)
            {
                using var netMessage = MemoryManager.MessagePool.Get(ChargeReleaseStateMessage.ClassId);
                var releaseState = (ChargeReleaseStateMessage) netMessage.Message;
                releaseState.SelectedActor = _selectedActor;
                releaseState.Axis.X = _power.x;
                releaseState.Axis.Y = 0.0f;
                releaseState.Axis.Z = _power.y;
                _released = false;
            }
            else
            {
                using var netMessage = MemoryManager.MessagePool.Get(ChargeStateMessage.ClassId);
                var releaseState = (ChargeStateMessage) netMessage.Message;
                releaseState.SelectedActor = _selectedActor;
                releaseState.XAxis.Value = _power.x;
                releaseState.YAxis.Value = _power.y;
            }
        }

        public void SetSelectedActor(uint actorId)
        {
            if (_selectedActor == actorId)
            {
                return;
            }
            _selectedActor = actorId;
            _dirty = true;
        }

        public void SetPower(float x, float y)
        {
            if (Mathf.Abs(x - _power.x) <= NetMath.Epsilon &&
                Mathf.Abs(y - _power.y) <= NetMath.Epsilon)
            {
                return;
            }

            _power.x = x;
            _power.y = y;
            _dirty = true;
        }

        public void Release(float x, float y)
        {
            _power.x = x;
            _power.y = y;
            _released = true;
            _dirty = true;
        }

        public override bool NetHandles(NetMessage netMessage)
        {
            return netMessage.Message.TypeId == ChargeStateMessage.ClassId ||
                   netMessage.Message.TypeId == ChargeReleaseStateMessage.ClassId;
        }

        public override void NetHandle(NetMessage netMessage)
        {
            switch (netMessage.Message.TypeId)
            {
                case ChargeStateMessage.ClassId:
                    break;
                case ChargeReleaseStateMessage.ClassId:
                    break;
            }
        }

        private bool _dirty;
        private uint _selectedActor;
        private bool _released = false;
        private Vector2 _power = Vector2.zero;
    }
}*/