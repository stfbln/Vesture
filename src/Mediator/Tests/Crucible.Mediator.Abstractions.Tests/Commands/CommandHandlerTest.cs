﻿using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Commands
{
    [SampleTest]
    [TestFixtureSource_Request_Commands]
    public class CommandHandlerTest<TCommand> : InvocationHandlerConformanceTestBase<TCommand, CommandResponse, CommandHandlerTest<TCommand>.SampleCommandHandler>
    {
        protected Mock<ICommandHandlerLifeCycle> LifeCycle { get; } = new();

        public CommandHandlerTest(TCommand command)
            : base(command) { }

        protected override SampleCommandHandler CreateInvocationHandler() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<TCommand, CommandResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        public interface ICommandHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(TCommand command, CancellationToken cancellationToken);
        }

        public class SampleCommandHandler : CommandHandler<TCommand>
        {
            private readonly ICommandHandlerLifeCycle _lifeCycle;

            public SampleCommandHandler(ICommandHandlerLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(command, cancellationToken);
            }
        }
    }
}
