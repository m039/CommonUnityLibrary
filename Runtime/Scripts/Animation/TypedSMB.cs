using System;
using UnityEngine;
using UnityEngine.Animations;

namespace m039.Common
{
    /// The code is heavily borrowed from 3D Game Kit's SceneLinkedSMB and modified.
    public class TypedSMB<T> : StateMachineBehaviour where T : MonoBehaviour
    {
        T _behaviour;

        protected T Behaviour => _behaviour;

        bool _firstFrameHappend;

        bool _lastFrameHappend;

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable)
        {
            _behaviour = animator.GetComponent<T>();
            if (_behaviour == null)
            {
                Debug.LogError($"Can't find a component of {typeof(T).Name} type.");
            }
            else
            {
                _firstFrameHappend = false;
                OnTStateEnter(animator, stateInfo, layerIndex, playable);
            }
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable)
        {
            if (_behaviour == null)
                return;

            if (!animator.gameObject.activeSelf)
                return;

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnTTransitionToStateUpdate(animator, stateInfo, layerIndex, playable);
            }

            if (!animator.IsInTransition(layerIndex) && !_firstFrameHappend)
            {
                _firstFrameHappend = true;

                OnTStatePostEnter(animator, stateInfo, layerIndex, playable);
            }

            if (!animator.IsInTransition(layerIndex) && _firstFrameHappend)
            {
                OnTStateNoTransitionUpdate(animator, stateInfo, layerIndex, playable);
            }

            if (animator.IsInTransition(layerIndex) && !_lastFrameHappend && _firstFrameHappend)
            {
                _lastFrameHappend = true;
                OnTStatePreExit(animator, stateInfo, layerIndex, playable);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnTTransitionFromStateUpdate(animator, stateInfo, layerIndex, playable);
            }
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable)
        {
            if (_behaviour != null)
            {
                _lastFrameHappend = false;
                OnTStateExit(animator, stateInfo, layerIndex, playable);
            }
        }

        #region Methods

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        protected virtual void OnTStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called after OnTStateEnter every frame during transition to the state.
        /// </summary>
        protected virtual void OnTTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        protected virtual void OnTStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        protected virtual void OnTStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        protected virtual void OnTStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called after OnTStatePreExit every frame during transition to the state.
        /// </summary>
        protected virtual void OnTTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        protected virtual void OnTStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable playable) { }

        #endregion
    }

}
