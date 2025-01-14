package com.polytech.contentservice.service.content;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.content.ContentSearchDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.QContent;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import com.querydsl.core.BooleanBuilder;
import com.querydsl.jpa.impl.JPAQuery;
import com.querydsl.jpa.impl.JPAQueryFactory;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.transaction.Transactional;
import java.util.List;
import java.util.Set;
import java.util.UUID;
import java.util.stream.Collectors;
import lombok.RequiredArgsConstructor;
import org.apache.commons.lang3.StringUtils;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link ContentService}.
 */
@Service
@RequiredArgsConstructor
public class ContentServiceImpl implements ContentService {
  private final QContent content = QContent.content;

  private final ContentRepository contentRepository;
  private final ContentMapper contentMapper;
  private final EpisodeMapper episodeMapper;

  @PersistenceContext
  private EntityManager entityManager;

  @Override
  public ContentDto createContent(ContentDto contentDto) {
    Content contentToSave = contentMapper.convertToContentEntity(contentDto);
    Content savedContent = contentRepository.save(contentToSave);
    return contentMapper.convertToContentDto(savedContent);
  }

  @Override
  @Transactional
  public void updateContent(UUID id, ContentDto contentDto) {
    Content oldContent = getContentById(id);
    Content updatedContent = contentMapper.patchUpdate(oldContent, contentDto);
    contentRepository.save(updatedContent);
  }

  @Override
  public Set<EpisodeDto> findAllEpisodesForContent(UUID contentId) {
    Content curContent = getContentById(contentId);
    return curContent.getEpisodes()
        .stream()
        .map(episodeMapper::convertToEpisodeDto)
        .collect(Collectors.toSet());
  }

  private Content getContentById(UUID contentId) {
    return contentRepository.findById(contentId)
        .orElseThrow(() -> new IllegalArgumentException("Episode not found"));
  }

  @Override
  public List<ContentDto> findAllContent(Pageable pageable) {
    Page<Content> contents = contentRepository.findAll(pageable);
    return contentMapper.convertToListOfContentDto(contents);
  }

  @Override
  public Page<ContentDto> getAllContentsByFilter(ContentSearchDto contentSearchDto) {
    JPAQueryFactory queryFactory = new JPAQueryFactory(entityManager);
    Pageable pageable = PageRequest.of(
        contentSearchDto.pageNumber(),
        contentSearchDto.pageSize());

    BooleanBuilder booleanBuilder = new BooleanBuilder();

    if (StringUtils.isNoneEmpty(contentSearchDto.title())) {
      booleanBuilder.and(content.title.containsIgnoreCase(contentSearchDto.title()));
    }
    if (contentSearchDto.genre() != null) {
      booleanBuilder.and(content.genre.eq(contentSearchDto.genre()));
    }
    if (contentSearchDto.ageRestriction() != null) {
      booleanBuilder.and(content.ageRestriction.eq(contentSearchDto.ageRestriction()));
    }
    if (contentSearchDto.year() != null) {
      booleanBuilder.and(content.year.eq(contentSearchDto.year()));
    }
    JPAQuery<Content> query = queryFactory.selectFrom(content)
        .where(booleanBuilder);

    List<ContentDto> products = query.offset(pageable.getOffset())
        .limit(pageable.getPageSize())
        .fetch()
        .stream()
        .map(contentMapper::convertToContentDto)
        .toList();

    return new PageImpl<>(products, pageable, query.fetchCount());
  }

  @Override
  public ContentDto findContentById(UUID id) {
    return contentMapper.convertToContentDto(getContentById(id));
  }
}
