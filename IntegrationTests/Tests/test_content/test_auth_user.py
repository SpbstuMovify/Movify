from conftest import logger
import pytest
import requests

pytestmark = pytest.mark.services("test-content-service-for-auth-service", "test-auth-service", "test-movify-db")

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

LOGIN_PAYLOAD = {
    "login":"aa",
    "password":"bb",
    "search_type": "LOGIN"
}

REGISTRATION_PAYLOAD = {
    "first_name":"Эрдем",
    "last_name": "Истаев",
    "login":"aa",
    "email":"aa@mail.ru",
    "password":"bb",
    "role": "ADMIN"
}

def test_create_content_with_auth():
    content_url = f"http://localhost:8085/v1/contents"
    login_url = f"http://localhost:8085/v1/users/login"
    registration_url = f"http://localhost:8085/v1/users/register"
    headers = {
        "Content-Type": "application/json",
        "ip": "127.0.0.1"
    }

    registration_response = requests.post(registration_url, json=REGISTRATION_PAYLOAD, headers=headers)
    assert registration_response.status_code == 200, f"Ожидался статус 200, получен {registration_response.status_code}, {registration_response.json()}"
    login_response = requests.post(login_url, json=LOGIN_PAYLOAD, headers=headers)
    assert login_response.status_code == 200, f"Ожидался статус 200, получен {login_response.status_code}, {login_response.json()}"
    auth_token_response = login_response.json().get("token")
    headers["Authorization"] = auth_token_response
    content_response = requests.post(content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert content_response.status_code == 200, f"Ожидался статус 200, получен {content_response.status_code}, {content_response.json()}"

def test_negative_login_with_not_existing_user():
    login_url = f"http://localhost:8085/v1/users/login"
    headers = {
        "Content-Type": "application/json",
        "ip": "127.0.0.1"
    }
    login_response = requests.post(login_url, json=LOGIN_PAYLOAD, headers=headers)
    assert login_response.status_code == 404, f"Ожидался статус 404, получен {login_response.status_code}, {login_response.json()}"

def test_login_after_two_fail():
    login_url = f"http://localhost:8085/v1/users/login"
    registration_url = f"http://localhost:8085/v1/users/register"
    headers = {
        "Content-Type": "application/json",
        "ip": "127.0.0.1"
    }

    registration_response = requests.post(registration_url, json=REGISTRATION_PAYLOAD, headers=headers)
    assert registration_response.status_code == 200, f"Ожидался статус 200, получен {registration_response.status_code}, {registration_response.json()}"
    invalid_login_payload = dict(LOGIN_PAYLOAD)
    invalid_login_payload["password"] = "akakkaka"
    login_response = requests.post(login_url, json=invalid_login_payload, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
    login_response = requests.post(login_url, json=invalid_login_payload, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
    login_response = requests.post(login_url, json=LOGIN_PAYLOAD, headers=headers)
    assert login_response.status_code == 200, f"Ожидался статус 200, получен {login_response.status_code}, {login_response.json()}"

def test_ban_after_3_attempts_for_login():
    login_url = f"http://localhost:8085/v1/users/login"
    registration_url = f"http://localhost:8085/v1/users/register"
    headers = {
        "Content-Type": "application/json",
        "ip": "127.0.0.1"
    }

    registration_response = requests.post(registration_url, json=REGISTRATION_PAYLOAD, headers=headers)
    assert registration_response.status_code == 200, f"Ожидался статус 200, получен {registration_response.status_code}, {registration_response.json()}"
    invalid_login_payload = dict(LOGIN_PAYLOAD)
    invalid_login_payload["password"] = "akakkaka"
    login_response = requests.post(login_url, json=invalid_login_payload, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
    login_response = requests.post(login_url, json=invalid_login_payload, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
    login_response = requests.post(login_url, json=invalid_login_payload, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
    login_response = requests.post(login_url, json=LOGIN_PAYLOAD, headers=headers)
    assert login_response.status_code == 401, f"Ожидался статус 401, получен {login_response.status_code}, {login_response.json()}"
