using DG.Tweening;
using Interfaces;
using Signals;
using UnityEngine;

namespace Abstracts
{
    public abstract class AStackable:MonoBehaviour,IStackable
    {
        public virtual void SetInit(Transform initTransform, Vector3 position)
        {

        }

        public virtual void SetVibration(bool isVibrate)
        {

        }

        public virtual void SetSound()
        {

        }

        public virtual void EmitParticle()
        {

        }

        public virtual void PlayAnimation()
        {

        }

        public abstract GameObject SendToStack();
        public virtual void SendPosition(Transform transform)
                {
                    DOVirtual.DelayedCall(0.1f, () => MoneyWorkerSignals.Instance.onSetMoneyPosition?.Invoke(transform));
                }
    }
}