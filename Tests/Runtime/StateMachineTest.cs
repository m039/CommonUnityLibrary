using System.Collections;
using m039.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static WaitUtils;

public class StateMachineTest
{
    [UnityTest]
    public IEnumerator StateMachineTestWithEnumeratorPasses()
    {
        var tester = new GameObject().AddComponent<StateMachineTester>();

        Assert.AreEqual(tester.IdleState, tester.StateMachine.CurrentState);

        Assert.AreEqual(DebugState.Entered, tester.IdleState.state);
        Assert.AreEqual(DebugState.Unknown, tester.AttackState.state);
        Assert.AreEqual(DebugState.Unknown, tester.JumpState.state);

        yield return null;

        Assert.AreEqual(DebugState.Running, tester.IdleState.state);
        Assert.AreEqual(DebugState.Unknown, tester.AttackState.state);
        Assert.AreEqual(DebugState.Unknown, tester.JumpState.state);

        tester.TriggerAttack();

        yield return null;

        Assert.AreEqual(tester.AttackState, tester.StateMachine.CurrentState);
        Assert.IsTrue(tester.AttackState.timer > 0);

        Assert.AreEqual(DebugState.Exited, tester.IdleState.state);
        Assert.AreEqual(DebugState.Entered, tester.AttackState.state);
        Assert.AreEqual(DebugState.Unknown, tester.JumpState.state);

        yield return WaitUntil(() => tester.AttackState.timer <= 0);
        yield return null;

        Assert.AreEqual(tester.StateMachine.CurrentState, tester.IdleState);

        Assert.AreEqual(DebugState.Running, tester.IdleState.state);
        Assert.AreEqual(DebugState.Exited, tester.AttackState.state);
        Assert.AreEqual(DebugState.Unknown, tester.JumpState.state);

        tester.TriggerJump();

        yield return null;

        Assert.AreEqual(tester.JumpState, tester.StateMachine.CurrentState);
        Assert.IsTrue(tester.JumpState.timer > 0);

        yield return WaitUntil(() => tester.JumpState.timer <= 0);

        Assert.AreEqual(tester.StateMachine.CurrentState, tester.IdleState);

        tester.TriggerAttack();

        yield return null;

        Assert.AreEqual(tester.AttackState, tester.StateMachine.CurrentState);
        Assert.IsTrue(tester.AttackState.timer > 0);

        tester.TriggerJump();

        yield return null;

        Assert.AreEqual(tester.JumpState, tester.StateMachine.CurrentState);
        Assert.IsTrue(tester.JumpState.timer > 0);
    }

    enum DebugState
    {
        Unknown, Entered, Running, Exited
    }

    class StateBase : IState
    {
        readonly public StateMachineTester tester;

        public DebugState state = DebugState.Unknown;

        public StateBase(StateMachineTester tester)
        {
            this.tester = tester;
        }

        public virtual void OnEnter()
        {
            state = DebugState.Entered;
        }

        public virtual void OnExit()
        {
            state = DebugState.Exited;
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnUpdate()
        {
            state = DebugState.Running;
        }
    }

    class IdleState : StateBase
    {
        public IdleState(StateMachineTester tester) : base(tester)
        {
        }
    }

    class AttackState : StateBase
    {
        public float timer;

        public AttackState(StateMachineTester tester) : base(tester)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.1f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            timer -= Time.deltaTime;
        }
    }

    class JumpState : StateBase
    {
        public float timer;

        public JumpState(StateMachineTester tester) : base(tester)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = 0.1f;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            timer -= Time.deltaTime;
        }
    }

    class StateMachineTester : MonoBehaviour
    {
        public StateMachine StateMachine { get; private set; } = new();

        public IdleState IdleState { get; private set; }

        public AttackState AttackState { get; private set; }

        public JumpState JumpState { get; private set; }

        public void TriggerJump() => _jumpTrigger = true;

        public void TriggerAttack() => _attackTrigger = true;

        bool _jumpTrigger = false;

        bool _attackTrigger = false;

        void Awake()
        {
            IdleState = new IdleState(this);
            AttackState = new AttackState(this);
            JumpState = new JumpState(this);

            StateMachine.AddTransition(IdleState, AttackState, () => _attackTrigger);
            StateMachine.AddTransition(AttackState, IdleState, () => AttackState.timer <= 0);
            StateMachine.AddAnyTransition(JumpState, () => _jumpTrigger);
            StateMachine.AddTransition(JumpState, IdleState, () => JumpState.timer <= 0);
            StateMachine.SetState(IdleState);
        }

        void Update()
        {
            StateMachine.Update();

            _jumpTrigger = false;
            _attackTrigger = false;
        }

        void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }
    }
}
