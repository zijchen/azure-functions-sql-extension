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
            @SqlInput(commandText = "select * from Products where cost = @Cost",
            commandType = "Text",
            parameters = "@Cost=100",
            connectionStringSetting = "SQLServerAuthentication") List<String> input,
            final ExecutionContext context) {
        context.getLogger().info("Java HTTP trigger processed a request.");

        return request.createResponseBuilder(HttpStatus.OK).body(input).build();
    }

    public static class Product
    {
        private int ProductID;
        private String Name;
        private int Cost;
        
        public int getProductID() { return this.ProductID; }
        public String getName() { return this.Name; }
        public int getCost() { return this.Cost; }
    }
}