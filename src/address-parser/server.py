from blacksheep.server import Application
from blacksheep.server.responses import pretty_json
from sweeper.address_parser import Address
from blacksheep.server.bindings import FromRoute

app = Application()

@app.router.get('/:in_address')
async def home(in_address: FromRoute()):
    address = Address(in_address)
    print(address)

    return pretty_json({
        'address_number': address.address_number,
        'address_number_suffix': address.address_number_suffix,
        'prefix_direction': address.prefix_direction,
        'street_name': address.street_name,
        'street_direction': address.street_direction,
        'street_type': address.street_type,
        'unit_type': address.unit_type,
        'unit_id': address.unit_id,
        'city': address.city,
        'zip_code': address.zip_code,
        'po_box': address.po_box
    })
