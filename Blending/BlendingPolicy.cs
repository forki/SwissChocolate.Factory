using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Blending
{
    public class BlendingPolicy : Saga<BlendingPolicyData>,
        IAmStartedByMessages<BlendChocolate>,
        IHandleMessages<VanillaAcquired>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BlendingPolicyData> mapper)
        {
            mapper.ConfigureMapping<BlendChocolate>(m => m.LotNumber).ToSaga(s => s.LotNumber);
        }

        public async Task Handle(BlendChocolate message, IMessageHandlerContext context)
        {
            Data.LotNumber = message.LotNumber;

            SpecialConsole.WriteLine($"['{message.LotNumber}' - Policy] Acquiring vanilla");

            await context.SendLocal(new AcquireVanilla { LotNumber = message.LotNumber });
        }

        public void Handle(VanillaAcquired message)
        {
            SpecialConsole.WriteLine($"['{message.LotNumber}' - Policy] Chocolate blended");

            Bus.Publish(new ChocolateBlended { LotNumber = message.LotNumber });

            MarkAsComplete();
        }
    }
}