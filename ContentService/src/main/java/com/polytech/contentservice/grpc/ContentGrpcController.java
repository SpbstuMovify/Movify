package com.polytech.contentservice.grpc;

import com.google.rpc.Code;
import com.google.rpc.Status;
import com.polytech.contentservice.common.EpisodeStatus;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.service.content.ContentService;
import com.polytech.contentservice.service.episode.EpisodeService;
import com.polytech.contentservice.service.user.UserService;
import content.ContentServiceGrpc;
import content.UserRoleRequest;
import content.UserRoleResponse;
import io.grpc.protobuf.StatusProto;
import io.grpc.stub.StreamObserver;
import java.util.Optional;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.lognet.springboot.grpc.GRpcService;

@GRpcService
@RequiredArgsConstructor
public class ContentGrpcController extends ContentServiceGrpc.ContentServiceImplBase {
  private final UserService userService;
  private final EpisodeService episodeService;
  private final ContentService contentService;

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

  @Override
  public void setContentImageUrl(content.SetContentImageUrlRequest request,
                                 io.grpc.stub.StreamObserver<com.google.protobuf.Empty> responseObserver) {
    contentService.updateContent(UUID.fromString(request.getContentId()),
        ContentDto.builder()
            .thumbnail(request.getUrl())
            .build());
    responseObserver.onNext(com.google.protobuf.Empty.getDefaultInstance());
    responseObserver.onCompleted();
  }

  @Override
  public void setEpisodeVideoUrl(content.SetEpisodeVideoUrlRequest request,
                                 io.grpc.stub.StreamObserver<com.google.protobuf.Empty> responseObserver) {
    episodeService.updateEpisodeInfo(UUID.fromString(request.getEpisodeId()),
        EpisodeDto.builder()
            .s3BucketName(request.getUrl())
            .status(EpisodeStatus.valueOf(request.getStatus()))
            .build());
    responseObserver.onNext(com.google.protobuf.Empty.getDefaultInstance());
    responseObserver.onCompleted();
  }
}
