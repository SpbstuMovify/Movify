package com.polytech.contentservice.config;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.info.Info;

/**
 * Конфигурация для OpenAPI.
 */
@OpenAPIDefinition(
    info = @Info(
        title = "Movify Service",
        description = "Service for saving data about films and serials", version = "1.0.0"
    )
)
public class OpenApiConfiguration {
}
