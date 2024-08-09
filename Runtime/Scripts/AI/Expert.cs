namespace m039.Common.AI
{
    public interface IExpert
    {
        int GetInsistence();
        void Execute();

        /// <summary>
        /// Executes after the arbiter calls <see cref="Execute"> on an expert with highest insistence.
        /// Use this method to clean up resources.
        /// </summary>
        void LateExecute();
    }
}
