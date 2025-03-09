import os
import subprocess
import time
import logging
from datetime import datetime
import pytest


def setup_logger():
    """Создаёт логгер с файловым и консольным выводом."""
    logger = logging.getLogger("docker_compose")
    logger.setLevel(logging.INFO)

    formatter = logging.Formatter("%(asctime)s - %(levelname)s - %(message)s")
    
    base_dir = os.path.abspath(os.path.dirname(__file__))
    logs_dir = os.path.join(base_dir, "logs")
    os.makedirs(logs_dir, exist_ok=True)

    log_filename = datetime.now().strftime("%Y-%m-%d_%H-%M-%S.log")
    file_handler = logging.FileHandler(os.path.join(logs_dir, log_filename))
    file_handler.setFormatter(formatter)
    file_handler.setLevel(logging.INFO)
    logger.addHandler(file_handler)

    stream_handler = logging.StreamHandler()
    stream_handler.setFormatter(formatter)
    stream_handler.setLevel(logging.INFO)
    logger.addHandler(stream_handler)

    return logger


logger = setup_logger()


@pytest.fixture(scope="function", autouse=True)
def docker_compose(request):
    """Автоматически поднимает и останавливает контейнеры перед/после каждого теста."""

    test_name = request.node.nodeid
    logger.info(f">>> Start of test: {test_name}")

    base_dir = os.path.abspath(os.path.dirname(__file__))
    compose_file = os.path.join(base_dir, "docker-compose.integration.yml")

    marker = request.node.get_closest_marker("services")
    services = list(marker.args) if marker else []
    services_info = services if services else "ALL"

    try:
        logger.info(f"Starting services: {services_info}")
        subprocess.check_call(["docker", "compose", "-f", compose_file, "up", "-d"] + services)

        time.sleep(15)

        yield

    finally:
        logger.info("Collecting container logs...")
        try:
            logs = subprocess.check_output(["docker", "compose", "-f", compose_file, "logs"])
            logger.info(f"Docker-compose logs:\n\n{logs.decode('utf-8')}")
        except subprocess.CalledProcessError as e:
            logger.error(f"Error collecting logs: {e}")

        logger.info("Stopping containers...")
        try:
            subprocess.check_call(["docker", "compose", "-f", compose_file, "down"])
        except subprocess.CalledProcessError as e:
            logger.error(f"Error stopping containers: {e}")

        logger.info(f"<<< End of test: {test_name}\n")
