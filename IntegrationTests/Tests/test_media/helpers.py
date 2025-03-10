import re
import requests
import os

CURRENT_DIR = os.path.dirname(os.path.abspath(__file__))

default_content_url = f"http://localhost:8085/v1/contents"
default_episode_url = f"http://localhost:8085/v1/episodes"
default_buckets_url = "http://localhost:8078/v1/buckets"
default_files_url = "http://localhost:8078/v1/buckets/movify/files"

headers = {
    "Authorization": "Bearer YOUR_ACCESS_TOKEN"
}

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
    ]
}

EPISODE_PAYLOAD = {
    "episode_num": 1,
    "season_num": 1,
    "title": "BBB",
    "description": "BBB"
}

def assert_response_status_code(response, expected_status):
    assert response.status_code == expected_status, (
        f"Ожидался статус {expected_status}, получен {response.status_code}, {response.json()}"
    )

def assert_failed_response(response, expected_status, expected_detail):
    json_response = response.json()
    assert_response_status_code(response, expected_status)
    assert json_response['body']['detail'] == expected_detail, (
        f"Ожидался detail '{expected_detail}', получен '{json_response['body']['detail']}'"
    )

def create_bucket(bucket_name):
    response = requests.post(f"{default_buckets_url}?bucket-name={bucket_name}", headers=headers)
    assert_response_status_code(response, 200)
    return response

def upload_file(file_name):
    params = {
        "prefix": "some/folder/",
        "process": "false",
        "destination": "Internal"
    }

    files = {
        "file": (file_name, open(os.path.join(CURRENT_DIR, file_name), "rb"))
    }

    response = requests.post(f"{default_files_url}", headers=headers, params=params, files=files)
    assert_response_status_code(response, 200)

def get_all_buckets(expected_buckets):
    response = requests.get(f"{default_buckets_url}", headers=headers)
    assert_response_status_code(response, 200)

    buckets = response.json()
    assert buckets == expected_buckets, f"Ожидался ответ {expected_buckets}, получен {buckets}"

def get_all_files(expected_files):
    response = requests.get(f"{default_files_url}", headers=headers)
    assert_response_status_code(response, 200)

    files = [
        {
            "bucketName": item["bucketName"],
            "presignedUrl": re.sub(r'\?.*$', '', item["presignedUrl"])
        }
        for item in response.json()
    ]

    assert files == expected_files, f"Ожидался ответ {expected_files}, получен {files}"