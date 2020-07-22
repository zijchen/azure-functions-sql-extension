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
 * <p>Annotation for Sql output binding</p>
 */
@Target(ElementType.PARAMETER)
@Retention(RetentionPolicy.RUNTIME)
@CustomBinding(direction = "out", name = "sqlOutput", type = "sql")
public @interface SqlOutput {

    String commandText();

    String commandType() default "";

    String parameters() default "";

    String dataType() default "";

    String connectionStringSetting();

}