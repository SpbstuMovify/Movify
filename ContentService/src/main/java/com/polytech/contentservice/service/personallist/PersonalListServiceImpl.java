package com.polytech.contentservice.service.personallist;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.personallist.PersonalListDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.PersonalList;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.repository.ContentRepository;
import com.polytech.contentservice.repository.PersonalListRepository;
import com.polytech.contentservice.repository.UserRepository;
import jakarta.transaction.Transactional;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class PersonalListServiceImpl implements PersonalListService {
  private final PersonalListRepository personalListRepository;
  private final ContentRepository contentRepository;
  private final UserRepository userRepository;
  private final ContentMapper contentMapper;

  @Transactional
  @Override
  public PersonalListDto addFavoriteMovie(PersonalListDto personalListDto) {
    User user = userRepository.findById(personalListDto.userId())
        .orElseThrow(() -> new NotFoundException("User not found"));
    Content content = contentRepository.findById(personalListDto.contentId())
        .orElseThrow(() -> new NotFoundException("Content not found"));

    PersonalList favoriteMovie = PersonalList.builder()
        .user(user)
        .content(content)
        .creationDate(LocalDateTime.now())
        .build();
    PersonalList savedPersonalList = personalListRepository.save(favoriteMovie);
    return PersonalListDto.builder()
        .contentId(savedPersonalList.getContent().getId())
        .personalListId(savedPersonalList.getId())
        .userId(savedPersonalList.getUser().getId())
        .build();
  }

  @Override
  @Transactional
  public void removeFavoriteMovie(UUID personalListId) {
    personalListRepository.findById(personalListId)
        .orElseThrow(() -> new NotFoundException("Personal list content does not found"));
    personalListRepository.deleteById(personalListId);
  }

  @Override
  public List<ContentDto> getFavoriteMoviesByUser(UUID userId) {
    return userRepository.findById(userId)
        .orElseThrow(() -> new NotFoundException("User not found"))
        .getPersonalList()
        .stream()
        .map(PersonalList::getContent)
        .map(contentMapper::convertToContentDto)
        .toList();
  }
}
