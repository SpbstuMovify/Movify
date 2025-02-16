import grpc
from concurrent import futures
import time

import chunker_pb2_grpc
from google.protobuf import empty_pb2

class ChunkerServiceMock(chunker_pb2_grpc.ChunkerServiceServicer):
    def ProcessVideo(self, request, context):
        print(f"Received ProcessVideo request: bucketName={request.bucketName}, key={request.key}, baseUrl={request.baseUrl}")
        return empty_pb2.Empty()

    def CancelVideoProcessing(self, request, context):
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method CancelVideoProcessing is not implemented in the mock service!')
        return empty_pb2.Empty()

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    chunker_pb2_grpc.add_ChunkerServiceServicer_to_server(ChunkerServiceMock(), server)
    server.add_insecure_port('0.0.0.0:8076')
    server.start()
    print("Chunker service mock is running on port 8076...")
    try:
        while True:
            time.sleep(86400)
    except KeyboardInterrupt:
        server.stop(0)

if __name__ == '__main__':
    serve()
