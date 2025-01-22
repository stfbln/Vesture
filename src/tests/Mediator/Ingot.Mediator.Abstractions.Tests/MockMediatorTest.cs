﻿using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Abstractions.Tests
{
    [MockTest]
    public class MockMediatorTest : MediatorConformanceTestBase<MockMediator>
    {
        private readonly ICollection<Action<MockMediator>> _setups = [];

        protected override MockMediator CreateMediator()
        {
            var mediator = new MockMediator();

            foreach (var setup in _setups)
            {
                setup.Invoke(mediator);
            }

            return mediator;
        }

        protected override void RegisterHandler<TRequest, TResponse>(
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            _setups.Add(m => m.AddHandler(handler));
        }

        protected override void RegisterMiddleware<TRequest, TResponse>(
            IInvocationMiddleware<TRequest, TResponse> middleware
        )
        {
            _setups.Add(m => m.AddMiddleware(middleware));
        }
    }
}
