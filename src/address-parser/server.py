import logging
import sys
from concurrent import futures

import address_parser_pb2
import address_parser_pb2_grpc
import grpc
from sweeper.address_parser import Address


class AddressServicer(address_parser_pb2_grpc.AddressServiceServicer):

    def Parse(self, request, context):
        logging.debug('input address: %s', request.address)
        address = Address(request.address)
        logging.debug('parsed address: %r', address)

        return address_parser_pb2.Address(
            address_number=address.address_number,
            address_number_suffix=address.address_number_suffix,
            prefix_direction=address.prefix_direction,
            street_name=address.street_name,
            street_direction=address.street_direction,
            street_type=address.street_type,
            unit_type=address.unit_type,
            unit_id=address.unit_id,
            city=address.city,
            zip_code=address.zip_code,
            po_box=address.po_box,
            normalized=address.normalized
        )


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    address_parser_pb2_grpc.add_AddressServiceServicer_to_server(AddressServicer(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig(
        level=logging.DEBUG,
        stream=sys.stdout,
        format='%(asctime)s:%(levelname)s: %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S'
    )
    logging.info('server listing on 50051')
    serve()
