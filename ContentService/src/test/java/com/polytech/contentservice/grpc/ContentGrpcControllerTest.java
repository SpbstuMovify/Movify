package com.polytech.contentservice.grpc;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.content.SetContentImageUrlRequest;
import com.content.SetEpisodeVideoUrlRequest;
import com.content.UserRoleRequest;
import com.content.UserRoleResponse;
import com.google.protobuf.Empty;
import com.google.rpc.Code;
import com.polytech.contentservice.common.EpisodeStatus;
import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.service.content.ContentService;
import com.polytech.contentservice.service.episode.EpisodeService;
import com.polytech.contentservice.service.user.UserService;
import io.grpc.StatusException;
import io.grpc.stub.StreamObserver;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentCaptor;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class ContentGrpcControllerTest {
  @Mock
  private UserService userService;
  @Mock
  private EpisodeService episodeService;
  @Mock
  private ContentService contentService;
  @InjectMocks
  private ContentGrpcController controller;
  @Mock
  private StreamObserver<UserRoleResponse> userRoleResponseObserver;
  @Mock
  private StreamObserver<Empty> emptyResponseObserver;

  @Test
  void testGetUserRole_UserExists() {
    UserDto userDto = UserDto.builder()
        .email("a@mail.ru")
        .role(Role.USER)
        .build();
    when(userService.getUserByEmailNonExcept("test@example.com"))
        .thenReturn(Optional.of(userDto));

    UserRoleRequest request = UserRoleRequest.newBuilder()
        .setEmail("test@example.com")
        .build();

    controller.getUserRole(request, userRoleResponseObserver);

    verify(userRoleResponseObserver).onNext(UserRoleResponse.newBuilder()
        .setRole("USER")
        .build());
    verify(userRoleResponseObserver).onCompleted();
  }

  @Test
  void testGetUserRole_UserNotFound() {
    when(userService.getUserByEmailNonExcept("test@example.com"))
        .thenReturn(Optional.empty());

    UserRoleRequest request = UserRoleRequest.newBuilder()
        .setEmail("test@example.com")
        .build();

    controller.getUserRole(request, userRoleResponseObserver);

    ArgumentCaptor<Throwable> exceptionCaptor = ArgumentCaptor.forClass(Throwable.class);
    verify(userRoleResponseObserver).onError(exceptionCaptor.capture());

    Throwable capturedException = exceptionCaptor.getValue();

    assertInstanceOf(StatusException.class, capturedException);
    StatusException statusException = (StatusException) capturedException;

    assertEquals(Code.NOT_FOUND.getNumber(), statusException.getStatus().getCode().value());
  }


  @Test
  void testSetContentImageUrl() {
    String contentId = UUID.randomUUID().toString();
    String imageUrl = "http://example.com/image.jpg";

    SetContentImageUrlRequest request = SetContentImageUrlRequest.newBuilder()
        .setContentId(contentId)
        .setUrl(imageUrl)
        .build();

    controller.setContentImageUrl(request, emptyResponseObserver);

    verify(contentService).updateContent(UUID.fromString(contentId),
        ContentDto.builder().thumbnail(imageUrl).build());
    verify(emptyResponseObserver).onNext(Empty.getDefaultInstance());
    verify(emptyResponseObserver).onCompleted();
  }

  @Test
  void testSetEpisodeVideoUrl() {
    String episodeId = UUID.randomUUID().toString();
    String videoUrl = "http://example.com/video.mp4";
    String status = "UPLOADED";

    SetEpisodeVideoUrlRequest request = SetEpisodeVideoUrlRequest.newBuilder()
        .setEpisodeId(episodeId)
        .setUrl(videoUrl)
        .setStatus(status)
        .build();

    controller.setEpisodeVideoUrl(request, emptyResponseObserver);

    verify(episodeService).updateEpisodeInfo(UUID.fromString(episodeId),
        EpisodeDto.builder().s3BucketName(videoUrl).status(EpisodeStatus.valueOf(status)).build());
    verify(emptyResponseObserver).onNext(Empty.getDefaultInstance());
    verify(emptyResponseObserver).onCompleted();
  }
}
