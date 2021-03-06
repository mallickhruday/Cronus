﻿using System.Collections.Generic;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Discoveries
{
    public class CronusWorkflowsDiscovery : DiscoveryBase<IWorkflow>
    {
        protected override DiscoveryResult<IWorkflow> DiscoverFromAssemblies(DiscoveryContext context)
        {
            return new DiscoveryResult<IWorkflow>(GetModels());
        }

        IEnumerable<DiscoveredModel> GetModels()
        {
            yield return new DiscoveredModel(typeof(Workflow<HandleContext>), typeof(MessageHandleWorkflow), ServiceLifetime.Transient);
        }
    }
}
