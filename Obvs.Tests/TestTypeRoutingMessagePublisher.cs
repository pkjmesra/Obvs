using System;
using System.Collections.Generic;
using FakeItEasy;
using Obvs.Types;
using Xunit;

namespace Obvs.Tests
{
    
    public class TestTypeRoutingMessagePublisher
    {
        [Fact]
        public void ShouldDispatchToCorrectPublisher()
        {
            IMessagePublisher<IMessage> eventPublisher = A.Fake<IMessagePublisher<IMessage>>();
            IMessagePublisher<IMessage> commandPublisher = A.Fake<IMessagePublisher<IMessage>>();
            IMessagePublisher<IMessage> messagePublisher = A.Fake<IMessagePublisher<IMessage>>();

            TypeRoutingMessagePublisher<IMessage> typeRoutingMessagePublisher =
                new TypeRoutingMessagePublisher<IMessage>(new[]
                {
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(IEvent), eventPublisher),
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(ICommand), commandPublisher),
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(IMessage), messagePublisher)
                });

            IEvent ev = A.Fake<IEvent>();
            ICommand command = A.Fake<ICommand>();
            IMessage message = A.Fake<IMessage>();

            typeRoutingMessagePublisher.PublishAsync(ev);
            typeRoutingMessagePublisher.PublishAsync(command);
            typeRoutingMessagePublisher.PublishAsync(message);

            A.CallTo(() => eventPublisher.PublishAsync(ev)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => eventPublisher.PublishAsync(message)).MustNotHaveHappened();
            A.CallTo(() => eventPublisher.PublishAsync(command)).MustNotHaveHappened();

            A.CallTo(() => commandPublisher.PublishAsync(ev)).MustNotHaveHappened();
            A.CallTo(() => commandPublisher.PublishAsync(message)).MustNotHaveHappened();
            A.CallTo(() => commandPublisher.PublishAsync(command)).MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => messagePublisher.PublishAsync(ev)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => messagePublisher.PublishAsync(command)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => messagePublisher.PublishAsync(message)).MustHaveHappened(1, Times.Exactly);
        }
    }
}