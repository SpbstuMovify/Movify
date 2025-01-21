package com.polytech.contentservice.service.content;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.content.ContentSearchDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import java.util.List;
import java.util.Set;
import java.util.UUID;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

/**
 * Описание взаимодействия с таблицей content.
 */
public interface ContentService {
  /**
   * Создание основаной информации о контенте.
   *
   * @param contentDto Сущность для сохранения контента
   * @return Сохранённые данные
   */
  ContentDto createContent(ContentDto contentDto);

  /**
   * Обновления данных в контенте по идентификатору.
   *
   * @param id         Идентификатор контента
   * @param contentDto Новые данные для сохранения
   */
  void updateContent(UUID id, ContentDto contentDto);

  /**
   * Получение контентов, которые уместятся на странице.
   *
   * @param pageable информации о кол-ве элементов на странице
   * @return Список отображаемых контентов
   */
  List<ContentDto> findAllContent(Pageable pageable);

  /**
   * Получение контента по id.
   *
   * @param id ИД контента
   * @return полученные контент
   */
  ContentDto findContentById(UUID id);

  /**
   * Удаление контента по id.
   *
   * @param id ИД контента
   */
  void deleteById(UUID id);

  /**
   * Получения всех эпизодов для заданного контента.
   *
   * @param contentId Идентификатор контента
   * @return коллекция эпизодов для заданного контента
   */
  Set<EpisodeDto> findAllEpisodesForContent(UUID contentId);

  /**
   * Получение контента по фильтрам.
   *
   * @param contentSearchDto фильтры
   * @return список полученных контентов
   */
  Page<ContentDto> getAllContentsByFilter(ContentSearchDto contentSearchDto);
}
