from conftest import logger
import pytest
import requests

pytestmark = pytest.mark.services("test-content-service-for-mock-auth-service", "test-auth-service-mock", "test-movify-db")

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

illegal_content_id = "bfa2b698-a1b5-4b2c-abfb-5feba20e2642"

UPDATED_CONTENT_PAYLOAD = {
    "publisher": "Zetflix",
}

CONTENT_PAYLOAD1 = {
    "title": "AAA",
    "quality": "P1080",
    "genre": "COMEDY",
    "category": "SERIES",
    "age_restriction": "TWELVE_PLUS",
    "year": 2013,
    "description": "abc...",
    "publisher": "Netflix"
}

CONTENT_SEARCH_PAYLOAD = {
    "title": "AAA",
    "page_size": 10,
    "page_number": 0
}

headers = {
    "Authorization": "YOUR_ACCESS_TOKEN",
    "Content-Type": "application/json"
}

default_content_url = f"http://localhost:8085/v1/contents"

def test_create_content1():

    response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert response.json()["quality"] == CONTENT_PAYLOAD["quality"], f'Создан content с неверным quality'
    assert response.json()["genre"] == CONTENT_PAYLOAD["genre"], f'Создан content с неверным genre'
    assert response.json()["category"] == CONTENT_PAYLOAD["category"], f'Создан content с неверным category'
    assert response.json()["age_restriction"] == CONTENT_PAYLOAD["age_restriction"], f'Создан content с неверным age_restriction'
    assert response.json()["year"] == CONTENT_PAYLOAD["year"], f'Создан content с неверным year'
    assert response.json()["description"] == CONTENT_PAYLOAD["description"], f'Создан content с неверным description'
    assert response.json()["publisher"] == CONTENT_PAYLOAD["publisher"], f'Создан content с неверным publisher'
    assert response.json()["id"] is not None, f'ID в ответе от сервера не должен быть null'

def test_negative_create_content1():
    CONTENT_PAYLOAD_COPY = dict(CONTENT_PAYLOAD)
    CONTENT_PAYLOAD_COPY["age_restriction"] = None
    CONTENT_PAYLOAD_COPY["quality"] = None
    CONTENT_PAYLOAD_COPY["title"] = None
    response = requests.post(default_content_url, json=CONTENT_PAYLOAD_COPY, headers=headers)
    assert response.status_code == 401, f"Ожидался статус 401, получен {response.status_code}, {response.json()}"

def test_get_page_of_contents():
    url = f"http://localhost:8085/v1/contents?page_number=0&page_size=4"

    id1 = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers).json()["id"]
    id2 = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers).json()["id"]
    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert len(response.json()) == 2, f'Неверное количество найденных элементов'
    assert all(item["id"] == id2 or item["id"] == id1 for item in response.json())

def test_empty_get_page_of_contents():
    url = f"http://localhost:8085/v1/contents?page_number=0&page_size=4"
    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert len(response.json()) == 0, f'Неверное количество найденных элементов'

def test_get_content_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    find_url = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.get(find_url, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert response.json()["id"] == created_content_response.json()["id"], f'Найден content с неверным id'

def test_negative_get_content_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    find_url = f"{default_content_url}/" + illegal_content_id
    response = requests.get(find_url, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_delete_content_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    url_with_content_id = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.delete(url_with_content_id, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    response = requests.get(url_with_content_id, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_negative_delete_content_by_id():
    url_with_content_id = f"{default_content_url}/" + illegal_content_id
    response = requests.delete(url_with_content_id, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_update_content():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    url_with_content_id = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.put(url_with_content_id, json=UPDATED_CONTENT_PAYLOAD, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    response = requests.get(url_with_content_id, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert response.json()["publisher"] == UPDATED_CONTENT_PAYLOAD["publisher"], f'Найден content с неверными данными'

def test_negative_update_content():
    url_with_content_id = f"{default_content_url}/" + str(illegal_content_id)
    response = requests.put(url_with_content_id, json=UPDATED_CONTENT_PAYLOAD, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"
    response = requests.get(url_with_content_id, headers=headers)

def test_get_content_with_filter():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"

    url_for_searching_content = f"{default_content_url}/search"
    response = requests.post(url_for_searching_content, json=CONTENT_SEARCH_PAYLOAD, headers=headers)

    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert len(response.json()['content']) == 1, f'Неверное количество найденных элементов'
    assert response.json()['content'][0]['id'] == created_content_response.json()["id"], f'Найден content с неверными данными'

def test_empty_get_content_with_filter():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"

    url_for_searching_content = f"{default_content_url}/search"
    search_payload_copy = dict(CONTENT_SEARCH_PAYLOAD)
    search_payload_copy["title"] = "aaaaaaa"
    response = requests.post(url_for_searching_content, json=search_payload_copy, headers=headers)

    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert len(response.json()['content']) == 0, f'Неверное количество найденных элементов'
