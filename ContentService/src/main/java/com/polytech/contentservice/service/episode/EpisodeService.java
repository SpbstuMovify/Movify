package com.polytech.contentservice.service.episode;

import com.polytech.contentservice.dto.episode.EpisodeDto;
import java.util.UUID;

/**
 * Описание бизнес слоя для эпизодов.
 */
public interface EpisodeService {
  /**
   * Создание нового эпихода в бд
   * @param contentId Идентификатор загружаемого ролика с основной информацией
   * @param episode Сущность для сохранения в нашем сервисе, чтобы остальные смогли смотреть ролики
   * @return Загруженная информация о ролике
   */
  EpisodeDto createNewEpisode(UUID contentId, EpisodeDto episode);

  /**
   * Обновление информации о эпизоде
   * @param episodeId Идентификатор эпизода
   * @param episode Данные для обновления эпизода
   */
  void updateEpisodeInfo(UUID episodeId, EpisodeDto episode);

  /**
   * Получения информации о конкретном ролике
   * @param id Идентификатор эпизода
   * @return Информация, которая хранится в нашей системе
   */
  EpisodeDto getEpisodeById(UUID id);
  /**
   * Удаление информации о конкретном эпизоде.
   * @param id Идентификатор эпизода
   */
  void deleteEpisodeById(UUID id);
}