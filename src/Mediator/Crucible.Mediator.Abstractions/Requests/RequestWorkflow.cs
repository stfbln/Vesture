﻿using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// Provides a base implementation of the <see cref="IInvocationWorkflow"/>.
    /// You should inherit from this class and override the 
    /// <see cref="RequestHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)"/> method 
    /// to manage and coordinate the flow of multiple operations across different handlers for a 
    /// specific <see cref="IRequest{TResponse}"/> contract and producing a <typeparamref name="TResponse"/> 
    /// result, as expected by the corresponding <typeparamref name="TRequest"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IInvocationWorkflow" path="/summary"/>
    /// </remarks>
    /// <typeparam name="TRequest">
    /// The <see cref="IRequest{TResponse}"/> contract type handled by this handler.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The response type produced by processing the <typeparamref name="TRequest"/>.
    /// </typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="RequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationWorkflow"/>
    /// <seealso cref="IMediator"/>
    public abstract class RequestWorkflow<TRequest, TResponse> : RequestHandler<TRequest, TResponse>, IInvocationWorkflow
        where TRequest : IRequest<TResponse>
    {
        private IMediator? _workflowMediator;

        private IMediator WorkflowMediator
        {
            get => _workflowMediator ?? throw new EntryPointNotFoundException("The Workflow mediator has not yet been initialized"); 
            set => _workflowMediator = value;
        }

        IMediator IInvocationWorkflow.Mediator { set => WorkflowMediator = value; }

        /// <exclude />
        /// <inheritdoc cref="IMediator.ExecuteAndCaptureAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => WorkflowMediator.ExecuteAndCaptureAsync(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.ExecuteAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => WorkflowMediator.ExecuteAsync(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.HandleAndCaptureAsync{TResponse}(object, CancellationToken)"/>
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default) => WorkflowMediator.HandleAndCaptureAsync<TResponse>(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.HandleAsync{TResponse}(object, CancellationToken)"/>
        public Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default) => WorkflowMediator.HandleAsync<TResponse>(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.InvokeAndCaptureAsync(ICommand, CancellationToken)"/>
        public Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default) => WorkflowMediator.InvokeAndCaptureAsync(command, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.InvokeAsync(ICommand, CancellationToken)"/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default) => WorkflowMediator.InvokeAsync(command, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.PublishAndCaptureAsync(IEvent, CancellationToken)"/>
        public Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default) => WorkflowMediator.PublishAndCaptureAsync(@event, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.PublishAsync(IEvent, CancellationToken)"/>
        public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default) => WorkflowMediator.PublishAsync(@event, cancellationToken);
    }
}
