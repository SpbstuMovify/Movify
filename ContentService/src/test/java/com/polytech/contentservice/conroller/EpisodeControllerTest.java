package com.polytech.contentservice.conroller;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.exception.UnauthorisedException;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.content.ContentService;
import com.polytech.contentservice.service.episode.EpisodeService;
import java.util.Set;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mockito;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.http.MediaType;
import org.springframework.test.context.junit.jupiter.SpringExtension;
import org.springframework.test.web.servlet.MockMvc;

@ExtendWith(SpringExtension.class)
@WebMvcTest(EpisodeController.class)
class EpisodeControllerTest {
  @Autowired
  private MockMvc mockMvc;
  @MockBean
  private EpisodeService episodeService;
  @MockBean
  private ContentService contentService;
  @MockBean
  private AuthService authService;
  @Autowired
  private ObjectMapper objectMapper;

  @Test
  void getEpisodeById_ShouldReturnEpisode() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    when(episodeService.getEpisodeById(any())).thenReturn(episodeDto);

    mockMvc.perform(get("/v1/episodes/{episode-id}", episodeDto.id()))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.id").value(episodeDto.id().toString()))
        .andExpect(jsonPath("$.title").value("Episode 1"));
  }

  @Test
  void deleteEpisodeById_ShouldReturnNoContent() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    doNothing().when(authService).checkTokenIsValid(any(), any());
    doNothing().when(episodeService).deleteEpisodeById(any());

    mockMvc.perform(delete("/v1/episodes/{episode-id}", episodeDto.id())
            .header("Authorization", "authToken"))
        .andExpect(status().isOk());

    verify(authService).checkTokenIsValid(any(), any());
    verify(episodeService).deleteEpisodeById(any());
  }

  @Test
  void deleteEpisodeById_thenThrowUnauthorisedException() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mockMvc.perform(delete("/v1/episodes/{episode-id}", episodeDto.id())
            .header("Authorization", "authToken"))
        .andExpect(status().isUnauthorized());

    verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void updateEpisodeById_ShouldReturnNoContent() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    Mockito.doNothing().when(authService).checkTokenIsValid(any(), any());
    Mockito.doNothing().when(episodeService).updateEpisodeInfo(any(), any());

    mockMvc.perform(put("/v1/episodes/{episode-id}", episodeDto.id())
            .header("Authorization", "authToken")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(episodeDto)))
        .andExpect(status().isOk());

    verify(authService).checkTokenIsValid(any(), any());
    verify(episodeService).updateEpisodeInfo(any(), any());
  }

  @Test
  void updateEpisodeById_thenThrowUnauthorisedException() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mockMvc.perform(put("/v1/episodes/{episode-id}", episodeDto.id())
            .header("Authorization", "authToken")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(episodeDto)))
        .andExpect(status().isUnauthorized());

    verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void createEpisodeForContent_ShouldReturnCreatedEpisode() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(episodeService.createNewEpisode(any(), any())).thenReturn(episodeDto);

    mockMvc.perform(post("/v1/episodes")
            .header("Authorization", "aaaa")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(episodeDto)))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.id").value(episodeDto.id().toString()))
        .andExpect(jsonPath("$.title").value("Episode 1"));

    verify(authService).checkTokenIsValid(any(), any());
    verify(episodeService).createNewEpisode(any(), any());
  }

  @Test
  void createEpisodeForContent_thenThrowUnauthorisedException() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mockMvc.perform(post("/v1/episodes")
            .header("Authorization", "aaaa")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(episodeDto)))
        .andExpect(status().isUnauthorized());

    verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void getAllEpisodesForContent_ShouldReturnEpisodesList() throws Exception {
    EpisodeDto episodeDto = getEpisodeDto();
    when(contentService.findAllEpisodesForContent(any()))
        .thenReturn(Set.of(episodeDto));

    mockMvc.perform(get("/v1/episodes")
            .param("content_id", episodeDto.contentId().toString()))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$[0].id").value(episodeDto.id().toString()))
        .andExpect(jsonPath("$[0].title").value("Episode 1"));

    verify(contentService).findAllEpisodesForContent(any());
  }

  private static EpisodeDto getEpisodeDto() {
    return EpisodeDto.builder()
        .id(UUID.randomUUID())
        .title("Episode 1")
        .contentId(UUID.randomUUID())
        .build();
  }
}
