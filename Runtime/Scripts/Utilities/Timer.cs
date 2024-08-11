using System;

namespace m039.Common
{
    public abstract class Timer
    {
        protected float initialTime;

        public float time { get; protected set; }

        public bool isRunning { get; protected set; }

        public virtual float progress => time / initialTime;

        public Action onStart = delegate { };

        public Action onStop = delegate { };

        public Action onTick = delegate { };

        public Timer(float initialTime)
        {
            this.initialTime = initialTime;
        }

        public void Start()
        {
            time = initialTime;
            if (!isRunning)
            {
                isRunning = true;
                onStart.Invoke();
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                onStop.Invoke();
            }
        }

        public void Resume() => isRunning = true;
        public void Pause() => isRunning = false;

        public virtual void Reset() { }

        public void Tick(float deltaTime)
        {
            if (!isRunning)
                return;

            OnTick(deltaTime);

            if (isRunning)
            {
                onTick.Invoke();
            }
        }

        protected abstract void OnTick(float deltaTime);
    }

    public class CountdownTimer : Timer
    {
        public override float progress => 1 - time / initialTime;

        public CountdownTimer(float initialTime) : base(initialTime)
        {
        }

        protected override void OnTick(float deltaTime)
        {
            if (time > 0)
            {
                time -= deltaTime;
            }

            if (time <= 0)
            {
                Stop();
            }
        }

        public bool isFinished => time <= 0;

        public override void Reset() => time = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }

    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        protected override void OnTick(float deltaTime)
        {
            time += deltaTime;
        }

        public override void Reset() => time = 0;
    }
}
