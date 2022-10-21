using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;
using Enum;
using Interfaces;
using Enums;
using Signals;

namespace StateMachines.AIBrain.Enemy.States
{
    public class BossAttackState : IState, IReleasePoolObject
    {
        #region Self Variables

        #region Private Variables

        private readonly BossEnemyBrain _bossEnemyBrain;
        private readonly Animator _animator;
        private readonly float _attackRange;
        private readonly Transform _bombHolder;
        private readonly string _attack = "Attack";

        #endregion

        #endregion
        public BossAttackState(Animator animator, BossEnemyBrain bossEnemyBrain, float attackRange,ref Transform bombHolder )
        {
            _bossEnemyBrain = bossEnemyBrain;
            _animator = animator;
            _attackRange = attackRange;
            _bombHolder = bombHolder;
        }

        public void OnEnter()
        {
            
            _animator.SetTrigger(_attack);
        }

        public void OnExit()
        {
            if (_bombHolder.childCount>0)
                ReleaseObject(_bombHolder.GetChild(0).gameObject, PoolObjectType.Bomb);
        }

        public void ReleaseObject(GameObject obj, PoolObjectType poolType)
        {
            PoolSignals.Instance.onReleaseObjectFromPool?.Invoke(obj,poolType);
        }

        public void Tick()
        {
           // _bossEnemyBrain.transform.LookAt(_bossEnemyBrain.PlayerTarget, Vector3.up);
        }

    } 
}
