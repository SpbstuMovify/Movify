package com.polytech.contentservice.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "content.client.auth.grpc")
public record AuthGrpcClientProperty(String host, int port) {
}
