import pytest
import requests

pytestmark = pytest.mark.services("test-content-service", "test-auth-service-mock", "test-movify-db")

CONTENT_PAYLOAD = {
    "title": "CC88df888888",
    "quality": "P1080",
    "genre": "COMEDY",
    "category": "SERIES",
    "age_restriction": "TWELVE_PLUS",
    "year": 2011,
    "description": "AAsddddddddddda trainsasdasds intensiaaasdfasdfvely to surpass his f sdf sdf ather...",
    "publisher": "Netflix",
    "cast_members": [
        {"employee_full_name": "Истаев Эрдем Эрдениеевич", "role_name": "Актёр"},
        {"employee_full_name": "Эрдемчик Эрдениеевич", "role_name": "Актёрище"},
        {"employee_full_name": "Истаев Эрдемище Эрдениеевич", "role_name": "Актёришка"},
        {"employee_full_name": "Путин Владимир Владимирович", "role_name": "Наставник по дзюдо"}
    ]
}

def test_create_content1():
    url = f"http://localhost:8085/v1/contents"
    headers = {
        "Authorization": "YOUR_ACCESS_TOKEN",  # Замените на ваш токен
        "Content-Type": "application/json"
    }

    response = requests.post(url, json=CONTENT_PAYLOAD, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"

