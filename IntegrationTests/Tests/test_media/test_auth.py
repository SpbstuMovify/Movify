import pytest
import requests

import test_media.helpers as helpers

pytestmark = pytest.mark.services("test-media-service-for-content-service-and-auth-service",
                                  "test-content-service-for-auth-service",
                                  "test-auth-service",
                                  "test-movify-db",
                                  "test-movify-s3")

registration_url = f"http://localhost:8085/v1/users/register"

headers = {
    "ip": "127.0.0.1"
}

ADMIN_REGISTRATION_PAYLOAD = {
    "first_name":"Эрдем",
    "last_name": "Истаев",
    "login":"aa",
    "email":"aa@mail.ru",
    "password":"bb",
    "role": "ADMIN"
}

USER_REGISTRATION_PAYLOAD = {
    "first_name":"Эрдем",
    "last_name": "Истаев",
    "login":"aa",
    "email":"aa@mail.ru",
    "password":"bb",
    "role": "USER"
}

def test_upload_file_with_auth_success():
    registration_response = requests.post(registration_url, json=ADMIN_REGISTRATION_PAYLOAD, headers=headers)
    assert registration_response.status_code == 200, f"Ожидался статус 200, получен {registration_response.status_code}, {registration_response.json()}"

    auth_token_response = registration_response.json().get("token")
    headers["Authorization"] = f"Bearer {auth_token_response}"

    response = requests.post(f"{helpers.default_buckets_url}?bucket-name=movify", headers=headers)
    helpers.assert_response_status_code(response, 200)

def test_upload_file_with_auth_failed():
    registration_response = requests.post(registration_url, json=USER_REGISTRATION_PAYLOAD, headers=headers)
    assert registration_response.status_code == 200, f"Ожидался статус 200, получен {registration_response.status_code}, {registration_response.json()}"

    auth_token_response = registration_response.json().get("token")
    headers["Authorization"] = f"Bearer {auth_token_response}"

    create_bucket_response = requests.post(f"{helpers.default_buckets_url}?bucket-name=movify", headers=headers)
    helpers.assert_failed_response(create_bucket_response, 403, "Forbidden")