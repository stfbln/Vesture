﻿using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Resolvers;

namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    /// <summary>
    /// Default implementation of <see cref="INoOpInvocationPipelineResolver"/>.
    /// </summary>
    /// <seealso cref="INoOpInvocationPipelineResolver"/>
    public class DefaultNoOpInvocationPipelineResolver : INoOpInvocationPipelineResolver
    {
        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly INoOpInvocationHandlerStrategyResolver _handlerStrategyResolver;

        /// <summary>
        /// Initializes a new <see cref="DefaultNoOpInvocationPipelineResolver"/> instance.
        /// </summary>
        /// <param name="contextFactory">The <see cref="IInvocationContextFactory"/> instance.</param>
        /// <param name="preInvocationPipelineMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> instance.</param>
        /// <param name="middlewares">The <see cref="IMiddlewareInvocationPipelineItem"/> instances.</param>
        /// <param name="preHandlerMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> instance.</param>
        /// <param name="handlerStrategyResolver">The <see cref="INoOpInvocationHandlerStrategyResolver"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="contextFactory"/> is <see langword="null" /> or <paramref name="preInvocationPipelineMiddlewareResolver"/> is <see langword="null" /> or <paramref name="middlewares"/> is <see langword="null" /> or <paramref name="preHandlerMiddlewareResolver"/> is <see langword="null" /> or <paramref name="handlerStrategyResolver"/> is <see langword="null" />.</exception>
        public DefaultNoOpInvocationPipelineResolver(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            INoOpInvocationHandlerStrategyResolver handlerStrategyResolver)
        {
            ArgumentNullException.ThrowIfNull(contextFactory, nameof(contextFactory));
            ArgumentNullException.ThrowIfNull(preInvocationPipelineMiddlewareResolver, nameof(preInvocationPipelineMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(middlewares, nameof(middlewares));
            ArgumentNullException.ThrowIfNull(preHandlerMiddlewareResolver, nameof(preHandlerMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(handlerStrategyResolver, nameof(handlerStrategyResolver));

            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handlerStrategyResolver = handlerStrategyResolver;
        }

        /// <inheritdoc />
        public IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>()
        {
            var handlerStrategy = _handlerStrategyResolver.ResolveNoOpInvocationHandlerStrategy<TResponse>(); ;
            return new DefaultInvocationPipeline<object, TResponse>(
                _contextFactory,
                _preInvocationPipelineMiddlewareResolver,
                _middlewares,
                _preHandlerMiddlewareResolver,
                handlerStrategy);
        }
    }
}
