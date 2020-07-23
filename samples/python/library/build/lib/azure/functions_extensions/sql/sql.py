import json
from typing import Dict, Any, List, Union, Optional, Mapping
import logging

from azure.functions import meta

class SqlConverter(meta.InConverter, meta.OutConverter,
                        binding='sql'):

    @classmethod
    def check_input_type_annotation(cls, pytype: type) -> bool:
        valid_types = (str, bytes)
        logging.info("hi: check input")
        return (
            meta.is_iterable_type_annotation(pytype, valid_types)
            or (isinstance(pytype, type) and issubclass(pytype, valid_types))
        )

    @classmethod
    def check_output_type_annotation(cls, pytype: type) -> bool:
        valid_types = (str, bytes)
        logging.info("hi: check output")
        return (
            meta.is_iterable_type_annotation(pytype, valid_types)
            or (isinstance(pytype, type) and issubclass(pytype, valid_types))
        )

    @classmethod
    def decode(
        cls, data: meta.Datum, *, trigger_metadata
    ) -> Union[bytes, List[bytes]]:
        logging.info("hi: decode")
        data_type = data.type

        if data_type in ['string', 'bytes', 'json']:
            return cls.decode_single_event(data, trigger_metadata)

        elif data_type in ['collection_bytes', 'collection_string']:
            return cls.decode_multiple_events(data, trigger_metadata)

        else:
            raise NotImplementedError(
                f'unsupported event data payload type: {data_type}')

    @classmethod
    def decode_single_event(cls, data,
                            trigger_metadata) -> bytes:
        if data.type in ['string', 'json']:
            body = data.value.encode('utf-8')

        elif data.type == 'bytes':
            body = data.value

        return body

    @classmethod
    def decode_multiple_events(
            cls, data, trigger_metadata
    ) -> List[bytes]:
        if data.type == 'collection_bytes':
            parsed_data = data.value.bytes

        elif data.type == 'collection_string':
            parsed_data = data.value.string

        listOfBytes = []
        for parsed_datum in parsed_data:
            listOfBytes.append(parsed_datum)

        return listOfBytes

    @classmethod
    def encode(cls, obj: Any, *,
               expected_type: Optional[type]
               ) -> meta.Datum:
        logging.info("hi: encode")
        data = meta.Datum(type=None, value=None)

        if isinstance(obj, str):
            data = meta.Datum(type='string', value=obj)

        elif isinstance(obj, bytes):
            data = meta.Datum(type='bytes', value=obj)

        elif isinstance(obj, list):
            data = meta.Datum(type='json', value=json.dumps(obj))
        
        elif isinstance(obj, collections.abc.Iterable):
            msgs: List[str] = []
            for item in obj:
                if isinstance(item, str):
                    msgs.append(item)
                else:
                    raise NotImplementedError(
                        'invalid data type in output '
                        'queue message list: {}'.format(type(item)))

            return meta.Datum(
                type='json',
                value=json.dumps(msgs)
            )

        return data
