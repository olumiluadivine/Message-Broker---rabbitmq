using MassTransit;
using SharedMessages.Messages;

namespace SagaOrchestration
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        // State
        public State Submitted { get; private set; }

        public State InventoryReserved { get; private set; }
        public State PaymentCompleted { get; private set; }

        // Events
        public Event<OrderPlaced> OrderPlacedEvent { get; private set; }

        public Event<InventoryReserved> InventoryReservedEvent { get; private set; }
        public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderPlacedEvent, x =>
            {
                x.CorrelateById(context => context.Message.OrderId);
            });
            Event(() => InventoryReservedEvent, x =>
            {
                x.CorrelateById(context => context.Message.OrderId);
            });
            Event(() => PaymentCompletedEvent, x =>
            {
                x.CorrelateById(context => context.Message.OrderId);
            });

            Initially(
                When(OrderPlacedEvent)
                    .Then(context =>
                    {
                        context.Instance.OrderId = context.Data.OrderId;
                        context.Instance.Quantity = context.Data.Quantity;
                        Console.WriteLine($"Order {context.Instance.OrderId} has been placed with quantity {context.Instance.Quantity}");
                    })
                    .TransitionTo(Submitted)
            );

            During(Submitted,
                When(InventoryReservedEvent)
                    .Then(context => Console.WriteLine($"Inventory has been reserved for order {context.Instance.OrderId}"))
                    .TransitionTo(InventoryReserved)
            );

            During(InventoryReserved,
                When(PaymentCompletedEvent)
                    .Then(context => Console.WriteLine($"Payment has been completed for order {context.Instance.OrderId}"))
                    .TransitionTo(PaymentCompleted)
            );

            SetCompletedWhenFinalized();
        }
    }
}