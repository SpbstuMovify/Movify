import grpc
from concurrent import futures
import time

import content_pb2
import content_pb2_grpc
from google.protobuf import empty_pb2

class ContentServiceMock(content_pb2_grpc.ContentServiceServicer):
    def GetUserRole(self, request, context):
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method GetUserRole is not implemented in the mock service!')
        return content_pb2.UserRoleResponse()

    def SetContentImageUrl(self, request, context):
        print(f"Received SetContentImageUrl request: contentId={request.contentId}, url={request.url}")
        return empty_pb2.Empty()

    def SetEpisodeVideoUrl(self, request, context):
        print(f"Received SetEpisodeVideoUrl request: episodeId={request.episodeId}, url={request.url}, status={request.status}")
        return empty_pb2.Empty()

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    content_pb2_grpc.add_ContentServiceServicer_to_server(ContentServiceMock(), server)
    server.add_insecure_port('0.0.0.0:9095')
    server.start()
    print("Content service mock is running on port 9095...")
    try:
        while True:
            time.sleep(86400)
    except KeyboardInterrupt:
        server.stop(0)

if __name__ == '__main__':
    serve()
