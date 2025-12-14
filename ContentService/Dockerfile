FROM eclipse-temurin:21-jdk-jammy AS builder
WORKDIR /opt/app

COPY gradlew ./
COPY gradle ./gradle
COPY build.gradle* settings.gradle* gradle.properties* ./

RUN chmod +x gradlew

RUN --mount=type=cache,target=/root/.gradle \
    ./gradlew --no-daemon dependencies

COPY . .

RUN --mount=type=cache,target=/root/.gradle \
    ./gradlew clean build --no-daemon

FROM eclipse-temurin:21-jre-jammy
WORKDIR /opt/app

RUN useradd --system --uid 10001 --create-home app
USER app

COPY --from=builder /opt/app/build/libs/ContentService-*.jar app.jar

EXPOSE 8085 9095

ENV JAVA_TOOL_OPTIONS="-XX:MaxRAMPercentage=75"

ENTRYPOINT ["java", "-jar", "app.jar"]
