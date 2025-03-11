import os
import time
import pytest
import requests

import test_media.helpers as helpers

pytestmark = pytest.mark.services("test-media-service-for-content-service",
                                  "test-content-service-for-mock-auth-service",
                                  "test-auth-service-mock",
                                  "test-movify-db",
                                  "test-movify-s3")

def test_upload_content_image_success():
    helpers.create_bucket("movify")

    create_content_response = requests.post(helpers.default_content_url, json=helpers.CONTENT_PAYLOAD, headers=helpers.headers)
    assert create_content_response.status_code == 200, f"Ожидался статус 200, получен {create_content_response.status_code}, {create_content_response.json()}"

    content_id = str(create_content_response.json()['id'])

    params = {
        "prefix": f"{content_id}/",
        "process": "false",
        "destination": "ContentImageUrl"
    }

    files = {
        "file": ("image.png", open(os.path.join(helpers.CURRENT_DIR, "image.png"), "rb"), "image/png")
    }

    upload_content_image_response = requests.post(f"{helpers.default_buckets_url}/movify/files", headers=helpers.headers, params=params, files=files)
    assert upload_content_image_response.status_code == 200, f"Ожидался статус 200, получен {upload_content_image_response.status_code}, {upload_content_image_response.json()}"

    time.sleep(5)

    get_content_response = requests.get(f"{helpers.default_content_url}/{content_id}", headers=helpers.headers)
    assert get_content_response.status_code == 200, f"Ожидался статус 200, получен {get_content_response.status_code}, {get_content_response.json()}"
    assert get_content_response.json()["thumbnail"] == f"/v1/buckets/movify/files/{content_id}/image.png"

def test_upload_content_image_failed():
    helpers.create_bucket("movify")

    create_content_response = requests.post(helpers.default_content_url, json=helpers.CONTENT_PAYLOAD, headers=helpers.headers)
    assert create_content_response.status_code == 200, f"Ожидался статус 200, получен {create_content_response.status_code}, {create_content_response.json()}"

    content_id = str(create_content_response.json()['id'])

    params = {
        "prefix": f"{content_id}/",
        "process": "false",
        "destination": "ContentImageUrl"
    }

    files = {
        "file": ("test.txt", open(os.path.join(helpers.CURRENT_DIR, "test.txt"), "rb"))
    }

    upload_content_image_response = requests.post(f"{helpers.default_buckets_url}/movify/files", headers=helpers.headers, params=params, files=files)
    helpers.assert_failed_response(upload_content_image_response, 400, "Validation failed: \n -- File: File must be an image for destination ContentImageUrl Severity: Error")

    get_content_response = requests.get(f"{helpers.default_content_url}/{content_id}", headers=helpers.headers)
    assert get_content_response.status_code == 200, f"Ожидался статус 200, получен {get_content_response.status_code}, {get_content_response.json()}"
    assert get_content_response.json()["thumbnail"] is None

def test_upload_episode_video_success():
    helpers.create_bucket("movify")

    create_content_response = requests.post(helpers.default_content_url, json=helpers.CONTENT_PAYLOAD, headers=helpers.headers)
    assert create_content_response.status_code == 200, f"Ожидался статус 200, получен {create_content_response.status_code}, {create_content_response.json()}"
    content_id = str(create_content_response.json()['id'])

    helpers.EPISODE_PAYLOAD['content_id'] = content_id
    created_episode_response = requests.post(helpers.default_episode_url, json=helpers.EPISODE_PAYLOAD, headers=helpers.headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    episode_id = str(created_episode_response.json()['id'])

    params = {
        "prefix": f"{content_id}/{episode_id}/",
        "process": "false",
        "destination": "EpisodeVideoUrl"
    }

    files = {
        "file": ("video.mp4", open(os.path.join(helpers.CURRENT_DIR, "video.mp4"), "rb"), "video/mp4")
    }

    upload_episode_video_response = requests.post(f"{helpers.default_buckets_url}/movify/files", headers=helpers.headers, params=params, files=files)
    assert upload_episode_video_response.status_code == 200, f"Ожидался статус 200, получен {upload_episode_video_response.status_code}, {upload_episode_video_response.json()}"

    time.sleep(5)

    get_episode_response = requests.get(f"{helpers.default_episode_url}/{episode_id}", headers=helpers.headers)
    assert get_episode_response.status_code == 200, f"Ожидался статус 200, получен {get_episode_response.status_code}, {get_episode_response.json()}"
    assert get_episode_response.json()["s3_bucket_name"] == f"/v1/buckets/movify/files/{content_id}/{episode_id}/video.mp4"

def test_upload_episode_video_failed():
    helpers.create_bucket("movify")

    create_content_response = requests.post(helpers.default_content_url, json=helpers.CONTENT_PAYLOAD, headers=helpers.headers)
    assert create_content_response.status_code == 200, f"Ожидался статус 200, получен {create_content_response.status_code}, {create_content_response.json()}"
    content_id = str(create_content_response.json()['id'])

    helpers.EPISODE_PAYLOAD['content_id'] = content_id
    created_episode_response = requests.post(helpers.default_episode_url, json=helpers.EPISODE_PAYLOAD, headers=helpers.headers)
    assert created_episode_response.status_code == 200, f"Ожидался статус 200, получен {created_episode_response.status_code}, {created_episode_response.json()}"
    episode_id = str(created_episode_response.json()['id'])

    params = {
        "prefix": f"{content_id}/{episode_id}/",
        "process": "false",
        "destination": "EpisodeVideoUrl"
    }

    files = {
        "file": ("test.txt", open(os.path.join(helpers.CURRENT_DIR, "test.txt"), "rb"))
    }

    upload_episode_video_response = requests.post(f"{helpers.default_buckets_url}/movify/files", headers=helpers.headers, params=params, files=files)
    helpers.assert_failed_response(upload_episode_video_response, 400, "Validation failed: \n -- File: File must be a video for destination EpisodeVideoUrl Severity: Error")

    time.sleep(5)

    get_episode_response = requests.get(f"{helpers.default_episode_url}/{episode_id}", headers=helpers.headers)
    assert get_episode_response.status_code == 200, f"Ожидался статус 200, получен {get_episode_response.status_code}, {get_episode_response.json()}"
    assert get_episode_response.json()["s3_bucket_name"] is None
