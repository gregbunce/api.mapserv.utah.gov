# Address Parsing Microservice

## Installation

### Dependencies

1. Install the agrc sweeper project
   - `pip install agrc-sweeper`
1. Install the grpc tools
   - `pip install grpcio-tools`
1. Build the proto client and server
   - `python -m grpc_tools.protoc -I ./protos --python_out=. --grpc_python_out=. ./protos/address_parser.proto`
