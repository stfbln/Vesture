﻿using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    /// <summary>
    /// The <see cref="DefaultPrePipelineAndHandlerMiddleware"/> is an implementation of
    /// <see cref="IPreInvocationPipelineMiddleware"/> and <see cref="IPreHandlerMiddleware"/>
    /// wrapping the next item in the <see cref="IInvocationPipeline{TResponse}"/> in
    /// a <c>try { } catch</c> and capturing the <see cref="Exception"/> that occured in
    /// the <see cref="IInvocationContext{TResponse}"/>.
    /// </summary>
    /// <seealso cref="IPreInvocationPipelineMiddleware"/>
    /// <seealso cref="IPreHandlerMiddleware"/>
    public class DefaultPrePipelineAndHandlerMiddleware : IPreInvocationPipelineMiddleware, IPreHandlerMiddleware
    {
        /// <exclude />
        public static readonly DefaultPrePipelineAndHandlerMiddleware Instance = new();

        /// <inheritdoc />
        public async Task HandleAsync(IInvocationContext<object, object> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            try
            {
                await next.Invoke(cancellationToken);
            }
            catch (Exception ex)
            {
                context.SetError(ex);
            }
        }
    }
}
