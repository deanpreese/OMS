from common.common_cli import  KafkaConsumerCli


broker_cli = KafkaConsumerCli()
consumer = broker_cli.get_consumer()

while True:
    #msg = consumer.receive()
    for message in consumer:
        try:
            print(f"Received message {message.data()}")
            # Acknowledge successful processing of the message
        except Exception:
            # Message failed to be processed
            print(message)

broker_cli.client.close()