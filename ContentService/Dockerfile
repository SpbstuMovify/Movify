FROM openjdk:21-jdk-slim AS builder
WORKDIR /opt/app
COPY . .
RUN ./gradlew clean build --no-daemon
FROM openjdk:21-jdk-slim
WORKDIR /opt/app
COPY --from=builder /opt/app/build/libs/ContentService-0.0.1-SNAPSHOT.jar app.jar
EXPOSE 8085 9095
ENTRYPOINT ["java", "-jar", "app.jar"]
