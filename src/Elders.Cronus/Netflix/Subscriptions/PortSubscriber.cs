﻿using Elders.Cronus.DomainModeling;
using Elders.Cronus.Logging;
using Elders.Cronus.MessageProcessingMiddleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elders.Cronus.Netflix
{
    public class PortSubscriber : ISubscriber
    {
        static ILog log = LogProvider.GetLogger(typeof(ProjectionSubscriber));

        readonly MessageHandlerMiddleware handlerMiddleware;

        readonly Type handlerType;

        public PortSubscriber(Type handlerType, PortsMiddleware handlerMiddleware)
        {
            if (handlerMiddleware == null) throw new ArgumentNullException(nameof(handlerMiddleware));

            this.handlerType = handlerType;
            this.handlerMiddleware = handlerMiddleware;
            if (typeof(IPort).IsAssignableFrom(handlerType) == false)
                throw new ArgumentException($"'{handlerType.FullName}' does not implement IProjection");
            Id = handlerType.FullName;
            MessageTypes = GetInvolvedMessageTypes(handlerType).ToList();
        }

        public string Id { get; private set; }

        /// <summary>
        /// Gets the message types which the subscriber can process.
        /// </summary>
        public List<Type> MessageTypes { get; private set; }

        public void Process(CronusMessage message)
        {
            var context = new HandleContext(message, handlerType);
            handlerMiddleware.Run(context);
        }

        private IEnumerable<Type> GetInvolvedMessageTypes(Type type)
        {
            var ieventHandler = typeof(IEventHandler<>);
            var interfaces = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == ieventHandler);
            foreach (var @interface in interfaces)
            {
                Type eventType = @interface.GetGenericArguments().FirstOrDefault();
                yield return eventType;
            }
        }
    }
}