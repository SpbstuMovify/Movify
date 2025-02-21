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

DEFAULT_EPISODE = {
    "episode_num": 1,
    "season_num": 1,
    "title": "BBB",
    "description": "BBB"
}
EPISODE1 = {
    "episode_num": 1,
    "season_num": 2,
    "title": "AC",
    "description": "AC"
}

headers = {
    "Authorization": "YOUR_ACCESS_TOKEN",
    "Content-Type": "application/json"
}
UPDATED_EPISODE = {
    "s3_bucket_name": "/v1/buckets/movify-videos/files/3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a/5b74a79a-a151-4338-84d7-586e2e40e556/hls/master.m3u8",
    "status": "UPLOADED"
}

default_content_url = f"http://localhost:8085/v1/contents"
default_episode_url = f"http://localhost:8085/v1/episodes"
illegal_episode_id = "bfa2b698-a1b5-4b2c-abfb-5feba20e2642"
def test_create_episode():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    assert created_episode_response.json()['episode_num'] == DEFAULT_EPISODE['episode_num'], f"Неверный episode_num при создании эпизода"
    assert created_episode_response.json()['season_num'] == DEFAULT_EPISODE['season_num'], f"Неверный season_num при создании эпизода"
    assert created_episode_response.json()['title'] == DEFAULT_EPISODE['title'], f"Неверный title при создании эпизода"
    assert created_episode_response.json()['description'] == DEFAULT_EPISODE['description'], f"Неверный description при создании эпизода"
    assert created_episode_response.json()["id"] is not None, f'ID в ответе от сервера не должен быть null'

def test_negative_create_duplicated_episode():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    assert created_episode_response.json()['episode_num'] == DEFAULT_EPISODE['episode_num'], f"Неверный episode_num при создании эпизода"
    assert created_episode_response.json()['season_num'] == DEFAULT_EPISODE['season_num'], f"Неверный season_num при создании эпизода"
    assert created_episode_response.json()['title'] == DEFAULT_EPISODE['title'], f"Неверный title при создании эпизода"
    assert created_episode_response.json()['description'] == DEFAULT_EPISODE['description'], f"Неверный description при создании эпизода"
    assert created_episode_response.json()["id"] is not None, f'ID в ответе от сервера не должен быть null'
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 400, f"Ожидался статус 400, получен {created_episode_response.status_code}, {created_episode_response.json()}"

def test_get_episode_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    find_url = f"{default_episode_url}/" + str(created_episode_response.json()['id'])
    response = requests.get(find_url, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert response.json()["id"] == created_episode_response.json()["id"], f"Найден неверный статус, ожидался id = {created_episode_response.json()['id']} найден эпизод с id = {response.json()['id']}"

def test_not_found_get_episode_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    find_url = f"{default_episode_url}/" + illegal_episode_id
    response = requests.get(find_url, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_delete_episode_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    url_with_episode_id = f"{default_episode_url}/" + str(created_episode_response.json()['id'])
    response = requests.delete(url_with_episode_id, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    response = requests.get(url_with_episode_id, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_not_found_delete_episode_by_id():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    url_with_episode_id = f"{default_episode_url}/" + illegal_episode_id
    response = requests.delete(url_with_episode_id, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_get_all_episodes_by_content():
    e1 = dict(DEFAULT_EPISODE)
    e2 = dict(DEFAULT_EPISODE)
    e3 = dict(EPISODE1)
    e4 = dict(EPISODE1)
    created_content_response1 = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response1.status_code == 200, f"Ожидался статус 200, получен {created_content_response1.status_code}, {created_content_response1.json()}"
    e1['content_id'] = created_content_response1.json()['id']
    e3['content_id'] = created_content_response1.json()['id']
    created_content_response2 = requests.post(default_content_url, json=CONTENT_PAYLOAD1, headers=headers)
    assert created_content_response2.status_code == 200, f"Ожидался статус 200, получен {created_content_response2.status_code}, {created_content_response2.json()}"
    e2['content_id'] = created_content_response2.json()['id']
    e4['content_id'] = created_content_response2.json()['id']

    e1_response = requests.post(default_episode_url, json=e1, headers=headers)
    assert e1_response.status_code == 200, f"Ожидался статус 200, получен {e1_response.status_code}, {e1_response.json()}"
    e2_response = requests.post(default_episode_url, json=e2, headers=headers)
    assert e2_response.status_code == 200, f"Ожидался статус 200, получен {e2_response.status_code}, {e2_response.json()}"
    e3_response = requests.post(default_episode_url, json=e3, headers=headers)
    assert e3_response.status_code == 200, f"Ожидался статус 200, получен {e3_response.status_code}, {e3_response.json()}"
    e4_response = requests.post(default_episode_url, json=e4, headers=headers)
    assert e4_response.status_code == 200, f"Ожидался статус 200, получен {e4_response.status_code}, {e4_response.json()}"

    response = requests.get(default_episode_url + "?content_id=" + created_content_response1.json()['id'], headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert len(response.json()) == 2, f"Найдено неверное количество эпизодов для контента с id = {str(created_content_response1.json()['id'])}"
    assert all(item["id"] == e1_response.json()['id'] or item["id"] == e3_response.json()['id'] for item in response.json())

def test_negative_get_all_episodes_by_content():
    response = requests.get(default_episode_url + "?content_id=" + illegal_episode_id, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"

def test_update_episode():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    created_episode_response = requests.post(default_episode_url, json=DEFAULT_EPISODE, headers=headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    url_with_episode_id = f"{default_episode_url}/" + str(created_episode_response.json()['id'])
    response = requests.put(url_with_episode_id, json=UPDATED_EPISODE, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    response = requests.get(url_with_episode_id, headers=headers)
    assert response.status_code == 200, f"Ожидался статус 200, получен {response.status_code}, {response.json()}"
    assert response.json()["status"] == UPDATED_EPISODE["status"], f"Статус эпизода с id = {created_episode_response.json()['id']} не были обновлены"

def test_not_found_update_episode():
    created_content_response = requests.post(default_content_url, json=CONTENT_PAYLOAD, headers=headers)
    assert created_content_response.status_code == 200, f"Ожидался статус 200, получен {created_content_response.status_code}, {created_content_response.json()}"
    DEFAULT_EPISODE['content_id'] = created_content_response.json()['id']
    url_with_episode_id = f"{default_episode_url}/" + illegal_episode_id
    response = requests.put(url_with_episode_id, json=UPDATED_EPISODE, headers=headers)
    assert response.status_code == 404, f"Ожидался статус 404, получен {response.status_code}, {response.json()}"
