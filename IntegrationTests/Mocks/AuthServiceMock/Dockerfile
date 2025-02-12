# Используем официальный slim-образ Python
FROM python:3.9-slim

WORKDIR /app

# Копируем файл с зависимостями
COPY requirements.txt .

# Устанавливаем зависимости
RUN pip install --no-cache-dir -r requirements.txt

# Копируем исходный код и сгенерированные файлы (auth_pb2.py и auth_pb2_grpc.py должны быть доступны)
COPY . .

# Открываем порт, на котором работает gRPC-сервис
EXPOSE 8079

# Запускаем mock-сервис
CMD ["python", "auth_service_mock.py"]
