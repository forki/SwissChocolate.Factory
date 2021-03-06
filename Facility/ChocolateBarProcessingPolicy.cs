﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace Facility
{
    public class ChocolateBarProcessingPolicy : Saga<ChocolateBarState>, 
        IAmStartedByMessages<ProduceChocolateBar>,
        IHandleMessages<BeansRoasted>,
        IHandleMessages<BeansGround>,
        IHandleMessages<ChocolateBlended>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ChocolateBarState> mapper)
        {
            mapper.ConfigureMapping<ProduceChocolateBar>(m => m.LotNumber).ToSaga(s => s.LotNumber);
            mapper.ConfigureMapping<BeansRoasted>(m => m.LotNumber).ToSaga(s => s.LotNumber);
            mapper.ConfigureMapping<BeansGround>(m => m.LotNumber).ToSaga(s => s.LotNumber);
        }

        public void Handle(ProduceChocolateBar message)
        {
            Data.LotNumber = message.LotNumber;

            Console.WriteLine($"['{message.LotNumber}' - Policy] Start roasting");
            Bus.Send(new RoastBeans { LotNumber = Data.LotNumber });
        }

        public void Handle(BeansRoasted message)
        {
            Console.WriteLine($"['{message.LotNumber}' - Policy] Start grinding");
            Bus.Send(new GrindBeans { LotNumber = Data.LotNumber });
        }

        public void Handle(BeansGround message)
        {
            Console.WriteLine($"['{message.LotNumber}' - Policy] Start blending");
            Bus.Send(new BlendChocolate { LotNumber = Data.LotNumber });
        }

        public void Handle(ChocolateBlended message)
        {
            Console.WriteLine($"['{message.LotNumber}' - Policy] Done");
            MarkAsComplete();
        }
    }
}
