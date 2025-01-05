package com.polytech.contentservice.service.personallist;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.personallist.PersonalListDeletionDto;
import com.polytech.contentservice.dto.personallist.PersonalListDto;
import java.util.List;
import java.util.UUID;

/**
 * Описание взаимодействия с таблицей personal_list.
 */
public interface PersonalListService {
  PersonalListDto addFavoriteMovie(PersonalListDto personalListDto);

  void removeFavoriteMovie(PersonalListDeletionDto dto);

  List<ContentDto> getFavoriteMoviesByUser(UUID userId);
}
