using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Infrastructure.Consumer
{
    public class SoulCoinConsumer : 
        IConsumer<SoulCoinEarnedEvent>,
        IConsumer<SoulCoinDeductedEvent>
    {
        private readonly IUserRepository _userRepository;

        public SoulCoinConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<SoulCoinEarnedEvent> context)
        {
            var message = context.Message;
            var user = await _userRepository.GetUserByIdAsync(message.UserId);
            
            if (user != null)
            {
                user.SoulCoinBalance += (int)message.Amount;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task Consume(ConsumeContext<SoulCoinDeductedEvent> context)
        {
            var message = context.Message;
            var user = await _userRepository.GetUserByIdAsync(message.UserId);

            if (user != null)
            {
                user.SoulCoinBalance = Math.Max(0, user.SoulCoinBalance - (int)message.Amount);
                await _userRepository.UpdateUserAsync(user);
            }
        }
    }
}
