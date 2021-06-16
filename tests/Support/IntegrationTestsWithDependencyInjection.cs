using System;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeCSharpPlayground.EntryCommands;

namespace Tests.Support
{
    public class IntegrationTestsWithDependencyInjection : IntegrationTestsFixture<ApiCommand.Startup>
    {
        public IntegrationTestsWithDependencyInjection(Action<IServiceCollection> customSetup = null) : base(customSetup)
        {
            // Just to avoid configuring ApiCommand.Startup as the generic type many times 😁       
        }
    }
}
