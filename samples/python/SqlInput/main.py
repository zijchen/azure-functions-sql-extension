import logging
import azure.functions

def main(req: azure.functions.HttpRequest, rows : str) -> str:
    logging.info(rows)
    return rows