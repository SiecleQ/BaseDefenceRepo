using System;
using System.Collections.Generic;
using AI;
using AI.EnemyAI;
using AI.States;
using Controllers;
using Data.UnityObjects;
using Data.ValueObjects;
using Data.ValueObjects.AiData;
using Data.ValueObjects.AiData.EnemyData;
using Enum;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AIBrains.EnemyBrain
{
    public class EnemyAIBrain : MonoBehaviour
    {

        #region Self Variables

        #region Public Variables
        public Transform TurretTarget;
        public int Health;
        public Transform MineTarget;
        public Transform PlayerTarget;
        public PoolObjectType EnemyType;
        public Transform SpawnPosition;

        #endregion

        #region Serialized Variables

        [SerializeField]
        private Animator animator;

        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private EnemyDamager enemyDamager;
        

        #endregion

        #region Private Variables
        private StateMachine _stateMachine;
        
        private float attackRange;
        private int damage;
        
        private float chaseSpeed;
        private float moveSpeed;
        private bool IsBombCantFind=>MineTarget!=null;
        private EnemyAIData EnemyAIData;
        private EnemyTypeData EnemyTypeData;
        public EnemyPhysicsController enemyPhysicsController;

        #endregion

        #endregion
        
       

        private void Awake()
        {
            //_levelID = LevelSignals.Instance.onGetLevel();//LevelIDsi turretler icin
            EnemyAIData=GetEnemyAIData();
            EnemyTypeData = GetEnemyType();
            TurretTarget = GetCurrentTurret();
            SendDataToControllers();
            
        }

        private void Start()
        {
            SetEnemyData();
            GetStatesReferences();
        }

        private void SendDataToControllers()
        {
            enemyDamager.SetEnemyData(GetEnemyType());
        }
        private Transform GetCurrentTurret()
        {
            return EnemyAIData.TargetList[Random.Range(0, EnemyAIData.TargetList.Count)];
        }

        private EnemyTypeData GetEnemyType()
        {
            return EnemyAIData.EnemyList[(int)EnemyType];
        }
        private void SetEnemyData()
        {
            damage=EnemyTypeData.Damage;
            Health=EnemyTypeData.Health;
            attackRange=EnemyTypeData.AttackRange;
            chaseSpeed=EnemyTypeData.ChaseSpeed;
            moveSpeed=EnemyTypeData.Speed;
            SpawnPosition = EnemyTypeData.SpawnPosition;
        }

        private EnemyAIData GetEnemyAIData()=>Resources.Load<CD_AI>("Data/CD_AI").EnemyAIData;

        private void GetStatesReferences()
        {
            var navmeshAgent = GetComponent<NavMeshAgent>();
            var search = new SearchState(this,navmeshAgent,SpawnPosition);
            var attack = new AttackState(navmeshAgent, animator,this,attackRange);
            var move = new EnemyMoveState(this,navmeshAgent, animator,moveSpeed);
            var death = new DeathState(this,navmeshAgent, animator,skinnedMeshRenderer);
            var chase = new ChaseState(navmeshAgent, animator,this,attackRange,chaseSpeed);
            var moveToBomb = new MoveToBombState(navmeshAgent, animator,this,attackRange,chaseSpeed);

            _stateMachine = new StateMachine();
            At(search, move, HasTurretTarget()); 
            At(move, chase, HasTarget());
            At(chase, attack, AmIAttackPlayer());
            At(attack, chase, ()=>attack.InPlayerAttackRange()==false);
            At(chase, move, HasTargetNull());
            _stateMachine.AddAnyTransition( death, AmIDead());
            _stateMachine.AddAnyTransition(moveToBomb, ()=>IsBombCantFind);
            At(moveToBomb, move, ()=>IsBombCantFind==false);
            
            _stateMachine.SetState(search);
            void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

            Func<bool> HasTarget() => () => PlayerTarget != null;
            Func<bool> HasTurretTarget() => () => TurretTarget != null;
            Func<bool> HasTargetNull() => () => PlayerTarget is null;
            Func<bool> AmIDead() => () => Health<=0;
            Func<bool> AmIAttackPlayer() => () => PlayerTarget != null && chase.isPlayerInRange();
            Func<bool> IsBombInRange() => () => MineTarget != null;
        }
        private void Update() => _stateMachine.Tick();
    }
}