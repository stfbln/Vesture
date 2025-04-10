﻿using Moq;
using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Vesture.Mediator.Abstractions.Tests.Invocation;
using Vesture.Mediator.Events;
using Vesture.Mediator.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Abstractions.Tests.Events
{
    [SampleTest]
    [TestFixtureSource_Request_Event]
    public class EventHandlerTest<TEvent>
        : InvocationHandlerConformanceTestBase<
            TEvent,
            EventResponse,
            EventHandlerTest<TEvent>.SampleEventHandler
        >
    {
        protected Mock<IEventHandlerLifeCycle> LifeCycle { get; } = new();

        public EventHandlerTest(TEvent @event)
            : base(@event) { }

        protected override SampleEventHandler CreateInvocationHandler() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle
                .Setup(m =>
                    m.InnerEntersHandleAsync(It.IsAny<TEvent>(), It.IsAny<CancellationToken>())
                )
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<TEvent, EventResponse>)Handler).HandleAsync(
                Request,
                CancellationToken
            );

            LifeCycle.Verify(
                m => m.InnerEntersHandleAsync(It.IsAny<TEvent>(), It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        public interface IEventHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(TEvent @event, CancellationToken cancellationToken);
        }

        public class SampleEventHandler : Mediator.Events.EventHandler<TEvent>
        {
            private readonly IEventHandlerLifeCycle _lifeCycle;

            public SampleEventHandler(IEventHandlerLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(
                TEvent @event,
                CancellationToken cancellationToken = default
            )
            {
                await _lifeCycle.InnerEntersHandleAsync(@event, cancellationToken);
            }
        }
    }
}
