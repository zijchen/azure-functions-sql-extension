package com.sql;

import java.util.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

import com.microsoft.azure.functions.sql.annotation.SqlOutput;


import java.util.Optional;

public class FunctionOutput {
    @FunctionName("SqlOutput-Java")
    public HttpResponseMessage InsertRow(
            @HttpTrigger(name = "req", methods = {HttpMethod.GET, HttpMethod.POST}, authLevel = AuthorizationLevel.ANONYMOUS) HttpRequestMessage<Optional<String>> request,
            @SqlOutput(commandText = "Products",
            //dataType = "binary",
            connectionStringSetting = "SQLServerAuthentication") OutputBinding<List<Product>> output,
            final ExecutionContext context) {
        context.getLogger().info("Java HTTP trigger processed a request.");

    
        output.setValue(new ArrayList<Product>());
        Product product = new Product();
        product.Cost = 100;
        product.Name = "Lipstick";
        product.ProductID = 11;
        output.getValue().add(product);
        product = new Product();
        product.Cost = 100;
        product.Name = "Lipstick";
        product.ProductID = 9;
        output.getValue().add(product);
        return request.createResponseBuilder(HttpStatus.OK).body(product).build();
    }

    public static class Product
    {
        public int ProductID;
        public String Name;
        public int Cost;
        
        public int getProductID() { return this.ProductID; }
        public String getName() { return this.Name; }
        public int getCost() { return this.Cost; }

        @Override
        public String toString()
        {
            return "{\"ProductID\":" + ProductID + ",\"Name\":" + "\"" + Name + "\"" + ",\"Cost\":" + Cost + "}";
        }
    }
}