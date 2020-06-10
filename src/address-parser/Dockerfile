FROM python:3-buster AS BUILD
WORKDIR /usr/src/build

RUN pip install grpcio-tools

COPY protos protos

RUN python -m grpc_tools.protoc -I ./protos --python_out=. --grpc_python_out=. ./protos/address_parser.proto

FROM python:3-buster AS app
WORKDIR /usr/src/app

COPY --from=BUILD /usr/src/build .
COPY server.py .

RUN pip install agrc-sweeper grpcio protobuf

EXPOSE 50051

ENTRYPOINT [ "python", "server.py" ]
