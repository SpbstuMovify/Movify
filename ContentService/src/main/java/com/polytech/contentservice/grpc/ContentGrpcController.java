package com.polytech.contentservice.grpc;

import com.google.rpc.Code;
import com.google.rpc.Status;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.service.user.UserService;
import content.ContentServiceGrpc;
import content.UserRoleRequest;
import content.UserRoleResponse;
import io.grpc.protobuf.StatusProto;
import io.grpc.stub.StreamObserver;
import java.util.Optional;
import lombok.RequiredArgsConstructor;
import org.lognet.springboot.grpc.GRpcService;

@GRpcService
@RequiredArgsConstructor
public class ContentGrpcController extends ContentServiceGrpc.ContentServiceImplBase {
  private final UserService userService;

  @Override
  public void getUserRole(UserRoleRequest userRoleRequest,
                          StreamObserver<UserRoleResponse> responseObserver) {
    Optional<UserDto> user = userService.getUserByEmailNonExcept(userRoleRequest.getEmail());
    if (user.isEmpty()) {
      Status status = Status.newBuilder()
          .setCode(Code.NOT_FOUND.getNumber())
          .setMessage("User not found")
          .build();
      responseObserver.onError(StatusProto.toStatusException(status));
      return;
    }
    responseObserver.onNext(
        UserRoleResponse.newBuilder()
            .setRole(user.get().role().toString())
            .build()
    );
    responseObserver.onCompleted();
  }
}
