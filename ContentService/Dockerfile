FROM openjdk:21-jdk-slim
WORKDIR /opt/app
COPY . .
EXPOSE 8085
EXPOSE 9095
ENTRYPOINT ["sh", "-c", "./gradlew clean build && java -jar build/libs/ContentService-0.0.1-SNAPSHOT.jar"]
