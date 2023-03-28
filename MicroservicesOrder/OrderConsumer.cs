using MassTransit;

namespace MicroservicesOrder
{
    public class OrderConsumer : IConsumer<UpdateOrder>
    {
        public Task Consume(ConsumeContext<UpdateOrder> context)
        {
            return Task.CompletedTask;
        }
    }
}
