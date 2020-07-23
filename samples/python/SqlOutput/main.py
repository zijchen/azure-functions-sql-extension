import logging
import azure.functions

def main(req: azure.functions.HttpRequest, row : azure.functions.Out[str]) -> str:
    row = "{\"ProductID\":" + str(1) + ",\"Name\":" + "\"" + "Bottle" + "\"" + ",\"Cost\":" + str(10) + "}"
    return row