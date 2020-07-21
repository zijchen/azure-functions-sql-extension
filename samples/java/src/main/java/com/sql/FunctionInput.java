package com.sql;

import java.util.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

import com.microsoft.azure.functions.sql.annotation.SqlInput;

import java.util.Optional;

public class FunctionInput {
    @FunctionName("SqlInput-Java")
    public HttpResponseMessage input(
            @HttpTrigger(name = "req", methods = {HttpMethod.GET, HttpMethod.POST}, authLevel = AuthorizationLevel.ANONYMOUS) HttpRequestMessage<Optional<String>> request,
<<<<<<< HEAD
            @SqlInput(commandText = "select * from Products where cost = @Cost",
=======
            @SqlInput(command = "select * from Products where cost = @Cost",
>>>>>>> 1dbc57290856ec30eacdc3e5c9abe54b2ef91f45
            commandType = "Text",
            parameters = "@Cost=100",
            connectionStringSetting = "SQLServerAuthentication") Optional<Product> input,
            final ExecutionContext context) {
        context.getLogger().info("Java HTTP trigger processed a request.");

        return request.createResponseBuilder(HttpStatus.OK).body(input).build();
    }

    public static class Product
    {
        private int productID;
        private String name;
        private int cost;
        
        public int getProductID() { return this.productID; }
        public String getName() { return this.name; }
        public int getCost() { return this.cost; }
    }
}