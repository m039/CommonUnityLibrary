namespace m039.Common.GOAP
{
    public interface IActionStrategy
    {
        bool canPerform { get; }

        bool complete { get; }

        void Start()
        {
            // noop
        }

        void Update(float deltaTime)
        {
            // noop
        }

        void Stop()
        {
            // noop
        }
    }
}
