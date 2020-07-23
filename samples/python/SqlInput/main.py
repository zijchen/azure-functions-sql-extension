import logging
import azure.functions
import typing

def main(req: azure.functions.HttpRequest, rows : str) -> str:
    logging.info(rows)
    logging.info(type(rows))
    return rows