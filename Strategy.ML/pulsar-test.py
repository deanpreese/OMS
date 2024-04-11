import pulsar

client = pulsar.Client('pulsar://10.0.0.82:6650')

"""
producer = client.create_producer('my-topic')

for i in range(10):
    producer.send(('Hello-%d' % i).encode('utf-8'))

client.close()
"""

topic = 'persistent://public/default/my-topic'
subs = 'my-subscription'

consumer = client.subscribe(topic, subs)

while True:
    msg = consumer.receive()
    try:
        print("Received message '{}' id='{}'".format(msg.data(), msg.message_id()))
        # Acknowledge successful processing of the message
        consumer.acknowledge(msg)
    except Exception:
        # Message failed to be processed
        consumer.negative_acknowledge(msg)

client.close()