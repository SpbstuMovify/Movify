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
    assert response.json()["quality"] == CONTENT_PAYLOAD["quality"]
    assert response.json()["genre"] == CONTENT_PAYLOAD["genre"]
    assert response.json()["category"] == CONTENT_PAYLOAD["category"]
    assert response.json()["age_restriction"] == CONTENT_PAYLOAD["age_restriction"]
    assert response.json()["year"] == CONTENT_PAYLOAD["year"]
    assert response.json()["description"] == CONTENT_PAYLOAD["description"]
    assert response.json()["publisher"] == CONTENT_PAYLOAD["publisher"]
    assert response.json()["id"] is not None

def test_get_page_of_contents():
    url = f"http://localhost:8085/v1/contents?page_number=0&page_size=4"

    id1 = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers).json()["id"]
    id2 = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers).json()["id"]
    response = requests.get(url, headers=headers)
    assert response.status_code == 200
    assert len(response.json()) == 2
    assert any(item["id"] == id2 for item in response.json())
    assert any(item["id"] == id1 for item in response.json())

def test_get_content_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200
    find_url = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.get(find_url, headers=headers)
    assert response.status_code == 200
    assert response.json()["id"] == created_content_response.json()["id"]

def test_delete_content_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200
    url_with_content_id = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.delete(url_with_content_id, headers=headers)
    assert response.status_code == 200
    response = requests.get(url_with_content_id, headers=headers)
    assert response.status_code == 404

def test_update_content():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)

    assert created_content_response.status_code == 200
    url_with_content_id = f"{default_content_url}/" + str(created_content_response.json()['id'])
    response = requests.put(url_with_content_id, json=UPDATED_CONTENT_PAYLOAD, headers=headers)
    assert response.status_code == 200
    response = requests.get(url_with_content_id, headers=headers)
    assert response.status_code == 200
    assert response.json()["publisher"] == UPDATED_CONTENT_PAYLOAD["publisher"]

def test_get_content_with_filter():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers)
    assert created_content_response.status_code == 200

    url_for_searching_content = f"{default_content_url}/search"
    response = requests.post(url_for_searching_content, json=CONTENT_SEARCH_PAYLOAD, headers=headers)

    assert response.status_code == 200
    assert len(response.json()['content']) == 1
    assert response.json()['content'][0]['id'] == created_content_response.json()["id"]
