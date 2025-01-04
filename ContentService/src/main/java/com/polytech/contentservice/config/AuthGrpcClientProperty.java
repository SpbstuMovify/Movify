package com.polytech.contentservice.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

/**
 * Конфигурация для подключения к auth сервису.
 *
 * @param host адрес сервиса
 * @param port порт сервиса
 */
@ConfigurationProperties(prefix = "content.client.auth.grpc")
public record AuthGrpcClientProperty(String host, int port) {
}
