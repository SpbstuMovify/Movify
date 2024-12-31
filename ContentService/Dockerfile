FROM openjdk:21-jdk-slim
WORKDIR /opt/app
COPY build/libs/ContentService-0.0.1-SNAPSHOT.jar app.jar
EXPOSE 8085
EXPOSE 9095
ENTRYPOINT ["java", "-jar", "app.jar"]