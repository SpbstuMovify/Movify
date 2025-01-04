package com.polytech.contentservice.conroller;

import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.service.content.ContentService;
import com.polytech.contentservice.service.episode.EpisodeService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import java.util.Set;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

/**
 * Контролер эпизодов.
 */
@RestController
@RequiredArgsConstructor
@RequestMapping("/v1/episodes")
@Tag(
    name = "Контролер эпизодов",
    description = "Позволяет манипулировать данными эпизодов")
public class EpisodeController {
  private final EpisodeService episodeService;
  private final ContentService contentService;

  @GetMapping("/{episode-id}")
  @Operation(
      summary = "Получить информацию о эпизоде",
      description = "Предоставляет полную информацию о эпизоде с заданным id")
  public EpisodeDto getEpisodeById(
      @Parameter(description = "ID эпизода", example = "d27ca94d-4206-45fe-b8c4-486403544d64")
      @PathVariable("episode-id")
      UUID episodeId) {
    return episodeService.getEpisodeById(episodeId);
  }

  @DeleteMapping("/{episode-id}")
  @Operation(
      summary = "Удалить информацию о эпизоде",
      description = "Удаляет информацию о эпизоде с заданным id")
  public void deleteEpisodeById(
      @Parameter(description = "ID эпизода", example = "d27ca94d-4206-45fe-b8c4-486403544d64")
      @PathVariable("episode-id")
      UUID episodeId) {
    episodeService.deleteEpisodeById(episodeId);
  }

  @PutMapping("/{episode-id}")
  @Operation(
      summary = "Обновление информации о эпизоде",
      description = "Обновляем информацию существующего эпизода с заданным id с помощью данных из тела запроса")
  public void updateEpisodeById(
      @Parameter(description = "ID эпизода", example = "d27ca94d-4206-45fe-b8c4-486403544d64")
      @PathVariable("episode-id")
      UUID episodeId,
      @RequestBody
      @Parameter(description = "Тело запроса для обновления существующего эпизода")
      EpisodeDto episodeDto) {
    episodeService.updateEpisodeInfo(episodeId, episodeDto);
  }

  @PostMapping
  @Operation(
      summary = "Создание нового эпизода",
      description = "Создаём новый эпизод для заданного контента с помощью данных из тела запроса")
  public EpisodeDto createEpisodeForContent(
      @Parameter(description = "Тело запроса для создания нового эпизода")
      @RequestBody
      EpisodeDto episodeDto) {
    return episodeService.createNewEpisode(episodeDto.contentId(), episodeDto);
  }

  @GetMapping
  @Operation(
      summary = "Получение всех эпизодоа, связанных с контентом",
      description = "Получаем все эпизоды, связанные с заданным контентом")
  public Set<EpisodeDto> getAllEpisodesForContent(
      @Parameter(description = "ID фильма или сериала", example = "1b12236a-aca9-47bc-95ac-f3978836de2c")
      @RequestParam(value = "content_id")
      UUID contentId) {
    return contentService.findAllEpisodesForContent(contentId);
  }
}
