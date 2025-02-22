package com.polytech.contentservice.conroller;

import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.content.ContentSearchDto;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.content.ContentService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import java.util.List;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
@RequestMapping("/v1/contents")
@Tag(
    name = "Контроллер для фильмов и сериалов",
    description = "Контроллер для манипулирования основаными данными, связанными с фильмами и сериалами")
public class ContentController {
  private final ContentService contentService;
  private final AuthService authService;

  @GetMapping
  @Operation(
      summary = "Получение фильмов или сериалов",
      description = "Позволяет получить несколько фильмов или сериалов для последующей загрузке на начальной странице с фильмами"
  )
  public List<ContentDto> getAllContent(
      @Parameter(description = "Параметр номера страницы", example = "0")
      @RequestParam(value = "page_number")
      int pageNumber,
      @Parameter(description = "Параметр количества отображаемых фильмов и сериалов на странице", example = "3")
      @RequestParam(value = "page_size")
      int pageSize) {
    return contentService.findAllContent(PageRequest.of(pageNumber, pageSize));
  }

  @DeleteMapping("/{content-id}")
  @Operation(
      summary = "Удаление описания фильма или сериала по ИД",
      description = "Позволяет удалить описание фильма или сериала по ИД"
  )
  public void deleteContentById(
      @PathVariable(name = "content-id") UUID contentId,
      @RequestHeader("Authorization") String token) {
    authService.checkTokenIsValid(token, Role.ADMIN);
    contentService.deleteById(contentId);
  }

  @GetMapping("/{content-id}")
  @Operation(
      summary = "Получение фильма или сериала по ИД",
      description = "Позволяет получить один фильм или сериал по ИД"
  )
  public ContentDto getContentById(
      @PathVariable(name = "content-id") UUID contentId) {
    return contentService.findContentById(contentId);
  }

  @PostMapping("/search")
  @Operation(
      summary = "Получение фильма или сериала по фильтрам",
      description = "Позволяет получить все фильмы или сериалы по фильтрам"
  )
  public Page<ContentDto> getContentByFilters(@RequestBody ContentSearchDto contentDto) {
    return contentService.getAllContentsByFilter(contentDto);
  }

  @PutMapping("/{content-id}")
  @Operation(
      summary = "Обновление информации о фильме или сериале",
      description = "Позволяет обновить главную информацию, связанную с фильмом или сериалом"
  )
  public void updateContentInfo(
      @Parameter(description = "ID фильма или сериала", example = "1b12236a-aca9-47bc-95ac-f3978836de2c")
      @PathVariable(value = "content-id")
      UUID contentId,
      @Parameter(description = "Сущность для манипулирования информацией по фильмам и сериалам")
      @RequestBody
      ContentDto contentDto,
      @RequestHeader("Authorization") String token) {
    authService.checkTokenIsValid(token, Role.ADMIN);
    contentService.updateContent(contentId, contentDto);
  }

  @PostMapping
  @Operation(
      summary = "Создание нового фильма или сериала",
      description = "Позволяет добавить информацию о фильме или сериале в сервис, чтобы другие люди смогли насладиться контентом за денежки"
  )
  public ContentDto createContent(
      @Parameter(description = "Сущность для манипулирования информацией по фильмам и сериалам")
      @RequestBody
      ContentDto contentDto,
      @RequestHeader("Authorization") String token) {
    authService.checkTokenIsValid(token, Role.ADMIN);
    return contentService.createContent(contentDto);
  }
}
