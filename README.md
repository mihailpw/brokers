# Broker Repository

This repository contains a collection of broker implementations, which serve as adapters between requests and responses for various systems.

## What is a Broker?
A broker is a software component that acts as an intermediary between two systems, translating requests from one system into a format that can be understood by another system, and then translating the responses from that system back into a format that can be understood by the original system.

Brokers are often used in distributed systems, where multiple systems need to communicate with each other, but may use different protocols or data formats.

## Repository Structure
This repository contains the following directories:

- `src/`: Contains the source code for the broker implementations.
    - `Broker/`: Contains Broker common code and some basic broker implementations.
    - `Broker.Brokers.*/`: Contains more specific broker implementations.
    - `Broker.Face.*/`: Contains services code for demo purposes.
    - `Broker.UnitTests/`: Contains unit tests for `Broker` code.
    - `Broker.Face.LoadTests/`: Contains load tests for faces.
    - `Broker.Benchmarks/`: Contains benchmark tests for `Broker` code.

## Getting Started
To get started with using the brokers in this repository, you can:

- Clone the repository to your local machine.
- Open Broker.sln with Visual Studio or JetBrains Rider
- Develop the code

Prerequisites to run:
- dotnet 6
- docker + docker-compose
- [curl](https://formulae.brew.sh/formula/curl)
- [grpcurl](https://formulae.brew.sh/formula/grpcurl)

To run the demo you can use docker-compose file:

- Set working directory `src/`
- To run the services, use
```
docker-compose up -d
```
- To observe the incoming messages to consumer, then use
```
docker-compose up consumers
```
- To send data via HTTP face, use
```
curl --request POST 'http://localhost:9000/event?broker=kafka-1' \
    --header 'Content-Type: text/plain' \
    --data-raw 'Hello, World!'
```
- To send data via GRPC face, use
```
grpcurl -plaintext \
    -d '{"broker": "http-post-1", "payload": "SGVsbG8sIFdvcmxkIQ=="}' \
    localhost:9100 broker.face.grpc.Event/Send
```
- To run unit tests, use
```
dotnet test
```
- To run benchmarks, use
```
dotnet run --project Broker.Benchmarks/Broker.Benchmarks.csproj -c Release
```
- To run load tests, use
```
dotnet run --project Broker.Face.LoadTests/Broker.Face.LoadTests.csproj -c Release
```

Each demo services contains already configured brokers. There are many of them:
- `http-post-1`
- `http-patch-1`
- `grpc-1`
- `file-1`
- `file-2`
- `console-1`
- `log-1`
- `batch-console-1`
- `batch-log-1`
- `composite-1`
- `kafka-1`

You can find settings files in repo: [HTTP service](https://github.com/mihailpw/brokers/blob/master/src/Broker.Face.Http/appsettings.json#L11), [GRPC service](https://github.com/mihailpw/brokers/blob/master/src/Broker.Face.Grpc/appsettings.json#L11).

## Broker Implementations
The following broker implementations are currently included in this repository:

### `HttpBroker`
A broker for HTTP calls, which translates requests and responses between HTTP requests.

The broker has following options:
- `RequestUri` - an URI of service the message should be redirected
- `HttpMethod` - a HTTP method of the request
- `Headers` - a key-value map with headers which will be included into each request 

### `KafkaBroker`
A broker for Kafka message queue, which translates requests and responses between

The broker has following options:
- `Topic` - the topic where messages will be producing
- `KafkaConfig` - the configuration of Kafka producer ([config doc](https://kafka.apache.org/documentation/#producerconfigs))

### `AzureBroker`
...

### `GrpcBroker`
...

### `ConsoleBroker`
...

### `LogBroker`
...

### `FileBroker`
...

### `BatchBrokerDecorator`
This is decorator that allows to send batched data to inner broker  

...

### `CompositeBrokerDecorator`
This is decorator that allows to send data to multiple inner brokers


...

---

Each implementation is designed to be modular and extensible, allowing users to customize the behavior to fit their specific needs.

## Contributing
Contributions to this repository are welcome! If you find a bug, have a feature request, or want to contribute code, please open an issue or pull request.

Please ensure that all contributions adhere to our code of conduct and follow the guidelines outlined in our contribution guidelines.

## License
This repository is licensed under the MIT license. See the LICENSE file for more information.



