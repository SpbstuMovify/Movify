import os
import time
import pytest
import requests

import test_media.helpers as helpers

pytestmark = pytest.mark.services("test-media-service-for-content-service-and-chunker-service",
                                  "test-content-service-for-mock-auth-service",
                                  "test-chunker-service",
                                  "test-auth-service-mock",
                                  "test-movify-db",
                                  "test-movify-s3")

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
        "process": "true",
        "destination": "EpisodeVideoUrl"
    }

    files = {
        "file": ("video.mp4", open(os.path.join(helpers.CURRENT_DIR, "video.mp4"), "rb"), "video/mp4")
    }

    upload_episode_video_response = requests.post(f"{helpers.default_buckets_url}/movify/files", headers=helpers.headers, params=params, files=files)
    assert upload_episode_video_response.status_code == 200, f"Ожидался статус 200, получен {upload_episode_video_response.status_code}, {upload_episode_video_response.json()}"

    time.sleep(15)

    get_episode_response = requests.get(f"{helpers.default_episode_url}/{episode_id}", headers=helpers.headers)
    assert get_episode_response.status_code == 200, f"Ожидался статус 200, получен {get_episode_response.status_code}, {get_episode_response.json()}"
    assert get_episode_response.json()["s3_bucket_name"] == f"/v1/buckets/movify/files/{content_id}/{episode_id}/hls/master.m3u8"