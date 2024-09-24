FROM rabbitmq:3.8.8-management

# Update package lists and install curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Download the delayed message exchange plugin
RUN curl -L https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v3.8.0/rabbitmq_delayed_message_exchange-3.8.0.ez \
  -o $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.8.0.ez

# Change ownership of the plugin file to the rabbitmq user
RUN chown rabbitmq:rabbitmq $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.8.0.ez

# Enable the delayed message exchange plugin
RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange
# Enable the consistent hash exchange plugin
RUN rabbitmq-plugins enable --offline rabbitmq_consistent_hash_exchange

# Expose RabbitMQ ports
EXPOSE 5672 15672
