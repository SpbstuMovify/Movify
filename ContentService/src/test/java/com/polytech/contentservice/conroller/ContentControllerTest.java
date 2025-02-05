package com.polytech.contentservice.conroller;

import static org.hamcrest.Matchers.hasSize;
import static org.hamcrest.Matchers.is;
import static org.mockito.ArgumentMatchers.any;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.content.ContentSearchDto;
import com.polytech.contentservice.exception.UnauthorisedException;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.content.ContentService;
import java.util.List;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mockito;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.data.domain.PageImpl;
import org.springframework.http.MediaType;
import org.springframework.test.context.junit.jupiter.SpringExtension;
import org.springframework.test.web.servlet.MockMvc;

@ExtendWith(SpringExtension.class)
@WebMvcTest(ContentController.class)
class ContentControllerTest {
  @Autowired
  private MockMvc mvc;
  @MockBean
  private ContentService contentService;
  @MockBean
  private AuthService authService;
  @Autowired
  private ObjectMapper objectMapper;

  @Test
  void getAllContent() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.findAllContent(any())).thenReturn(List.of(contentDto));

    mvc.perform(get("/v1/contents")
            .param("page_number", "0")
            .param("page_size", "10")
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$", hasSize(1)))
        .andExpect(jsonPath("$[0].id", is(contentDto.id().toString())));
  }

  @Test
  void deleteContentById_thenSuccess() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.findAllContent(any()))
        .thenReturn(List.of(contentDto));
    Mockito.doNothing()
        .when(authService).checkTokenIsValid(any(), any());

    mvc.perform(delete("/v1/contents/{content-id}", contentDto.id().toString())
            .header("Authorization", "Bearer token")
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isOk());

    Mockito.verify(contentService).deleteById(any());
    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void deleteContentById_thenNotFound() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mvc.perform(delete("/v1/contents/{content-id}", contentDto.id().toString())
            .header("Authorization", "Bearer token")
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isUnauthorized());

    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void getContentById() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.findContentById(any()))
        .thenReturn(contentDto);

    mvc.perform(get("/v1/contents/{content-id}", contentDto.id().toString())
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.id", is(contentDto.id().toString())));

    Mockito.verify(contentService).findContentById(any());
  }

  @Test
  void getContentByFilters() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.getAllContentsByFilter(any()))
        .thenReturn(new PageImpl<>(List.of(contentDto)));

    mvc.perform(post("/v1/contents/search")
            .contentType(MediaType.APPLICATION_JSON)
            .accept(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(ContentSearchDto.builder().genre(Genre.BLOCKBUSTER).build())))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.content", hasSize(1)))
        .andExpect(jsonPath("$.content[0].id", is(contentDto.id().toString())));

    Mockito.verify(contentService).getAllContentsByFilter(any());
  }

  @Test
  void updateContentInfo_thenThrowUnauthorisedException() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mvc.perform(put("/v1/contents/{content-id}", UUID.randomUUID())
            .header("Authorization", "Bearer token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(contentDto)))
        .andExpect(status().isUnauthorized());

    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void updateContentInfo_thenSuccess() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.findAllContent(any()))
        .thenReturn(List.of(contentDto));
    Mockito.doNothing()
        .when(authService).checkTokenIsValid(any(), any());

    mvc.perform(put("/v1/contents/{content-id}", UUID.randomUUID())
            .header("Authorization", "Bearer token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(contentDto)))
        .andExpect(status().isOk());

    Mockito.verify(contentService).updateContent(any(), any());
    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void createContent_thenThrowUnauthorisedException() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.doThrow(new UnauthorisedException("aaa"))
        .when(authService)
        .checkTokenIsValid(any(), any());

    mvc.perform(post("/v1/contents")
            .header("Authorization", "Bearer token")
            .content(objectMapper.writeValueAsString(contentDto))
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isUnauthorized());

    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  @Test
  void createContent_thenSuccess() throws Exception {
    ContentDto contentDto = getContentDto();

    Mockito.when(contentService.createContent(any()))
        .thenReturn(contentDto);
    Mockito.doNothing()
        .when(authService).checkTokenIsValid(any(), any());

    mvc.perform(post("/v1/contents")
            .header("Authorization", "Bearer token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(contentDto)))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.id", is(contentDto.id().toString())));

    Mockito.verify(contentService).createContent(any());
    Mockito.verify(authService).checkTokenIsValid(any(), any());
  }

  private static ContentDto getContentDto() {
    return ContentDto.builder()
        .id(UUID.randomUUID())
        .title("title")
        .build();
  }
}