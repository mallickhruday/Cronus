﻿using System;
using Elders.Cronus.Middleware;

namespace Elders.Cronus.MessageProcessingMiddleware
{
    public class MessageHandlerMiddleware : Middleware<HandlerContext>
    {
        public Middleware<Type, IHandlerInstance> CreateHandler { get; private set; }

        public Middleware<HandleContext> BeginHandle { get; private set; }

        public Middleware<HandleContext> EndHandle { get; private set; }

        public Middleware<HandleContext> ActualHandle { get; private set; }

        public Middleware<ErrorContext> Error { get; private set; }

        public MessageHandlerMiddleware(IHandlerFactory factory)
        {
            CreateHandler = MiddlewareExtensions.Lambda<Type, IHandlerInstance>((handlerType, execution) => factory.Create(handlerType));

            BeginHandle = MiddlewareExtensions.Lamda<HandleContext>();

            ActualHandle = MiddlewareExtensions.Lamda<HandleContext>((context, execution) =>
            {
                dynamic handler = context.HandlerInstance;
                handler.Handle((dynamic)context.Message);
            });

            EndHandle = MiddlewareExtensions.Lamda<HandleContext>();
            Error = MiddlewareExtensions.Lamda<ErrorContext>();
        }
        protected override void Invoke(MiddlewareContext<HandlerContext> middlewareControl)
        {
            try
            {
                using (var handler = CreateHandler.Invoke(middlewareControl.Context.HandlerType))
                {
                    var handleContext = new HandleContext(middlewareControl.Context.Message, handler.Current);
                    BeginHandle.Invoke(handleContext);
                    ActualHandle.Invoke(handleContext);
                    EndHandle.Invoke(handleContext);
                }
            }
            catch (Exception ex)
            {
                Error.Invoke(new ErrorContext(ex, middlewareControl.Context.Message, middlewareControl.Context.HandlerType));
                throw ex;// Throwing the exception becouse the retries are currently not here :)
            }
        }

        public class ErrorContext
        {
            public ErrorContext(Exception error, object message, Type handlerType)
            {
                Error = error;
                Message = message;
                HandlerType = handlerType;
            }
            public Exception Error { get; private set; }

            public object Message { get; private set; }

            public Type HandlerType { get; private set; }
        }

        public class HandleContext
        {
            public HandleContext(object message, object handlerInstance)
            {
                Message = message;
                HandlerInstance = handlerInstance;
            }
            public object Message { get; private set; }

            public object HandlerInstance { get; private set; }
        }
    }
}