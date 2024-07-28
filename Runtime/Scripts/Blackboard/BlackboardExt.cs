namespace m039.Common.Blackboard
{
    public static class BlackboardExt 
    {
        public static void SetValues(this Blackboard blackboard, BlackboardData data)
        {
            data?.SetValuesOnBlackboard(blackboard);
        }

        public static void SetValues(this Blackboard blackboard, BlackboardEntryData entryData)
        {
            entryData?.SetValueOnBlackboard(blackboard);
        }
    }
}
