import azure.functions as func
import typing

def main(req: func.HttpRequest, rows: func.Out[str]) -> func.HttpResponse:
    row_1 = "{\"ProductID\":" + str(1) + ",\"Name\":" + "\"" + "Bottle" + "\"" + ",\"Cost\":" + str(10) + "}"
    row_2 = "{\"ProductID\":" + str(2) + ",\"Name\":" + "\"" + "Bottle" + "\"" + ",\"Cost\":" + str(10) + "}"
    rows.set("[" + row_1 + "," + row_2 + "]")
    return row_2