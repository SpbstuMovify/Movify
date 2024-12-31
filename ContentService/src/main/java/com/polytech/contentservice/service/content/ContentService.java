package com.polytech.contentservice.service.content;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Set;
import java.util.UUID;

public interface ContentService {
  /**
   * Создание основаной информации о контенте
   *
   * @param contentDto Сущность для сохранения контента
   * @return Сохранённые данные
   */
  ContentDto createContent(ContentDto contentDto);

  /**
   * Обновления данных в контенте по идентификатору
   *
   * @param id         Идентификатор контента
   * @param contentDto Новые данные для сохранения
   */
  void updateContent(UUID id, ContentDto contentDto);

  /**
   * Получение контентов, которые уместятся на странице
   *
   * @param pageable
   * @return Список отображаемых контентов
   */
  List<ContentDto> findAllContent(Pageable pageable);

  /**
   * Получение контента по id
   *
   * @param id ИД контента
   * @return полученные контент
   */
  ContentDto findContentById(UUID id);

  /**
   * Получения всех эпизодов для заданного контента
   *
   * @param contentId Идентификатор контента
   * @return коллекция эпизодов для заданного контента
   */
  Set<EpisodeDto> findAllEpisodesForContent(UUID contentId);
}
