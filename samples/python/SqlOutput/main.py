import logging
import azure.functions

def main(req: azure.functions.HttpRequest, row : azure.functions.Out[str]) -> str:
    string_row = "{\"ProductID\":" + str(1) + ",\"Name\":" + "\"" + "Bottle" + "\"" + ",\"Cost\":" + str(10) + "}"
    row.set(string_row)
    return string_row