using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateMachines.AIBrain.Workers.MoneyStates;
using Sirenix.OdinInspector;
using Enums;
using Controllers;
using Data.UnityObject;
using Data.ValueObject.AIDatas;
using Interfaces;
using System;
using AI;
using Signals;
namespace StateMachines.AIBrain.Workers
{
    public class MoneyWorkerAIBrain : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        [BoxGroup("Public Variables")]
        public Transform CurrentTarget;

        #endregion

        #region Serilizable Variables

        [BoxGroup("Serializable Variables")]
        [SerializeField]
        private WorkerType workerType;
        [BoxGroup("Serializable Variables")]
        [SerializeField]
        private MoneyWorkerPhysicController moneyWorkerDetector;

        #endregion

        #region Private Variables

        [ShowInInspector]
        private WorkerAITypeData _workerTypeData;
        private Animator _animator;
        private NavMeshAgent _navmeshAgent;

        #region States

        private MoveToGateState _moveToGateState;
        private SearchState _searchState;
        private WaitOnGateState _waitOnGateState;
        private StackMoneyState _stackMoneyState;
        private DropMoneyOnGateState _dropMoneyOnGateState;
        private StateMachine _stateMachine;

        #endregion

        #region Worker Game Variables
        [ShowInInspector]
        private int _currentStock = 0;
        private float _delay = 0.05f;

        #endregion

        #endregion

        #endregion

        private void Awake()
        {
            _workerTypeData = GetWorkerType();
            SetWorkerComponentVariables();
            InitWorker();
            GetReferenceStates();
        }

        /*#region Event Subscriptions

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EnemySignals.Instance.onEnemyDead += OnGetEnemyPositon;
        }
        private void UnsubscribeEvents()
        {
            EnemySignals.Instance.onEnemyDead -= OnGetEnemyPositon;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        private void OnGetEnemyPositon(Transform pos)
        {
            moneyTargetList.Add(pos);
        }

        #endregion*/


        #region Data Jobs

        private WorkerAITypeData GetWorkerType()
        {
            return MoneyWorkerSignals.Instance.onGetMoneyAIData?.Invoke(workerType);
        }

        private void SetWorkerComponentVariables()
        {
            _navmeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }
        #endregion

        #region Worker State Jobs

        private void GetReferenceStates()
        {
            _searchState = new SearchState(_navmeshAgent, _animator, this);
            _moveToGateState = new MoveToGateState(_navmeshAgent, _animator, ref _workerTypeData.StartTarget);
            _waitOnGateState = new WaitOnGateState(_navmeshAgent, _animator, this);
            _stackMoneyState = new StackMoneyState(_navmeshAgent, _animator, this);
            _dropMoneyOnGateState = new DropMoneyOnGateState(_navmeshAgent, _animator, ref _workerTypeData.StartTarget);

            _stateMachine = new StateMachine();

            At(_moveToGateState, _searchState, HasArrive());
            At(_searchState, _stackMoneyState, HasCurrentTargetMoney());
            At(_stackMoneyState, _searchState, _stackMoneyState.IsArriveToMoney());
            At(_stackMoneyState, _dropMoneyOnGateState, HasCapacityFull());
            At(_dropMoneyOnGateState, _searchState, HasCapacityNotFull());

            _stateMachine.SetState(_moveToGateState);
            void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

            Func<bool> HasArrive() => () => _moveToGateState.IsArrive;
            Func<bool> HasCurrentTargetMoney() => () => CurrentTarget != null;
            Func<bool> HasCapacityFull() => () => !IsAvailable();
            Func<bool> HasCapacityNotFull() => () => IsAvailable();
        }

        private void Update() => _stateMachine.Tick();

        #endregion

        #region General Jobs
        private void InitWorker()
        {

        }
        public bool IsAvailable() => _currentStock < _workerTypeData.CapacityOrDamage;

        public void SetDest()
        {
            CurrentTarget = GetMoneyPosition();
            if (CurrentTarget)
                _navmeshAgent.SetDestination(CurrentTarget.position);
        }

        public Transform GetMoneyPosition()
        {
            return MoneyWorkerSignals.Instance.onGetTransformMoney?.Invoke(this.transform);
        }

        private IEnumerator SearchTarget()
        {
            while (!CurrentTarget)
            {
                SetDest();
                yield return new WaitForSeconds(_delay);
            }
        }
        public void StartSearch(bool isStartedSearch)
        {
            if(isStartedSearch)
                StartCoroutine(SearchTarget());
            else
            {
                StopCoroutine(SearchTarget());
            }
        }

        public void SetCurrentStock()
        {
            if (_currentStock < _workerTypeData.CapacityOrDamage)
                _currentStock++;
        }

        public void RemoveAllStock()
        {
            for (int i = 0; i < _workerTypeData.CapacityOrDamage; i++)
            {
                if (_currentStock > 0)
                    _currentStock--;
                else
                    _currentStock = 0;
            }

            //remove all stack
        }

        #endregion

    }
}