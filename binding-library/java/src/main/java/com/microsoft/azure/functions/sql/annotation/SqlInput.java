/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 */
package com.microsoft.azure.functions.sql.annotation;

import com.microsoft.azure.functions.annotation.CustomBinding;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * <p>Annotation for Sql input binding</p>
 */
@Target(ElementType.PARAMETER)
@Retention(RetentionPolicy.RUNTIME)
<<<<<<< HEAD
@CustomBinding(direction = "in", name = "sqlInput", type = "sql")
public @interface SqlInput {

    String commandText();
=======
@CustomBinding(direction = "in", name = "sqlInput", type = "SqlBinding")
public @interface SqlInput {

    String command();
>>>>>>> 1dbc57290856ec30eacdc3e5c9abe54b2ef91f45

    String commandType();

    String parameters();

    String connectionStringSetting();

}