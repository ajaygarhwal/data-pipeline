namespace Microsoft.Practices.DataPipeline.Tests.Mocks
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;

    public static class MockPartitionContext
    {
        public static PartitionContext Create(string partitionId, Func<Task> checkpointAsync)
        {
            // We want to make sure that we are getting the constructor that accepts
            // an instance of `ICheckpointManager`.
            var ctr = typeof(PartitionContext)
                .GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(ICheckpointManager) },
                    null);

            var context = (PartitionContext)ctr.Invoke(new object[] { new MockCheckpointManager(checkpointAsync) });
            context.Lease = new Lease { PartitionId = partitionId };

            return context;
        }

        public static PartitionContext CreateWithNoopCheckpoint(string partitionId)
        {
            return Create(partitionId, () => Task.FromResult(true));
        }
    }
}