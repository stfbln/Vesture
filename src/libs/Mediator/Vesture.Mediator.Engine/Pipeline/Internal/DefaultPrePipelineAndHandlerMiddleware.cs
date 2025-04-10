﻿using Microsoft.Extensions.Logging;
using Vesture.Mediator.Engine.Pipeline.Extensions;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Pipeline.Internal
{
    /// <summary>
    /// The <see cref="DefaultPrePipelineAndHandlerMiddleware"/> is an implementation of
    /// <see cref="IPrePipelineMiddleware"/> and <see cref="IPreHandlerMiddleware"/>
    /// wrapping the next item in the <see cref="IInvocationPipeline{TResponse}"/> in
    /// a <c>try { } catch</c> and capturing the <see cref="Exception"/> that occured in
    /// the <see cref="IInvocationContext{TResponse}"/>.
    /// </summary>
    /// <seealso cref="IPrePipelineMiddleware"/>
    /// <seealso cref="IPreHandlerMiddleware"/>
    public class DefaultPrePipelineAndHandlerMiddleware
        : IPrePipelineMiddleware,
            IPreHandlerMiddleware
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new <see cref="DefaultPrePipelineAndHandlerMiddleware"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
        public DefaultPrePipelineAndHandlerMiddleware(
            ILogger<DefaultPrePipelineAndHandlerMiddleware> logger
        )
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        /// <inheritdoc />
        public async Task HandleAsync(
            IInvocationContext<object, object> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        )
        {
            try
            {
                await next.Invoke(cancellationToken);
            }
            catch (Exception ex)
            {
                context.AddError(ex);
                _logger.PrePipelineAndHandlerMiddlewareUnhandledException(context, ex);
            }
        }
    }
}
