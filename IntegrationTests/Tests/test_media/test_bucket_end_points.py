import time
import pytest
import requests

import test_media.helpers as helpers

pytestmark = pytest.mark.services("test-media-service-for-mocks", "test-auth-service-mock", "test-chunker-service-mock", "test-content-service-mock", "test-movify-s3")

def test_get_all_buckets():
    helpers.create_bucket("bucket1")
    helpers.create_bucket("bucket2")
    helpers.get_all_buckets([{"name": "bucket1"}, {"name": "bucket2"}])

def test_create_bucket_success():
    helpers.create_bucket("movify")
    helpers.get_all_buckets([{"name": "movify"}])

def test_create_bucket_failed():
    helpers.create_bucket("movify")
    response = requests.post(f"{helpers.default_buckets_url}?bucket-name=movify", headers=helpers.headers)
    helpers.assert_failed_response(response, 500, 'Failed to create bucket')

def test_delete_bucket_success():
    helpers.create_bucket("movify")

    response = requests.delete(f"{helpers.default_buckets_url}/movify", headers=helpers.headers)
    helpers.assert_response_status_code(response, 204)

    helpers.get_all_buckets([])

def test_delete_bucket_failed():
    response = requests.delete(f"{helpers.default_buckets_url}/movify", headers=helpers.headers)
    helpers.assert_failed_response(response, 404, 'Bucket movify does not exist')

def test_get_all_files_success():
    helpers.create_bucket("movify")
    helpers.upload_file("test.txt")
    helpers.upload_file("test1.txt")

    time.sleep(5)
    helpers.get_all_files([{
        "bucketName": "movify",
        "presignedUrl": "https://test-movify-s3:9000/movify/some/folder/test.txt"
    }, {
        "bucketName": "movify",
        "presignedUrl": "https://test-movify-s3:9000/movify/some/folder/test1.txt"
    }])

def test_get_all_files_failed():
    response = requests.get(f"{helpers.default_files_url}", headers=helpers.headers)
    helpers.assert_failed_response(response, 404, "Bucket movify does not exist")

def test_upload_file_success():
    helpers.create_bucket("movify")
    helpers.upload_file("test.txt")
    time.sleep(5)
    helpers.get_all_files([{
        "bucketName": "movify",
        "presignedUrl": "https://test-movify-s3:9000/movify/some/folder/test.txt"
    }])

def test_get_file_success():
    helpers.create_bucket("movify")
    helpers.upload_file("test.txt")
    time.sleep(5)

    response = requests.get(f"{helpers.default_files_url}/some/folder/test.txt")
    helpers.assert_response_status_code(response, 200)
    assert response.content, "Получен файл"

def test_get_file_failed_no_bucket():
    response = requests.get(f"{helpers.default_files_url}/some/folder/test.txt")
    helpers.assert_failed_response(response, 404, "Bucket movify does not exist")

def test_get_file_failed_no_file():
    helpers.create_bucket("movify")
    response = requests.get(f"{helpers.default_files_url}/some/folder/test.txt")
    helpers.assert_failed_response(response, 404, "File 'some/folder/test.txt' in bucket 'movify' does not exist")

def test_delete_file_success():
    helpers.create_bucket("movify")
    helpers.upload_file("test.txt")
    time.sleep(5)

    response = requests.delete(f"{helpers.default_files_url}/some/folder/test.txt", headers=helpers.headers)
    helpers.assert_response_status_code(response, 204)

def test_delete_file_failed_no_bucket():
    response = requests.delete(f"{helpers.default_files_url}/some/folder/test.txt", headers=helpers.headers)
    helpers.assert_failed_response(response, 404, "Bucket movify does not exist")

def test_delete_file_failed_no_file():
    helpers.create_bucket("movify")
    response = requests.delete(f"{helpers.default_files_url}/some/folder/test.txt", headers=helpers.headers)
    helpers.assert_failed_response(response, 404, "File 'some/folder/test.txt' in bucket 'movify' does not exist")

