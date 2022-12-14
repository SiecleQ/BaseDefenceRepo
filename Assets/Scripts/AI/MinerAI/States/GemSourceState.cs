using AI.MinerAI;
using Enum;
using Managers;
using UnityEngine;

namespace AI.States
{
    public class GemSourceState:IState
    {
        private HostageManager _hostageManager;
        public bool IsMiningTimeUp=>_timer>=_minerAIBrain.GemCollectionOffset;
        private MinerAIBrain _minerAIBrain;
        private MinerItems _minerItems;
        private MinerAnimationStates _minerAnimationState;
        private float _timer;
        public GemSourceState(MinerAIBrain minerAIBrain, HostageManager hostageManager,
            MinerAnimationStates minerAnimationState, MinerItems minerItems)
        {
            _minerAIBrain = minerAIBrain;
            _minerItems = minerItems;
            _hostageManager = hostageManager;
            _minerAnimationState = minerAnimationState;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
        }

        public void OnEnter()
        {
        _minerAIBrain.MinerAIItemController.OpenItem(_minerItems);
        _hostageManager.ChangeAnimation(_minerAnimationState);
        
        }

        public void OnExit()
        {
            ResetTimer();
            _minerAIBrain.MinerAIItemController.CloseItem(_minerItems);
            _minerAIBrain.SetTargetForGemHolder();
        }

        private void ResetTimer()
        {
            _timer = 0;
        }
    }
}