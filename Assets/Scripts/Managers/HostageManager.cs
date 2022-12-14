using System;
using AI.MinerAI;
using Controllers;
using Data.UnityObjects;
using Data.ValueObjects;
using Enum;
using Signals;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class HostageManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        public MinerAIBrain minerAIBrain;
        
        #endregion

        #region Serialized Variables
        
        public HostageType CurrentType=HostageType.HostageWaiting; 
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject helpTexture;


        #endregion

        #region Private Variables

        private int _currentLevel; //LevelManager uzerinden cekilecek
        private Transform _currentMinePlace;


        #endregion

        #endregion
        private void Awake()
        {
            //_currentMinePlace=mineBaseManager.GetRandomMineTarget();
        }

        #region Event Subscription

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            DropzoneSignals.Instance.onDropZoneFull += OnDropZoneFull;
        }

        private void OnDropZoneFull(bool _state)
        {
            minerAIBrain.IsDropZoneFullStatus=_state;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            DropzoneSignals.Instance.onDropZoneFull -= OnDropZoneFull;
        }

        #endregion

        public void ChangeAnimation(MinerAnimationStates minerAnimationStates)
        {
            
            animator.SetTrigger(minerAnimationStates.ToString());
        }

        public void AddToHostageStack()
        {
            CurrentType = HostageType.Hostage;
            helpTexture.SetActive(false);
            HostageSignals.Instance.onAddHostageStack?.Invoke(this);
        }

        
    }
}