import grpc
from concurrent import futures
import time

import auth_pb2
import auth_pb2_grpc

class AuthServiceMock(auth_pb2_grpc.AuthServiceServicer):
    def ValidateToken(self, request, context):
        return auth_pb2.ValidationTokenResponse(email="mock@example.com", role="ADMIN")
    
    def RegisterUser(self, request, context):
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('The method is not implemented in the mock service!')
        return auth_pb2.RegisterUserResponse()
    
    def LoginUser(self, request, context):
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('The method is not implemented in the mock service!')
        return auth_pb2.LoginUserResponse()

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    auth_pb2_grpc.add_AuthServiceServicer_to_server(AuthServiceMock(), server)
    server.add_insecure_port('0.0.0.0:8079')
    server.start()
    print("Auth-service mock running on port 8079...")
    try:
        while True:
            time.sleep(86400)
    except KeyboardInterrupt:
        server.stop(0)

if __name__ == '__main__':
    serve()
