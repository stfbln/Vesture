﻿using Moq;
using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Vesture.Mediator.Engine.Pipeline.Strategies;
using Vesture.Testing;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class SequentialHandlersStrategyTest<TRequest, TResponse>
        : EngineMultiInvocationHandlerStrategyTestBase<
            TRequest,
            TResponse,
            SequentialHandlersStrategy<TRequest, TResponse>
        >
    {
        protected NUnitTestContextMsLogger<
            SequentialHandlersStrategy<TRequest, TResponse>
        > Logger { get; } = new();

        public SequentialHandlersStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override SequentialHandlersStrategy<TRequest, TResponse> CreateStrategy() =>
            new(Logger, Resolvers);

        [Test]
        public void Ctor_ArgumentNullException_IfLoggerIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new ParallelHandlersStrategy<TRequest, TResponse>(null, Resolvers)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentNullException_IfResolversIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new SequentialHandlersStrategy<TRequest, TResponse>(Logger, null)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentException_IfResolversIsEmpty()
        {
            // Arrange
            // No arrange required

            // Act / Assert
            Assert.Throws<ArgumentException>(
                () => new SequentialHandlersStrategy<TRequest, TResponse>(Logger, [])
            );
        }

        [Test]
        public async Task HandleAsync_OnlyFirstHandlerIsInvoked_WhenTheFirstHandlerFails()
        {
            // Arrange
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act
            try
            {
                await Strategy.HandleAsync(Context, Next, CancellationToken);
            }
            catch { }

            // Assert
            OtherHandler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Never,
                failMessage: "HandleAsync should called resolved handler"
            );
        }

        [Test]
        public async Task HandleAsync_OnlyFirstHandlerIsResolved_WhenTheFirstHandlerFails()
        {
            // Arrange
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act
            try
            {
                await Strategy.HandleAsync(Context, Next, CancellationToken);
            }
            catch { }

            // Assert
            OtherResolver.Mock.Verify(
                m => m.ResolveComponent(),
                Times.Never,
                failMessage: "ResolveComponent should be called everytime HandleAsync is invoked"
            );
        }

        [Test]
        public async Task HandleAsync_HandlersAreInvokedInSequence()
        {
            // Arrange
            var tcsHandler = new TaskCompletionSource<TResponse>();
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(tcsHandler.Task);

            var tcsOtherHandler = new TaskCompletionSource<TResponse>();
            OtherHandler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(tcsOtherHandler.Task);

            // Act
            var task = Strategy.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Handler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
            OtherHandler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Never
            );

            tcsHandler.SetResult(Response);

            Handler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
            OtherHandler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Cleanup
            tcsOtherHandler.SetResult(Response);
            await task;
        }
    }
}
